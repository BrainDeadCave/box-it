using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugActions : MonoBehaviour
{
    [SerializeField]
    private GameObject BallPrefab;
    [SerializeField]
    private Camera mainCam;
    public InputAction mouseInput;
    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

            //Debug.Log(Mouse.current.position.ReadValue());
            //Instantiate(BallPrefab, mainCam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0)), Quaternion.identity);

        }
    }
    private void OnEnable()
    {
        mouseInput.Enable();
    }

    private void OnDisable()
    {
        mouseInput.Disable();
    }
    private void Awake()
    {
        mouseInput.performed += _ =>
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 spawnPos = mainCam.ScreenToWorldPoint(mousePos);
            spawnPos.z = 0;
            Debug.LogFormat("Trying to spawn ball at {0}", spawnPos);
            Instantiate(BallPrefab, spawnPos, Quaternion.identity);
        };
    }
}
