using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using StateMachine;

public static class EnemyStateDictionary
{
    public static Dictionary<GameEnums.EnemyFollowType, State<Enemy>> stateDictionary { get; private set; }

    public static void CreateStateDictionary()
    {
        stateDictionary = new Dictionary<GameEnums.EnemyFollowType, State<Enemy>>(5);
        stateDictionary.Add(GameEnums.EnemyFollowType.PathFollow, PathFollowState.Instance);
        stateDictionary.Add(GameEnums.EnemyFollowType.PlayerFollow, PlayerFollowState.Instance);
        //stateDictionary.Add(GameEnums.EnemyFollowType.PlayerRam, PlayerRamState.Instance);
        //stateDictionary.Add(GameEnums.EnemyFollowType.PlayerOverTaking, PlayerOvertakingState.Instance);
        //stateDictionary.Add(GameEnums.EnemyFollowType.SingleTest, SingleTestState.Instance);
    }
}

public class Enemy : MonoCached, INeedTarget, IPoolReturner
{
    
    public GameEnums.EnemyFollowType followType;

    [SerializeField]
    private Transform sensorsPosition;

    [SerializeField]
    private float sensorsLength = 5f, sideSensorsOffset = 1f, sensorsAngle = 15f;

    public Truck truck { get; set; }

    //для объезда препятствий
    private Vector3 sensorsStartPos, sensorsRSidePos, sensorsLSidePos, sideSensorsOffsetVec;

    private bool avoiding = false;

    public List<Node> path { get; set; }
    public Node currentNode { get; set; }
    public int targetIndex { get; set; }
    
    //для стрельбы и преследования игрока
    public TargetData targetData { get; set; }

    public StateMachine<Enemy> followTypeStateController { get; private set; }

    #region UnityCallbacks
    
    public static void CreateStateDictionary()
    {
        EnemyStateDictionary.CreateStateDictionary();

        Debug.Log($"<color=black> firepoints {System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length}</color>");
        Debug.Log($"<color=black> trucks {System.Enum.GetNames(typeof(GameEnums.Truck)).Length}</color>");

    }
    private void OnEnable()
    {
        allTicks.Add(this);
    }
    private void OnDisable()
    {
        allTicks.Remove(this);
        if(truck!=null)
        {
            truck.trucksCondition.OnZeroCondition -= ReturnObjectsToPool;
        }
    }

    public void AwakeEnemy()
    {
        truck = GetComponent<Truck>();

        followTypeStateController = new StateMachine<Enemy>(this);
        truck.SetUpTruck();

        truck.trucksCondition.OnZeroCondition += ReturnObjectsToPool;

        followTypeStateController.ChangeState(EnemyStateDictionary.stateDictionary[followType]);

        sideSensorsOffsetVec = sensorsPosition.right * sideSensorsOffset;
        sensorsStartPos = sensorsPosition.position;
        sensorsRSidePos = sensorsStartPos + sideSensorsOffsetVec;
        sensorsLSidePos = sensorsStartPos - sideSensorsOffsetVec;
    }

    public void ReturnObjectsToPool()
    {
        truck.TruckData.ReturnObjectsToPool(truck);
    }

    public void RandomizeData()
    {
        int randomState = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.EnemyFollowType)).Length);
        followType = (GameEnums.EnemyFollowType)randomState;
        int randomTruck = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.Truck)).Length);
        truck.TruckData.truckType = (GameEnums.Truck)randomTruck;
        int randomFirePoint = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length);
        truck.TruckData.firePointType = (GameEnums.FirePointType)randomFirePoint;

        Debug.Log($"<color=blue> {gameObject.name} has {followType.ToString()} type with {truck.TruckData.truckType.ToString()} and {truck.TruckData.firePointType.ToString()} </color>");
    }
    #endregion

    #region My

    public void InjectNewTargetData(Rigidbody targetRigidbody)
    {
        targetData = new TargetData(targetRigidbody);

        for (int i = 0; i < truck.firePoint.FirstTrackingGroupGuns.Count; i++)
        {
            truck.firePoint.FirstTrackingGroupGuns[i].SetTargetData(targetData);
        }
    }

    public override void OnTick()
    {
        followTypeStateController.UpdateMachine();
        AttackTarget();
    }

    public void AttackTarget()
    {
        if((targetData.target_rigidbody.position - truck._transform.position).magnitude < 40f)
        {
            truck.firePoint.FirstTrackingAttack();
            truck.firePoint.StaticAttack();
        }
    }
  
    public void ReTracePath(List<Node> newPath)
    {
        path = newPath;
        targetIndex = 0;
        if(PathCheck())
        {
            currentNode = path[targetIndex];
        }
    }
    
    public bool PathCheck()
    {
        return (!ReferenceEquals(path, null) && path.Count > 0);
    }

    public float AvoidForce()
    {
        sensorsStartPos = sensorsPosition.position;
        sensorsRSidePos = sensorsStartPos + sideSensorsOffsetVec;
        sensorsLSidePos = sensorsStartPos - sideSensorsOffsetVec;
        float avoidForce = 0;
        RaycastHit hit;

        for (int i = 0; i <2 ; i++)
        {
            if (Physics.Raycast(sensorsRSidePos, Quaternion.AngleAxis(sensorsAngle + i * sensorsAngle, sensorsPosition.up) * sensorsPosition.forward, out hit, sensorsLength))
            {
                avoiding = true;
                Debug.DrawLine(sensorsRSidePos, hit.point, Color.red);
                avoidForce -= 0.2f;
            }
            else { avoiding = false; }

            if (Physics.Raycast(sensorsLSidePos, Quaternion.AngleAxis(- sensorsAngle + i * -sensorsAngle, sensorsPosition.up) * sensorsPosition.forward, out hit, sensorsLength))
            {
                avoiding = true;
                Debug.DrawLine(sensorsLSidePos, hit.point, Color.red);
                avoidForce += 0.2f;
            }
            else { avoiding = false; }
        }

        return avoidForce;
    }
    

    //private void OnDrawGizmos()
    //{
    //    if (path != null)
    //    {
    //        for (int i = 0; i < path.Count; i++)
    //        {

    //            Gizmos.color = Color.green;
    //            if (path[i].worldPosition == currentNode.worldPosition)
    //            {
    //                Gizmos.color = Color.red;
    //            }
    //            Gizmos.DrawSphere(path[i].worldPosition, 0.7f);
    //        }
    //    }
    //}
    #endregion
}
