using System.Collections.Generic;
using UnityEngine;

public class MonoCached : MonoBehaviour
{
    public static List<MonoCached> allTicks = new List<MonoCached>(1000);
    public static List<MonoCached> allFixedTicks = new List<MonoCached>(1000);
    public static List<MonoCached> allLateTicks = new List<MonoCached>(1000);

    private void OnEnable()
    {
        allTicks.Add(this);
        allFixedTicks.Add(this);
        allLateTicks.Add(this);
    }

    private void OnDisable()
    {
        allTicks.Remove(this);
        allFixedTicks.Remove(this);
        allLateTicks.Remove(this);
    }
    

    public void Tick()
    {
        OnTick();
    }

    public void FixedTick()
    {
        OnFixedTick();
    }

    public void LateTick()
    {
        OnLateTick();
    }

    public virtual void OnTick()
    {

    }

    public virtual void OnFixedTick()
    {

    }

    public virtual void OnLateTick()
    {

    }
}
