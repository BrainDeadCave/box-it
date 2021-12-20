using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
/// <summary>
/// Controlls specific actions on the main menu, mostly feedback panels (error, loading, success, etc.)
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private float boxesPerSecond = 1f;
    [SerializeField]
    private GameObject boxPrefabToSpawn;
    [SerializeField]
    private float spawnOffsetY = 0f;

    [SerializeField]
    private float loginTimeoutTime = 10f;
    private float m_RemainingLoginTimeoutTime;
    private bool m_InLoginTimeoutWindow;
    [SerializeField]
    private float registerTimeoutTime = 10f;
    private float m_RemainingRegisterTimeoutTime;
    private bool m_InRegisterTimeoutWindow;

    [SerializeField]
    private GameObject errorPanel;
    [SerializeField]
    private GameObject successPanel;
    [SerializeField]
    private GameObject registerSuccessPanel;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private GameObject connectErrorPanel;

    private Vector3 spawnAreaLeft;
    private Vector3 spawnAreaRight;

    private float m_TimeUntilNextBox = 0f;
    private Camera m_MainCamera;

    private bool m_DoSuccessNextFrame = false;
    private string m_NextSuccessMessage = "Init";

    [SerializeField]
    private GameObject networkManagerPrefab;

    /// <summary>
    /// The singleton instance of MainMenuController
    /// </summary>
    public static MainMenuController Instance;

	private void Start()
	{
		if (!Instance)
		{
            Instance = this;
		}
		else
		{
            Debug.LogWarning($"Disabling MainMenuController {name} because {Instance.name} is the existing singleton MainMenuController!!");
            enabled = false;
            return;
		}
        spawnAreaLeft = new Vector3(0, Screen.height + 1.5f, 0);
        spawnAreaRight = new Vector3(Screen.width, Screen.height + 1.5f, 0);
        m_MainCamera = Camera.main;
        m_RemainingLoginTimeoutTime = loginTimeoutTime;
        m_InLoginTimeoutWindow = false;
		if (!NetworkManager.Instance)
		{
            Instantiate(networkManagerPrefab, null);
		}
		else
		{
            NetworkManager.Instance.Restart();
        }
    }
	// Update is called once per frame
	void Update()
    {
		if (m_TimeUntilNextBox <= 0.0f)
		{
            // Spawn Box
            float spawnRotation = Random.Range(0.0f, 180.0f);
            Vector3 spawnPosition = m_MainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(Screen.height, Screen.height), m_MainCamera.farClipPlane));
            spawnPosition.y += spawnOffsetY;

            Instantiate(boxPrefabToSpawn, spawnPosition, Quaternion.AngleAxis(spawnRotation, Vector3.forward));
            m_TimeUntilNextBox = 1/boxesPerSecond;
		}
		else
		{
            m_TimeUntilNextBox -= Time.deltaTime;
		}

        if(m_InLoginTimeoutWindow && m_RemainingLoginTimeoutTime <= 0f)
		{
            CloseLoadingWindow();
            Error("Login timed out");
            m_InLoginTimeoutWindow = false;
        }
		else if(m_InLoginTimeoutWindow)
		{
            m_RemainingLoginTimeoutTime -= Time.deltaTime;
        }
        else if (m_InRegisterTimeoutWindow && m_RemainingRegisterTimeoutTime <= 0f)
        {
            CloseLoadingWindow();
            Error("Registration timed out");
            m_InRegisterTimeoutWindow = false;
        }
        else if (m_InRegisterTimeoutWindow)
        {
            m_RemainingRegisterTimeoutTime -= Time.deltaTime;
        }

		if (m_DoSuccessNextFrame)
		{
            Success(m_NextSuccessMessage);
            m_DoSuccessNextFrame = false;
		}
    }

    /// <summary>
    /// Show the error panel with an error message
    /// </summary>
    public void Error(string message)
	{
        errorPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        errorPanel.SetActive(true);
        CloseLoadingWindow();
    }
    /// <summary>
    /// Show the loading panel with an error message
    /// </summary>
    public void LoadingWindow(string message)
	{
        if (!loadingPanel.activeSelf)
        {
            loadingPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            loadingPanel.SetActive(true);
        }
		else
		{
            Debug.LogWarning($"Tried to open loading window for message \"{message}\", but it is already open for message \"{loadingPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text}\"!");
		}
    }
    /// <summary>
    /// Close the loading window
    /// </summary>
    public void CloseLoadingWindow()
	{
		if (!loadingPanel.activeSelf)
		{
            return;
		}
        ClickToClosePanel click = loadingPanel.GetComponent<ClickToClosePanel>();
		if (click)
		{
            click.ClosePanel();
		}
		else
		{
            loadingPanel.SetActive(false);
		}
    }
    /// <summary>
    /// Start timeout period for loging in
    /// </summary>
    public void StartLoginTimeoutWindow()
	{
        m_InLoginTimeoutWindow = true;
        m_RemainingLoginTimeoutTime = loginTimeoutTime;
    }
    /// <summary>
    /// End timeout period for loging in
    /// </summary>
    public void AbortLoginTimeout()
    {
        m_InLoginTimeoutWindow = false;
    }
    /// <summary>
    /// Start timeout period for registering
    /// </summary>
    public void StartRegisterTimeoutWindow()
	{
        m_InRegisterTimeoutWindow = true;
        m_RemainingRegisterTimeoutTime = registerTimeoutTime;
    }
    /// <summary>
    /// Stop timeout period for registering
    /// </summary>
    public void AbortRegisterTimeout()
    {
        m_InRegisterTimeoutWindow = false;
    }
    /// <summary>
    /// Show the success panel with a message
    /// </summary>
    public void Success(string message)
	{
        if (!successPanel.activeSelf)
        {
            successPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            CloseLoadingWindow();
            successPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Tried to open success window for message \"{message}\", but it is already open for message \"{successPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text}\"!");
        }
    }
    /// <summary>
    /// Show the success panel next frame (used for safe threading)
    /// </summary>
    public void SuccessNextFrame(string message)
    {
        m_DoSuccessNextFrame = true;
        m_NextSuccessMessage = message;
    }
    /// <summary>
    /// Show the Register complete panel
    /// </summary>
    public void RegisterCompleteWindow()
    {
        if (!registerSuccessPanel.activeSelf)
        {
            registerSuccessPanel.SetActive(true);
            CloseLoadingWindow();
        }
        else
        {
            Debug.LogWarning("Tried to open register success window, but it is already open!");
        }
    }
    /// <summary>
    /// Close the registration complete window
    /// </summary>
    public void CloseRegisterCompleteWindow()
    {
        if (!registerSuccessPanel.activeSelf)
        {
            return;
        }
        ClickToClosePanel click = registerSuccessPanel.GetComponent<ClickToClosePanel>();
        if (click)
        {
            click.ClosePanel();
        }
        else
        {
            registerSuccessPanel.SetActive(false);
        }
    }
    /// <summary>
    /// Show the connection error panel
    /// </summary>
    public void ConnectionErrorWindow()
	{
        if (!connectErrorPanel.activeSelf)
        {
            connectErrorPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Tried to open connection error window, but it is already open! How did I mess this up THIS much ;w;");
        }
    }
    /// <summary>
    /// Reset the main menu scene
    /// </summary>
    public void ResetScene()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

    /// <summary>
    /// Goes to game scene
    /// </summary>
    public void ToGame()
    {
        SceneManager.LoadScene(1);
    }
}
