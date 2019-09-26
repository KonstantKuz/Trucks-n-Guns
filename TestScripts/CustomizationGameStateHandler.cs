using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationGameStateHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolersHolder objectPoolersHolder;

    private void OnEnable()
    {
        StartCustomization();
    }

    public void StartCustomization()
    {
        objectPoolersHolder.AwakeCustomizationGameStatePooler();
    }
}
