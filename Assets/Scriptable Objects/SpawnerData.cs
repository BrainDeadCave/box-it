using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Data that spawners are created with
/// </summary>
[CreateAssetMenu(fileName = "New SpawnerData", menuName = "Spawner Data", order = 51)]
public class SpawnerData : ScriptableObject
{
	[SerializeField]
	private GameObject ballToSpawn;
	[SerializeField]
	private float timeBetweenSpawns;
}
