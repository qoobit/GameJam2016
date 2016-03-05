using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public bool SfxOn;
    public bool MusicOn;

    public List<AudioSource> Sfx = new List<AudioSource>();
    public List<AudioSource> Bgm = new List<AudioSource>();

    public List<float> SfxInitialVolume = new List<float>();
    public List<float> BgmInitialVolume = new List<float>();

    public float sfxVolumeScale = 1f;
    public float bgmVolumeScale = 1f;

    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
