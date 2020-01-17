using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicWindow : MonoBehaviour
{
    public Text author;
    public Text track;

    public void ShowMusic(string authorName, string trackName)
    {
        author.text = authorName;
        track.text = trackName;
        gameObject.SetActive(true);
        StartCoroutine(DelayedHide());
    }

    private IEnumerator DelayedHide()
    {
        yield return new WaitForSecondsRealtime(10f);
        gameObject.SetActive(false);
    }
}
