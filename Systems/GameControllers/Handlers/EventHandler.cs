using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoCached
{
    [SerializeField]
    private Vector2 traveledDistaceToGenerateEventMinMax;

    public Vector3 playerPos_fromLastEvent { get; set; }
    public float traveledDistance { get; set; }

    private Transform lastEvent;
    private float distanceToRemoveLastEvent;

    private ObjectPoolerBase eventPooler;
    private float traveledDistaceToGenerateEvent;

    private Rigidbody player_rigidbody;

    private void Awake()
    {
        eventPooler = ObjectPoolersHolder.Instance.EventPooler;
        distanceToRemoveLastEvent = 300;
        traveledDistance = 0;
    }

    public void StartCheckDistance(Vector3 player_startPos, Rigidbody player_rigidbody)
    {
        this.player_rigidbody = player_rigidbody;
        StartCoroutine(CheckTraveledDistance(player_startPos));
    }

    private IEnumerator CheckTraveledDistance(Vector3 player_Pos)
    {
        if(traveledDistaceToGenerateEvent == 0)
        {
            traveledDistaceToGenerateEvent = Random.Range(traveledDistaceToGenerateEventMinMax.x, traveledDistaceToGenerateEventMinMax.y);
        }

        yield return new WaitForSecondsRealtime(2f);
        playerPos_fromLastEvent = player_Pos;

        if ((player_rigidbody.position - playerPos_fromLastEvent).magnitude > traveledDistaceToGenerateEvent)
        {
            GenerateEvent(player_rigidbody);
            playerPos_fromLastEvent = player_rigidbody.position;
            traveledDistaceToGenerateEvent = 0;
        }
        if(!ReferenceEquals(lastEvent, null))
        {
            if ((player_rigidbody.position - lastEvent.position).magnitude > distanceToRemoveLastEvent && lastEvent.GetComponent<IRoadEvent>().isActive)
            {
                lastEvent.GetComponent<IPoolReturner>().ReturnObjectsToPool();
            }
        }

        yield return StartCoroutine(CheckTraveledDistance(playerPos_fromLastEvent));
    }
    private void GenerateEvent(Rigidbody player_rigidbody)
    {
        if (!ReferenceEquals(lastEvent, null) && lastEvent.GetComponent<IRoadEvent>().isActive)
        {
            return;
        }

        GameObject eventObject = eventPooler.SpawnWeightedRandom(Vector3.zero, Quaternion.identity);
        eventObject.GetComponent<IRoadEvent>().AwakeEvent(player_rigidbody.position);
        lastEvent = eventObject.transform;
    }

   
}
