using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedParticles : MonoBehaviour
{
    private List<ParticleSystem> particles;

    private void Awake()
    {
        particles = new List<ParticleSystem>(10);
        if(GetComponent<ParticleSystem>() != null)
        {
            particles.Add(GetComponent<ParticleSystem>());
        }
        foreach (Transform child  in transform)
        {
            if(child.GetComponent<ParticleSystem>() !=null)
            {
                particles.Add(child.GetComponent<ParticleSystem>());
            }
        }
    }

    public void PlayParticles()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }
    }
}
