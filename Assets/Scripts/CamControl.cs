using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamControl : MonoBehaviour
{
    public void OnMouseDrag(InputAction.CallbackContext context)
    {
        Vector3 newPos = transform.position;
        newPos.y += context.ReadValue<Vector2>().y;
        transform.position = newPos;
    }
}
