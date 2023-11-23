using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float timeLeft;
    public bool timerOn = false;

    public Text timerText;

    float minutes;
    float seconds;

    // Start is called before the first frame update
    void Start()
    {
        timerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                // timerText.text = timerLeft.ToString("0.00");
                UpdateTimer(timeLeft);
            }
            else
            {
                Debug.Log("time's up");
                timeLeft = 0;
                // timerText.text = timeLeft.ToString("0.00");
                timerOn = false;
            }
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
