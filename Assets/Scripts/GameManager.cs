using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

/*! \mainpage Box'it index page
 *
 * \section intro_sec Introduction
 *
 * Welcome to the Box'it documentation!
 * To get started, look at the classes and files in the menu above!
 *
 */

/// <summary>
/// Controls overarching game logic and cross-component data flows.
/// </summary>
public class GameManager : MonoBehaviour
{
	/// <summary>
	/// The singleton instance of the GameManager
	/// </summary>
	public static GameManager Instance;
    [SerializeField]
	private Camera mainCam;
	public InputAction mouseInput;

	/// <summary>
	/// The main camera being used
	/// </summary>
	public Camera MainCam { get => mainCam; set => mainCam = value; }

	// Start is called before the first frame update
	void Start()
    {
		if (!Instance)
		{
            Instance = this;
		}
		else
		{
			Debug.LogWarning("Only one GameManager should exist! You should remove this instance of the component!");
			this.enabled = false;
			return;
		}

		TouchSimulation.Enable();

		mouseInput.performed += _ =>
		{
			Debug.Log("AAA");
			Debug.Log(_.ReadValue<Vector2>());
		};
	}
}
