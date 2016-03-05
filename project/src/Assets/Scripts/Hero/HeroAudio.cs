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
        ShotAudio = gameObject.AddComponent<AudioSource>();
        WalkAudio = gameObject.AddComponent<AudioSource>();
        DashAudio = gameObject.AddComponent<AudioSource>();
        LockAudio = gameObject.AddComponent<AudioSource>();
        DeathAudio = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void Play(Clip clip)
    {
        if (!GameControl.control.SfxOn) return;

        switch (clip)
        {
            case Clip.JUMP:
                JumpAudio.PlayOneShot(JumpAudioClip);
                break;
            case Clip.SHOT:
                ShotAudio.PlayOneShot(ShotAudioClip);
                break;
            case Clip.WALK:
                WalkAudio.PlayOneShot(WalkAudioClip);
                break;
            case Clip.DASH:
                DashAudio.PlayOneShot(DashAudioClip);
                break;
            case Clip.LOCK:
                LockAudio.PlayOneShot(LockAudioClip);
                break;
            case Clip.DEATH:
                DeathAudio.PlayOneShot(DeathAudioClip);
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

    public void StopAll()
    {
        JumpAudio.Stop();
        ShotAudio.Stop();
        WalkAudio.Stop();
        DashAudio.Stop();
        LockAudio.Stop();
        DeathAudio.Stop();
        
    }
}
