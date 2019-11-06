using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoCached
{
    public enum EventType
    {
        //ValleyOfBullets,   //аллея пулеметов
        //RoadBlock,           // дорожный боевой блокпост
        //HailOfStones,      //обвал камней рантайм
        //StoneBlockage,     //завал дороги большим камнем
        //SandStorm,         // песчаная буря под вопросом
        //Blockade,          // живая застава
        //Helicopter,        // преследующий вертолет
        //ShellingHelicopter,  // обстреливающий по полосе вертолет
        //Minefield,         // Минное поле
    }

    [SerializeField]
    private Vector2 traveledDistaceToGenerateEventMinMax;
    [SerializeField]
    private Vector3 offsetFromPlayer;

    public Vector3 playerPos_fromLastEvent { get; set; }
    public float traveledDistance { get; set; }
    //private IPoolReturner lastEventReturner;
    //private IRoadEvent lastEvent;
    private Transform lastEvent;
    private float distanceToRemoveLastEvent;

    private ObjectPoolerBase eventPooler;
    private float traveledDistaceToGenerateEvent;

    private void Awake()
    {
        eventPooler = ObjectPoolersHolder.Instance.EventPooler;
        distanceToRemoveLastEvent = offsetFromPlayer.z * 2;
    }

    public void StartCheckDistance(Vector3 player_startPos, Rigidbody player_rigidbody)
    {
        StartCoroutine(CheckTraveledDistance(player_startPos, player_rigidbody));
    }

    private IEnumerator CheckTraveledDistance(Vector3 player_Pos, Rigidbody player_rigidbody)
    {
        if(traveledDistaceToGenerateEvent == 0)
        {
            traveledDistaceToGenerateEvent = Random.Range(traveledDistaceToGenerateEventMinMax.x, traveledDistaceToGenerateEventMinMax.y);
        }

        yield return new WaitForSeconds(2f);
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

        yield return StartCoroutine(CheckTraveledDistance(playerPos_fromLastEvent, player_rigidbody));
    }
    private void GenerateEvent(Rigidbody player_rigidbody)
    {
        if (!ReferenceEquals(lastEvent, null) && lastEvent.GetComponent<IRoadEvent>().isActive)
        {
            return;
        }

        //int randomEventNum = Random.Range(0, System.Enum.GetNames(typeof(EventType)).Length);
        //EventType myType = (EventType)randomEventNum;
        //string randomEventName = myType.ToString();

        //GameObject eventObject = eventPooler.Spawn(randomEventName);
        GameObject eventObject = eventPooler.SpawnWeightedRandom(Vector3.zero, Quaternion.identity);
        eventObject.transform.position = new Vector3(eventObject.transform.position.x, eventObject.transform.position.y, player_rigidbody.position.z + offsetFromPlayer.z);
        eventObject.GetComponent<IRoadEvent>().AwakeEvent();
        lastEvent = eventObject.transform;
    }

   
}
