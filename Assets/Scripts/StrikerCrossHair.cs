using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerCrossHair : MonoBehaviour
{
    public float rotationSpeed = 100f; // Degrees per second
    [SerializeField] StrikerController strikerController;
    [SerializeField] float collisionRadius = 0.5f; // Radius for the circle overlap
    [SerializeField] LayerMask collisionLayer; // Layer mask to filter which colliders to detect

    [SerializeField] MeshRenderer crosshairRenderer; // Reference to the Renderer of the current GameObject

    void Start()
    {
        // Get the Renderer component from the current GameObject
       
        //Color currentTint = crosshairRenderer.material.GetColor("_Tint");
        //Debug.Log("here in equal to null " + currentTint);
       
    }

    void Update()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        // Check for collisions using OverlapCircle
        Collider2D[] colliders = Physics2D.OverlapCircleAll(strikerController.transform.position, 0.5f);
       

        // Filter out the strikerController from the colliders array
        List<Collider2D> filteredColliders = new List<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != strikerController.gameObject)
            {
                filteredColliders.Add(collider);
            }
        }

        if (filteredColliders.Count > 0)
        {
            Debug.Log("the name of the colliuder "+ filteredColliders[0].name);
            // Change the material color to red
            if (crosshairRenderer != null)
            {
                crosshairRenderer.material.SetColor("_Color", Color.red);
            }
        }
        else
        {
            // Rotate the object around the Z-axis (clockwise)
         
            // Change the material color to yellow
            if (crosshairRenderer != null)
            {
                crosshairRenderer.material.SetColor("_Color", Color.yellow);
            }
        }
    }
    
}
