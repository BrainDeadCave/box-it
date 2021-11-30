using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using RabbitMQ.Client.Events;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour
{
	public static NetworkManager Instance;

	[SerializeField]
	private string rmqUsername;
	[SerializeField]
	private string rmqPassword;
	[SerializeField]
	private string rmqHostName;
	[SerializeField]
	private List<string> fallbackNodes;

	private Dictionary<IConnection,IModel> validConnections = new Dictionary<IConnection, IModel>();

	private bool m_DoConsumeEvalNextFrame = false;
	private string m_ConsumeEvalMessage = "{}";
	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
		else
		{
			Debug.LogWarning($"NetworkManager {Instance.name} already exists and is a singleton! NetworkManager {name} will be disabled.");
			this.enabled = false;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start()
	{
		CreateConnections();
	}

	private void Update()
	{
		if (m_DoConsumeEvalNextFrame)
		{
			BackendResponse response = JsonUtility.FromJson<BackendResponse>(m_ConsumeEvalMessage);

			if (response.status.Equals("FAILURE"))
			{
				MainMenuController.Instance.Error("Celery Error, see console");
			}
			else if (response.status.Equals("SUCCESS"))
			{
				if (response.result.task.Equals("login"))
				{
					if (response.result.status.Equals("success"))
					{
						//log in successful, show the userID in the happy box
						MainMenuController.Instance.Success($"Logged in! UserID: {response.result.userid}");
					}
					else
					{
						//log in unsuccessful, show error in sad box
						MainMenuController.Instance.Error(response.result.error);
					}
				}
				else if (response.result.task.Equals("register"))
				{
					if (response.result.status.Equals("success"))
					{
						//resgister successful, show the box
						MainMenuController.Instance.RegisterCompleteWindow();
					}
					else
					{
						//register unsuccessful, show error in sad box
						MainMenuController.Instance.Error(response.result.error);
					}
				}
			}
			m_DoConsumeEvalNextFrame = false;
		}
	}

	private void CreateConnections()
	{
		validConnections = new Dictionary<IConnection, IModel>();
		//Start with trying main node, then go down backup list
		CreateConnection(rmqHostName);
		foreach (string hostName in fallbackNodes)
		{
			CreateConnection(hostName);
		}
		if (validConnections.Count == 0)
		{
			Debug.LogError("Failed to connect to any rmq node!");
			//If we have no more in the backup list, alert the user to try again later with an option to restart the scene
			MainMenuController.Instance.ConnectionErrorWindow();
		}
	}

	private bool CreateConnection(string hostName)
	{
		IConnection connection;
		IModel channel;
		ConnectionFactory factory = new ConnectionFactory();
		factory.HostName = hostName;
		factory.UserName = rmqUsername;
		factory.Password = rmqPassword;
		factory.RequestedConnectionTimeout = 1000;
		factory.AutomaticRecoveryEnabled = true;
		factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
		try
		{
			connection = factory.CreateConnection();
		}
		catch (Exception)
		{
			Debug.LogWarning($"Could not connect to rmq node {hostName}!");
			return false;
		}
		Debug.Log($"Connected to rmq node {hostName}!");
		channel = connection.CreateModel();

		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var ansBody = ea.Body;
			var ansMessage = Encoding.UTF8.GetString(ansBody);
			Debug.Log($" [x] Received {ansMessage}");
			MainMenuController.Instance.AbortLoginTimeout();
			MainMenuController.Instance.AbortRegisterTimeout();
			//MainMenuController.Instance.SuccessNextFrame($" [x] Received {ansMessage}");
			m_DoConsumeEvalNextFrame = true;
			m_ConsumeEvalMessage = ansMessage;
			Debug.Log(" [x] Done");
		};
		channel.BasicConsume(queue: "amq.rabbitmq.reply-to", noAck: true, consumer: consumer);
		validConnections.Add(connection, channel);
		return true;
	}


	public void Login(string username, string password)
	{
		IModel channel = GetValidChannel();
		if(channel == null)
		{
			MainMenuController.Instance.ConnectionErrorWindow();
			return;
		}
		IDictionary<string, object> headers = new Dictionary<string, object>();
		headers.Add("task", "tasks.login");
		Guid id = Guid.NewGuid();
		headers.Add("id", id.ToString());

		IBasicProperties props = channel.CreateBasicProperties();
		props.Headers = headers;
		props.CorrelationId = (string)headers["id"];
		props.ContentEncoding = "utf-8";
		props.ContentType = "application/json";
		props.ReplyTo = "amq.rabbitmq.reply-to";

		object[] taskArgs = new object[] { username, password };

		object[] arguments = new object[] { taskArgs, new object(), new object() };

		MemoryStream stream = new MemoryStream();
		DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(object[]));
		ser.WriteObject(stream, arguments);
		stream.Position = 0;
		StreamReader sr = new StreamReader(stream);
		string message = sr.ReadToEnd();

		var body = Encoding.UTF8.GetBytes(message);

		channel.BasicPublish(exchange: "",
						 routingKey: "celery",
						 basicProperties: props,
						 body: body);
		MainMenuController.Instance.StartLoginTimeoutWindow();

		Debug.Log($"Attempting sign in. UN: {username}  PW: {password}");
		//Show spinny boi
		MainMenuController.Instance.LoadingWindow("Signing in...");
	}

	private IModel GetValidChannel()
	{
		foreach (KeyValuePair<IConnection, IModel> entry in validConnections)
		{
			if (entry.Key.IsOpen && entry.Value.IsOpen)
			{
				Debug.Log($"Using {entry.Key.RemoteEndPoint.ToString()} to send request!");
				return entry.Value;
			}
		}
		Debug.LogWarning("Could not find an open connection or channel to send to!");
		return null;
	}

	public void Register(string username, string password)
	{
		IModel channel = GetValidChannel();
		if (channel == null)
		{
			MainMenuController.Instance.ConnectionErrorWindow();
			return;
		}
		IDictionary<string, object> headers = new Dictionary<string, object>();
		headers.Add("task", "tasks.register");
		Guid id = Guid.NewGuid();
		headers.Add("id", id.ToString());

		IBasicProperties props = channel.CreateBasicProperties();
		props.Headers = headers;
		props.CorrelationId = (string)headers["id"];
		props.ContentEncoding = "utf-8";
		props.ContentType = "application/json";
		props.ReplyTo = "amq.rabbitmq.reply-to";

		object[] taskArgs = new object[] { username, password };

		object[] arguments = new object[] { taskArgs, new object(), new object() };

		MemoryStream stream = new MemoryStream();
		DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(object[]));
		ser.WriteObject(stream, arguments);
		stream.Position = 0;
		StreamReader sr = new StreamReader(stream);
		string message = sr.ReadToEnd();

		var body = Encoding.UTF8.GetBytes(message);

		channel.BasicPublish(exchange: "",
						 routingKey: "celery",
						 basicProperties: props,
						 body: body);
		MainMenuController.Instance.StartRegisterTimeoutWindow();
	}

	public void Restart()
	{
		CreateConnections();
	}
}

