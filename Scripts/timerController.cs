using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class timerController : MonoBehaviour
{
    public static timerController ins;

    private void Awake()
    {
        ins = this;
    }

    Text timerText;
    private bool timerGoing;
    private TimeSpan timePlaying;
    public float elapsedTime;
    public float fastestLap = 0;


    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();

        timerGoing = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startTimer()
    {
        timerGoing = true;
        elapsedTime = 0;

        StartCoroutine(updateTimer());
    }

    public void stopTimer()
    {
        timerGoing = false;
        if (fastestLap == 0 || fastestLap > elapsedTime)
        {
            fastestLap = elapsedTime;
        }
        elapsedTime = 0;
        timerGoing = true;
    }


    private IEnumerator updateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string activePlayTimeStr = "Lap Time: " + timePlaying.ToString("mm':'ss'.'ff");
            timerText.text = activePlayTimeStr;
            yield return null;
        }
    }
}
