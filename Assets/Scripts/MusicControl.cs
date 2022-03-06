using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
    AudioSource audioSource;

    bool en = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            en = !en;
            audioSource.volume = en ? 1 : 0;
        }
    }
}
