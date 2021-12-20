using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns boxes. Variable spawn rates and box values.
/// </summary>
public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballToSpawn;

    [SerializeField]
    private float time;
    private float currtime = 0;

    private int valueIncrease;

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
    /// <summary>
    /// Spawn a ball at this spawner's location
    /// </summary>
    public void SpawnBall()
	{
		if (!ballToSpawn){return;}
        Vector3 spawnPos = transform.position;
        spawnPos.z = 0;
        //Debug.LogFormat("Trying to spawn ball at {0} with spawner", spawnPos);
        GameObject newBall = Instantiate(ballToSpawn, spawnPos, Quaternion.identity);
        newBall.GetComponent<Ball>().AddValue(valueIncrease);
    }

    public void SetValueIncrease(int newIncrease)
	{
        valueIncrease = newIncrease;
	}

    public void SetSpawnTime(float newTime)
	{
        time = newTime;

    }
}
