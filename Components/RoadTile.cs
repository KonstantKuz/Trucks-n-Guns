using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoCached
{
    public RoadTileType roadTileType;

    public RoadTileTransitionCapabilitiesData NextRoadTileCapabilitiesHolder;

    public PhysicalEnvironment[] env;
    public GameObject[] objects;

    private void Awake()
    {
        env = gameObject.GetComponentsInChildren<PhysicalEnvironment>();

    }

    [ContextMenu("SetUpPossibleTilesData")]
    public void SetUpPossibleTilesData()
    {
        roadTileType.tileName = gameObject.name;
        NextRoadTileCapabilitiesHolder.SetUpCapabilities(roadTileType);
    }

    public void ResetEnv(float allRoadLEngt)
    {
        StartCoroutine(ResetEnvCall(allRoadLEngt));
    }

    private IEnumerator ResetEnvCall(float allRoadLEngt)
    {
        for (int i = 0; i < env.Length; i++)
        {
            env[i].ResetMe();
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
