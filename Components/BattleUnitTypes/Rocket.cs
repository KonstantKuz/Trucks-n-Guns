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

        trail = effectPooler.SpawnFromPool("RocketTrail", transform.position, transform.rotation);
        foreach (var item in trail.transform.GetComponentsInChildren<ParticleSystem>())
        {
            item.Play();
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
            _deltaPosition = _forwardDirection * myData.speed;
            _transform.position += _deltaPosition;

            if(myData.battleType == GameEnums.BattleType.Tracking && targetData.target_rigidbody !=null)
                _transform.rotation = Quaternion.LookRotation(targetData.target_rigidbody.position - _transform.position);

            trail.transform.position = _transform.position;
            trail.transform.rotation = _transform.rotation;
            SearchTargets();
        }
    }

    public override void SearchTargets()
    {
        if(Physics.CheckSphere(_transform.position, myData.speed/3))
        {
            Explosion();
        }
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(2f);
        Explosion();
    }

    void Explosion()
    {
            hits = Physics.OverlapSphere(_transform.position, myData.damageRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                SetDamage(hits[i].GetComponent<EntityCondition>());
            }

        GameObject expl = effectPooler.SpawnFromPool("SmallExplosion", transform.position, Quaternion.identity);
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
        base.SetDamage(targetToHit);
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
