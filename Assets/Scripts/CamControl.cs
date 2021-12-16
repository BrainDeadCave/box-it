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
    public void OnMouseDrag(InputAction.CallbackContext context)
    {
        Vector3 newPos = transform.position;
        newPos.y += context.ReadValue<Vector2>().y;
        transform.position = newPos;
    }
}
