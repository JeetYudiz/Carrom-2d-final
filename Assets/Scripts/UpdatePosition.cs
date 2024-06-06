using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour
{
    [SerializeField] Transform followPosition;
    [SerializeField] GameObject LinePivotRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followPosition.position;
        transform.rotation = LinePivotRotation.transform.rotation;
    }
}
