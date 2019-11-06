using UnityEngine;
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

        for (int i = 0; i < MonoCached.allTicks.Count; i++)
        {
            MonoCached.allTicks[i].Tick();
        }

        for (int i = 0; i < MonoCached.allFixedTicks.Count; i++)
        {
            MonoCached.allFixedTicks[i].FixedTick();
        }

        yield return StartCoroutine(UpdateMonocached());
    }

    private void Update()
    {
        for (int i = 0; i < MonoCached.allTicks.Count; i++)
        {
            MonoCached.allTicks[i].Tick();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < MonoCached.allFixedTicks.Count; i++)
        {
            MonoCached.allFixedTicks[i].FixedTick();
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < MonoCached.allLateTicks.Count; i++)
        {
            MonoCached.allLateTicks[i].LateTick();
        }
    }
}
