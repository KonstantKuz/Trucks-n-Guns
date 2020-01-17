using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEnvironment : MonoBehaviour
{
    public Rigidbody[] connectedBodies;
    private bool hasConnections;

    private Rigidbody _rigidbody;
    private int collisionCounter;

    private Vector3 localStartPos;
    private bool isDefeated = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (ReferenceEquals(connectedBodies, null))
        {
            connectedBodies = GetComponentsInChildren<Rigidbody>();
        }
        if(!ReferenceEquals(connectedBodies,null) && connectedBodies.Length>0 && !ReferenceEquals(connectedBodies[0], null))
        {
            hasConnections = true;
        }

        ResetParameters();
    }

    public void ResetParameters()
    {
        isDefeated = false;
        _rigidbody.mass++;
        collisionCounter = 0;

        TriggerControl(false);
        FreezeControl(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!ReferenceEquals(collision.rigidbody, null) && collision.relativeVelocity.magnitude > 1)
        {
            KnockDown(collision.rigidbody, collision.relativeVelocity);
        }
    }

    private void KnockDown(Rigidbody kickedRigidbody, Vector3 kickVelocity)
    {
        if(kickedRigidbody.mass >= _rigidbody.mass && !isDefeated)
        {
            FreezeControl(false);

            kickedRigidbody.velocity = ((kickedRigidbody.mass - _rigidbody.mass) * kickVelocity) / (kickedRigidbody.mass + _rigidbody.mass);
            _rigidbody.velocity = kickedRigidbody.mass * kickVelocity / (kickedRigidbody.mass + _rigidbody.mass);
            kickedRigidbody.velocity = kickVelocity;
            StartCoroutine(Deactivate());
        }
    }
    
    private IEnumerator Deactivate()
    {
        isDefeated = true;
        yield return new WaitForEndOfFrame();
        _rigidbody.mass--;
        yield return new WaitForSecondsRealtime(0.5f);
        TriggerControl(true);
        yield return new WaitForSecondsRealtime(2f);
        FreezeControl(true);
    }

    private void FreezeControl(bool enabled)
    {
        _rigidbody.isKinematic = enabled;
        if (hasConnections)
        {
            for (int i = 0; i < connectedBodies.Length; i++)
            {
                connectedBodies[i].isKinematic = enabled;
            }
        }
    }

    private void TriggerControl(bool enabled)
    {
        _rigidbody.GetComponent<Collider>().isTrigger = enabled;

        if (hasConnections)
        {
            for (int i = 0; i < connectedBodies.Length; i++)
            {
                connectedBodies[i].GetComponent<Collider>().isTrigger = enabled;
            }
        }
    }
}
