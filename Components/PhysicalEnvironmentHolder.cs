using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEnvironmentHolder : MonoCached
{
    public PhysicalEnvironment[] physicalChilds;
    public Vector3[] startChildsLocalPositions;
    public Quaternion[] startChildsLocalRotations;
    public Vector3 startLocalPosition;
    public Quaternion startLocalRotation;

    private void Awake()
    {
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;

        physicalChilds = transform.GetComponentsInChildren<PhysicalEnvironment>();
        startChildsLocalPositions = new Vector3[physicalChilds.Length];
        startChildsLocalRotations = new Quaternion[physicalChilds.Length];

        for (int i = 0; i < physicalChilds.Length; i++)
        {
            startChildsLocalPositions[i] = physicalChilds[i].transform.localPosition;
            startChildsLocalRotations[i] = physicalChilds[i].transform.localRotation;
        }
    }

    public void ResetEnvironment()
    {
        transform.localPosition = startLocalPosition;
        transform.localRotation = startLocalRotation;
        for (int i = 0; i < physicalChilds.Length; i++)
        {
            physicalChilds[i].transform.localPosition = startChildsLocalPositions[i];
            physicalChilds[i].transform.localRotation = startChildsLocalRotations[i];
            physicalChilds[i].ResetParameters();
        }
    }
}
