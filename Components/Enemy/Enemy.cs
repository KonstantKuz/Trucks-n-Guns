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
    
    public static void CreateStateDictionary()
    {
        EnemyStateDictionary.CreateStateDictionary();
    }
    private void OnEnable()
    {
        customUpdates.Add(this);
    }
    private void OnDisable()
    {
        customUpdates.Remove(this);
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
        int randomTruck = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.Truck)).Length);
        truck.TruckData.truckType = (GameEnums.Truck)randomTruck;

        int playersFirePointType = (int)PlayerHandler.PlayerInstance.truck.TruckData.firePointType;

        int randomFirePoint = Random.Range(playersFirePointType-2, playersFirePointType+2);
        if(randomFirePoint < (int)GameEnums.FirePointType.D_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.D_FPType;
        }
        if(randomFirePoint > (int)GameEnums.FirePointType.DCMP_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.DCMP_FPType;
        }
        truck.TruckData.firePointType = (GameEnums.FirePointType)randomFirePoint;
        int[] gunDataTypes = { 213, 223, 233, 313, 323, 333, 212, 222, 232, 312, 322, 332 };
        for (int i = 0; i < truck.TruckData.firePointData.gunsConfigurations.Length; i++)
        {
            int randomGun = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.Gun)).Length);
            int randomGunData = Random.Range(0, gunDataTypes.Length);
            truck.TruckData.firePointData.gunsConfigurations[i].gunType = (GameEnums.Gun)randomGun;
            truck.TruckData.firePointData.gunsConfigurations[i].gunDataType = (GameEnums.GunDataType)gunDataTypes[randomGunData];
        }
    }

    #region My

    public void InjectNewTargetData(Rigidbody targetRigidbody)
    {
        targetData = new TargetData(targetRigidbody, null);
        truck.firePoint.SetUpTargets(targetData, GameEnums.TrackingGroup.FirstTrackingGroup);
    }

    public override void CustomUpdate()
    {
        followTypeStateController.UpdateMachine();
        AttackTarget();
    }

    public void AttackTarget()
    {
        if((targetData.target_rigidbody.position - truck._transform.position).magnitude < 50f)
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

        if (Physics.Raycast(sensorsRSidePos, Quaternion.AngleAxis(sensorsAngle, sensorsPosition.up) * sensorsPosition.forward, sensorsLength))
        {
            avoiding = true;
            avoidForce -= 0.2f;
        }
        else { avoiding = false; }

        if (Physics.Raycast(sensorsLSidePos, Quaternion.AngleAxis(-sensorsAngle, sensorsPosition.up) * sensorsPosition.forward, sensorsLength))
        {
            avoiding = true;
            avoidForce += 0.2f;
        }
        else { avoiding = false; }

        return avoidForce;
    }
    
    #endregion
}
