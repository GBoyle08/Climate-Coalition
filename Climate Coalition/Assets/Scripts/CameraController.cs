using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 20f;

    public float zoomSpeed = 10f;

    public float minHeight = 10f;

    public float maxHeight = 50f;

    private void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            movement += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            movement -= transform.forward;

        if (Keyboard.current.aKey.isPressed)
            movement -= transform.right;

        if (Keyboard.current.dKey.isPressed)
            movement += transform.right;

        movement.y = 0;

        transform.position +=
            movement.normalized *
            moveSpeed *
            Time.deltaTime;

        float scroll =
            Mouse.current.scroll.ReadValue().y;

        Vector3 pos = transform.position;

        pos.y -= scroll *
                 zoomSpeed *
                 Time.deltaTime;

        pos.y = Mathf.Clamp(
            pos.y,
            minHeight,
            maxHeight
        );

        transform.position = pos;
    }
}