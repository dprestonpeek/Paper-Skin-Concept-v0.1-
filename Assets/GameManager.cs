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

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Is there a better way to do this? Especially with more tracks
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
                audioSource.clip = track1;
            }
            audioSource.Play();
        }
    }
}
