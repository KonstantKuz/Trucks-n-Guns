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
        customFixedUpdates.Add(this);
    }
    private void OnDisable()
    {
        customUpdates.Remove(this);
        customFixedUpdates.Remove(this);
    }

    public void AwakeEvent(Vector3 playerPosition)
    {
        RandomizeData();
        _transform = transform;
        _transform.position = playerPosition - new Vector3(0, 0, helicopterData.zSpawnOffset);
        condition = GetComponent<EntityCondition>();
        condition.OnZeroCondition += ReturnObjectsToPool;
        helicopterData.PermanentSetUpHelicopter(this);

        GameObject player = GameObject.Find("PlayerTruckPreset(Clone)");
        if(!ReferenceEquals(player,null))
        {
            InjectNewTargetData(player.GetComponent<Rigidbody>());
        }
        condition.ResetCondition(helicopterData.maxCondition);

    }
    public void RandomizeData()
    {
        int playersFirePointType = (int)PlayerHandler.PlayerInstance.truck.TruckData.firePointType;

        int randomFirePoint = Random.Range(playersFirePointType - 2, playersFirePointType + 1);
        if (randomFirePoint < (int)GameEnums.FirePointType.D_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.D_FPType;
        }
        if (randomFirePoint > (int)GameEnums.FirePointType.DCMP_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.DCMP_FPType;
        }
        helicopterData.firePointType = (GameEnums.FirePointType)Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length);
        int[] gunDataTypes = { 222, 232, 223, 233 };
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
        RotateRotors();

        UpdateRotation();

        Attack();
    }

    public override void CustomFixedUpdate()
    {
        UpdatePosition();
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
        if (CheckTarget())
        {
            followingVector.x = targetData.target_rigidbody.position.x;
            followingVector.z = targetData.target_rigidbody.position.z - helicopterData.offsetFromTarget.z;
            followingVector.y = helicopterData.offsetFromTarget.y;

            _transform.position = Vector3.Lerp(_transform.position, followingVector, Time.deltaTime);
        }
    }

    private void UpdateRotation()
    {
        if(CheckTarget())
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
       
    }
    private bool CheckTarget()
    {
        return !ReferenceEquals(targetData.target_rigidbody, null) && targetData.target_rigidbody.gameObject.activeInHierarchy;
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
