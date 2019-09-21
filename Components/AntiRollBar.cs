using UnityEngine;
using System.Collections;

public class AntiRollBar : MonoCached
{
    public WheelCollider WheelL, WheelR;
    public float AntiRoll = 5000.0f;

    private Rigidbody carRigidBody;
    private WheelHit hit;
    private Transform WheelL_transform, WheelR_transform;
    void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
        hit = new WheelHit();
        WheelL_transform = WheelL.transform;
        WheelR_transform = WheelR.transform;
    }

    public override void OnFixedTick()
    {
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WheelL.GetGroundHit(out hit);

        if (groundedL)
        {
            travelL = (-WheelL_transform.InverseTransformPoint(hit.point).y
                    - WheelL.radius) / WheelL.suspensionDistance;
        }

        bool groundedR = WheelR.GetGroundHit(out hit);

        if (groundedR)
        {
            travelR = (-WheelR_transform.InverseTransformPoint(hit.point).y
                    - WheelR.radius) / WheelR.suspensionDistance;
        }

        var antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            carRigidBody.AddForceAtPosition(WheelL_transform.up * -antiRollForce,
                WheelL_transform.position);
        if (groundedR)
            carRigidBody.AddForceAtPosition(WheelR_transform.up * antiRollForce,
                WheelR_transform.position);
    }

}