using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField]
    AudioClip track1;
    [SerializeField]
    AudioClip track2;
    [SerializeField]
    AudioClip track3;
    [SerializeField]
    AudioClip track4;
    [SerializeField]
    AudioClip track5;
    [SerializeField]
    AudioClip track6;
    [SerializeField]
    AudioClip track7;
    [SerializeField]
    AudioClip track8;
    [SerializeField]
    AudioClip track9;
    [SerializeField]
    AudioClip track10;
    [SerializeField]
    AudioClip track11;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            if (audioSource.clip == track1)
            {
                audioSource.clip = track2;
            }
            else if (audioSource.clip == track2)
            {
                audioSource.clip = track3;
            }
            else if (audioSource.clip == track3)
            {
                audioSource.clip = track4;
            }
            else if (audioSource.clip == track4)
            {
                audioSource.clip = track5;
            }
            else if (audioSource.clip == track5)
            {
                audioSource.clip = track6;
            }
            else if (audioSource.clip == track6)
            {
                audioSource.clip = track7;
            }
            else if (audioSource.clip == track7)
            {
                audioSource.clip = track8;
            }
            else if (audioSource.clip == track8)
            {
                audioSource.clip = track9;
            }
            else if (audioSource.clip == track9)
            {
                audioSource.clip = track10;
            }
            else if (audioSource.clip == track10)
            {
                audioSource.clip = track11;
            }
            else if (audioSource.clip == track11)
            {
                audioSource.clip = track1;
            }
            audioSource.Play();
        }
    }
}
