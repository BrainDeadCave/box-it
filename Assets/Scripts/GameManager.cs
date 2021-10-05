using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]
	private Camera mainCam;
	public InputAction mouseInput;

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
