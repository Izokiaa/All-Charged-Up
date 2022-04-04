using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager ins;
    private void Awake()
    {
        ins = this;
    }
    public int totalScore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalcFinalScore()
    {
        if (timerController.ins.fastestLap != 0)
            totalScore = Mathf.RoundToInt(1000 + ((1 + GameManager.ins.laps) * (10000 / (timerController.ins.fastestLap)) * (1 + GameManager.ins.totalPickUps) / 5));
        else
            totalScore = Mathf.RoundToInt(1000 + ((1 + GameManager.ins.laps) * (1) * (1 + GameManager.ins.totalPickUps) / 5));

        if (totalScore > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", totalScore);
            PlayerPrefs.Save();
        }

        LeaderBoardController.ins.playerScore = totalScore;
        LeaderBoardController.ins.submitScore();
    }
}
