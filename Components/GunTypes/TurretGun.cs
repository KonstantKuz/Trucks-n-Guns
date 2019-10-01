using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGun : GunParent
{
    [SerializeField]
    private GunData gunDataToCopy;

    public override GunData myData { get; set; }
    public override TargetData targetData { get; set; }

    public Transform head, headHolder;
    public Transform[] gunsBarrels;

    private Quaternion bulletSpreadedRotation, rotationFromCurve;
    private Vector3[] gunsBarrelsUpDirections;

    private Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    private Transform headHolderRoot;

    private ObjectPoolerBase effectPooler, battleUnitPooler;

    public override void SetTargetData(TargetData targetData)
    {
        this.targetData = targetData;
    }

    private void Awake()
    {
        myData = Instantiate(gunDataToCopy);
        myData.CreateBattleUnitInstance();

        headHolderRoot = headHolder.root;

        gunsBarrelsUpDirections = new Vector3[gunsBarrels.Length];
        for (int i = 0; i < gunsBarrels.Length; i++)
        {
            gunsBarrelsUpDirections[i] = gunsBarrels[i].transform.up;
        }

        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
        battleUnitPooler = ObjectPoolersHolder.Instance.BattleUnitPooler;
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
            myData.timeSinceLastShot = myData.rateofFire;
            for (int i = 0; i < gunsBarrels.Length; i++)
            {
                battleUnitPooler.SpawnFromPool(myData.battleUnitToCopy.name, gunsBarrels[i].position, bulletSpreadedRotation);
                effectPooler.SpawnFromPool("TurretFlash", gunsBarrels[i].transform.position, gunsBarrels[i].transform.rotation).GetComponent<ParticleSystem>().Play();
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
        targetDirection = (targetData.target_rigidbody.position + targetData.target_rigidbody.velocity * 0.1f) - headHolder.position;

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.right);

        float headForwardAngle = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, headHolder.forward, headHolder.right);
        float headRightAngle = Vector3.Angle(targetDirection_XZprojection.normalized, headHolderRoot.forward);

        if(headRightAngle<120)
        {
            headHolder.rotation = Quaternion.LookRotation(targetDirection_XZprojection, headHolder.up);

            if (headForwardAngle < 60 && headForwardAngle > -20 && headRightAngle < 120)
            {
                head.rotation = Quaternion.LookRotation(targetDirection_ZYprojection, headHolder.up);
            }
        }
    }

    void UpdateTimers()
    {
        float randomDifference = Random.Range(0.01f, 0.05f);
        myData.timeElapsed += randomDifference;
        myData.timeSinceLastShot -= randomDifference;
    }
   
}
