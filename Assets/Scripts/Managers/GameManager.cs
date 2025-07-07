using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    
    public enum GameStates
    {
         inGame,  death, newGame
    }



    public static bool ConnectedToInternet = false;
    public static GameManager _instance { get; private set; }
    public static GameStates gameState;    
    public static bool isChickenAlive = true;
    public static bool isClownHere = false; //Gets enabled after a certain amount of time
    private static bool isInitialized = false;

    public int chickenAllowedJumpCount = 2;


    public int Score { get; private set; } = 0;
    private int amount = 1;
    private float timeForScoreIncrease = 1f;
    private float timeForScoreIncreaseCount = 0;


    public float GameTime {  get; private set; }

    public float CubeVanishTime {  get; private set; }
    public float CubeSpawnTime { get; private set; }
    private float cubeTimeRate;

    public static int ClownSpawnInstancesToDate = 0; //Register the times the clownie has appeared 
    public float ClownSpawnTimer { get; private set; } = 0;//Decrease it as the game goes on (Make it spawn more often)
    public float ClownSpawnTimerCount { get; private set; } = 0;



    public static GooglePlay gp;
    public Leaderboard Leaderboard; //For uploading the score through the lootlocker sdk
    public GameObject StageManager; //For creating the floor
    public GameObject ThePlayer; //For accessing the player
    public GameObject UiManager;

    private FloorGenerator StageManagerFunctions;
    

    public static event Action OnPlayerDeath; //Called once the chicken variable is set to false;
    public static event Action OnStartGame;

    private delegate void CallbackFunction();



    //States

   


    private void Awake()
    {

        //GameManager.gameState = GameManager.GameStates.inGame;
        //if (_instance != null && _instance != this)  
        //{
        //    Destroy(this); 

        //} else
        //{


        //    //DontDestroyOnLoad(this.gameObject); //There has to be a reason this was commented, leave it as it is
        //    _instance = this;
        //}


        gameState = GameStates.inGame;

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

        }



        StageManagerFunctions = StageManager.GetComponent<FloorGenerator>();
        chickenAllowedJumpCount = ThePlayer.transform.GetChild(0).GetComponent<ChickenMovement>().jumpCount;


        

        if (!isInitialized)
        {

            gp = new GooglePlay();
            gp.SignIn();


            //FindAnyObjectByType<PersistentData>().playerName = "dada"; //HERE WE INITIALIZE THE NAME AND USE THE GOOGLE PLAY SERVICES.
            //print(FindAnyObjectByType<PersistentData>().playerName);
            isInitialized = true;
        }


        //Debug.developerConsoleVisible = true;

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Leaderboard = GameObject.Find("Leaderboard").GetComponent<Leaderboard>();
        StageManager = GameObject.Find("Stage").gameObject;
        ThePlayer = GameObject.Find("Player").gameObject;
        UiManager = GameObject.Find("UI").gameObject;
        StageManagerFunctions = StageManager.GetComponent<FloorGenerator>();
        chickenAllowedJumpCount = ThePlayer.transform.GetChild(0).GetComponent<ChickenMovement>().jumpCount;
        ValuesInitialized();
        //myName = GameObject.Find("YourNameBottom").GetComponent<TMPro.TextMeshProUGUI>();
    }


    private void Start()
    {
        //Debug.developerConsoleVisible = true;


        ValuesInitialized();

        StartCoroutine(CheckInternetConnection());




    }


    void ValuesInitialized()
    {
        print("start from GAMeManager");
        isChickenAlive = true;
        isClownHere = false;
        GameManager._instance.SwitchGameState(GameStates.inGame);
        ClownSpawnTimer = 3f; //Let's make the clown appear sooner so that the player gets adapted to it, then it gets set back to its default value
        ClownSpawnTimerCount = 0;
        cubeTimeRate = 2;
        Score = 0;
        GameTime = 0;
    }
    private void OnDestroy()
    {



        Destroy(this);

        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }



    }


    private void Update()
    {
        //Debug.Log("Game state is:" + GameManager.gameState);

        if (GameManager.isChickenAlive)
        {
            GameManager._instance.timeForScoreIncreaseCount += Time.deltaTime;
        
            if(GameManager._instance.timeForScoreIncreaseCount >= GameManager._instance.timeForScoreIncrease) //Changed to this update() method, when count reaches the timeForScoreIncrease, thepoints are added,  more flexible
            {
                GameManager._instance.timeForScoreIncreaseCount = 0;  
                UpdateScore(); //Increases the score by the "amount" variable
            }
            

        }


        GameTime += Time.deltaTime; //The survival time

        if (!GameManager.isChickenAlive && GameManager.gameState == GameStates.inGame)
        {
            //print("YOU HAVE DIED!"); //THIS IS THE ROOT OF THE PROBLEM
            GameManager._instance.SwitchGameState(GameStates.death);            //Once it is time to die
        }


        GameCubeSpawner(); //For the cubes
        ClownSpawner(); //For the clown
        DifficultyIncreaser(); //For making the game harder as the time goes by
        
        if(ThePlayer != null) chickenAllowedJumpCount = ThePlayer.transform.GetChild(0).GetComponent<ChickenMovement>().jumpCount;



    }

    //Custom methods
    //Custom methods
    //Custom methods
    //Custom methods

    private void DifficultyIncreaser()
    {
        
    }

    private void ClownSpawner()
    {

        //print("Clown's spawn count:" + ClownSpawnTimerCount);
        //print("Clown's spawn total:" + ClownSpawnTimer);
        if (GameManager.isChickenAlive && GameManager.gameState == GameStates.inGame)
        {
            if(GameManager._instance.GameTime > 2) //Clown should start appearing after 1 min, for debug purposes i will put it at 2 
            {
                if(!isClownHere) ClownSpawnTimerCount += Time.deltaTime;  //If the clown isn't in scene, count until its spawn


                if (ClownSpawnTimerCount >= ClownSpawnTimer && !isClownHere) //Once the time has met the threshold(goal value) spawn it
                {
                        
                        isClownHere = true;
                        ClownSpawnTimerCount = 0;
                        ClownSpawnTimer = UnityEngine.Random.Range(10, 20);   //How long will it take for the clown to turn hostile again
                        ClownSpawnInstancesToDate++; //Register the number of times the clown has spawned
                       
                }

            }
        } else
        {
            
            isClownHere = false;
        }

    }

    void GameCubeSpawner()
    {
        if (GameManager._instance.GameTime > 3 && GameManager.gameState == GameStates.inGame)
        {

            this.CubeVanishTime += Time.deltaTime;

            if (this.CubeVanishTime > cubeTimeRate) //Timerate defines how much time between each banishment
            {

                StartCoroutine(CallWithDelay(() => GameManager._instance.StageManagerFunctions.CubeVanisher(true, false), .5f));

                this.CubeVanishTime = 0;

                if (UnityEngine.Random.Range(1f, 10f) <= 5f) //50% Chance of underneath tile getting vanished
                {
                    var floorTile = ThePlayer.transform.GetChild(0).GetComponent<ChickenMovement>().floorUnderneathNotSafe.GetComponent<scriptCube>();
                    
                    if (floorTile == null) return;

                    if (!floorTile.isVanished && !floorTile.isVanishing)
                    {

                        floorTile.ToggleVanish();

                    }
                }



            }

        }
    }

    private void SwitchGameState(GameStates newGameState)
    {
        
        gameState = newGameState;

        switch (newGameState) {

            
            case GameStates.inGame:
                OnStartGame?.Invoke();
                //FindAnyObjectByType<SAudioManager>().Play("Theme");

                break;
          
            case GameStates.death:
                StartCoroutine(CheckInternetConnection());
                OnPlayerDeath?.Invoke();
                isClownHere = false;
                FindAnyObjectByType<SAudioManager>().Stop("Theme");
                FindAnyObjectByType<SAudioManager>().Play("Death");
                //print(GameManager.ConnectedToInternet);
                SubmitScore();



                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
        
        
        }  

    }

    //Implementations
    public void startGame()
    {
        GameManager._instance.SwitchGameState(GameManager.GameStates.inGame);


    }

   private void UpdateScore() //What actually does the score increment 
    {
        GameManager._instance.Score += amount;


    }



    private IEnumerator CallWithDelay(CallbackFunction callback, float delay) {

        yield return new WaitForSeconds(delay);
        callback();
    }


    public void DestroyThyself() {
        Destroy(gameObject); _instance = null; 
    }

    public void SubmitScore()
    {

        if (GameManager.ConnectedToInternet == true)
        {
            UiManager.transform.GetChild(0).Find("NoInternet").gameObject.SetActive(false);

            StartCoroutine(Leaderboard.SubmitScoreRoutine(Score));

        }
        else
        {
            UiManager.transform.GetChild(0).Find("NoInternet").gameObject.SetActive(true);
        }



     


    }


    public IEnumerator CheckInternetConnection()
    {

        UnityWebRequest request = new UnityWebRequest("http://google.com");


        yield return request.SendWebRequest();

        if (request.error != null)
        {

            GameManager.ConnectedToInternet = false;
            print("Internet? " + GameManager.ConnectedToInternet);

            yield return false;
            yield break;
        }
        else
        {

            //All good.

            GameManager.ConnectedToInternet = true;
            print("Internet? " + GameManager.ConnectedToInternet);
            yield return true;
            yield break;

        }
    }

}//End of class

