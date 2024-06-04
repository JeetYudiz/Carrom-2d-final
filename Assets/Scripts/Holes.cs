using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Holes : MonoBehaviour
{
    public float moveDuration = 1f;
    public Ease moveEase = Ease.Linear;
    public float fadeDuration = 1f;
    public Ease fadeEase = Ease.Linear;
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
                Debug.Log("STRIKER");
                bool realPlayerTurn = !GameManager.Instance.playerTurn;
                BoardManager.Instance.LastPocketedString("Striker");
                BoardManager.Instance.HandleStrikerCollision(realPlayerTurn);
                other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                break;
            case "Black":
                Debug.Log("BLACK");
                BoardManager.Instance.HandleBlackCoinCollision();
                //here i have to call the coin animation
                other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                CoinMoveAnimation(BoardManager.Instance.AI.transform.position,other.gameObject);
                //Destroy(other.gameObject);
                break;
            case "White":
                Debug.Log("WHITE");
                BoardManager.Instance.HandleWhiteCoinCollision();
                other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                CoinMoveAnimation(BoardManager.Instance.Player.transform.position, other.gameObject);
                //Destroy(other.gameObject);
                break;

            case "Queen":
                Debug.Log("QUEEN");
                BoardManager.Instance.HandleQueenCollision();
                other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Destroy(other.gameObject);
                break;
        }
    }
    public void CoinMoveAnimation(Vector3 targetPosition,GameObject coin)
    {
        Debug.Log("here in coin mover");
        coin.transform.DOMove(targetPosition, moveDuration)
         .SetEase(moveEase)
         .OnComplete(() =>
         {
             // Fade the color of the GameObject's renderer
             Renderer renderer = coin.GetComponent<Renderer>();
             if (renderer != null)
             {
                 renderer.material.DOColor(Color.clear, fadeDuration)
                     .SetEase(fadeEase)
                     .OnComplete(() =>
                     {
                         // Destroy the GameObject after fading
                         Destroy(coin.gameObject);
                     });
             }
             else
             {
                 // If no renderer is found, destroy the GameObject immediately
                 Destroy(coin.gameObject);
             }
         });


    }
}
