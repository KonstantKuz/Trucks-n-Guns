using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpeedKMH : MonoCached
{
    public Text speedMeter;
    Truck truck;

    // Update is called once per frame
    public override void OnTick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            if(hit.transform.GetComponent<Truck>())
            {
                truck = hit.transform.GetComponent<Truck>();
            }
        }
        if(truck != null)
        {
            speedMeter.text = truck.CurrentSpeed().ToString() + string.Empty + truck.name;
        }
    }
}
