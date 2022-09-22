using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillitate : MonoBehaviour
{
    Vector3 startingPosition;
    [SerializeField] Vector3 movementVector;
    [Range(0, 1)] [SerializeField] float movementFactor;
    [SerializeField] float period = 2f;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
        {
            return; // check that the period is not zero, if the period is less than the absolute lowest value of a float, then make the object stantionary 
        }
        float cycles = Time.time / period; // count the cycles over a period of time

        const float tau = Mathf.PI * 2; // constant value of 6.283 or whole circle
        float rawSinWave = Mathf.Sin(cycles * tau); // going from -1 to 1, takes a radiant as a parameter

        movementFactor = (rawSinWave + 1f) / 2; // recalculated to be from 0 to 1

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}

