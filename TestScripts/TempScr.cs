using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempScr : MonoBehaviour
{
    public Transform[] oldFenceComplex;
    public GameObject newFenceToRebuild;
    public Transform bigparent;
    public Transform medparent;
    public Transform smallparent;

    //[ContextMenu("BuildNewFence")]
    //public void BuildNewFence()
    //{
    //    for (int i = 0; i < oldFenceComplex.Length; i++)
    //    {
    //        GameObject newFence = UnityEditor.PrefabUtility.InstantiatePrefab(newFenceToRebuild) as GameObject;
    //        newFence.transform.position = oldFenceComplex[i].transform.position;
    //        newFence.transform.rotation = oldFenceComplex[i].transform.rotation;
    //        newFence.name = newFence.name.Replace("Clone", $"{i}");

    //        if (oldFenceComplex[i].parent.name.Contains("Big"))
    //            newFence.transform.parent = bigparent;
    //        else if (oldFenceComplex[i].parent.name.Contains("Medium"))
    //            newFence.transform.parent = medparent;
    //        else
    //            newFence.transform.parent = smallparent;

    //        oldFenceComplex[i].gameObject.SetActive(false);
    //    }
    //}
}
