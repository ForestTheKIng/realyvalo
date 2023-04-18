using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public float _progress;
    public float speed;
    public Vector2 _mousePos;
    public Vector2 _worldPos;
    void Awake(){
        speed = 0.0001f;
    }

    // Update is called once per frame
    void Update()
    {
        _mousePos = Input.mousePosition;
        _worldPos = Camera.main.ScreenToWorldPoint(_mousePos);
        _progress += speed;
        transform.position = Vector2.Lerp(this.transform.position, _worldPos, _progress);
        Debug.Log(Input.mousePosition);
    }
}
