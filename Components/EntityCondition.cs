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
        AddDamage(Mathf.Abs(collision.relativeVelocity.magnitude*10f));
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
            GameObject expl = effectPooler.Spawn("BigExplosion", transform.position + new Vector3(Random.Range(-explosionCount/5, explosionCount/5), Random.Range(-explosionCount/1, explosionCount/2),
                Random.Range(-explosionCount/3, explosionCount/3)), Quaternion.identity);
            expl.GetComponent<ParticleSystem>().Play();
            foreach (var item in expl.transform.GetComponentsInChildren<ParticleSystem>())
            {
                item.Play();
            }
        }
        gameObject.SetActive(false);
    }

   
}
