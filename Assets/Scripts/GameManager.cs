using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    bool gameOver = false;
    bool isPaused = false;

    // TextMeshProUGUI variables for displaying scores, game over text, and instructions.
    [SerializeField]
    TextMeshProUGUI scoreTextEnemy;

    [SerializeField]
    TextMeshProUGUI scoreTextPlayer;

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

    [SerializeField]
    GameObject playerStriker;

    [SerializeField]
    GameObject enemyStriker;

    [SerializeField]
    GameObject turnText;

    [SerializeField]
    GameObject slider;

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



        timerScript = GetComponent<TimerScript>();
        isbot = false;
        // Check if it's the first time launching the game.
        //if (PlayerPrefs.GetInt(FirstTimeLaunchKey, 0) == 0)
        //{
        //    timerScript.isTimerRunning = false;
        //    Time.timeScale = 0;
        //    instructionsMenu.SetActive(true);
        //    PlayerPrefs.SetInt(FirstTimeLaunchKey, 1);
        //}
        //else
        //{
        //    timerScript.isTimerRunning = true;
        //    instructionsMenu.SetActive(false);
        //}
        
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
    
    private void SwitchPosition()
    {
        Debug.Log("here in switch positionsa");
        //Debug.Log("here in switch positions "+GameManager.Instance.playerTurn);
        if (playerTurn == true)
        {
            
            Debug.Log("here in player turn");
            Debug.Log("true");
            isbot = false;
            slider.SetActive(true);
            turnText.SetActive(true);
            CurrentTurnGameObject = playerStriker;
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
            Debug.Log("here in opponents turn");
            isbot = true;
            slider.SetActive(false);
            turnText.SetActive(false);

            playerStriker.gameObject.SetActive(false);
         
            if (enemyStriker.gameObject.activeInHierarchy == false)
            {
                  enemyStriker.gameObject.SetActive(true);
                Debug.Log("after set active false");
            }
            CurrentTurnGameObject = enemyStriker;
        }
        //if (BoardManager.Instance.scoreEnemy >= 8 || BoardManager.Instance.scorePlayer >= 8 || timerScript.timeLeft <= 0 || CheckGameOverCondition())
          if (BoardManager.Instance.scoreEnemy >= 8 || BoardManager.Instance.scorePlayer >= 8 || CheckGameOverCondition())
        {
            onGameOver();
        }


    }

    private bool CheckGameOverCondition()
    {
        int whiteCoinCount = GameObject.FindGameObjectsWithTag("White").Length;
        int blackCoinCount = GameObject.FindGameObjectsWithTag("Black").Length;
        bool isQueenOnBoard = GameObject.FindGameObjectWithTag("Queen") != null;

        if (whiteCoinCount == 1 && blackCoinCount == 0 && isQueenOnBoard)
        {
            // Player must net the queen first
            return true;
        }
        else if (whiteCoinCount == 0 && blackCoinCount == 1 && isQueenOnBoard)
        {
            // Opponent must net the queen first
            return true;
        }

        return false;
    }
    private void LateUpdate()
    {
        if (!gameOver)
        {
            scoreTextEnemy.text = BoardManager.Instance.scoreEnemy.ToString();
            scoreTextPlayer.text = BoardManager.Instance.scorePlayer.ToString();
        }
    }

    IEnumerator playAnimation()
    {
        animator.SetTrigger("fade");
        yield return new WaitForSeconds(1f);
    }

    void onGameOver()
    {
        //here in ongameover 
        gameOver = true;
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;
        ////////////////////////////

        //if (BoardManager.Instance.scoreEnemy > BoardManager.Instance.scorePlayer)
        //{
        //    gameOverText.text = "You Lose!";
        //}
        //else if (BoardManager.Instance.scoreEnemy < BoardManager.Instance.scorePlayer)
        //{
        //    gameOverText.text = "You Win!";
        //}
        //else
        //{
        //    gameOverText.text = "Draw!";
        //}
        ///////////////////////////to get the previops condition

        if (CheckGameOverCondition())
        {
            if (BoardManager.Instance.lastPocketedObject == "White")
            {
                // Player netted the white coin instead of the queen, opponent wins
                gameOverText.text = "You Lose!";
            }
            else if (BoardManager.Instance.lastPocketedObject == "Black")
            {
                // Opponent netted the black coin instead of the queen, player wins
                gameOverText.text = "You Win!";
            }
        }
        else
        {
            if (BoardManager.Instance.scoreEnemy > BoardManager.Instance.scorePlayer)
            {
                gameOverText.text = "You Lose!";
            }
            else if (BoardManager.Instance.scoreEnemy < BoardManager.Instance.scorePlayer)
            {
                gameOverText.text = "You Win!";
            }
            else
            {
                gameOverText.text = "Draw!";
            }
        }

        //////////////////////////
    }

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
