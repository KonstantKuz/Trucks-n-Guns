using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGun : GunParent
{
    [SerializeField]
    private GunData gunDataToCopy;

    public override GunData myData { get; set; }
    public override TargetData targetData { get; set; }

    public Transform head, platform;
    public Transform[] gunsBarrels;

    private Quaternion bulletSpreadedRotation, rotationFromCurve;
    private Vector3[] gunsBarrelsUpDirections;
    private Vector3 tempTargetForPlatform = Vector3.zero, tempTargetForHead = Vector3.zero;

    private ObjectPoolerBase effectPooler, battleUnitPooler;

    public override void SetTargetData(TargetData targetData)
    {
        this.targetData = targetData;
    }

    private void Awake()
    {
        myData = Instantiate(gunDataToCopy);
        myData.CreateBattleUnitInstance();

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
        CalculateBulletsSpread();

        if(myData.battleType == GameEnums.BattleType.Tracking && targetData.target_rigidbody != null)
        {
            LookAtTarget();
        }

        UpdateTimers();
       
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
        tempTargetForHead = (targetData.target_rigidbody.position + targetData.target_rigidbody.velocity * 0.1f) - platform.position;

        tempTargetForPlatform = Vector3.ProjectOnPlane(tempTargetForHead, platform.up);
        
        head.rotation = Quaternion.LookRotation(tempTargetForHead, platform.up);
        platform.rotation = Quaternion.LookRotation(tempTargetForPlatform, platform.up);
    }

    void UpdateTimers()
    {
        myData.timeElapsed += Time.deltaTime;
        myData.timeSinceLastShot -= Time.deltaTime + Random.Range(0.01f, 0.005f);
    }
   
}
