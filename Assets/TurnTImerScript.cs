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
        Debug.Log("here in update timer");
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
                GameManager.Instance.EndTurn();
            }
        }

    }

    void UpdateTimeText()
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time % 1f) * 100f);

        text.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);
    }
}
