using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity;
using UnityEditor;
public class GunDataTuner : MonoBehaviour
{
    public DataReturnerBase gunDataReturner;

    public GunData[] datas;

    public string HighDamageBattleUnitName;
    public string MediumDamageBattleUnitName;
    public string LowDamageBattleUnitName;

    public float highRateOfFireValue;
    public float mediumRateOfFireValue;
    public float lowRateOfFireValue;

    public float highTargetingSpeedValue;
    public float mediumTargetingSpeedValue;
    public float lowTargetingSpeedValue;

    [ContextMenu("setupdataRETURNER")]
    public void SetUpDataReturner()
    {
        gunDataReturner.dataToHold.Clear();
        for (int i = 0; i < datas.Length; i++)
        {
            DataReturnerBase.DataToHold dataToHold = new DataReturnerBase.DataToHold();
            dataToHold.data = datas[i];
            dataToHold.tag = datas[i].name;
            gunDataReturner.dataToHold.Add(dataToHold);
            //AssetDatabase.SaveAssets();
        }
        //AssetDatabase.SaveAssets();
    }

    [ContextMenu("setupdatas")]
    public void SetUpDatas()
    {
        for (int i = 0; i < datas.Length; i++)
        {
            //datas[i].name = System.Enum.GetNames(typeof(GameEnums.GunDataType))[i];

            //string assetPath = AssetDatabase.GetAssetPath(datas[i].GetInstanceID());
            //AssetDatabase.RenameAsset(assetPath, System.Enum.GetNames(typeof(GameEnums.GunDataType))[i]);
            //AssetDatabase.SaveAssets();

            Debug.Log($"Name of data = {datas[i].name} and it would be {System.Enum.GetNames(typeof(GameEnums.GunDataType))[i].ToString()}");

            GameEnums.GunDataType value = (GameEnums.GunDataType)System.Enum.Parse(typeof(GameEnums.GunDataType), System.Enum.GetNames(typeof(GameEnums.GunDataType))[i]);
            int dataTypeValue = (int)value;
            Debug.Log($"dataType value = {dataTypeValue} and it would be {value.ToString() + (int)value}");
            char[] dataTypeChars = dataTypeValue.ToString().ToCharArray();
            for (int j = 0; j < dataTypeChars.Length; j++)
            {
                Debug.Log($"<color=red> {dataTypeChars[j] - 48} </color>");
            }
            //RateOfFire
            if(dataTypeChars[0]-48 == 1)
            {
                datas[i].rateofFire = lowRateOfFireValue;
            }
            else if(dataTypeChars[0]-48 == 2)
            {
                datas[i].rateofFire = mediumRateOfFireValue;

            }
            else if (dataTypeChars[0] - 48 == 3)
            {
                datas[i].rateofFire = highRateOfFireValue;
            }
            //Damage
            if (dataTypeChars[1] - 48 == 1)
            {
                datas[i].battleUnit = LowDamageBattleUnitName;
            }
            else if (dataTypeChars[1] - 48 == 2)
            {
                datas[i].battleUnit = MediumDamageBattleUnitName;
            }
            else if (dataTypeChars[1] - 48 == 3)
            {
                datas[i].battleUnit = HighDamageBattleUnitName;
            }
            //Accuracy
            if (dataTypeChars[2] - 48 == 1)
            {
                datas[i].targetingSpeed = lowTargetingSpeedValue;
            }
            else if (dataTypeChars[2] - 48 == 2)
            {
                datas[i].targetingSpeed = mediumTargetingSpeedValue;
            }
            else if (dataTypeChars[2] - 48 == 3)
            {
                datas[i].targetingSpeed = highTargetingSpeedValue;
            }
        }

        SetUpDataReturner();

        //AssetDatabase.SaveAssets();
    }
}
