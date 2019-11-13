using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoCached, BattleUnit
{
    [SerializeField]
    private BattleUnitData battleUnitDataToCopy;

    public BattleUnitData battleUnitData { get; set; }

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
        battleUnitData = Instantiate(battleUnitDataToCopy);
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

    public void Fly()
    {
    }
    public void SearchTargets()
    {
    }

    void Explosion()
    {
       
    }

    public void SetDamage(EntityCondition targetToHit)
    {
    }

    public void Deactivate()
    {
    }
}
