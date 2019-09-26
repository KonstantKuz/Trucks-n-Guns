using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour
{
   
    private void Update()
    {
        GetComponent<GunParent>().Fire();
    }
}
