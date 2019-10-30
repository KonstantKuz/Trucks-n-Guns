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

    private Vector3 followingVector = Vector3.zero;

    private float targetForwardVelocity, zOffset_withVelocity, xOffset;

    public void AwakeEvent()
    {
        helicopterData.firePointType = (GameEnums.FirePointType)Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1);

        allFixedTicks.Add(this);
        _transform = transform;
        //_transform.position -= Vector3.forward * 150f;
        condition = GetComponent<EntityCondition>();
        condition.ResetCurrentCondition(helicopterData.maxCondition);
        condition.OnZeroCondition += ReturnObjectsToPool;
        helicopterData.PermanentSetUpHelicopter(this);

        InjectNewTargetData(GameObject.Find("PlayerTruckPreset(Clone)").GetComponent<Rigidbody>());
    }

    public void InjectNewTargetData(Rigidbody targetRigidbody)
    {
        targetData = new TargetData(targetRigidbody);

        for (int i = 0; i < firePoint.FirstTrackingGroupGuns.Count; i++)
        {
            firePoint.FirstTrackingGroupGuns[i].SetTargetData(targetData);
        }
    }
    public override void OnFixedTick()
    {
        RotateRotors();

        UpdatePosition();

        UpdateRotationAndAttack();
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

        _transform.position = Vector3.Lerp(_transform.position, followingVector, 0.02f);
    }

    private void UpdateRotationAndAttack()
    {
        followingVector = targetData.target_rigidbody.position - _transform.position;
        xOffset = followingVector.x;

        followingVector = Quaternion.LookRotation(followingVector).eulerAngles;

        targetForwardVelocity = targetData.target_rigidbody.velocity.z;
        zOffset_withVelocity = ((targetData.target_rigidbody.position.z + targetForwardVelocity) - _transform.position.z);

        if(targetForwardVelocity > 18f)
        {
            followingVector.x = zOffset_withVelocity * 0.25f;
            firePoint.FirstTrackingAttack();
        }
        else if (targetForwardVelocity > -5f && targetForwardVelocity < 18f)
        {
            followingVector.x -= zOffset_withVelocity * 0.05f;
            firePoint.FirstTrackingAttack();
        }
        else
        {
            followingVector.x = zOffset_withVelocity * 0.4f;
        }

        followingVector.y = 0;

        followingVector.z = -xOffset * 3f;

        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(followingVector), Time.deltaTime);
    }

    public void ReturnObjectsToPool()
    {
        allFixedTicks.Remove(this);
        if (condition != null)
        {
            condition.OnZeroCondition -= ReturnObjectsToPool;
        }
        helicopterData.ReturnObjectsToPool(this);
    }
}
