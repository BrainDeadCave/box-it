using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private float boxesPerSecond = 1f;
    [SerializeField]
    private GameObject boxPrefabToSpawn;
    [SerializeField]
    private float spawnOffsetY = 0f;

    private Vector3 spawnAreaLeft;
    private Vector3 spawnAreaRight;

    private float m_TimeUntilNextBox = 0f;
    private Camera m_MainCamera;

	private void Start()
	{
        spawnAreaLeft = new Vector3(0, Screen.height + 1.5f, 0);
        spawnAreaRight = new Vector3(Screen.width, Screen.height + 1.5f, 0);
        m_MainCamera = Camera.main;
    }
	// Update is called once per frame
	void Update()
    {
		if (m_TimeUntilNextBox <= 0.0f)
		{
            // Spawn Box
            float spawnRotation = Random.Range(0.0f, 180.0f);
            Vector3 spawnPosition = m_MainCamera.ScreenToWorldPoint(new Vector3(Random.Range(spawnAreaLeft.x, spawnAreaRight.x), Random.Range(spawnAreaLeft.y, spawnAreaRight.y), m_MainCamera.nearClipPlane));
            spawnPosition.y += spawnOffsetY;

            Instantiate(boxPrefabToSpawn, spawnPosition, Quaternion.AngleAxis(spawnRotation, Vector3.forward));
            m_TimeUntilNextBox = 1/boxesPerSecond;
		}
		else
		{
            m_TimeUntilNextBox -= Time.deltaTime;
		}
    }
}
