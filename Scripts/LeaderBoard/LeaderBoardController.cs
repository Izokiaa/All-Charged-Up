using UnityEngine.UI;
using LootLocker.Requests;
using UnityEngine;

public class LeaderBoardController : MonoBehaviour
{
    public static LeaderBoardController ins;

    private void Awake()
    {
        ins = this;
    }

    private string memberID;
    public int iD;
    public int playerScore;
    public void Start()
    {
        memberID = PlayerPrefs.GetString("UserId", $"Player{Random.Range(10, 99999999)}");
        Debug.Log("Retrieved Member ID: " + memberID);
        Debug.LogError("Retrieved Member ID: " + memberID);
        LootLockerSDKManager.StartGuestSession("Player", (response) =>
        {
            if (response.success)
            {
                Debug.Log("connection Success");
            }
            else
            {
                Debug.Log("connection Failed" + response.Error);
                Debug.LogError("connection Failed : " + response.Error);
            }
        });
    }

    public void submitScore()
    {
        LootLockerSDKManager.SubmitScore(memberID, playerScore, iD, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Upload Success");
            }
            else
            {
                Debug.Log("connection Failed : " + response.Error);
                Debug.LogError("connection Failed : " + response.Error);
            }
        });
    }

}
