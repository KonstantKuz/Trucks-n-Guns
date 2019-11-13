﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UpdateManager : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        //StartCoroutine(UpdateMonocached());
    }

    private IEnumerator UpdateMonocached()
    {
        yield return new WaitForSeconds(0.016f);

        for (int i = 0; i < MonoCached.customUpdates.Count; i++)
        {
            MonoCached.customUpdates[i].CustomUpdatesCall();
        }

        for (int i = 0; i < MonoCached.customFixedUpdates.Count; i++)
        {
            MonoCached.customFixedUpdates[i].CustomFixedUpdatesCall();
        }

        yield return StartCoroutine(UpdateMonocached());
    }

    private void Update()
    {
        for (int i = 0; i < MonoCached.customUpdates.Count; i++)
        {
            MonoCached.customUpdates[i].CustomUpdatesCall();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < MonoCached.customFixedUpdates.Count; i++)
        {
            MonoCached.customFixedUpdates[i].CustomFixedUpdatesCall();
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < MonoCached.customLateUpdates.Count; i++)
        {
            MonoCached.customLateUpdates[i].CustomLateUpdatesCall();
        }
    }
}
