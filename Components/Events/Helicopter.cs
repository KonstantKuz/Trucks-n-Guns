using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoCached, INeedTarget, IRoadEvent
{
    public HelicopterData helicopterData;

    public FirePoint firePoint { get; set; }
    public TargetData targetData { get ; set ; }

    private EntityCondition condition;

    private Transform _transform;

    private Vector3 followingVector = Vector3.zero;

    public void AwakeEvent()
    {
        //throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        allFixedTicks.Add(this);
        _transform = transform;
        condition = GetComponent<EntityCondition>();
        condition.ResetCurrentCondition(helicopterData.maxCondition);
        helicopterData.PermanentSetUpHelicopter(this);

        InjectNewTargetData(GameObject.Find("PlayerTruckPreset(Clone)").GetComponent<Rigidbody>());
    }

    private void OnDisable()
    {
        allFixedTicks.Remove(this);
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
        followingVector.x = targetData.target_rigidbody.position.x;
        followingVector.z = targetData.target_rigidbody.position.z - helicopterData.offsetFromTarget.z;
        followingVector.y = helicopterData.offsetFromTarget.y;

        _transform.position = Vector3.Slerp(_transform.position, followingVector, targetData.target_rigidbody.velocity.magnitude*0.0005f);

        firePoint.FirstTrackingAttack();
    }

}
