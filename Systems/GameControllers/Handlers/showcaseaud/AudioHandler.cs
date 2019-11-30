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
        int randomMus = Random.Range(0, music.Length);
        if (!source.isPlaying)
        {
            source.PlayOneShot(music[randomMus]);
        }
        yield return StartCoroutine(CheckMus(source));
    }
}
