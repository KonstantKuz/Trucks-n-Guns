using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoCached, INeedObjectPooler
{
    public enum EventType
    {
       // ValleyOfBullets, //аллея пулеметов
        RoadBlock,  // дорожный боевой блокпост
        //HailOfStones, //обвал камней рантайм
        // SandStorm, // песчаная буря под вопросом
        // Blockade, // живая застава
        StoneBlockage //завал дороги большим камнем
    }

    [SerializeField]
    private float traveledDistanceToGenerateNewEventMin, traveledDistanceToGenerateNewEventMax;
    [SerializeField]
    private Vector3 offsetFromPlayer;

    public Vector3 playerPos_fromLastEvent { get; set; }
    public float traveledDistance { get; set; }

    private ObjectPoolerBase eventPooler;
    private float traveledDistanceToGenerateNewEvent;

    public void InjectNeededPooler(ObjectPoolerBase objectPooler)
    {
        eventPooler = objectPooler;
    }

    public void StartCheckDistance(Vector3 player_startPos, Rigidbody player_rigidbody)
    {
        StartCoroutine(CheckTraveledDistance(player_startPos, player_rigidbody));
    }

    private IEnumerator CheckTraveledDistance(Vector3 player_startPos, Rigidbody player_rigidbody)
    {
        if(traveledDistanceToGenerateNewEvent == 0)
        {
            traveledDistanceToGenerateNewEvent = Random.Range(traveledDistanceToGenerateNewEventMin, traveledDistanceToGenerateNewEventMax);
        }

        yield return new WaitForSeconds(5f);
        playerPos_fromLastEvent = player_startPos;

        if ((player_rigidbody.position - playerPos_fromLastEvent).magnitude > traveledDistanceToGenerateNewEvent)
        {
            GenerateEvent(player_rigidbody);
            playerPos_fromLastEvent = player_rigidbody.position;
            traveledDistanceToGenerateNewEvent = 0;
        }

        yield return StartCoroutine(CheckTraveledDistance(playerPos_fromLastEvent, player_rigidbody));
    }
    int eventcount = 0;
    private void GenerateEvent(Rigidbody player_rigidbody)
    {
        int randomEventNum = Random.Range(0, System.Enum.GetNames(typeof(EventType)).Length);
        EventType myType = (EventType)randomEventNum;
        string randomEventName = myType.ToString();

        GameObject eventObject = eventPooler.SpawnFromPool(randomEventName);
        eventObject.transform.position = new Vector3(player_rigidbody.position.x + offsetFromPlayer.x, eventObject.transform.position.y, eventObject.transform.position.z);

        Debug.Log($"<color=red> EVENT {eventcount}{eventcount}{eventcount}{eventcount}{eventcount} GENERATED!!! </color>");
        eventcount++;
    }

   
}
