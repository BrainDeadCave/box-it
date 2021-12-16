using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Main Menu ball deleter, deletes the falling boxes on screen when they leave the screen
/// </summary>
public class BallDeleter_MM : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Destroy(collision.gameObject);
	}
}
