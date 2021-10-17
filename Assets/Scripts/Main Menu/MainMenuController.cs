using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    private GameObject loadingPanel;

    private Vector3 spawnAreaLeft;
    private Vector3 spawnAreaRight;

    private float m_TimeUntilNextBox = 0f;
    private Camera m_MainCamera;

    private bool m_DoSuccessNextFrame = false;
    private string m_NextSuccessMessage = "Init";

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
    }
	// Update is called once per frame
	void Update()
    {
		if (m_TimeUntilNextBox <= 0.0f)
		{
            // Spawn Box
            float spawnRotation = Random.Range(0.0f, 180.0f);
            Vector3 spawnPosition = m_MainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(Screen.height, Screen.height), m_MainCamera.nearClipPlane));
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

    public void Error(string message)
	{
        errorPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        errorPanel.SetActive(true);
	}

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

    public void StartLoginTimeoutWindow()
	{
        m_InLoginTimeoutWindow = true;
        m_RemainingLoginTimeoutTime = loginTimeoutTime;
    }
    public void AbortLoginTimeout()
    {
        m_InLoginTimeoutWindow = false;
    }

    public void StartRegisterTimeoutWindow()
	{
        m_InRegisterTimeoutWindow = true;
        m_RemainingRegisterTimeoutTime = registerTimeoutTime;
    }
    public void AbortRegisterTimeout()
    {
        m_InRegisterTimeoutWindow = false;
    }

    public void Success(string message)
	{
        Debug.Log("what");
        Debug.Log(successPanel.activeSelf);
        if (!successPanel.activeSelf)
        {
            Debug.Log("what2");
            successPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            CloseLoadingWindow();
            successPanel.SetActive(true);
        }
        else
        {
            Debug.Log("what3");
            Debug.LogWarning($"Tried to open success window for message \"{message}\", but it is already open for message \"{successPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text}\"!");
        }
    }

    public void SuccessNextFrame(string message)
    {
        m_DoSuccessNextFrame = true;
        m_NextSuccessMessage = message;
    }
}
