using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoom : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] float zoomIn;
    float defaultZoom;
    void Start()
    {
        defaultZoom = camera.fieldOfView;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            camera.fieldOfView = (defaultZoom / zoomIn);
        }
        else
        {
            camera.fieldOfView = defaultZoom;
        }
    }
}
