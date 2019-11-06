using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCondition : MonoCached
{
    [SerializeField]
    [Range(1, 10)]
    private int explosionCount = 1;

    public float currentCondition { get; private set; }

    public float maxCondition { get; set; }

    private Truck truck;

    private ObjectPoolerBase effectPooler;

    public delegate void EntityConditionEvent();
    public event EntityConditionEvent OnZeroCondition;

    private string bigExplosionName = "BigExplosion";

    private Vector3 randomExplosionPosition;

    private void OnEnable()
    {
        currentCondition = maxCondition;
        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
    }

    public void ResetCurrentCondition(float value)
    {
        currentCondition = value;
    }
    private void OnCollisionEnter(Collision collision)
    {
        float damageRange = collision.relativeVelocity.sqrMagnitude * 0.1f;
        if (damageRange > 800)
        {
            AddDamage(damageRange);
        }
    }
    public void AddHealth(float amount)
    {
        currentCondition += amount;
        CheckMyHealth();
    }
    public void AddDamage(float amount)
    {
        currentCondition -= amount;
        CheckMyHealth();
    }
    private void CheckMyHealth()
    {
        //Debug.Log($"<color=green> {gameObject.name} has {currentCondition} condition </color>");
        if (currentCondition < 1)
        {
            ZeroConditionInvoke();
            Death();
        }
    }
    public void ZeroConditionInvoke()
    {
        OnZeroCondition?.Invoke();
    }
    private void Death()
    {
        for (int i = 0; i < explosionCount; i++)
        {
            randomExplosionPosition.x = Random.Range(-explosionCount / 5, explosionCount / 5);
            randomExplosionPosition.y = Random.Range(-explosionCount / 1, explosionCount / 2);
            randomExplosionPosition.z = Random.Range(-explosionCount / 3, explosionCount / 3);
            effectPooler.Spawn(bigExplosionName, transform.position + randomExplosionPosition,Quaternion.identity).GetComponent<CachedParticles>().PlayParticles();
        }
        gameObject.SetActive(false);
    }
}
