using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] float rotationSpeed_X, rotationSpeed_Y, rotationSpeed_Z;
    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed_X, rotationSpeed_Y, rotationSpeed_Z) * Time.deltaTime);
    }
}
