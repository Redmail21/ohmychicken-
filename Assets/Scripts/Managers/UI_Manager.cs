using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{

    TextMeshProUGUI TXT_GameOver;
    TextMeshProUGUI TXT_GameOverSubtitle;
    TextMeshProUGUI TXT_Score; //Default size 5.6
    TextMeshProUGUI TXT_Jumps; //Default size 4.1
    TextMeshProUGUI TXT_JumpsScore;
    private GameObject staticGameOverCanvasObj;

    [SerializeField] private GameObject scoreObj;
    [SerializeField] private GameObject jumpsObj;
    [SerializeField] private GameObject gameOverObj;
    [SerializeField] private GameObject uiTouchControls; //Default position in Y: -556.38
    [SerializeField] private GameObject scoreboardObj;




    private void OnEnable()
    {
        
        GameManager.OnPlayerDeath += ShowGameOver;
        GameManager.OnStartGame += HideGameOver;
        GameManager.OnStartGame += UI_GamePopUp;
        
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDeath -= ShowGameOver;
        GameManager.OnStartGame -= HideGameOver;
        GameManager.OnStartGame -= UI_GamePopUp;
        
    }

    private void Awake()
    {

        //staticGameOverCanvasObj = transform.Find("gameOverCanvas").gameObject;
        

        LoadVariables();

    }

    private void Start()
    {
        //staticGameOverCanvasObj.SetActive(false);
        
    }
    private void Update()
    {
        
        ChangeScore(GameManager._instance.Score);
        ChangeJumps(GameManager._instance.chickenAllowedJumpCount);

    }


    //Custom Methods
    //
    //

    private void LoadVariables() {


            TXT_GameOver = gameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            TXT_GameOverSubtitle = gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            TXT_Score = scoreObj.GetComponent<TextMeshProUGUI>();
            TXT_Jumps = jumpsObj.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TXT_JumpsScore = jumpsObj.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
     }

    void HideGameOver()
    {
        scoreboardObj.SetActive(false);
        gameOverObj.SetActive(false);
        uiTouchControls.SetActive(true);
        TXT_Score.gameObject.GetComponent<RectTransform>().localScale = new Vector2(0, 0);
        //Hide leaderboard
        jumpsObj.gameObject.GetComponent<RectTransform>().localScale = new Vector2(0, 0);
       
        

       
    }


    void ShowGameOver()
    {
        scoreboardObj.transform.Find("YourNameBottom").GetComponent<TextMeshProUGUI>().SetText(FindAnyObjectByType<PersistentData>().playerName);
        scoreboardObj.SetActive(true);
        gameOverObj.SetActive(true);
        jumpsObj.SetActive(false);
        uiTouchControls.SetActive(false);
        gameOverObj.transform.Find("NoInternet").gameObject.SetActive(false);




        Vector3 newScorePos = TXT_Score.transform.position; //newScorePos.y -= 80;
        TXT_Score.transform.position = newScorePos;


    }

    private void UI_GamePopUp()
    {
        
        //TXT_Jumps.enabled = true;
        //TXT_JumpsScore.enabled = true;        

        jumpsObj.SetActive(true);

        StartCoroutine(AnimateScale(TXT_Score.gameObject.GetComponent<RectTransform>() , new(1.7f,1.7f), 1.5f));
        StartCoroutine(AnimateScale(jumpsObj.gameObject.GetComponent<RectTransform>(), new(1.1f, 1.1f), 1.5f));

        
        //StartCoroutine(AnimateMove(uiTouchControls.GetComponent<RectTransform>(), new(0, -556.38f, 0), 1.5f));

    }


    public static IEnumerator AnimateScale(RectTransform rectTransform, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = rectTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        rectTransform.localScale = targetScale; // Ensure the scale is set to the target scale at the end
    }

    public static IEnumerator AnimateMove(RectTransform rectTransform, Vector3 targetpos, float duration)
    {
        Vector3 initialpos = rectTransform.position;        

        while ( initialpos != targetpos)
        {
            initialpos = rectTransform.localPosition;
            rectTransform.localPosition = Vector3.MoveTowards(initialpos,targetpos, duration);   
            
            yield return null; // Wait for the next frame
        }

        rectTransform.localPosition = targetpos; 
    }

    void ChangeScore(float newVal)
    {
        this.TXT_Score.SetText(newVal.ToString());

    }

    void ChangeJumps(int newVal)
    {
        this.TXT_Jumps.SetText(newVal.ToString());
    }

}//End of main class

