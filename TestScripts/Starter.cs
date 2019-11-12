using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Starter : MonoBehaviour
{
    private AsyncOperation asyncLoad;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => StartLevel());

        StartCoroutine(AsyncLoad());
    }
    private IEnumerator AsyncLoad()
    {
        yield return new WaitForSeconds(1f);
        asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GeneralGameState");
        asyncLoad.allowSceneActivation = false;
    }
    private void StartLevel()
    {
        PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, new PlayerSessionData(0,0));
        asyncLoad.allowSceneActivation = true;
    }
}
