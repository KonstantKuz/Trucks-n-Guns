using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoCached, INeedTarget, IRoadEvent, IPoolReturner
{
    public bool isActive { get { return gameObject.activeInHierarchy; } set { } }

    public HelicopterData helicopterData;

    public Transform mainRotor, backRotor;

    public FirePoint firePoint { get; set; }
    public TargetData targetData { get; set; }

    private EntityCondition condition;

    public Transform _transform { get; set; }

    private Vector3 followingVector, rotorRotation;

    private float targetForwardVelocity, zOffset_withVelocity, xOffset;

    private bool canAttack;

    private void OnEnable()
    {
        customUpdates.Add(this);
    }
    private void OnDisable()
    {
        customUpdates.Remove(this);
    }

    public void AwakeEvent(Vector3 playerPosition)
    {
        RandomizeData();
        _transform = transform;
        _transform.position = playerPosition - new Vector3(0, 0, helicopterData.zSpawnOffset);
        condition = GetComponent<EntityCondition>();
        condition.OnZeroCondition += ReturnObjectsToPool;
        helicopterData.PermanentSetUpHelicopter(this);

        InjectNewTargetData(PlayerHandler.PlayerInstance.truck._rigidbody);

        condition.ResetCondition(EnemyHandler.EnemyConditionCalculatedFromPlayerLevelAndComplexity());
    }

    public void RandomizeData()
    {
        int randomFirePoint = EnemyHandler.RandomFirePointTypeFromComplexity();

        if (randomFirePoint < (int)GameEnums.FirePointType.D_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.D_FPType;
        }
        if (randomFirePoint > (int)GameEnums.FirePointType.DCMP_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.DCMP_FPType;
        }
        helicopterData.firePointType = (GameEnums.FirePointType)Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length);
        int[] gunDataTypes = EnemyHandler.PossibleGunDataTypesFromComplexity() ;

        for (int i = 0; i < helicopterData.firePointData.gunsConfigurations.Length; i++)
        {
            int randomGun = Random.Range(1, System.Enum.GetNames(typeof(GameEnums.Gun)).Length);
            int randomGunData = Random.Range(0, gunDataTypes.Length);
            helicopterData.firePointData.gunsConfigurations[i].gunType = (GameEnums.Gun)randomGun;
            helicopterData.firePointData.gunsConfigurations[i].gunDataType = (GameEnums.GunDataType)gunDataTypes[randomGunData];
        }
    }
    public void InjectNewTargetData(Rigidbody targetRigidbody)
    {
        targetData = new TargetData(targetRigidbody, targetRigidbody.GetComponent<EntityCondition>());
        targetData.target_condition.OnZeroCondition += ReturnObjectsToPool;
        firePoint.SetUpTargets(targetData, GameEnums.TrackingGroup.FirstTrackingGroup);
    }

    public override void CustomUpdate()
    {
        UpdatePosition();

        RotateRotors();

        UpdateRotation();

        Attack();
    }

    private void RotateRotors()
    {
        rotorRotation.x = 25;
        rotorRotation.y = 0;
        backRotor.localEulerAngles -= rotorRotation;
        rotorRotation.x = 0;
        rotorRotation.y = 25;
        mainRotor.localEulerAngles -= rotorRotation;
    }

    private void UpdatePosition()
    {
        followingVector.x = targetData.target_rigidbody.position.x;
        followingVector.z = targetData.target_rigidbody.position.z - helicopterData.offsetFromTarget.z;
        followingVector.y = helicopterData.offsetFromTarget.y;

        _transform.position = Vector3.Lerp(_transform.position, followingVector, Time.deltaTime);
    }

    private void UpdateRotation()
    {
        followingVector = targetData.target_rigidbody.position - _transform.position;
        xOffset = followingVector.x;

        followingVector = Quaternion.LookRotation(followingVector).eulerAngles;

        targetForwardVelocity = targetData.target_rigidbody.velocity.z;
        zOffset_withVelocity = ((targetData.target_rigidbody.position.z + targetForwardVelocity) - _transform.position.z);

        if (targetForwardVelocity > 5f)
        {
            followingVector.x -= zOffset_withVelocity * 0.1f;
            canAttack = true;
        }
        else
        {
            followingVector.x = zOffset_withVelocity * 0.4f;
            canAttack = false;
        }

        followingVector.y = 0;

        followingVector.z = -xOffset * 3f;

        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(followingVector), Time.deltaTime);
    }

    private void Attack()
    {
        if(canAttack)
        {
            firePoint.FirstTrackingAttack();
        }
    }

    public void ReturnObjectsToPool()
    {
        targetData.target_condition.OnZeroCondition -= ReturnObjectsToPool;
        if (condition != null)
        {
            condition.OnZeroCondition -= ReturnObjectsToPool;
        }
        helicopterData.ReturnObjectsToPool(this);
    }
}
