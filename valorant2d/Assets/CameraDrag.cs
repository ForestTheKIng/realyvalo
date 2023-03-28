using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private float dragSpeed = 10;
    private Vector3 dragOrigin;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = this.GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = mainCamera.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
            Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

            transform.Translate(move, Space.Self);
            dragOrigin = Input.mousePosition;
        }
    }
}
