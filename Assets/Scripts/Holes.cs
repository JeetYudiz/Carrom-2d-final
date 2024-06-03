using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hrer in on trigger");
        // Play audio when a coin/striker enters the pocket
        GetComponent<AudioSource>().Play();

        switch (other.gameObject.tag)
        {
            case "Striker":
                bool realPlayerTurn = !GameManager.Instance.playerTurn;
                BoardManager.Instance.LastPocketedString("Striker");
                BoardManager.Instance.HandleStrikerCollision(realPlayerTurn);
                other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                break;
            case "Black":
                BoardManager.Instance.HandleBlackCoinCollision();
                Destroy(other.gameObject);
                break;
            case "White":
                BoardManager.Instance.HandleWhiteCoinCollision();
                Destroy(other.gameObject);
                break;

            case "Queen":
                BoardManager.Instance.HandleQueenCollision();
                Destroy(other.gameObject);
                break;
        }
    }
}
