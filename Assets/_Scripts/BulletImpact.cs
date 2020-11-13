using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    public AudioSource aus;
    public AudioClip[] wall;
    public AudioClip[] flesh;
    [Space]
    public GameObject stoneImpact;
    public GameObject fleshImpact;

    public enum HitSound { wall, flesh}

    public void PlaySound(HitSound hs)
    {
        switch (hs)
        {
            case HitSound.wall:
                aus.clip = ClipToPlay(wall);
                Instantiate(stoneImpact, transform);
                break;
            case HitSound.flesh:
                aus.clip = ClipToPlay(flesh);
                Instantiate(fleshImpact, transform);
                break;
            default:
                break;
        }
        aus.Play();
        Destroy(gameObject, 2);
    }

    AudioClip ClipToPlay(AudioClip[] clips)
    {
        AudioClip ac = clips[Random.Range(0, clips.Length)];
        return ac;
    }

}
