using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DisplayLeaderBoard : MonoBehaviour
{
    private string memberID;
    int maxScores = 5;
    public int iD;
    public Text[] Entries;
    public Text userId;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        memberID = PlayerPrefs.GetString("UserId", $"Player{Random.Range(10, 99999999)}");
        PlayerPrefs.SetString("UserId", memberID);
        Debug.Log("MemberID: " + memberID + "StoredID: " + PlayerPrefs.GetString("UserId"));
        userId.text = "Your UserID: " + memberID + "   Top Score: " + PlayerPrefs.GetInt("BestScore", 0);
        PlayerPrefs.Save();
        StartCoroutine(LoginRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showScores()
    {
        LootLockerSDKManager.GetScoreList(iD, maxScores, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] scores = response.items;
                for (int i = 0; i < scores.Length; i++)
                {
                    Entries[i].text = (scores[i].rank + ".   " + scores[i].member_id + "  " + scores[i].score);
                }
                if (scores.Length < maxScores)
                {
                    for (int i = scores.Length; i < maxScores; i++)
                    {
                        Entries[i].text = (i + 1).ToString() + ".   empty";
                    }
                }
            }
            else
            {
                Debug.Log("connection Failed");
            }
        });
    }
    
    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("connection Success");
                showScores();
            }
            else
            {
                Debug.Log("connection Failed");
            }
        });
       // showScores();
        yield return new WaitWhile(() => done == false);
    }

    public void storeName()
    {
        PlayerPrefs.SetString("UserId", memberID);
        Debug.Log(memberID);
    }
}
