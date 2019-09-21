using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCondition : MonoCached
{
    public float currentCondition { get; private set; }

    public float maxCondition { get; set; }

    private Truck truck;

    private ObjectPoolerBase effectPooler;

    public delegate void EntityConditionEvent();
    public event EntityConditionEvent OnZeroCondition;

    private void OnEnable()
    {
        if(maxCondition == 0)
        {
            maxCondition = 100000000;
        }
        //truck = GetComponent<Truck>();
        currentCondition = maxCondition;
        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
    }
    private void OnCollisionEnter(Collision collision)
    {
            AddDamage(Mathf.Abs(collision.relativeVelocity.magnitude*100f));
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
            Death();
        }

    }

    private void Death()
    {
        GameObject expl = effectPooler.SpawnFromPool("BigExplosion", transform.position, Quaternion.identity);
        expl.GetComponent<ParticleSystem>().Play();
        foreach (var item in expl.transform.GetComponentsInChildren<ParticleSystem>())
        {
            item.Play();
        }
        gameObject.SetActive(false);
    }
   
}
