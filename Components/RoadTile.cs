using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoCached
{
    public RoadTileConfiguration roadTileConfiguration;

    public RoadTileTransitionCapabilitiesData NextRoadTileCapabilitiesHolder;

    public GameObject[] objects;

    public List<PhysicalEnvironmentHolder> environmentHolders;

    private void Awake()
    {
        foreach (Transform child  in transform)
        {
            if(!ReferenceEquals(child.GetComponent<PhysicalEnvironmentHolder>(), null))
            {
                environmentHolders.Add(child.GetComponent<PhysicalEnvironmentHolder>());
            }
        }
    }

    [ContextMenu("SetUpPossibleTilesData")]
    public void SetUpPossibleTilesData()
    {
        roadTileConfiguration.tileName = gameObject.name;
        NextRoadTileCapabilitiesHolder.SetUpCapabilities(roadTileConfiguration);
    }

    public void ResetEnv(float allRoadLEngt)
    {
        StartCoroutine(ResetEnvCall(allRoadLEngt));
    }

    private IEnumerator ResetEnvCall(float allRoadLEngt)
    {
        for (int i = 0; i < environmentHolders.Count; i++)
        {
            environmentHolders[i].ResetEnvironment();
             yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(ActivateProps(true));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(ActivateProps(false));
        }
    }

    private IEnumerator ActivateProps(bool enabled)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(enabled);
            yield return new WaitForEndOfFrame();
        }
    }
}
