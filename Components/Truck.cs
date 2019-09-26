using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Truck : MonoCached
{

    [SerializeField]
    private TruckData truckDataToCopy;

    public TruckData TruckData { get; private set; }

    public FirePoint firePoint { get; set; }

    public EntityCondition trucksCondition { get; private set; }

    [SerializeField]
    private RaycastWheel[] frontWheels, rearWheels;

    [SerializeField]
    private Transform centerOfMass;

    private List<RaycastWheel> drivingWheels;
    private Transform[] wheelsVisual;

    public Rigidbody _rigidbody { get; private set; }
    public Transform _transform { get; private set; }
    public Transform CenterOfMass { get { return centerOfMass; } }

    public void SetUpTruck()
    {
        SetUpCash();
        SetUpData();
        SetUpWheels();
    }
    
    private void SetUpCash()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;
        trucksCondition = GetComponent<EntityCondition>();
    }

    private void SetUpData()
    {
        TruckData = Instantiate(truckDataToCopy);
        trucksCondition.maxCondition = TruckData.maxTrucksCondition;
        TruckData.SetUpTruck(this);
        if (firePoint == null)
        {
            firePoint = GetComponentInChildren<FirePoint>();
        }
    }

    public void SetUpWheels()
    {
        switch (TruckData.driveType)
        {
            case GameEnums.DriveType.FrontWheelDrive:
                drivingWheels = new List<RaycastWheel>(frontWheels.Length);
                foreach (RaycastWheel frontWheel in frontWheels)
                {
                    drivingWheels.Add(frontWheel);
                }
                break;
            case GameEnums.DriveType.RearWheelDrive:
                drivingWheels = new List<RaycastWheel>(rearWheels.Length);
                foreach (RaycastWheel rearWheel in rearWheels)
                {
                    drivingWheels.Add(rearWheel);
                }
                break;
            case GameEnums.DriveType.AllWheelDrive:
                drivingWheels = new List<RaycastWheel>(frontWheels.Length + rearWheels.Length);
                foreach (RaycastWheel frontWheel in frontWheels)
                {
                    drivingWheels.Add(frontWheel);
                }
                foreach (RaycastWheel rearWheel in rearWheels)
                {
                    drivingWheels.Add(rearWheel);
                }
                break;
        }
    }

    public void SetUpFrontWheelsVisual(Transform visual)
    {
        //wheelsVisual = new Transform[6];
        //for (int i = 0; i < visual.childCount; i++)
        //{
        //    wheelsVisual[i] = visual.GetChild(i);
        //}
        //wheelsVisual[0] = visual.GetChild(0);
        //wheelsVisual[1] = visual.GetChild(1);
        //wheelsVisual[0].parent = frontWheels[0].transform;
        //wheelsVisual[1].parent = frontWheels[1].transform;
        //wheelsVisual[0].localPosition = Vector3.zero;
        //wheelsVisual[1].localPosition = Vector3.zero;
    }

    public void UpdateFrontWheelsVisuals()
    {
        //frontWheels[0].VisualWheelSync(wheelsVisual[0]);
        //frontWheels[1].VisualWheelSync(wheelsVisual[1]);
        //rearWheels[0].VisualWheelSync(wheelsVisual[2]);
        //rearWheels[1].VisualWheelSync(wheelsVisual[3]);
        //if(!ReferenceEquals(rearWheels[2], null))
        //{
        //    rearWheels[2].VisualWheelSync(wheelsVisual[4]);
        //    rearWheels[3].VisualWheelSync(wheelsVisual[5]);
        //}
    }

    public void LaunchTruck()
    {
        _rigidbody.drag = 0.05f;
    }

    public void MoveTruck(float torqueForce)
    {
        //_rigidbody.drag = Mathf.Abs(CurrentSteerAngle()) * 0.001f;
        foreach (RaycastWheel drivingWheel in drivingWheels)
        {
            drivingWheel.motorTorque = TruckData.maxMotorTorque *( torqueForce - Mathf.Abs(CurrentSteerAngle()) * 0.01f);
        }
    }

    public void SteeringWheels(float steeringForce)
    {
        foreach (RaycastWheel frontWheel in frontWheels)
        {
            frontWheel.steerAngle = Mathf.Lerp(CurrentSteerAngle(), steeringForce* TruckData.maxSteerAngle, TruckData.wheelSteeringSpeed );
        }
        UpdateFrontWheelsVisuals();
    }

    public void SetBoost(float force)
    {
        _rigidbody.AddForce(_transform.forward * force, ForceMode.Acceleration);

        //if (CurrentSpeed()<70f && force>0)
        //{
        //    _rigidbody.AddForce(_transform.forward * force, ForceMode.Acceleration);
        //}
        //if(RelativeVelocity() < 5f && force <0)
        //{
        //    _rigidbody.AddForce(_transform.forward * force, ForceMode.Acceleration);
        //}
    }

    public void StopTruckSlow(float force)
    {
        _rigidbody.drag -= force;
    }

    public void StopTruck()
    {
        _rigidbody.drag = 2f;

        foreach (RaycastWheel drivingWheel in drivingWheels)
        {
            drivingWheel.motorTorque = 0;
        }
    }


    public float CurrentSpeed()
    {
        return _rigidbody.velocity.magnitude * 3.6f;
    }
    public float RelativeVelocity()
    {
        return Vector3.Dot(Vector3.forward, _rigidbody.velocity);
    }
    public float CurrentSteerAngle()
    {
       return frontWheels[0].steerAngle;
    }
    
    public float CurrentXAcceleration()
    {
        return _rigidbody.velocity.x /Time.deltaTime;
    }
}