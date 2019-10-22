using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeedEffect
{
    void CallAndPlayEffect(string name, Vector3 position, Quaternion rotation);
}
