using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgainButton : MonoBehaviour
{    

    public void RestartScene()
    {
        // Get the current active scene and reload it                
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

}
