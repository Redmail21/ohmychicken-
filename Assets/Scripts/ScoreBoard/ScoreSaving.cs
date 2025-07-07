using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSaving : MonoBehaviour
{
    //public TMPro.TextMeshProUGUI myName;
    //public TMPro.TextMeshProUGUI myScore;
    public int currentScore;


    private void OnEnable()
    {
        GameManager.OnPlayerDeath += SendScore;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDeath -= SendScore;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentScore = (int) GameManager._instance.Score;

    }

    public void SendScore()
    {
        if (currentScore > 0 ) { 
        
            //print(myName.text);
            print("Total: " + currentScore.ToString());

        }

    }
}
