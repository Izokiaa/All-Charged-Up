using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuButtonController : MonoBehaviour
{
    public GameObject playBtn, instructBtn, lbBtn;
    public GameObject instructionsPanel;
    public GameObject transitionPanel;
    public GameObject lbPanel;
    public bool activatedLeaderBoard = false;
    // Start is called before the first frame update
    void Start()
    {
        transitionPanel = GameObject.FindGameObjectWithTag("transitionPanel");
        playBtn.transform.LeanMoveLocal(new Vector2(-750, 0), 1.8f).setEaseOutBack();
        instructBtn.transform.LeanMoveLocal(new Vector2(-750, -200), 2.3f).setEaseOutBack();
        lbBtn.transform.LeanMoveLocal(new Vector2(-750, -400), 2.8f).setEaseOutBack();
        transitionPanel.transform.LeanMoveLocal(new Vector2(0, -1400), 0.8f).setEaseInOutQuad();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playPressed()
    {
        playBtn.transform.LeanMoveLocal(new Vector2(-1600, 0), 1).setEaseInBack();
        instructBtn.transform.LeanMoveLocal(new Vector2(-1600, -200), 1).setEaseInBack();
        lbBtn.transform.LeanMoveLocal(new Vector2(-1600, -400), 1).setEaseInBack();
        disableButton();
        //load scene
        StartCoroutine(waitToLoadScene(2));
    }
    public void instructionPressed()
    {
        //pull up instructions
        playBtn.transform.LeanMoveLocal(new Vector2(-1600, 0), 1).setEaseInBack();
        instructBtn.transform.LeanMoveLocal(new Vector2(-1600, -200), 1).setEaseInBack();
        lbBtn.transform.LeanMoveLocal(new Vector2(-1600, -400), 1).setEaseInBack();
        instructionsPanel.transform.LeanMoveLocal(new Vector2(0, 0), 1.5f).setEaseInOutQuad();
        disableButton();
    }
    public void lbPressed()
    {
        //open leaderboard or even have it up in main scene
        if (!activatedLeaderBoard)
        {
            lbPanel.transform.LeanMoveLocal(new Vector2(500, -150), 1).setEaseInBack();
            activatedLeaderBoard = true;
        }
    }

    public void disableButton()
    {
        playBtn.GetComponent<Button>().enabled = false;
        instructBtn.GetComponent<Button>().enabled = false;
        lbBtn.GetComponent<Button>().enabled = false;
    }

    public void moveInstructionScreenAway()
    {
        playBtn.transform.LeanMoveLocal(new Vector2(-750, 0), 1.8f).setEaseOutBack();
        instructBtn.transform.LeanMoveLocal(new Vector2(-750, -200), 2.3f).setEaseOutBack();
        lbBtn.transform.LeanMoveLocal(new Vector2(-750, -400), 2.8f).setEaseOutBack();
        instructionsPanel.transform.LeanMoveLocal(new Vector2(0, -1200), 2).setEaseOutBack();
        enableButton();
    }
    public void enableButton()
    {
        playBtn.GetComponent<Button>().enabled = true;
        instructBtn.GetComponent<Button>().enabled = true;
        lbBtn.GetComponent<Button>().enabled = true;
    }


    IEnumerator waitToLoadScene(int waitTime)
    {
        yield return new WaitForSeconds(waitTime/2);
        transitionPanel.transform.LeanMoveLocal(new Vector2(0, 0), 0.5f).setEaseInOutQuad();
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(1);
    }
}
