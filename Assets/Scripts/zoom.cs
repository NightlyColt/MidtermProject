using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoom : MonoBehaviour
{
    [SerializeField] public new Camera camera;
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
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, defaultZoom / zoomIn, Time.deltaTime * 4);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, defaultZoom, Time.deltaTime * 4);
        }
    }
}
