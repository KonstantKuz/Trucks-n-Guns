using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : BattleUnitParent
{
    [SerializeField]
    private BattleUnitData battleUnitDataToCopy;

    public override BattleUnitData myData { get; set; }

    private Ray ray;
    private RaycastHit hit;
    private float rayDistance;
    private Transform _transform;
    private GameObject _gameObject;
    private Vector3 _forwardDirection, _deltaPosition;
    private Collider[] hits;
    private ObjectPoolerBase effectPooler;

    private void Awake()
    {
        myData = Instantiate(battleUnitDataToCopy);
        _transform = transform;
        _gameObject = gameObject;
        _deltaPosition = Vector3.zero;
        ray = new Ray();
        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
    }

    private void OnTriggerEnter(Collider other)
    {
        Explosion();
    }

    public override void Fly()
    {
        throw new System.Exception("Снаряды ловушек не исполняют поведения Fly()");
    }
    public override void SearchTargets()
    {
        throw new System.Exception("Снаряды ловушек не исполняют поведения SearchTargets()");
    }

    void Explosion()
    {
        hits = Physics.OverlapSphere(_transform.position, myData.damageRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            SetDamage(hits[i].GetComponent<EntityCondition>());
        }

        GameObject expl = effectPooler.Spawn("SmallExplosion", transform.position, Quaternion.identity);
        expl.GetComponent<ParticleSystem>().Play();
        foreach (var item in expl.transform.GetComponentsInChildren<ParticleSystem>())
        {
            item.Play();
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
}
