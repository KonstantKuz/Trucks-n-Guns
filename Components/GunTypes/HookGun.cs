using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookGun : MonoCached, Gun
{
    public GunData gunData { get ; set ; }
    public TargetData targetData { get ; set ; }
    public GunAnglesData allowableAngles { get ; set ; }
    public GameEnums.BattleType battleType { get; set; }

    public Transform[] gunsBarrels;
    private Quaternion bulletSpreadedRotation, rotationFromCurve;
    private Vector3[] gunsBarrelsUpDirections;
    private float nextSpreadStep;

    private float fireTimer;

    private ObjectPoolerBase battleUnitPooler;

    private void Awake()
    {
        gunsBarrelsUpDirections = new Vector3[gunsBarrels.Length];
        for (int i = 0; i < gunsBarrels.Length; i++)
        {
            gunsBarrelsUpDirections[i] = gunsBarrels[i].transform.up;
        }

        SetAnglesData(null);

        battleUnitPooler = ObjectPoolersHolder.Instance.BattleUnitPooler;
    }

    public void SetGunData(GameEnums.GunDataType gunData)
    {
        this.gunData = DataReturnersHolder.Instance.TurretGunDataReturner.GetData(gunData.ToString()) as GunData;
    }

    public void SetTargetData(TargetData targetData)
    {
        this.targetData = targetData;
    }

    public void SetAnglesData(GunAnglesData anglesData)
    {
        if (anglesData == null)
        {
            allowableAngles = ScriptableObject.CreateInstance<GunAnglesData>();

            allowableAngles.HeadHolderMaxAngle = GunAnglesData.headHolderMaxAngle;
            allowableAngles.HeadMaxAngle = GunAnglesData.headMaxAngle;
            allowableAngles.HeadMinAngle = GunAnglesData.headMinAngle;
            allowableAngles.StartDirectionAngle = 0;
        }
        else
        {
            allowableAngles = anglesData;
        }

        transform.localEulerAngles = new Vector3(0, allowableAngles.StartDirectionAngle, 0);
    }

    public void Fire()
    {
        if (Time.time >= fireTimer)
        {
            fireTimer = Time.time + gunData.rateofFire + Random.Range(0.01f, 0.05f);

            for (int i = 0; i < gunsBarrels.Length; i++)
            {
                battleUnitPooler.Spawn(gunData.battleUnit, gunsBarrels[i].position, bulletSpreadedRotation);
            }
        }

        CalculateBulletsSpread();
    }

    void CalculateBulletsSpread()
    {
        nextSpreadStep = gunData.spreadCurve.Evaluate(fireTimer) * gunData.spreadForce;
        for (int i = 0; i < gunsBarrels.Length; i++)
        {
            rotationFromCurve = Quaternion.AngleAxis(nextSpreadStep, gunsBarrelsUpDirections[i]);
            bulletSpreadedRotation = gunsBarrels[i].rotation * rotationFromCurve;
        }
    }
}
