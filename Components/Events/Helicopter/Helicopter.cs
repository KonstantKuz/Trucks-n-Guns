using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoCached, INeedTarget, IRoadEvent, IPoolReturner
{
    public bool isActive { get { return gameObject.activeInHierarchy; } set { } }

    public HelicopterData helicopterData;

    public Transform mainRotor, backRotor;

    public FirePoint firePoint { get; set; }
    public TargetData targetData { get ; set ; }

    private EntityCondition condition;

    public Transform _transform { get; set; }

    private Vector3 followingVector;

    private float targetForwardVelocity, zOffset_withVelocity, xOffset;

    private bool canAttack;

    public void AwakeEvent(Vector3 playerPosition)
    {

        helicopterData.firePointType = (GameEnums.FirePointType)Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1);

        allFixedTicks.Add(this);
        allTicks.Add(this);
        _transform = transform;
        _transform.position = playerPosition - new Vector3(0,0, helicopterData.zSpawnOffset);
        //_transform.position -= Vector3.forward * 150f;
        condition = GetComponent<EntityCondition>();
        condition.ResetCurrentCondition(helicopterData.maxCondition);
        condition.OnZeroCondition += ReturnObjectsToPool;
        helicopterData.PermanentSetUpHelicopter(this);

        GameObject player = GameObject.Find("PlayerTruckPreset(Clone)");

        InjectNewTargetData(player.GetComponent<Rigidbody>());
    }

    public void InjectNewTargetData(Rigidbody targetRigidbody)
    {
        targetData = new TargetData(targetRigidbody, null);

        firePoint.SetUpTargets(targetData, GameEnums.TrackingGroup.FirstTrackingGroup);
    }

    public override void OnTick()
    {
        RotateRotors();

        UpdateRotation();

        Attack();
    }

    public override void OnFixedTick()
    {
        UpdatePosition();
    }

    private void RotateRotors()
    {
        mainRotor.RotateAround(mainRotor.position, mainRotor.up, -25f);
        backRotor.RotateAround(backRotor.position, backRotor.right, 25f);
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
            followingVector.x -= zOffset_withVelocity * 0.05f;
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
        allFixedTicks.Remove(this);
        allTicks.Remove(this);
        if (condition != null)
        {
            condition.OnZeroCondition -= ReturnObjectsToPool;
        }
        helicopterData.ReturnObjectsToPool(this);
    }
}
