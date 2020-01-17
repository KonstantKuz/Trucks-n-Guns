using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [SerializeField]
    private Vector2 traveledDistaceToGenerateEventMinMax;

    public Vector3 playerPos_fromLastEvent { get; set; }
    public float traveledDistance { get; set; }

    private Transform lastEvent;
    //private float distanceToRemoveLastEvent;

    private ObjectPoolerBase eventPooler;
    private float traveledDistaceToGenerateEvent;

    private Rigidbody player_rigidbody;

    private void Awake()
    {
        eventPooler = ObjectPoolersHolder.Instance.EventPooler;
        //distanceToRemoveLastEvent = 300;
        traveledDistance = 0;
    }

    public void StartCheckDistance(Vector3 player_startPos, Rigidbody player_rigidbody)
    {
        this.player_rigidbody = player_rigidbody;
        playerPos_fromLastEvent = player_rigidbody.position;
        StartCoroutine(CheckTraveledDistance());
    }

    private IEnumerator CheckTraveledDistance()
    {
        if(traveledDistaceToGenerateEvent == 0)
        {
            traveledDistaceToGenerateEvent = Random.Range(traveledDistaceToGenerateEventMinMax.x, traveledDistaceToGenerateEventMinMax.y);
        }

        yield return new WaitForSecondsRealtime(2f);

        if ((player_rigidbody.position - playerPos_fromLastEvent).magnitude > traveledDistaceToGenerateEvent)
        {
            GenerateEvent(player_rigidbody);
        }

        yield return StartCoroutine(CheckTraveledDistance());
    }
    private void GenerateEvent(Rigidbody player_rigidbody)
    {
        playerPos_fromLastEvent = player_rigidbody.position;

        if (!ReferenceEquals(lastEvent, null) && lastEvent.GetComponent<IRoadEvent>().isActive)
        {
            return;
        }

        GameObject eventObject = eventPooler.SpawnWeightedRandom(Vector3.zero, Quaternion.identity);
        eventObject.GetComponent<IRoadEvent>().AwakeEvent(player_rigidbody.position);
        lastEvent = eventObject.transform;
    }

   
}
