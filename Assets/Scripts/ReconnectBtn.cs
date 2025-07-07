using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ReconnectBtn : MonoBehaviour
{
    public Button reconnectButton;
    private bool isReconnecting = false;
    public float cooldownTime = 2.0f; // Cooldown time in seconds

    PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        //reconnectButton = GameObject.Find("NoInternetBtn").GetComponent<Button>();
    }

  public void RecconnectAndSubmit()
    {
         if (!isReconnecting)
        {
            StartCoroutine(ReconnectAndSubmitRoutine());
        }


    }






    private IEnumerator ReconnectAndSubmitRoutine()
    {
        isReconnecting = true;
        reconnectButton.interactable = false;

        // Reconnect
        yield return StartCoroutine(GameManager._instance.CheckInternetConnection());

        // Submit score
        GameManager._instance.SubmitScore();

        // Setup player manager
        playerManager.SetupRoutine();

        // Cooldown period before allowing another reconnection attempt
        yield return new WaitForSeconds(cooldownTime);

        reconnectButton.interactable = true;
        isReconnecting = false;
    }



}


