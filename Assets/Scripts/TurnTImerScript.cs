using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnTimerScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float time;
    public float reftime;

    void Start()
    {
        // Initialize the text component if needed
        text = GetComponent<TextMeshProUGUI>();
    }

    public void TimerStart()
    {
        time = reftime;
        Debug.Log("here");
        UpdateTimeText();
        StartCoroutine(UpdateTimer());
    }

    IEnumerator UpdateTimer()
    {
        while (time > 0f )
        {
            yield return null; // Wait for the next frame
            time -= Time.deltaTime; // Subtract the time passed since the last frame
            if (time > 0f)
            {
                UpdateTimeText();
            }
            else
            {
                time = 0f;
                UpdateTimeText();
                //keep a boolen
                if (!GameManager.Instance.hasTurnEnded) // Check the flag before ending the turn
                {
                    GameManager.Instance.hasTurnEnded = true; // Set the flag to true
                    GameManager.Instance.playerTurn = !GameManager.Instance.playerTurn;
                    //GameManager.Instance.DisableStrikerComponents()
                    //game manager slider set active false
                    GameManager.Instance.playerStriker.gameObject.SetActive(false);
                    GameManager.Instance.slider.SetActive(false);
                    GameManager.Instance.EndTurn(); 
                }
                //GameManager.Instance.EndTurn();
            }
        }

    }

    void UpdateTimeText()
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time % 1f) * 100f);
        text.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);
    }
    public void RestartTimer()
    {
        StopCoroutine(UpdateTimer()); // Stop the coroutine if it's running
        TimerStart(); // Restart the timer
    }
    private void OnDisable()
    {
        Debug.Log("here in on disable");
        GameManager.Instance.hasTurnEnded = false;
    }
}
