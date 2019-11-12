using UnityEngine;

public interface IRoadEvent
{
    bool isActive { get; set; }
    void AwakeEvent(Vector3 playerPosition);
}
