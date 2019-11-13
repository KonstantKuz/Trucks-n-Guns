using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Truck : MonoCached
{
    [SerializeField]
    private TruckData truckDataToCopy;

    public TruckData TruckData { get; set; }

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

    private Quaternion clampedRotation;

    private AudioSource engineSoundSource;

    public override void CustomUpdate()
    {
        clampedRotation = _transform.rotation;
        clampedRotation.x = Mathf.Clamp(clampedRotation.x, -0.15f, 0.15f);
        clampedRotation.y = Mathf.Clamp(clampedRotation.y, -0.4f, 0.4f);
        clampedRotation.z = Mathf.Clamp(clampedRotation.z, -0.15f, 0.15f);
        _transform.rotation = clampedRotation;
    }

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
        engineSoundSource = GetComponent<AudioSource>();
    }

    private void SetUpData()
    {
        if(!ReferenceEquals(truckDataToCopy, null))
        {
            TruckData = truckDataToCopy;
        }

        trucksCondition.maxCondition = TruckData.maxTrucksCondition;
        trucksCondition.ResetCurrentCondition(TruckData.maxTrucksCondition);

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

        if (visual.childCount > 5)
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

        rearWheels[0].transform.position = rearWheelsVisual[0].position;
        rearWheels[1].transform.position = rearWheelsVisual[1].position;
        //rearWheelsVisual[0].transform.parent = rearWheels[0].transform;
        //rearWheelsVisual[1].transform.parent = rearWheels[1].transform;
        //rearWheelsVisual[0].localPosition = Vector3.zero;
        //rearWheelsVisual[1].localPosition = Vector3.zero;
        if (rearWheelsVisual.Count > 2)
        {
            rearWheels[2].transform.position = rearWheelsVisual[2].position;
            rearWheels[3].transform.position = rearWheelsVisual[3].position;
            //rearWheelsVisual[2].transform.parent = rearWheels[2].transform;
            //rearWheelsVisual[3].transform.parent = rearWheels[3].transform;
            //rearWheelsVisual[2].localPosition = Vector3.zero;
            //rearWheelsVisual[3].localPosition = Vector3.zero;
        }

        //frontWheelsVisual[0].transform.parent = frontWheels[0].transform;
        //frontWheelsVisual[1].transform.parent = frontWheels[1].transform;
        //frontWheelsVisual[0].localPosition = Vector3.zero;
        //frontWheelsVisual[1].localPosition = Vector3.zero;
    }
    
    public void LaunchTruck()
    {
        _rigidbody.drag = 0.05f;
    }

    public void Moving(float torqueForce)
    {
        foreach (RaycastWheel drivingWheel in drivingWheels)
        {
            drivingWheel.motorTorque = TruckData.maxMotorTorque *( torqueForce - Mathf.Abs(CurrentSteerAngle()) * 0.05f);
        }
        UpdateEngineSound();
    }

    public void Steering(float steeringForce)
    {
        foreach (RaycastWheel frontWheel in frontWheels)
        {
            frontWheel.steerAngle = Mathf.Lerp(CurrentSteerAngle(), steeringForce* TruckData.maxSteerAngle, TruckData.wheelSteeringSpeed );
        }
        UpdateWheelsVisual();
    }

    public void SetBoost(float force)
    {
        if(drivingWheels[0].IsGrounded || drivingWheels[1].IsGrounded)
        {
            if (force > 0 && CurrentSpeed() < 120f)
            {
                _rigidbody.AddForce(_transform.forward * force, ForceMode.Acceleration);
            }
            if (force < 0 && CurrentSpeed() > -30f)
            {
                _rigidbody.AddForce(_transform.forward * force, ForceMode.Acceleration);
            }
        }
       
        //if (CurrentSpeed() < 120f && CurrentSpeed() > -30f && drivingWheels[0].IsGrounded)
        //{
        //    _rigidbody.AddForce(_transform.forward * force, ForceMode.Acceleration);
        //}
    }

    public void StopTruck(float force)
    {
        _rigidbody.drag = force;
    }

    public void StopTruck()
    {
        _rigidbody.drag = 2f;

        foreach (RaycastWheel drivingWheel in drivingWheels)
        {
            drivingWheel.motorTorque = 0;
        }
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

    private void UpdateEngineSound()
    {
        float engineSoundPitch = _rigidbody.velocity.magnitude * 0.05f;
        engineSoundSource.pitch = Mathf.Clamp(engineSoundPitch, 0.9f, 2f);
    }

    public float CurrentSpeed()
    {
        return _rigidbody.velocity.magnitude * 3.6f * Mathf.Sign(_rigidbody.velocity.z);
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