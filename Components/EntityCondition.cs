using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCondition : MonoBehaviour
{
    [SerializeField]
    [Range(1, 10)]
    private int explosionCount = 1;
    [SerializeField]
    [Range(0,5)]
    private int onEnableInvincibleTime;

    public float currentCondition { get; private set; }

    public float maxCondition { get; private set; }

    private bool invincible;
    private ObjectPoolerBase effectPooler;

    public delegate void EntityConditionEvent();
    public event EntityConditionEvent OnZeroCondition;
    public event EntityConditionEvent OnCurrentConditionChanged;

    private string bigExplosionName = "BigExplosion";

    private Vector3 randomExplosionPosition;

    private void OnEnable()
    {
        StartCoroutine(InvincibleForSeconds(onEnableInvincibleTime));
        effectPooler = ObjectPoolersHolder.Instance.EffectPooler;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private IEnumerator InvincibleForSeconds(float time)
    {
        invincible = true;
        yield return new WaitForSecondsRealtime(time);
        invincible = false;
    }
    public void ResetCondition(float value)
    {
        maxCondition = value;
        currentCondition = maxCondition;
    }
    private void OnCollisionEnter(Collision collision)
    {
        float damageRange;

        if(collision.gameObject.layer != LayerMask.NameToLayer("Ground"))
        {
            if (!ReferenceEquals(collision.rigidbody, null))
            {
                Rigidbody collidedRigidbody = collision.rigidbody;
                Vector3 myBodyVelocity = collision.rigidbody.velocity - collision.relativeVelocity;

                if (collision.rigidbody.mass > 30000 && collidedRigidbody.velocity.sqrMagnitude > myBodyVelocity.sqrMagnitude)
                {
                    damageRange = collision.relativeVelocity.sqrMagnitude * 3f;
                    AddDamage(damageRange);
                }
                else
                {
                    damageRange = collision.relativeVelocity.sqrMagnitude / 5f;
                    AddHealth(damageRange);
                }
                //if(myBodyVelocity.sqrMagnitude > collidedRigidbody.velocity.sqrMagnitude)
                //{
                //    damageRange = collision.relativeVelocity.sqrMagnitude / 3f;
                //    AddHealth(damageRange);
                //}
            }
            else
            {
                damageRange = collision.relativeVelocity.sqrMagnitude * 5f;
                AddDamage(damageRange);
            }
        }
        
    }
    public void AddHealth(float amount)
    {
        currentCondition += amount;
        CheckMyHealth();
    }
    public void AddDamage(float amount)
    {
        if(!invincible)
        {
            currentCondition -= amount;
            CheckMyHealth();
        }
    }
    private void CheckMyHealth()
    {
        CurrentConditionChanhedInvoke();
        if (currentCondition < 10)
        {
            ZeroConditionInvoke();
        }
        if(currentCondition>maxCondition)
        {
            currentCondition = maxCondition;
        }
    }
    public void ZeroConditionInvoke()
    {
        OnZeroCondition?.Invoke();

        Death();
    }
    public void CurrentConditionChanhedInvoke()
    {
        OnCurrentConditionChanged?.Invoke();
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
