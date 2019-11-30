using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckPrefabsREBuilder : MonoBehaviour
{
    public GameObject[] OldTrucksToReBuild;

    public GameObject NewTruckWheel;
    public GameObject[] NewTrucks;

    public GameObject TruckColliders;
    public GameObject Doors;

    [ContextMenu("RebuildTrucks")]
    public void RebuildTrucks()
    {
        for (int i = 0; i < OldTrucksToReBuild.Length; i++)
        {
            GameObject newTruck = NewTrucks[i];
            newTruck.transform.position = OldTrucksToReBuild[i].transform.position;
            FirePoint newTrucksNewFirePoint = newTruck.GetComponent<FirePoint>();
            
            for (int j = 0; j < newTrucksNewFirePoint.gunsPoints.Count; j++)
            {
                newTrucksNewFirePoint.gunsPoints[j].allowableAnglesOnPoint = OldTrucksToReBuild[i].GetComponent<FirePoint>().gunsPoints[j].allowableAnglesOnPoint;
            }

            for (int j = 0; j < OldTrucksToReBuild[i].transform.childCount; j++)
            {
                if(OldTrucksToReBuild[i].transform.GetChild(j).name.Contains("Wheel"))
                {
                    GameObject newTrucksNewWheel = Instantiate(NewTruckWheel) as GameObject;
                    newTrucksNewWheel.transform.parent = newTruck.transform;
                    newTrucksNewWheel.name = newTrucksNewWheel.name.Replace("(Clone)", "");
                    newTrucksNewWheel.transform.position = OldTrucksToReBuild[i].transform.GetChild(j).position;

                }
                if (OldTrucksToReBuild[i].transform.GetChild(j).name.Contains("Doors"))
                {
                    GameObject newTrucksNewDoors = Instantiate(Doors) as GameObject;
                    newTrucksNewDoors.name= newTrucksNewDoors.name.Replace("(Clone)", "");
                    newTrucksNewDoors.transform.parent = newTruck.transform;
                    for (int x = 0; x < newTrucksNewDoors.transform.childCount; x++)
                    {
                        newTrucksNewDoors.transform.GetChild(x).transform.position = OldTrucksToReBuild[i].transform.GetChild(j).transform.GetChild(x).position;
                        newTrucksNewDoors.transform.GetChild(x).transform.rotation = OldTrucksToReBuild[i].transform.GetChild(j).transform.GetChild(x).rotation;
                        newTrucksNewFirePoint.gunsPoints[x].gunsLocation = newTrucksNewDoors.transform.GetChild(x).transform;
                    }
                }
                if (OldTrucksToReBuild[i].transform.GetChild(j).name.Contains("Collider"))
                {
                    GameObject newTrucksNewColliders = Instantiate(TruckColliders) as GameObject;
                    newTrucksNewColliders.name = newTrucksNewColliders.name.Replace("(Clone)", "");

                    newTrucksNewColliders.transform.parent = newTruck.transform;
                    SphereCollider oldCollider = OldTrucksToReBuild[i].transform.GetChild(j).transform.GetComponent<SphereCollider>();
                    SphereCollider newCollider = newTrucksNewColliders.GetComponent<SphereCollider>();
                    newCollider.radius = oldCollider.radius;
                    newCollider.center = oldCollider.center;


                    CapsuleCollider[] newColliders = newTrucksNewColliders.GetComponents<CapsuleCollider>();
                    CapsuleCollider[] oldColliders = OldTrucksToReBuild[i].transform.GetChild(j).GetComponents<CapsuleCollider>();

                    for (int x = 0; x < newColliders.Length; x++)
                    {
                        newColliders[x].radius = oldColliders[x].radius;
                        newColliders[x].height = oldColliders[x].height;
                        newColliders[x].center = oldColliders[x].center;
                    }
                }
            }
        }
    }
}
