using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGun : GunParent
{
    [SerializeField]
    private GunData gunDataToCopy;

    public override GunData myData { get; set; }
    public override TargetData targetData { get; set; }
    public override GunAnglesData allowableAngles { get; set; }
    
    public Transform head, headHolder;
    public Transform[] gunsBarrels;

    private Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    private Transform headHolderRoot;

    private ObjectPoolerBase battleUnitPooler;

    private void Awake()
    {
        myData = Instantiate(gunDataToCopy);
        myData.CreateBattleUnitInstance();
        battleUnitPooler = ObjectPoolersHolder.Instance.BattleUnitPooler;
        headHolderRoot = headHolder.parent;
    }

    public override void SetTargetData(TargetData targetData)
    {
        switch (myData.battleType)
        {
            case GameEnums.BattleType.Tracking:
                this.targetData = targetData;
                break;
            case GameEnums.BattleType.Static:
                this.targetData = null;
                break;
        }
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

        transform.localEulerAngles = new Vector3(0, allowableAngles.StartDirectionAngle, 0);
    }

    public override void Fire()
    {
        if (myData.battleType == GameEnums.BattleType.Tracking)
        {
            LookAtTarget();
        }

        UpdateTimers();

        if (myData.timeSinceLastShot <= 0)
        {
           
            myData.timeSinceLastShot = myData.rateofFire;

            for (int i = 0; i < gunsBarrels.Length; i++)
            {
                battleUnitPooler.Spawn(myData.battleUnitToCopy.name, gunsBarrels[i].position, gunsBarrels[i].rotation).GetComponent<Rocket>().Launch(targetData);
            }

             if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Play();
            }
        }
    }
    private Vector3 targetPosSpeedBased, targetPosHeightDiffBased;
    void LookAtTarget()
    {
        if(myData.battleUnitToCopy.name.Contains("Static"))
        {
            targetPosSpeedBased = targetData.target_rigidbody.position + targetData.target_rigidbody.velocity * 0.25f;
            targetPosHeightDiffBased = headHolderRoot.up * (targetData.target_rigidbody.position.y - headHolderRoot.position.y) * 0.1f;
            targetDirection = (targetPosSpeedBased + targetPosHeightDiffBased) - headHolder.position;
        }
        else
        {
            targetDirection = targetData.target_rigidbody.position- headHolder.position;
        }

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.right);

        float angleBtwn_targetDirZY_hhFWD = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, headHolder.forward, headHolder.right);
        float angleBtwn_targetDirXZ_hhrootFWD = Vector3.Angle(targetDirection_XZprojection.normalized, headHolderRoot.forward);

        if (angleBtwn_targetDirXZ_hhrootFWD < allowableAngles.HeadHolderMaxAngle)
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
        myData.timeElapsed += 0.01f;
        myData.timeSinceLastShot -= Time.fixedDeltaTime + Random.Range(0.0001f, 0.0005f);
    }
}
