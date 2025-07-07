using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;


public class GooglePlay : MonoBehaviour
{


    public void SignIn()
    {

        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);

    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services

            //print("Success connecting to googlePlay!");

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            FindAnyObjectByType<PersistentData>().playerName = name;
        }
        else
        {
            FindAnyObjectByType<PersistentData>().playerName = "Guest";
            //print("Fail connecting to googlePlay!");
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
}
