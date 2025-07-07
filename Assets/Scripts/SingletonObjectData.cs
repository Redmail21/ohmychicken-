using LootLocker.Requests;
using Unity.VisualScripting;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData instance { get;  set; }  //THIS THING ONLY HAS A SINGLE FUNCTION, AND IT IS STORTING DATA ACROSS SCENE RELOADS

    public string playerName = "Guest";
    public int playerScore = 0;


    private void Start()
    {
        
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

   

}
