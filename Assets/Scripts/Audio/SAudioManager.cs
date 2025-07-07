using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

//Credit to Brackeys youtube tutorial on Audio managers, as the majority of this code and learning how to use it was made by him.


public partial class SAudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static SAudioManager instance;
    //AudioManager

    void Awake()
    {

        if (instance == null)
        {


            instance = this;
            


        }
        else
        {
            Destroy(gameObject);
            return;
        }

            DontDestroyOnLoad(gameObject);
        

            initializeSounds();
            Play("Theme");

        //print("AWAKE SOUND");
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Play("Theme"); // Play the main theme when the scene is loaded
    }


    void Start()
    {

        



    }

    private void initializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    //this addition to the code was made by me, the rest was from Brackeys tutorial
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Stop();
    }


    public void Update()
    {
        foreach (Sound s in sounds)
        {
           
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    } // End of Update


}//End of Class