using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEnvironment : MonoCached
{
    public Rigidbody[] connectedBodies;
    private bool hasConnections;

    private Rigidbody _rigidbody;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private int collisionCounter;

    private Vector3 localStartPos;
    private bool isDefeated = false;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if(ReferenceEquals(connectedBodies, null))
        {
            connectedBodies = new Rigidbody[transform.childCount];
            connectedBodies = GetComponentsInChildren<Rigidbody>();
        }
        if(!ReferenceEquals(connectedBodies,null) && connectedBodies.Length>0 && !ReferenceEquals(connectedBodies[0], null))
        {
            hasConnections = true;
        }
        startPosition = _rigidbody.transform.localPosition;
        startRotation = _rigidbody.transform.localRotation;

    }

    public void ResetMe()
    {
        //_rigidbody.isKinematic = true;

        //if (hasConnections)
        //{
        //    for (int i = 0; i < connectedBodies.Length; i++)
        //    {
        //        connectedBodies[i].isKinematic = true;
        //    }
        //}

        //_rigidbody.transform.localPosition = startPosition;
        //_rigidbody.transform.localRotation = startRotation;
        //collisionCounter = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!ReferenceEquals(collision.rigidbody, null) && !isDefeated)
        {
            KnockDown(collision.rigidbody, collision.relativeVelocity);
        }
    }

    private void KnockDown(Rigidbody kickedRigidbody, Vector3 kickVelocity)
    {
        if(kickedRigidbody.mass >= _rigidbody.mass)
        {
            _rigidbody.isKinematic = false;
            if (hasConnections)
            {
                for (int i = 0; i < connectedBodies.Length; i++)
                {
                    connectedBodies[i].isKinematic = false;
                }
            }
            kickedRigidbody.velocity = ((kickedRigidbody.mass - _rigidbody.mass) * kickVelocity) / (kickedRigidbody.mass + _rigidbody.mass);
            _rigidbody.velocity = /*2 * */kickedRigidbody.mass * kickVelocity / (kickedRigidbody.mass + _rigidbody.mass);
            //_rigidbody.velocity = kickVelocity;
            kickedRigidbody.velocity = kickVelocity;
            StartCoroutine(Deactivate());
        }

        //float m_1 = kickedRigidbody.mass;
        //Vector3 v_1 = kickVelocity;
        //float m_2 = _rigidbody.mass;
        //Vector3 u_1;
        //Vector3 u_2;

        //u_1 = ((m_1 - m_2) * v_1) / (m_1 + m_2);
        //u_2 = 2 * m_1 * v_1 / (m_1 + m_2);

        //kickedRigidbody.velocity = kickVelocity;
        //_rigidbody.velocity = u_2;
    }
    
    private IEnumerator Deactivate()
    {
        isDefeated = true;
        yield return new WaitForSeconds(0.01f);
        _rigidbody.mass--;
        //_rigidbody.GetComponent<Collider>().isTrigger = true;

        yield return new WaitForSeconds(0.5f);
        _rigidbody.GetComponent<Collider>().isTrigger = true;
        yield return new WaitForSeconds(1f);
        _rigidbody.gameObject.SetActive(false);
    }
}
