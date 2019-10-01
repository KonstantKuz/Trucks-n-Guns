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

    private List<Transform> frontWheelsVisual;
    private List<Transform> rearWheelsVisual;

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
        TruckData.firePointData = Instantiate(TruckData.firePointDataToCopy);
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

    public void SetUpWheelsVisual(Transform visual)
    {
        frontWheelsVisual = new List<Transform>();
        rearWheelsVisual = new List<Transform>();

        frontWheelsVisual.Add(visual.GetChild(0));
        frontWheelsVisual.Add(visual.GetChild(1));

        rearWheelsVisual.Add(visual.GetChild(2));
        rearWheelsVisual.Add(visual.GetChild(3));

        if (visual.childCount > 4)
        {
            rearWheelsVisual.Add(visual.GetChild(4));
            rearWheelsVisual.Add(visual.GetChild(5));
        }

        //for (int i = 0; i < frontWheelsVisual.Count; i++)
        //{
        //    frontWheels[i]._transform.position = frontWheelsVisual[i].position;
        //}
        //for (int i = 0; i < rearWheelsVisual.Count; i++)
        //{
        //    rearWheels[i]._transform.position = rearWheelsVisual[i].position;
        //}

        frontWheels[0].transform.position = frontWheelsVisual[0].position;
        frontWheels[1].transform.position = frontWheelsVisual[1].position;

        frontWheelsVisual[0].transform.parent = frontWheels[0].transform;
        frontWheelsVisual[1].transform.parent = frontWheels[1].transform;
        frontWheelsVisual[0].localPosition = Vector3.zero;
        frontWheelsVisual[1].localPosition = Vector3.zero;
    }

    public void UpdateWheelsVisual()
    {
        for (int i = 0; i < frontWheelsVisual.Count; i++)
        {
            frontWheels[i].VisualWheelSync(frontWheelsVisual[i]);
        }
        for (int i = 0; i < rearWheelsVisual.Count; i++)
        {
            rearWheels[i].VisualWheelSync(rearWheelsVisual[i]);
        }

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
        UpdateWheelsVisual();
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