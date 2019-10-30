using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : BattleUnitParent
{
    [SerializeField]
    private BattleUnitData battleUnitDataToCopy;

    public override BattleUnitData myData { get; set; }
    
    private Transform _transform;
    private GameObject _gameObject;
    private Vector3 _forwardDirection, _deltaPosition;
    private Collider[] hits;
    private Quaternion lookRotation;

    private TargetData targetData;
    private bool isLaunched;
    private GameObject trail;
    
    private ObjectPoolerBase effectPooler;

    private void Awake()
    {
        myData = Instantiate(battleUnitDataToCopy);

        _transform = transform;
        _gameObject = gameObject;
        _deltaPosition = Vector3.zero;
        targetData = new TargetData(null);
        isLaunched = false;
        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
    }
    public void Launch(TargetData targetData)
    {
        this.targetData = targetData;
        
        isLaunched = true;

        trail = effectPooler.Spawn("RocketTrail", transform.position, transform.rotation);
        foreach (var effect in trail.transform.GetComponentsInChildren<ParticleSystem>())
        {
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            effect.Play();
        }
        StartCoroutine(AutoDestruct());
    }

    //private IEnumerator LateLaunch()
    //{
    //}

    public override void OnFixedTick()
    {
        Fly();
    }

    public override void Fly()
    {
        if(isLaunched == true)
        {
            _forwardDirection = _transform.forward;
            _deltaPosition = _forwardDirection * Time.fixedDeltaTime * myData.speed;
            _transform.position += _deltaPosition;

            if(myData.battleType == GameEnums.BattleType.Tracking && targetData.target_rigidbody !=null && targetData.target_rigidbody.gameObject.activeInHierarchy)
            {
                Quaternion lookRotation = Quaternion.LookRotation(targetData.target_rigidbody.position - _transform.position);
                _transform.rotation = Quaternion.Slerp(_transform.rotation, lookRotation, 0.5f);
            }

            trail.transform.position = _transform.position;
            trail.transform.rotation = _transform.rotation;
            SearchTargets();
        }
    }

    public override void SearchTargets()
    {
        if(Physics.CheckSphere(_transform.position, myData.speed * 0.01f, myData.interactibleWith))
        {
            Explosion();
        }
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(10f);
        Explosion();
    }

    void Explosion()
    {
        hits = Physics.OverlapSphere(_transform.position, myData.damageRadius, myData.interactibleWith);
        for (int i = 0; i < hits.Length; i++)
        {
            SetDamage(hits[i].gameObject.GetComponentInParent<EntityCondition>());
            //Debug.Log(hits[i].gameObject.name + "was hited with rocket");
        }

        GameObject expl = effectPooler.Spawn("SmallExplosion", transform.position, Quaternion.identity);
        expl.GetComponent<ParticleSystem>().Play();
        foreach (var item in expl.transform.GetComponentsInChildren<ParticleSystem>())
        {
            item.Play();
        }
        if(hits.Length == 0)
        {
            Deactivate();
        }
    }

    public override void SetDamage(EntityCondition targetToHit)
    {
        //base.SetDamage(targetToHit);
        if (targetToHit != null)
        {
            targetToHit.AddDamage(myData.damage - ((_transform.position - targetToHit.transform.position).magnitude*10f));
            //Debug.Log(myData.damage - ((_transform.position - targetToHit.transform.position).magnitude * 10f));
        }
        Deactivate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, battleUnitDataToCopy.damageRadius);
    }
}
