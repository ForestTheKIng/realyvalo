using System;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public Camera cam;
    private Vector3 _dragOrigin;

    [SerializeField] private float zoomStep, minCamSize, maxCamSize;
    
    private void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            var difference = _dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            
        }
    }


    public void ZoomIn()
    {
        var newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    }

    public void ZoomOut()
    {
        var newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    }
}