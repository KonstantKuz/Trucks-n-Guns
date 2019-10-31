using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGun : GunParent
{
    [SerializeField]
    private GunData gunDataToCopy;

    public override GunData myData { get; set; }
    public override TargetData targetData { get; set; }
    public override GunAnglesData allowableAngles { get; set; }

    public Transform head, headHolder;
    public Transform[] gunsBarrels;

    private Quaternion bulletSpreadedRotation, rotationFromCurve;
    private Vector3[] gunsBarrelsUpDirections;

    private Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    private Transform headHolderRoot;

    private ObjectPoolerBase effectPooler, battleUnitPooler;

    private void Awake()
    {
        myData = Instantiate(gunDataToCopy);
        myData.CreateBattleUnitInstance();

        headHolderRoot = headHolder.parent;

        gunsBarrelsUpDirections = new Vector3[gunsBarrels.Length];
        for (int i = 0; i < gunsBarrels.Length; i++)
        {
            gunsBarrelsUpDirections[i] = gunsBarrels[i].transform.up;
        }

        SetUpAngles(null);

        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
        battleUnitPooler = ObjectPoolersHolder.Instance.BattleUnitPooler;
    }

    public override void SetTargetData(TargetData targetData)
    {
        this.targetData = targetData;
    }

    public override void SetUpAngles(GunAnglesData anglesData)
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
    public override void Fire()
    {
        UpdateTimers();

        if (myData.battleType == GameEnums.BattleType.Tracking && targetData.target_rigidbody != null)
        {
            LookAtTarget();
        }

        CalculateBulletsSpread();

        if (myData.timeSinceLastShot <= 0)
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Play();
            }
            myData.timeSinceLastShot = myData.rateofFire;
            for (int i = 0; i < gunsBarrels.Length; i++)
            {
                battleUnitPooler.Spawn(myData.battleUnitToCopy.name, gunsBarrels[i].position, bulletSpreadedRotation);
                effectPooler.Spawn("TurretFlash", gunsBarrels[i].transform.position, gunsBarrels[i].transform.rotation).GetComponent<ParticleSystem>().Play();
            }
        }
    }

    void CalculateBulletsSpread()
    {
        float nextSpreadStep = myData.spreadCurve.Evaluate(myData.timeElapsed) * myData.spreadForce;
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
            }
        }
    }

    void UpdateTimers()
    {
        float randomDifference = Random.Range(0.001f, 0.05f);
        myData.timeElapsed += randomDifference;
        myData.timeSinceLastShot -= randomDifference;
    }
   
}
