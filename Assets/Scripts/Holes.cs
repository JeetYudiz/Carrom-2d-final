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
    public Collider2D collider2d;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    Debug.Log("here in on trigger " + other.gameObject.name);
    //    Debug.Log("here in trigger " + gameObject.name);
    //    // Play audio when a coin/striker enters the pocket
    //    GetComponent<AudioSource>().Play();

    //    // Store the original position of the object
    //    Vector3 originalPosition = other.transform.position;

    //    // Move the object down along the z-axis to match the hole's position
    //    //Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
    //    //other.gameObject.GetComponent<Collider2D>().enabled = false;
    //    //other.transform.DOMove(targetPosition, 0.25f)
    //    //    .SetEase(Ease.InQuad)
    //    //    .OnComplete(() =>
    //    //    {
    //            // Execute the switch statement after the object has moved down
    //            switch (other.gameObject.tag)
    //            {
    //                case "Striker":
    //                    Debug.Log("STRIKER");
    //                    bool realPlayerTurn;
    //                    if (!GameManager.Instance.isbot)
    //                    {
    //                        realPlayerTurn = GameManager.Instance.orgplayerturn;
    //                    }
    //                    else
    //                    {
    //                        realPlayerTurn = GameManager.Instance.playerTurn;
    //                    }
    //                    BoardManager.Instance.LastPocketedString("Striker");
    //                    BoardManager.Instance.HandleStrikerCollision(realPlayerTurn, other);
    //                    other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    //                    break;
    //                case "Black":
    //                    Debug.Log("BLACK");
    //                    BoardManager.Instance.HandleBlackCoinCollision();
    //                    other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //                    CoinMoveAnimation(BoardManager.Instance.AI.transform.position, other.gameObject);
    //                    break;
    //                case "White":
    //                    Debug.Log("WHITE");
    //                    BoardManager.Instance.HandleWhiteCoinCollision();
    //                    other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //                    CoinMoveAnimation(BoardManager.Instance.Player.transform.position, other.gameObject);
    //                    break;
    //                case "Queen":
    //                    Debug.Log("QUEEN");
    //                    BoardManager.Instance.HandleQueenCollision();
    //                    other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //                    Destroy(other.gameObject);
    //                    break;
    //            }

    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("here in on trigger " + other.gameObject.name);
        Debug.Log("here in trigger " + gameObject.name);
        // Play audio when a coin/striker enters the pocket
        GetComponent<AudioSource>().Play();
        other.gameObject.GetComponent<Collider2D>().enabled = false;
        //gameObject.GetComponent<Collider2D>().enabled = false;

        // Get the TagManager component from the collided GameObject
        TagManager tagManager = other.gameObject.GetComponent<TagManager>();

        if (tagManager != null)
        {
            // Execute the switch statement based on the tagType of the TagManager
            switch (tagManager.tagType)
            {
                case Tags.Striker:
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
                    BoardManager.Instance.LastPocketedString(Tags.Striker.ToString());
                    BoardManager.Instance.HandleStrikerCollision(realPlayerTurn, other);
                    other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    break;
                case Tags.Black:
                    Debug.Log("BLACK");

                    BoardManager.Instance.HandleBlackCoinCollision();
                    CoinMoveAnimation(BoardManager.Instance.AI.transform.position, other.gameObject);
                    break;
                case Tags.White:
                    Debug.Log("WHITE");
                    //other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    BoardManager.Instance.HandleWhiteCoinCollision();
                    CoinMoveAnimation(BoardManager.Instance.Player.transform.position, other.gameObject);
                    break;
                case Tags.Queen:
                    Debug.Log("QUEEN");
                    BoardManager.Instance.HandleQueenCollision();
                    other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    Destroy(other.gameObject);
                    break;
            }
        }
    }
    public void CoinMoveAnimation(Vector3 playertargetPosition, GameObject coin)
    {
        Debug.Log("here in coin mover");

        Collider2D coinCollider = coin.GetComponent<Collider2D>();

        Rigidbody2D coinRigidbody = coin.GetComponent<Rigidbody2D>();
        if (coinRigidbody == null)
        {
            Debug.LogWarning("Coin does not have a Rigidbody2D component.");
            return;
        }

        // Get the initial velocity of the coin when it enters the trigger
        Vector3 initialVelocity = coinRigidbody.velocity;

        // Calculate the target position based on the initial velocity direction
        Vector3 targetPos = coin.transform.position + initialVelocity.normalized * 0.15f;
        Debug.Log("target pos " + initialVelocity.normalized * 10f);

        // Calculate the duration based on the coin's initial velocity magnitude
        float initialVelocityMagnitude = initialVelocity.magnitude;
        float moveDuration = Mathf.Clamp(1f / initialVelocityMagnitude, 0.05f, 0.5f);
        Debug.Log("after move duration " + moveDuration);
        // Move the coin to the target position with the calculated duration
        coin.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // Move the coin to the first target position
        coin.transform.DOMove(targetPos, moveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // After moving, scale down the coin to 0.1f
                coin.transform.DOScale(0.1f, 0.2f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        // Fade the color of the coin to transparent
                        SpriteRenderer spriteRenderer = coin.GetComponent<SpriteRenderer>();
                        spriteRenderer.DOFade(0.5f, 0.5f);

                        // Wait for a delay before moving the coin to the player target position
                        float delay = 0.5f; // Adjust the delay duration as needed
                        DOVirtual.DelayedCall(delay, () =>
                        {
                            // Move the coin to the player target position
                            coin.transform.DOMove(playertargetPosition, 0.5f)
                                .SetEase(Ease.InOutQuad)
                                .OnComplete(() =>
                                {
                                    // Destroy the GameObject after the animation is complete
                                    Destroy(coin.gameObject);
                                  
                                });
                        });
                    });
            });
        //gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
