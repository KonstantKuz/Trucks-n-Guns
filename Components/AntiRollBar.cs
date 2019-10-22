using UnityEngine;
using System.Collections;

public class AntiRollBar : MonoCached
{
    public RaycastWheel WheelL, WheelR;
    public float AntiRoll = 5000.0f;

    private Rigidbody carRigidBody;
    private Transform WheelL_transform, WheelR_transform;
    void OnEnable()
    {
        carRigidBody = GetComponentInParent<Rigidbody>();
        WheelL_transform = WheelL.transform;
        WheelR_transform = WheelR.transform;
    }

    public override void OnFixedTick()
    {
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WheelL.IsGrounded;

        if (groundedL)
        {
            travelL = (-WheelL_transform.InverseTransformPoint(WheelL.Hit.point).y
                    - WheelL.wheelRadius) / WheelL.suspensionRange;
        }

        bool groundedR = WheelR.IsGrounded;

        if (groundedR)
        {
            travelR = (-WheelR_transform.InverseTransformPoint(WheelR.Hit.point).y
                    - WheelR.wheelRadius) / WheelR.suspensionRange;
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