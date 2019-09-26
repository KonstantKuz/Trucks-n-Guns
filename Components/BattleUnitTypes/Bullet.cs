using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bullet : BattleUnitParent
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

    private void Awake()
    {
        myData = Instantiate(battleUnitDataToCopy);
        _transform = transform;
        _gameObject = gameObject;
        _deltaPosition = Vector3.zero;
        ray = new Ray();
    }

    public override void OnFixedTick()
    {
        Fly();
    }

    public override void Fly()
    {
        _forwardDirection = _transform.forward;
        _deltaPosition = _forwardDirection * Time.fixedDeltaTime * myData.speed;
        _transform.position += _deltaPosition;
        SearchTargets();
    }

    public override void SearchTargets()
    {
        rayDistance = myData.speed * 0.01f;
        ray.origin = _transform.position;
        ray.direction = _forwardDirection;
        if(Physics.Raycast(ray, rayDistance))
        {
            SetDamage();
        }
    }

    private void SetDamage()
    {
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            var health = hit.transform.GetComponent<EntityCondition>();
            SetDamage(health);
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
