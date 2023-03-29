using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 _origin;
    private Vector3 _difference;
    private Vector3 _resetCamera;
    public Camera cam;

    private bool _drag = false;

    

    private void Start()
    {
        _resetCamera = cam.transform.position;
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            _difference = (cam.ScreenToWorldPoint(Input.mousePosition)) - cam.transform.position;
            if(_drag == false)
            {
                _drag = true;
                _origin = cam.ScreenToWorldPoint(Input.mousePosition);
            }

        }
        else
        {
            _drag = false;
        }

        if (_drag)
        {
            cam.transform.position = _origin - _difference * 0.5f;
        }

        if (Input.GetMouseButton(1))
            cam.transform.position = _resetCamera;

    }
}