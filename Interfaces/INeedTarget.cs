using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeedTarget 
{
    TargetData targetData { get; set; }
    void InjectNewTargetData(Rigidbody targetRigidbody);
}
