using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGun : MonoCached, Gun
{
    public GunData gunData { get; set; }
    public TargetData targetData { get; set; }
    public GunAnglesData allowableAngles { get; set; }
    public GameEnums.BattleType battleType { get; set; }

    public Transform head, headHolder;
    public Transform[] gunsBarrels;

    [SerializeField]
    private CachedParticles[] fireEffect;
    
    private Quaternion bulletSpreadedRotation, rotationFromCurve;
    private Vector3[] gunsBarrelsUpDirections;

    private Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    private Transform headHolderRoot;

    private bool targetIsVisible = true;
    private float fireTimer;

    private float nextSpreadStep;

    private ObjectPoolerBase battleUnitPooler;

    private AudioSource turretShotSource;

    private void Awake()
    {
        headHolderRoot = headHolder.parent;

        gunsBarrelsUpDirections = new Vector3[gunsBarrels.Length];
        for (int i = 0; i < gunsBarrels.Length; i++)
        {
            gunsBarrelsUpDirections[i] = gunsBarrels[i].transform.up;
        }

        turretShotSource = GetComponent<AudioSource>();

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
        targetIsVisible = true;
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

        head.localEulerAngles = Vector3.zero;
        headHolder.localEulerAngles = Vector3.zero;

        transform.localEulerAngles = new Vector3(0, allowableAngles.StartDirectionAngle, 0);
    }

    public void Fire()
    {
        if(Time.time >= fireTimer && targetIsVisible)
        {
            fireTimer = Time.time + gunData.rateofFire + Random.Range(0.01f, 0.05f);

            for (int i = 0; i < gunsBarrels.Length; i++)
            {
                battleUnitPooler.Spawn(gunData.battleUnit, gunsBarrels[i].position, bulletSpreadedRotation);
                fireEffect[i].PlayParticles();
            }

            //turretShotSource.PlayOneShot(turretShotSource.clip);
            if (!turretShotSource.isPlaying)
            {
                turretShotSource.Play();
            }
        }

        if (battleType == GameEnums.BattleType.Tracking)
        {
            LookAtTarget();
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

    void LookAtTarget()
    {
        targetDirection = (targetData.target_rigidbody.position + targetData.target_rigidbody.velocity * 0.1f - headHolderRoot.up) - headHolder.position;

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.right);

        float angleBtwn_targetDirZY_hhFWD = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, headHolder.forward, headHolder.right);
        float angleBtwn_targetDirXZ_hhrootFWD = Vector3.Angle(targetDirection_XZprojection.normalized, headHolderRoot.forward);

        if(angleBtwn_targetDirXZ_hhrootFWD < allowableAngles.HeadHolderMaxAngle)
        {
            headHolder.rotation = Quaternion.LookRotation(targetDirection_XZprojection, headHolder.up);

            if (angleBtwn_targetDirZY_hhFWD < allowableAngles.HeadMaxAngle && angleBtwn_targetDirZY_hhFWD > allowableAngles.HeadMinAngle)
            {
                head.rotation = Quaternion.LookRotation(targetDirection_ZYprojection, headHolder.up);
                targetIsVisible = true;
            }
            else
            {
                targetIsVisible = false;
            }
        }
        else
        {
            targetIsVisible = false;
        }
    }

}
