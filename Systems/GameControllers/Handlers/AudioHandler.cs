using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

public class AudioHandler : MonoBehaviour
{
    [System.Serializable]
    public class GameMusic
    {
        public AudioClip music;
        public string author;
        public string track;
    }

    public GameMusic[] music;

    private void Start()
    {
        StartCoroutine(CheckMus(GetComponent<AudioSource>()));
    }

    private IEnumerator CheckMus(AudioSource source)
    {
        yield return new WaitForSecondsRealtime(1f);
        if (!source.isPlaying)
        {
            int randomMus = Random.Range(0, music.Length);
            source.PlayOneShot(music[randomMus].music);
            GeneralGameUIHolder.Instance.windows.musicWindow.ShowMusic(music[randomMus].author, music[randomMus].track);
        }
        yield return StartCoroutine(CheckMus(source));
    }
}
