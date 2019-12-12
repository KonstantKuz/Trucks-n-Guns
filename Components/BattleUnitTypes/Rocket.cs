using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoCached, BattleUnit
{
    [SerializeField]
    private BattleUnitData battleUnitDataToCopy;

    public BattleUnitData battleUnitData { get; set; }
    
    private Transform _transform;
    private GameObject _gameObject;
    private Vector3 _forwardDirection, _deltaPosition;
    private Collider[] hits;
    private Quaternion lookRotation;

    private TargetData targetData;
    private bool isLaunched;
    private GameObject trail;
    
    private ObjectPoolerBase effectPooler;

    private string smallExplosionName = "SmallExplosion";
    private string rocketTrailName = "RocketTrail";

    private void Awake()
    {
        battleUnitData = Instantiate(battleUnitDataToCopy);

        _transform = transform;
        _gameObject = gameObject;
        _deltaPosition = Vector3.zero;
        targetData = new TargetData(null, null);
        isLaunched = false;
        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
    }

    public void Launch(TargetData targetData)
    {
        this.targetData = targetData;
        
        isLaunched = true;

        trail = effectPooler.Spawn(rocketTrailName, transform.position, transform.rotation);
        trail.GetComponent<CachedParticles>().PlayParticles();

        StartCoroutine(AutoDestruction());
    }

    public IEnumerator AutoDestruction()
    {
        yield return new WaitForSecondsRealtime(3f);
        Explosion();
    }

    public override void CustomUpdate()
    {
        Fly();
    }

    public void Fly()
    {
        if(isLaunched == true)
        {
            _forwardDirection = _transform.forward;
            _deltaPosition = _forwardDirection * Time.deltaTime * battleUnitData.speed;
            _transform.position += _deltaPosition;

            if(battleUnitData.battleType == GameEnums.BattleType.Tracking && targetData.target_rigidbody !=null && targetData.target_rigidbody.gameObject.activeInHierarchy)
            {
                Quaternion lookRotation = Quaternion.LookRotation(targetData.target_rigidbody.position - _transform.position);
                _transform.rotation = Quaternion.Slerp(_transform.rotation, lookRotation, 0.5f);
            }

            trail.transform.position = _transform.position;
            trail.transform.rotation = _transform.rotation;
            SearchTargets();
        }
    }

    public void SearchTargets()
    {
        if(Physics.CheckSphere(_transform.position, battleUnitData.speed * 0.005f, battleUnitData.interactibleWith))
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        hits = Physics.OverlapSphere(_transform.position, battleUnitData.damageRadius, battleUnitData.damageable);
        for (int i = 0; i < hits.Length; i++)
        {
            SetDamage(hits[i].GetComponentInParent<EntityCondition>());
        }

        Deactivate();

        effectPooler.Spawn(smallExplosionName, transform.position, Quaternion.identity).GetComponent<CachedParticles>().PlayParticles();
    }

    public void SetDamage(EntityCondition targetToHit)
    {
        if (!ReferenceEquals(targetToHit, null))
        {
            targetToHit.AddDamage(battleUnitData.damage - ((_transform.position - targetToHit.transform.position).magnitude*10f));
        }
    }

    public void Deactivate()
    {
        _gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, battleUnitDataToCopy.damageRadius);
    }
}
