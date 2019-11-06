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

    private Collider _collider;
    private void Awake()
    {
        myData = Instantiate(battleUnitDataToCopy);
        _transform = transform;
        _gameObject = gameObject;
        _deltaPosition = Vector3.zero;
        ray = new Ray();

        rayDistance = myData.speed * 0.01f;

        _collider = GetComponent<Collider>();
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(2f);
        Deactivate();
    }

    public override void OnTick()
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
        ray.origin = _transform.position;
        ray.direction = _forwardDirection;
        if (Physics.Raycast(ray, out hit, rayDistance, myData.interactibleWith))
        {
            SetDamage(hit.collider.GetComponentInParent<EntityCondition>());
        }
    }

    public override void SetDamage(EntityCondition targetToHit)
    {
        if (!ReferenceEquals(targetToHit, null))
        {
            targetToHit.AddDamage(myData.damage);
        }
        Deactivate();
    }

    public override void Deactivate()
    {
        _gameObject.SetActive(false);
    }
}
