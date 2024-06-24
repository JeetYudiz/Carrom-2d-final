using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public bool gameOver = false;
    bool isPaused = false;

    // TextMeshProUGUI variables for displaying scores, game over text, and instructions.
  

    [SerializeField]
    TextMeshProUGUI gameOverText;

    [SerializeField]
    TextMeshProUGUI instructionsText;

    // Game object variables for menus, strikers, turn text, and a slider.
    [SerializeField]
    GameObject instructionsMenu;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject gameOverMenu;

    
    public StrikerController playerStriker;

    [SerializeField]
    GameObject enemyStriker;

    [SerializeField]
    GameObject turnText;

    
    public GameObject slider;

    [SerializeField]
    Animator animator;

    TimerScript timerScript;
    public bool turncheck;
    private const string FirstTimeLaunchKey = "FirstTimeLaunch";
    public static GameManager Instance { get; private set; }
    public bool playerTurn;
    public GameObject CurrentTurnGameObject;
    public static event Action OnTurnEnd;
    public bool previousTurnState;
    public bool isbot;
    public bool orgplayerturn;
    public string winner;
    //public TextMeshProUGUI TurnTimer;
    public TurnTimerScript turntimer;
    public bool hasTurnEnded = false;
    public bool TurnEnd;
    void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        Application.targetFrameRate = 150;
        timerScript = GetComponent<TimerScript>();
        isbot = false;
        turntimer.TimerStart();

    }
    private void OnEnable()
    {
        OnTurnEnd += SwitchPosition;
        //OnTurnEnd += DebugDisplay;
    }
    private void OnDisable()
    {
        OnTurnEnd -= SwitchPosition;
        //OnTurnEnd -= DebugDisplay;
    }
    void Start()
    {
        playerTurn = true;
        previousTurnState = playerTurn; 
        Time.timeScale = 1;
        BoardManager.Instance.scoreEnemy = 0;
        BoardManager.Instance.scorePlayer = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    /////////////////////////////////////////  
    private void SwitchPosition()
    {
        StartCoroutine(SwitchPositionCoroutine());
    }

    private IEnumerator SwitchPositionCoroutine()
    {
        // Add a delay before switching positions
        yield return new WaitForSeconds(2f); // Adjust the delay duration as needed
        //yield return null;
        if (playerTurn == true)
        {
            //turntimer.gameObject.SetActive(false);
            //can we increase

            //here we need to start the timer

            //call onturn start
            Debug.Log("here in plaayer turn");
            GameManager.Instance.turntimer.gameObject.SetActive(true);
            turntimer.TimerStart();
            isbot = false;
            slider.SetActive(true);
            turnText.SetActive(true);
            CurrentTurnGameObject = playerStriker.gameObject;

            if (playerStriker.gameObject.activeInHierarchy == false)
            {
                Debug.Log("active in hierechy false");
                playerStriker.gameObject.SetActive(true);
            }
            Debug.Log("after set active true");
            enemyStriker.gameObject.SetActive(false);
        }
        else
        {

            //here we need to start the timer
            //turntimer.TimerStart();
            GameManager.Instance.turntimer.gameObject.SetActive(false);
            Debug.Log("here in opponents turn");
            isbot = true;
         
            turnText.SetActive(false);
            Debug.Log("after turn text false");
            if (enemyStriker.gameObject.activeInHierarchy == true)
            {
                playerStriker.gameObject.SetActive(false);
            }
            if (enemyStriker.gameObject.activeInHierarchy == false)
            {
                enemyStriker.gameObject.SetActive(true);
                Debug.Log("after set active false");
            }
            CurrentTurnGameObject = enemyStriker;
        }

        //if (BoardManager.Instance.scoreEnemy >= 8 || BoardManager.Instance.scorePlayer >= 8 || CheckGameOverCondition())
        //{
        //    Debug.Log("inside boardmanager >=8");
        //    onGameOver();
        //}

        //if (!gameOver)
        //{
        //    BoardManager.Instance.ResetlastPocketedObject();
        //}
        if (CheckGameOverCondition())
        {
            Debug.Log("Game Over");
            //onGameOver
            //ticktimer setactive false
            onGameOver();
        }
        if (!gameOver)
        {
            BoardManager.Instance.ResetlastPocketedObject();
        }
    }

    /// <summary>
    /// /////////////
    /// </summary>
    /// <returns></returns>
    [Obsolete]
    private bool CheckGameOverCondition()
    {
        int whiteCoinCount = FindObjectsOfType<TagManager>().Count(t => t.tagType == Tags.White);
        int blackCoinCount = FindObjectsOfType<TagManager>().Count(t => t.tagType == Tags.Black);
        bool isQueenOnBoard = FindObjectsOfType<TagManager>().Any(t => t.tagType == Tags.Queen);

        // Check if the queen is already pocketed
        if (!isQueenOnBoard)
        {
            Debug.Log("not queen boards");
            // If any player or opponent has a score of 9, they win
            if (BoardManager.Instance.scoreEnemy == 9 || BoardManager.Instance.scorePlayer == 9)
            {
                winner = BoardManager.Instance.scoreEnemy == 9 ? "Opponent" : "Player";
                return true;
            }
        }
        else
        {

            Debug.Log("in queen boards");
            // Queen is still on the board
            if (GameManager.Instance.playerTurn)
            {
                // Player's turn
                if (whiteCoinCount == 0 && blackCoinCount >= 1)
                {
                    // Player must pocket the queen first
                    if (BoardManager.Instance.lastPocketedObject == "White")
                    {
                        Debug.Log("Player pocketed the white coin first. Opponent wins.");
                        winner = "Opponent";
                        return true;
                    }
                }
                else if (whiteCoinCount == 0 && blackCoinCount >= 0)
                {
                    // Player must pocket the queen to win
                    if (BoardManager.Instance.lastPocketedObject == "Queen")
                    {
                        winner = "Player";
                        Debug.Log("Player pocketed the queen. Player wins.");
                        return true;
                    }
                }
            }
            else
            {
                // Opponent's turn
                if (blackCoinCount == 0 && whiteCoinCount >= 1)
                {
                    // Opponent must pocket the queen first
                    if (BoardManager.Instance.lastPocketedObject == "Black")
                    {
                        Debug.Log("Opponent pocketed the black coin first. Player wins.");
                        winner = "Player";
                        return true;
                    }
                }
                else if (blackCoinCount == 0 && whiteCoinCount >= 0)
                {
                    // Opponent must pocket the queen to win
                    if (BoardManager.Instance.lastPocketedObject == "Queen")
                    {
                        winner = "Opponent";
                        Debug.Log("Opponent pocketed the queen. Opponent wins.");
                        return true;
                    }
                }
            }
        }

        return false;
    }







    /////////////////////////////////////////  
    //private bool CheckGameOverCondition()
    //{
    //    int whiteCoinCount = GameObject.FindGameObjectsWithTag("White").Length;
    //    int blackCoinCount = GameObject.FindGameObjectsWithTag("Black").Length;
    //    bool isQueenOnBoard = GameObject.FindGameObjectWithTag("Queen") != null;

    //    if (whiteCoinCount == 1 && blackCoinCount == 0 && isQueenOnBoard)
    //    {
    //        Debug.Log("here in white count 1");
    //        // Player must net the queen first
    //        return true;
    //    }
    //    else if (whiteCoinCount == 0 && blackCoinCount == 1 && isQueenOnBoard)
    //    {
    //        Debug.Log("here in black count 1");
    //        // Opponent must net the queen first
    //        return true;
    //    }

    //    return false;
    //}
    //private void LateUpdate()
    //{
    //    if (!gameOver)
    //    {
    //        scoreTextEnemy.text = BoardManager.Instance.scoreEnemy.ToString();
    //        scoreTextPlayer.text = BoardManager.Instance.scorePlayer.ToString();
    //    }
    //}
    
    IEnumerator playAnimation()
    {
        animator.SetTrigger("fade");
        yield return new WaitForSeconds(1f);
    }
    void onGameOver()
    {
        gameOver = true;
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;

        if (CheckGameOverCondition())
        {
            if (winner == "Player")
            {
                Debug.Log("Player wins!");
                gameOverText.text = "You Win!";
            }
            else if (winner == "Opponent")
            {
                Debug.Log("Opponent wins!");
                gameOverText.text = "You Lose!";
            }
        }
    }
    //void onGameOver()
    //{
    //    //here in ongameover 
    //    gameOver = true;
    //    gameOverMenu.SetActive(true);
    //    Time.timeScale = 0;
    //    ////////////////////////////



    //    if (CheckGameOverCondition())
    //    {
    //        Debug.Log("here in white pocketed");
    //        if (BoardManager.Instance.lastPocketedObject == "White")
    //        {

    //            // Player netted the white coin instead of the queen, opponent wins
    //            Debug.Log("lose text display");
    //            gameOverText.text = "You Lose!";
    //        }
    //        else if (BoardManager.Instance.lastPocketedObject == "Black")
    //        {
    //            // Opponent netted the black coin instead of the queen, player wins
    //            Debug.Log("win text display");
    //            gameOverText.text = "You Win!";
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("here in chgeck gameover conditiobn false");
    //        if (BoardManager.Instance.scoreEnemy > BoardManager.Instance.scorePlayer)
    //        {
    //            gameOverText.text = "You Lose!";
    //        }
    //        else if (BoardManager.Instance.scoreEnemy < BoardManager.Instance.scorePlayer)
    //        {
    //            gameOverText.text = "You Win!";
    //        }
    //        else
    //        {
    //            gameOverText.text = "Draw!";
    //        }
    //    }

    //    //////////////////////////
    //}

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        StartCoroutine(playAnimation());
        SceneManager.LoadScene(0);
    }

    public void NextPage()
    {
        instructionsText.pageToDisplay++;
        if (instructionsText.pageToDisplay == 3)
        {
            Time.timeScale = 1;
            timerScript.isTimerRunning = true;
            instructionsMenu.SetActive(false);
        }
    }
    public void EndTurn()
    {
        OnTurnEnd?.Invoke();
    }
}
