using UnityEngine;
using System.Collections;

public class HeroAudio : MonoBehaviour
{
    public enum Clip { JUMP, SHOT, WALK, DASH, LOCK, DEATH };

    private AudioSource JumpAudio;
    private AudioSource ShotAudio;
    private AudioSource WalkAudio;
    private AudioSource DashAudio;
    private AudioSource LockAudio;
    private AudioSource DeathAudio;

    public AudioClip JumpAudioClip;
    public AudioClip ShotAudioClip;
    public AudioClip WalkAudioClip;
    public AudioClip DashAudioClip;
    public AudioClip DeathAudioClip;
    public AudioClip LockAudioClip;

    void Start()
    {
        JumpAudio = gameObject.AddComponent<AudioSource>();
        JumpAudio.clip = JumpAudioClip;
        ShotAudio = gameObject.AddComponent<AudioSource>();
        ShotAudio.clip = ShotAudioClip;
        WalkAudio = gameObject.AddComponent<AudioSource>();
        WalkAudio.clip = WalkAudioClip;
        DashAudio = gameObject.AddComponent<AudioSource>();
        DashAudio.clip = DashAudioClip;
        LockAudio = gameObject.AddComponent<AudioSource>();
        LockAudio.clip = LockAudioClip;
        DeathAudio = gameObject.AddComponent<AudioSource>();
        DeathAudio.clip = DeathAudioClip;
    }

    void Update()
    {

    }

    public void Play(Clip clip)
    {
        switch (clip)
        {
            case Clip.JUMP:
                JumpAudio.Play();
                break;
            case Clip.SHOT:
                ShotAudio.Play();
                break;
            case Clip.WALK:
                WalkAudio.Play();
                break;
            case Clip.DASH:
                DashAudio.Play();
                break;
            case Clip.LOCK:
                LockAudio.Play();
                break;
            case Clip.DEATH:
                DeathAudio.Play();
                break;
        }
    }

    public void Stop(Clip clip)
    {
        switch (clip)
        {
            case Clip.JUMP:
                JumpAudio.Stop();
                break;
            case Clip.SHOT:
                ShotAudio.Stop();
                break;
            case Clip.WALK:
                WalkAudio.Stop();
                break;
            case Clip.DASH:
                DashAudio.Stop();
                break;
            case Clip.LOCK:
                LockAudio.Stop();
                break;
            case Clip.DEATH:
                DeathAudio.Stop();
                break;
        }
    }
}
