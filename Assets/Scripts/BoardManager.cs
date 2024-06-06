using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public  int scoreEnemy = 0;
    public  int scorePlayer = 0;

    TextMeshProUGUI popUpText;
    public string lastPocketedObject = "";
    [SerializeField] private GameObject blackCoinPrefab;
    [SerializeField] private GameObject whiteCoinPrefab;
    public GameObject queenPrefab;
    public  bool queenPocketed = false;
    public static BoardManager Instance { get; private set; }
    public float CoinSize;
    public Vector3 targetPosition;
   
    public GameObject Player;
    public GameObject AI;
    public GameObject queenplayerui;
    public GameObject queenaiui;

    [SerializeField]
    TextMeshProUGUI scoreTextEnemy;

    [SerializeField]
    TextMeshProUGUI scoreTextPlayer;
    //public RefStriker refStriker;
    private void Awake()
    {
        CoinSize = blackCoinPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        //GameManager.OnTurnEnd += ResetlastPocketedObject;
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
            return;
        }
      
    }
  
   
    private void Start()
    {
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

  
    public void ChangeTurn()
    {
        
        GameManager.Instance.playerTurn = !GameManager.Instance.playerTurn;
    }
    public void LastPocketedString(string lastpocketed)
    {
        lastPocketedObject = lastpocketed;
    }
    public void ResetlastPocketedObject()
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
                    Debug.Log("her in colliders length" + colliders);
                    coin = Instantiate(coinPrefab, newPosition, Quaternion.identity);
                    validPositionFound = true;
                }
                attempts++;
            }
        }
        ///////////////////////////////////////////////////////////
       

        // Determine the starting position based on the turn
        Vector3 startPosition = !GameManager.Instance.playerTurn ? BoardManager.Instance.Player.transform.position : BoardManager.Instance.AI.transform.position;
        Vector3 randomPosition=Vector3.zero;
        if (coin == null)
        {
            randomPosition = returnPosition + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * 2f;
            coin = Instantiate(coinPrefab, startPosition, Quaternion.identity);
        }
        // Move the coin from the starting position to the random position
        coin.transform.position = startPosition;
        coin.GetComponent<Collider2D>().enabled = false;


        SpriteRenderer coinRenderer = coin.GetComponent<SpriteRenderer>();

        // Set the alpha of the coin to 0
        if (coinRenderer != null)
        {
            Color coinColor = coinRenderer.color;
            coinColor.a = 0.3f;
            coinRenderer.color = coinColor;
        }

        // Move the coin to the random position using DOTween
        coin.transform.DOMove(randomPosition, 0.25f)
      .SetEase(Ease.InOutQuad)
      .OnComplete(() =>
      {
        // Enable the coin's collider
        coin.GetComponent<Collider2D>().enabled = true;

        // Change the alpha of the coin back to 1
        if (coinRenderer != null)
          {
              Color coinColor = coinRenderer.color;
              coinColor.a = 1f;
              coinRenderer.color = coinColor;
          }
      });
        yield return new WaitForSeconds(1f);

        // If no valid position found, instantiate the coin at the return position as a fallback
        //if (coin == null)
        //{
        //    Vector3 randomPosition = returnPosition + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * 2f;
        //    coin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        //    //here the cpin shpudl gop from p[layer to randpm, position and tge same as ebnemt
        //}

        //// Wait for a short duration before enabling the collider of the returned coin
        //Debug.Log("here in return position " + returnPosition);
        //coin.GetComponent<Collider2D>().enabled = false;
        //yield return new WaitForSeconds(0.5f);
        //coin.GetComponent<Collider2D>().enabled = true;
    }
  
    public void HandleStrikerCollision(bool realPlayerTurn,Collider2D coin)
    {
        Debug.Log("real player turn" + realPlayerTurn);
        Debug.Log("score enemy " + scoreEnemy);
        if (realPlayerTurn)
        {
            Debug.Log("here in realplayer turn true");
            if (scorePlayer > 0)
            {
                Debug.Log("here in white coin retiurn");
                ScorePlayerAddition(--scorePlayer);
                StartCoroutine(ReturnCoin(whiteCoinPrefab));
            }
        }
        else
        {
            Debug.Log("here in realplayer turn false");
            if (scoreEnemy > 0)
            {
                Debug.Log("here in black coin retiurn");
                ScoreEnemyAddition(--scoreEnemy);
                StartCoroutine(ReturnCoin(blackCoinPrefab));
            }
        }
        Debug.Log("here in player turn " + GameManager.Instance.playerTurn);
        StartCoroutine(textPopUp("Striker Lost! -1 to " + (realPlayerTurn ? "Player" : "Enemy")));
        coin.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        coin.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        coin.gameObject.GetComponent<SpriteRenderer>().enabled = false;

    }
    public void HandleBlackCoinCollision()
    {
        ScoreEnemyAddition(++scoreEnemy);
        Debug.Log("here in white coin");
        if (lastPocketedObject == "Striker")
        {
            ChangeTurn();
            //ResetlastPocketedObject();
        }
        lastPocketedObject = "Black";
   
        StartCoroutine(textPopUp("Black Coin Entered! +1 to Enemy"));
        Debug.Log("here in black");

        if (GameManager.Instance.isbot)
        {
            GameManager.Instance.playerTurn = false;
        }
    }
    public void HandleWhiteCoinCollision()
    {
        ScorePlayerAddition(++scorePlayer);
        if (lastPocketedObject == "Striker")
        {
            ChangeTurn();
            //ResetlastPocketedObject();
        }
        lastPocketedObject = "White";
        StartCoroutine(textPopUp("White Coin Entered! +1 to Player"));
        Debug.Log("here in white");

        if (!GameManager.Instance.isbot)
        {
            GameManager.Instance.playerTurn = true;
        }
    }
    public void HandleQueenCollision()
    {
        if (GameManager.Instance.isbot)
        {
            GameManager.Instance.playerTurn = false;
        }
        else if (!GameManager.Instance.isbot)
        {
            GameManager.Instance.playerTurn = true;
        }
        queenPocketed = true;
        lastPocketedObject = "Queen";
        StartCoroutine(textPopUp("Queen Entered! Hit Cover " + (GameManager.Instance.playerTurn ? "Player" : "Enemy")));
        Debug.Log("player ");
    }
    public IEnumerator CheckQueenPocket()
    {
        if (queenPocketed && lastPocketedObject!="Queen")
        {
            if (GameManager.Instance.playerTurn && lastPocketedObject == "White")
            {
                // Player pocketed the white coin after the queen, activate queenPlayerUI
                queenplayerui.SetActive(true);
            }
            else if (!GameManager.Instance.playerTurn && lastPocketedObject == "Black")
            {
                // Opponent pocketed the black coin after the queen, activate queenAIUI
                queenaiui.SetActive(true);
            }
            else if (GameManager.Instance.playerTurn && lastPocketedObject != "White")
            {
                // Player did not pocket the white coin after the queen, return the queen
                yield return StartCoroutine(ReturnCoin(queenPrefab));
            }
            else if (!GameManager.Instance.playerTurn && lastPocketedObject != "Black")
            {
                // Opponent did not pocket the black coin after the queen, return the queen
                yield return StartCoroutine(ReturnCoin(queenPrefab));
            }

            queenPocketed = false; // Reset the flag
        }
    }

    public void ScorePlayerAddition(float scoreplayer)
    {
        if (!GameManager.Instance.gameOver)
        {
            Debug.Log("the scpre player"+scorePlayer);
            scoreTextPlayer.text = scoreplayer.ToString();
        }
    }
    public void ScoreEnemyAddition(float scoreenemy)
    {
        if (!GameManager.Instance.gameOver)
        {
            Debug.Log("the scpre enemy" + scoreenemy);
            scoreTextEnemy.text = scoreenemy.ToString();
        }
    }


}
