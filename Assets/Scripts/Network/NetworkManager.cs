using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using RabbitMQ.Client.Events;

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
	private string rmqVirtualHost;

	private IModel channel;
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
		ConnectionFactory factory = new ConnectionFactory();
		factory.HostName = rmqHostName;
		factory.UserName = rmqUsername;
		factory.Password = rmqPassword;
		factory.VirtualHost = rmqVirtualHost;
		IConnection connection = factory.CreateConnection();
		channel = connection.CreateModel();

		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var ansBody = ea.Body;
			var ansMessage = Encoding.UTF8.GetString(ansBody);
			Debug.Log($" [x] Received {ansMessage}");
			Debug.Log(" [x] Done");
			MainMenuController.Instance.AbortLoginTimeout();
			Debug.Log(" [x] Here");
			MainMenuController.Instance.SuccessNextFrame($" [x] Received {ansMessage}");
		};
		channel.BasicConsume(queue: "amq.rabbitmq.reply-to", noAck: true, consumer: consumer);
	}


	public void Login(string username, string password)
	{
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
	}

	public void Register(string username, string password)
	{
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
}
