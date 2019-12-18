using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bullet : MonoCached, BattleUnit
{
    [SerializeField]
    private BattleUnitData battleUnitDataToCopy;

    public BattleUnitData battleUnitData { get; set; }

    private Ray rayFWD, rayBWD;
    private RaycastHit hit;
    private float rayDistance;
    private Transform _transform;
    private GameObject _gameObject;
    private Vector3 _forwardDirection, _deltaPosition;

    private void Awake()
    {
        battleUnitData = Instantiate(battleUnitDataToCopy);
        _transform = transform;
        _gameObject = gameObject;
        _deltaPosition = Vector3.zero;
        rayFWD = new Ray();
        rayBWD = new Ray();
        rayDistance = battleUnitData.speed * 0.02f;
    }
    private void OnEnable()
    {
        customUpdates.Add(this);
        StartCoroutine(AutoDestruction());
    }
    private void OnDisable()
    {
        customUpdates.Remove(this);
    }
    private IEnumerator AutoDestruction()
    {
        yield return new WaitForSecondsRealtime(2f);
        Deactivate();
    }

    public override void CustomUpdate()
    {
        Fly();
    }

    public void Fly()
    {
        _forwardDirection = _transform.forward;
        _deltaPosition = _forwardDirection * Time.deltaTime * battleUnitData.speed;
        _transform.position += _deltaPosition;
        SearchTargets();
    }

    public void SearchTargets()
    {
        rayFWD.origin = _transform.position;
        rayFWD.direction = _forwardDirection;
        if (Physics.Raycast(rayFWD, out hit, rayDistance, battleUnitData.interactibleWith))
        {
            SetDamage(hit.collider.GetComponentInParent<EntityCondition>());
            return;
        }
        rayBWD.origin = _transform.position;
        rayBWD.direction = _forwardDirection;
        if (Physics.Raycast(rayBWD, out hit, rayDistance, battleUnitData.interactibleWith))
        {
            SetDamage(hit.collider.GetComponentInParent<EntityCondition>());
        }
    }

    public void SetDamage(EntityCondition targetToHit)
    {
        if (!ReferenceEquals(targetToHit, null))
        {
            targetToHit.AddDamage(battleUnitData.damage);
        }
        Deactivate();
    }

    public void Deactivate()
    {
        _gameObject.SetActive(false);
    }
}
