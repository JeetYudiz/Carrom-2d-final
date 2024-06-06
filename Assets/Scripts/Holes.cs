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
        Debug.Log("here in on trigger "+other.gameObject.name);
        Debug.Log("here in trigger " + gameObject.name);
        // Play audio when a coin/striker enters the pocket
        GetComponent<AudioSource>().Play();

        // Store the original position of the object
        Vector3 originalPosition = other.transform.position;

        // Move the object down along the z-axis to match the hole's position
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
        other.gameObject.GetComponent<Collider2D>().enabled = false;
        other.transform.DOMove(targetPosition, 0.25f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
            // Execute the switch statement after the object has moved down
            switch (other.gameObject.tag)
                {
                    case "Striker":
                        Debug.Log("STRIKER");
                        bool realPlayerTurn;
                        if (!GameManager.Instance.isbot)
                        {
                            realPlayerTurn = GameManager.Instance.orgplayerturn;
                        }
                        else
                        {
                            realPlayerTurn = GameManager.Instance.playerTurn;
                        }
                        BoardManager.Instance.LastPocketedString("Striker");
                        BoardManager.Instance.HandleStrikerCollision(realPlayerTurn, other);
                        other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        break;
                    case "Black":
                        Debug.Log("BLACK");
                        BoardManager.Instance.HandleBlackCoinCollision();
                        other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        CoinMoveAnimation(BoardManager.Instance.AI.transform.position, other.gameObject);
                        break;
                    case "White":
                        Debug.Log("WHITE");
                        BoardManager.Instance.HandleWhiteCoinCollision();
                        other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        CoinMoveAnimation(BoardManager.Instance.Player.transform.position, other.gameObject);
                        break;
                    case "Queen":
                        Debug.Log("QUEEN");
                        BoardManager.Instance.HandleQueenCollision();
                        other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        Destroy(other.gameObject);
                        break;
                }
            });
    }
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    Debug.Log("hrer in on trigger");
    //    // Play audio when a coin/striker enters the pocket
    //    GetComponent<AudioSource>().Play();

    //    switch (other.gameObject.tag)
    //    {
    //        case "Striker":
    //            Debug.Log("STRIKER");
    //            //change according to enemy or player
    //            //if its is bot
    //            bool realPlayerTurn;
    //            if (!GameManager.Instance.isbot)
    //            {
    //                 realPlayerTurn = GameManager.Instance.orgplayerturn;
    //            }
    //            else
    //            {
    //                realPlayerTurn = GameManager.Instance.playerTurn;
    //            }
    //            BoardManager.Instance.LastPocketedString("Striker");
    //            BoardManager.Instance.HandleStrikerCollision(realPlayerTurn,other);

    //            //Debug.Log("STRIKER");
    //            //BoardManager.Instance.LastPocketedString("Striker");
    //            //BoardManager.Instance.HandleStrikerCollision(!GameManager.Instance.playerTurn);
    //            other.gameObject.GetComponent<SpriteRenderer>().enabled =false;
    //            break;
    //        case "Black":
    //            Debug.Log("BLACK");
    //            BoardManager.Instance.HandleBlackCoinCollision();
    //            //here i have to call the coin animation
    //            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

    //            CoinMoveAnimation(BoardManager.Instance.AI.transform.position,other.gameObject);
    //            //Destroy(other.gameObject);
    //            break;
    //        case "White":
    //            Debug.Log("WHITE");
    //            BoardManager.Instance.HandleWhiteCoinCollision();
    //            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //            CoinMoveAnimation(BoardManager.Instance.Player.transform.position, other.gameObject);
    //            //Destroy(other.gameObject);
    //            break;

    //        case "Queen":
    //            Debug.Log("QUEEN");
    //            BoardManager.Instance.HandleQueenCollision();
    //            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //            Destroy(other.gameObject);
    //            break;
    //    }
    //}
    public void CoinMoveAnimation(Vector3 targetPosition, GameObject coin)
    {
        Debug.Log("here in coin mover");

        // Disable the collider of the coin
        Collider2D coinCollider = coin.GetComponent<Collider2D>();
        SpriteRenderer coinRenderer = coin.GetComponent<SpriteRenderer>();
        if (coinCollider != null)
        {
            coinCollider.enabled = false;
        }
        if (coinRenderer != null)
        {
            Color currentColor = coinRenderer.material.color;
            currentColor.a = 0.20f; // Set alpha to 0.43 (43% opacity)
            coinRenderer.material.color = currentColor;
        }
        //// Fade the color of the GameObject's renderer
        //Renderer renderer = coin.GetComponent<Renderer>();
        //if (renderer != null)
        //{
        //    renderer.material.DOColor(Color.clear, fadeDuration)
        //        .SetEase(fadeEase);
        //}

        coin.transform.DOMove(targetPosition, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
            // Destroy the GameObject after the animation is complete
            Destroy(coin.gameObject);
            });
    }
}
