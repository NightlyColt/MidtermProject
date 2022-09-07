using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    [Range(0, 350)][SerializeField] int sensVert;
    [Range(0, 350)][SerializeField] int sensHori;

    [Range(0, -75)][SerializeField] int lockVertMin;
    [Range(0, 75)][SerializeField] int lockVertMax;

    [SerializeField] bool invert;

    float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    // Camera movement should be in LateUpdate() to avoid jerky movement
    void LateUpdate()
    {
        // get the input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHori;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;


        if (invert)
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        // clamp rotation
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);


        // rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate the playeer
        transform.parent.Rotate(Vector3.up * mouseX);

    }
}