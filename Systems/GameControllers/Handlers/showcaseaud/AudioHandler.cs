using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

public class AudioHandler : Singleton<AudioHandler>
{
    [System.Serializable]
    public class AudioIdentity
    {
        public string tag;
        public AudioClip clip;
    }

    public AudioSource source;

    public List<AudioIdentity> audioClips;

    private Dictionary<string, AudioClip> audioClipsDict;

    private void Awake()
    {
        audioClipsDict = new Dictionary<string, AudioClip>(audioClips.Count);

        for (int i = 0; i < audioClips.Count; i++)
        {
            audioClipsDict.Add(audioClips[i].tag, audioClips[i].clip);
        }
    }
    public AudioClip GetClip(string tag)
    {
        return audioClipsDict[tag];
    }
}
