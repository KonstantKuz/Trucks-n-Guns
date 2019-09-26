using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTroubleCulling : MonoBehaviour
{
    public List<Collider> roadTroubleColliders = new List<Collider>(50);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            for (int i = 0; i < other.GetComponents<Collider>().Length; i++)
            {
                roadTroubleColliders.Add(other.GetComponents<Collider>()[i]);
            }
        }
        if (other.CompareTag("Player"))
        {
            RoadTroubleCollidersSetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoadTroubleCollidersSetActive(false);
            roadTroubleColliders.Clear();
        }
    }

    private void RoadTroubleCollidersSetActive(bool enabled)
    {
        for (int i = 0; i < roadTroubleColliders.Count; i++)
        {
            roadTroubleColliders[i].enabled = enabled;
        }
    }
}
