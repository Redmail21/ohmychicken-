using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Networking;
public class Leaderboard : MonoBehaviour
{
    
    string leaderboard_KEY= "globalHighscore";

    public TextMeshProUGUI[] namesGUI;
    public TextMeshProUGUI[] scoresGUI;

    

    private void Start()
    {
       
    }


 

    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        bool done = false;

        string player_ID = PlayerPrefs.GetString("PlayerID");

        //PlayerPrefs.SetString("PlayerName", "john");

        SetPlayerName(FindAnyObjectByType<PersistentData>().playerName);

        LootLockerSDKManager.SubmitScore(player_ID, scoreToUpload, leaderboard_KEY, (response) =>
        {

            if (response.success)
            {
                Debug.Log("Successfuly loaded the score");
                done = true;
            }
            else
            {
                Debug.Log("Failed to upload" + response.errorData.message);
                done = true;
            }

        });


        yield return new WaitWhile( ()=>done == false );
    }


    void SetPlayerName(string playerName)
    {
        LootLockerSDKManager.SetPlayerName(playerName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Player name set successfully");
            }
            else
            {
                Debug.LogError("Error setting player name");
            }
        });
    }


    public IEnumerator FetchTopHighschoresRoutine()
    {

        bool done = false;

        LootLockerSDKManager.GetScoreList(leaderboard_KEY, 8, 0, (response) => {
        

            if (response.success) {


                LootLockerLeaderboardMember[] members = response.items;


                //Debug.Log("scoresGUI.Length: " + scoresGUI.Length);
                //Debug.Log("namesGUI.Length: " + namesGUI.Length);
                //Debug.Log("members.Length: " + members.Length);

                for (int i = 0; i < members.Length; i ++)
                {

                    if (members[i] != null)
                    {

                        scoresGUI[i].text = members[i].score.ToString();
                        namesGUI[i].text += members[i].player.name.ToString(); ;  //IMPORTANT, CHANGE IT FOR THE PLAYSTORE/APPLESTORE USERNAME

                    }

                    //print(members[i].score);
                    //print(members[i].player.id);
                }

                done = true;
            }
            else
            {
                Debug.Log("Failed to upload" + response.errorData.message);
                done = true;

            }

        
        });


        yield return new WaitWhile(() => done == false);

    }

}
