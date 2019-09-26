using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerCulling : MonoBehaviour
{
    public MeshRenderer[] meshes;


    private void OnEnable()
    {
        RendererSetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            RendererSetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RendererSetActive(false);
        }
    }
    

    private void RendererSetActive(bool enabled)
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = enabled;
        }
    }

  
}
