using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound {

	public string name;

	public AudioClip clip;

    

	[Range(0f, 1f)]
	public float volume = 1f;
	[Range(0f, 1f)]
	public float volumeVariance = .1f;

	[Range(.1f, 3f)]
	public float pitch = 1f;
	[Range(0f, 1f)]
	public float pitchVariance = .1f;
    public AudioMixerGroup mixerGroup;

    
 

	[HideInInspector]
	public AudioSource source;

	public bool loop = false;
    public bool spatialize = true;

   


    [Header("Spatialization Parameters")]
    public float minDistance = 1.5f;
    public float maxDistance = 3.0f;
    [Range(0f, 360f)]
    public float spread = 0f;



}
