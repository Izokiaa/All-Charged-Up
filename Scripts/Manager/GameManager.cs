using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    public static GameManager ins;
    void Awake()
    {
        ins = this;
    }

    // Start is called before the first frame update
    [Header("Timers")]
    [HideInInspector]public Timer decreaseBatteryTimer;
    public float decreaseBatteryTime;
    [HideInInspector] public Timer activateSceneTimer;
    public float activateSceneTime;
    [Header("Battery")]
    public int batteryCount = 100; //battery left
    public Text batteryTxt;
    public int totalPickUps = 0;
    [Header("LapInfo")]
    public int laps = 0; //what lap we are on
    public int checkPointIndex = 0; //what check point we are currently at
    public int checkPointLength;
    public Text lapText;
    public GameObject[] checkPoints;
    [Header("PreActive Stuff")]
    public bool canPlay = false;
    public bool gameOver = false;
    public bool startCountDown = false;
    public int countDown = 3;
    AudioSource countDownAudio;
    public Text countDownTxt;
    bool canPlayAudio = true;
    [Header("SpawnBatteries")]
    public GameObject batteries;
    public int batteriesPerlap = 6;
    [Header("TransitionUI")]
    public GameObject transitionPanel;
    public bool changingScenes = false;
    [Header("GameOverUI")]
    public GameObject finalScorePanel;
    public Text lapsTxt;
    public Text chargesCollectedTxt;
    public Text fastestLapTxt;
    public Text totScoreTxt;
    void Start()
    {
        SpawnBatteries();
        lapText = GameObject.FindGameObjectWithTag("lapText").GetComponent<Text>();
        batteryTxt = GameObject.FindGameObjectWithTag("batText").GetComponent<Text>();
        countDownTxt = GameObject.FindGameObjectWithTag("cdText").GetComponent<Text>();
        checkPointLength = GameObject.FindGameObjectsWithTag("CheckPoint").Length;
        checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
        transitionPanel = GameObject.FindGameObjectWithTag("transitionPanel");
        countDownAudio = GetComponent<AudioSource>();
        laps = 1;
        checkPointIndex = 0;

        transitionPanel.transform.LeanMoveLocal(new Vector2(0, -1400), 0.8f).setEaseInOutQuad();
        StartCoroutine(startGame(1));


        activateSceneTimer = gameObject.AddComponent<Timer>();
        activateSceneTimer.Duration = activateSceneTime;
        activateSceneTimer.AddTimerFinishedListener(() => {
            countDown--;
            if (countDown == 1)
            {
                canPlay = true;
                timerController.ins.startTimer();
            }
        });

        decreaseBatteryTimer = gameObject.AddComponent<Timer>();
        decreaseBatteryTimer.Duration = decreaseBatteryTime;
        decreaseBatteryTimer.AddTimerFinishedListener(() => {
            batteryCount--;
        });
    }

    // Update is called once per frame
    void Update()
    {

        if (batteryCount > 100)
            batteryCount = 100;
        if ((!canPlay || countDown == 1) && !activateSceneTimer.Running && !changingScenes)
        {
            if (!countDownAudio.isPlaying && canPlayAudio && !changingScenes)
            {
                canPlayAudio = false;
                countDownAudio.Play();
            }
            if (countDown != 1 && !changingScenes)
                activateSceneTimer.Run();
            switch (countDown)
            {
                case 3:
                    countDownTxt.text = "Ready";
                    break;
                case 2:
                    countDownTxt.text = "Set";
                    break;
                case 1: countDownTxt.text = "GO!";
                    break;
            }
        }

        if(countDown == 1 && canPlay && startCountDown)
        {
            var col = countDownTxt.color;
            col.a = Mathf.Lerp(col.a, 0, 5 * Time.deltaTime);
            if (col.a < 0.02f)
            {
                col.a = 0;
                countDown = 0;
            }
            countDownTxt.color = col;
        }

        if(batteryCount <= 0 || gameOver)
        {
            GameOver();
        }


        if (!decreaseBatteryTimer.Running && canPlay)
        {
            decreaseBatteryTimer.Run();
        }
        lapText.text = $"Lap: {laps}";
        batteryTxt.text = $"Charge: {batteryCount}%";
    }

    public void completedLap()
    {
        //decrease the amount of time it takes for the battery to go down;
        if (decreaseBatteryTimer.Running)
            decreaseBatteryTimer.Stop(true);
        timerController.ins.stopTimer();
        for (int i = 0; i < checkPoints.Length; i++)
        {
            checkPoints[i].GetComponent<Collider2D>().enabled = true;
            checkPoints[i].GetComponent<SpriteRenderer>().enabled = true;
        }
        decreaseBatteryTime = decreaseBatteryTime / (1f + 0.1f);
        decreaseBatteryTimer.Duration = decreaseBatteryTime; //print out the new battery loss persecond time
        laps += 1;
        checkPointIndex = 0;
        if ((batteryCount + 30) < 100)
            batteryCount += 30; //only batteries will replenish in a little bit
        else
            batteryCount = 100;
        GameObject[] x = GameObject.FindGameObjectsWithTag("batOBJ");
        for (int i = 0; i < x.Length; i++)
        {
            Destroy(x[i]);
        }
        increaseBatteryOnField();
        SpawnBatteries();
    }

    void SpawnBatteries()
    {
        for(int i = 0; i < batteriesPerlap; i++)
        {
            Instantiate(batteries, AllRoadTiles.ins.openTilePos[Random.Range(0, AllRoadTiles.ins.openTilePos.Length)], Quaternion.identity);
        }
    }

    void GameOver()
    {

        canPlay = false;
        gameOver = true;
        if (CarController2.ins.rb.velocity.magnitude <= 1 && !changingScenes)
        {
            //load main menu
            ScoreManager.ins.CalcFinalScore();
            changingScenes = true;
            totScoreTxt.text = $"Score: {ScoreManager.ins.totalScore}";
            chargesCollectedTxt.text = $"Charges Collected: {totalPickUps}";
            lapsTxt.text = $"Laps Completed: {laps}";
            fastestLapTxt.text = $"Fastest Lap: {timerController.ins.fastestLap} sec";
            finalScorePanel.transform.LeanMoveLocal(new Vector2(0, 0), 0.8f).setEaseInOutQuad();
            Debug.Log("Panel MOVE");
            //transitionPanel.transform.LeanMoveLocal(new Vector2(0, 0), 0.8f).setEaseInOutQuad();
            //StartCoroutine(LoadScene(1.5f));
        }

        countDownAudio.Stop();
        CarController2.ins.driftAudio.Stop();
        if (changingScenes && Input.GetKeyDown(KeyCode.Space))
        {
            backToHome();
        }
    }

    IEnumerator LoadScene(float sec)
    {
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene(0);
    }

    IEnumerator startGame(float sec)
    {
        yield return new WaitForSeconds(sec);
        startCountDown = true;
    }


    void increaseBatteryOnField()
    {
        switch (laps)
        {
            case 3:
                batteriesPerlap++;
                break;
            case 6:
                batteriesPerlap++;
                break;
            case 9: batteriesPerlap++;
                break;
            case 12:
                batteriesPerlap++;
                break;
            case 15:
                batteriesPerlap += 2;
                break;
            case 18:
                batteriesPerlap += 2;
                break;
            case 21:
                batteriesPerlap += 2;
                break;
            case 24:
                batteriesPerlap += 2;
                break;
        }
    }

    public void backToHome()
    {
        transitionPanel.transform.LeanMoveLocal(new Vector2(0, 0), 0.8f).setEaseInOutQuad();
        StartCoroutine(LoadScene(1.5f));
    }
}
