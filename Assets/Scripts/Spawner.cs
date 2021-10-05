using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballToSpawn;

    [SerializeField]
    private float time;
    private float currtime = 0;

	private void Start()
	{
         GetComponent<Image>().color = ballToSpawn.GetComponent<Ball>().BallData.Color;
	}

	// Update is called once per frame
	void Update()
    {
        AddTime(Time.deltaTime);
    }

    void AddTime(float delta)
	{
        currtime = currtime + delta;
        if(currtime >= time)
		{
            currtime = currtime % time;
            SpawnBall();
		}
        GetComponent<Image>().fillAmount = currtime/time;
    }

    public void SpawnBall()
	{
		if (!ballToSpawn){return;}
        Vector3 spawnPos = transform.position;
        spawnPos.z = 0;
        //Debug.LogFormat("Trying to spawn ball at {0} with spawner", spawnPos);
        Instantiate(ballToSpawn, spawnPos, Quaternion.identity);
    }
}
