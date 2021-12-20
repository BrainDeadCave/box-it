using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the camera movement using the scroll wheel or dragging
/// </summary>
public class CamControl : MonoBehaviour
{
    /// <summary>
    /// When moving the mouse, drag the camera around 
    /// </summary>
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float upperBound;
    private Vector3 upperBoundPos;
    [SerializeField]
    private float lowerBound;
    private Vector3 lowerBoundPos;

    private Camera mainCamera;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private float oldY;

	private void Awake()
	{
        mainCamera = Camera.main;
        upperBoundPos = mainCamera.transform.position;
        upperBoundPos.y = upperBound;
        lowerBoundPos = mainCamera.transform.position;
        lowerBoundPos.y = lowerBound;
    }

	private void OnEnable()
	{
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }
    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }
    private void MousePressed(InputAction.CallbackContext context)
	{
        oldY = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y;
        StartCoroutine(DragUpdate());
    }

	private IEnumerator DragUpdate()
	{
        while(mouseClick.ReadValue<float>() != 0)
		{
            float newY = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y;
            float difference = newY - oldY;
            Vector3 camPos = mainCamera.transform.position;
            camPos.y -= difference;
            if(camPos.y > upperBound)
			{
                mainCamera.transform.position = upperBoundPos;
            }
            else if (camPos.y < lowerBound)
            {
                mainCamera.transform.position = lowerBoundPos;
            }
			else
			{
                mainCamera.transform.position = camPos;
            }
            oldY = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y;
            yield return waitForFixedUpdate;
        }
    }
}
