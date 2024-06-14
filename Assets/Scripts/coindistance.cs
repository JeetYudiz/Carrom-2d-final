using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coindistance : MonoBehaviour
{
    public string holeTag = "Pocket"; // Tag used to identify holes

    private GameObject nearestHole;
    [SerializeField] float nearestDistance;
    [SerializeField] GameObject striker;
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
        // Initializing nearestDistance to a high value
        nearestDistance = Mathf.Infinity;
        nearestHole = null;
    }

    // Update is called once per frame
    void Update()
    {
        ////FindNearestHole();
        if (striker != null)
        {
            angle = CalculateAngle(transform.position, striker.transform.position);
        }
    }

    void FindNearestHole()
    {
        //// Get all the holes in the scene
        //GameObject[] holes = GameObject.FindGameObjectsWithTag(holeTag);

        //nearestDistance = Mathf.Infinity;
        //nearestHole = null;

        //// Iterate through each hole to find the nearest one
        //foreach (GameObject hole in holes)
        //{
        //    float distance = Vector3.Distance(transform.position, hole.transform.position);

        //    if (distance < nearestDistance)
        //    {
        //        nearestDistance = distance;
        //        nearestHole = hole;
        //    }
        //}

        //// Optionally, display the nearest hole and its distance in the console
        //if (nearestHole != null)
        //{
        //    Debug.Log("Nearest hole: " + nearestHole.name + ", Distance: " + nearestDistance);
        //}
        //else
        //{
        //    Debug.Log("No holes found.");
        //}
    }

    float CalculateAngle(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = (toPosition - fromPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
    //public GameObject GetNearestHole()
    //{
    //    return nearestHole;
    //}

    //public float GetNearestDistance()
    //{
    //    return nearestDistance;
    //}
}
