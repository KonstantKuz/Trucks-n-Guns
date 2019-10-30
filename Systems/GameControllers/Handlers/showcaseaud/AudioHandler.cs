using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

public class AudioHandler : Singleton<AudioHandler>
{
    public AudioClip[] music;

    private void Start()
    {
        StartCoroutine(CheckMus(GetComponent<AudioSource>()));
    }

    private IEnumerator CheckMus(AudioSource source)
    {
        yield return new WaitForSeconds(1f);
        if(!source.isPlaying)
        {
            source.PlayOneShot(music[Random.Range(0, music.Length)]);
        }
        yield return StartCoroutine(CheckMus(source));
    }
}
