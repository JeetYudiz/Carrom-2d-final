using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairMove : MonoBehaviour
{
    public float rotationSpeed = 100f; // Degrees per second

    void Update()
    {
        // Rotate the object around the Y-axis
        transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
    }
}
