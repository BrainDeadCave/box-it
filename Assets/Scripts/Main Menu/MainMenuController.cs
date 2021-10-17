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
    private GameObject errorPanel;
    private Animator errorPanelAnimator;
    [SerializeField]
    private GameObject loadingPanel;
    private Animator loadingPanelAnimator;

    private Vector3 spawnAreaLeft;
    private Vector3 spawnAreaRight;

    private float m_TimeUntilNextBox = 0f;
    private Camera m_MainCamera;

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
        errorPanelAnimator = errorPanel.GetComponent<Animator>();
        loadingPanelAnimator = loadingPanel.GetComponent<Animator>();
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
}
