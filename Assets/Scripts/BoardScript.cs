using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardScript : MonoBehaviour
{
    public static int scoreEnemy = 0;
    public static int scorePlayer = 0;

    TextMeshProUGUI popUpText;
    public static string lastPocketedObject = "";
    [SerializeField] private GameObject blackCoinPrefab;
    [SerializeField] private GameObject whiteCoinPrefab;
    public GameObject queenPrefab;
    public static bool queenPocketed = false;
    //public static  BoardScript Instance { get; private set; }
    private void Awake()
    {
        //GameManager.OnTurnEnd += ResetlastPocketedObject;
        //if (Instance == null)
        //{
        //    Instance = this;
         
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}
    }
    private void Start()
    {
        // Find the UpdatesText object and get the TextMeshProUGUI component
        popUpText = GameObject.Find("UpdatesText").GetComponent<TextMeshProUGUI>();
    }

    IEnumerator textPopUp(string text)
    {
        // Set the text and activate the UpdatesText object
        popUpText.text = text;
        popUpText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        // Deactivate the UpdatesText object after 3 seconds
        popUpText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hrer in on trigger");
        // Play audio when a coin/striker enters the pocket
        GetComponent<AudioSource>().Play();

        switch (other.gameObject.tag)
        {
            case "Striker":
                //the below condiution will not workl
                bool realplayerturn = !GameManager.Instance.playerTurn;
                lastPocketedObject = "Striker";
                //if (lastPocketedObject == "Black" || lastPocketedObject == "White" || lastPocketedObject == "Queen")
                //{

                //    ChangeTurn();

                //}
                if (realplayerturn == true)
                {
                    if (scorePlayer > 0)
                    {
                        scorePlayer--;
                        StartCoroutine(ReturnCoin(whiteCoinPrefab));
                    }// Decrement the player's score by 1
                }
                else
                {
                    if (scoreEnemy > 0)
                    {
                        scoreEnemy--; // Decrement the enemy's score by 1
                        StartCoroutine(ReturnCoin(blackCoinPrefab));
                    }
                }

                
                StartCoroutine(textPopUp("Striker Lost! -1 to " + (realplayerturn ? "Player" : "Enemy"))); // Show the pop-up text with appropriate message
                other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Set the velocity of the Striker to zero
                break;

            case "Black":
                if (lastPocketedObject == "Striker")
                {
                    ChangeTurn();
                    //ResetlastPocketedObject();
                }
                lastPocketedObject = "Black";
                scoreEnemy++; // Increment the enemy's score by 1

                StartCoroutine(textPopUp("Black Coin Entered! +1 to Enemy")); // Show the pop-up text with appropriate message
                Debug.Log("here in black");
                Destroy(other.gameObject); // Destroy the collided Black coin
               

                if(GameManager.Instance.isbot)
                {
                    GameManager.Instance.playerTurn = false;
                }
            
                break;

            case "White":
                if (lastPocketedObject == "Striker")
                {
                    ChangeTurn();
                    //ResetlastPocketedObject();
                }
                lastPocketedObject = "White";
                scorePlayer++; // Increment the player's score by 1
                StartCoroutine(textPopUp("White Coin Entered! +1 to Player")); // Show the pop-up text with appropriate message
                Debug.Log("here in white");
                Destroy(other.gameObject); // Destroy the collided White coin
                if (!GameManager.Instance.isbot)
                {
                    GameManager.Instance.playerTurn = true;
                }
                break;

            case "Queen":

               
                if (GameManager.Instance.isbot)
                {
                    GameManager.Instance.playerTurn = false;
                }
                else
                if (!GameManager.Instance.isbot)
                {
                    GameManager.Instance.playerTurn = true;
                }
                queenPocketed = true; 
                StartCoroutine(textPopUp("Queen Entered! +2 to " + (GameManager.Instance.playerTurn ? "Player" : "Enemy"))); // Show the pop-up text with appropriate message
                Debug.Log("player ");
                Destroy(other.gameObject); // Destroy the collided Queen
                break;
        }
    }
    private void ChangeTurn()
    {
        GameManager.Instance.playerTurn = !GameManager.Instance.playerTurn;
    }
    private void ResetlastPocketedObject()
    {
        lastPocketedObject = null;
    }
    public IEnumerator ReturnCoin(GameObject coinPrefab)
    {
        yield return new WaitForSeconds(1f);
        Vector3 returnPosition = new Vector3(0f, -0.57f, 0f);
        GameObject coin = null;

        // Check for colliders at the return position
        //0.1f
        Collider2D[] colliders = Physics2D.OverlapCircleAll(returnPosition, 0.3f);
        if (colliders.Length == 0)
        {
            Debug.Log("if colliders length ==0");
            // No colliders found, instantiate the coin at the return position
            coin = Instantiate(coinPrefab, returnPosition, Quaternion.identity);
        }
        else
        {
            // Colliders found, try to find a valid position nearby
            bool validPositionFound = false;
            int attempts = 0;
            while (!validPositionFound && attempts < 50)
            {
                Vector3 randomOffset = Random.insideUnitCircle * 1.5f;
                Vector3 newPosition = returnPosition + randomOffset;
                colliders = Physics2D.OverlapCircleAll(newPosition, 0.3f);
                if (colliders.Length == 0)
                {
                    Debug.Log("her in colliders length"+colliders);
                    coin = Instantiate(coinPrefab, newPosition, Quaternion.identity);
                    validPositionFound = true;
                }
                attempts++;
            }
        }

        // If no valid position found, instantiate the coin at the return position as a fallback
        if (coin == null)
        {
            Vector3 randomPosition = returnPosition + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * 2f;
            coin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        }

        // Wait for a short duration before enabling the collider of the returned coin
        Debug.Log("here in return position " + returnPosition);
        coin.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        coin.GetComponent<Collider2D>().enabled = true;
    }
 
}

