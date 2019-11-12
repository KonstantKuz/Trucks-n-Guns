using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
public class PlayerHandler : MonoCached
{
    [SerializeField]
    private GameObject playerPreafab;
    [SerializeField]
    private GameObject cameraPrefab;
    [SerializeField]
    private GameObject playerSeekPoint;

    [SerializeField]
    private float playerStartingForce;

    private Camera playerCamera;
    public Transform camera_transform { get; set; }
    public Vector3 cameraFixedPos { get; set; }

    public Player playerInstance { get; private set; }
    public GameObject player { get; private set; }
    public Transform player_transform { get; private set; }
    public Rigidbody player_rigidbody { get; private set; }

    public Vector3 playerStartPosition { get; private set; }

    public static PlayerSessionData currentSessionData;

    public void CreatePlayer(InputHandler inputHandler)
    {
        GameObject seekPnt = Instantiate(playerSeekPoint);

        GameObject pl = Instantiate(playerPreafab, GameObject.Find("Scene").transform);
        player = pl;
        playerInstance = player.GetComponent<Player>();
        playerInstance.seekPoint = seekPnt.transform;
        playerInstance.InjectPlayerIntoInput(inputHandler);

        player_transform = playerInstance.transform;

        playerStartPosition = player_transform.position;

        player_rigidbody = playerInstance.GetComponent<Rigidbody>();

        player_rigidbody.AddForce(player_rigidbody.transform.forward * playerStartingForce, ForceMode.VelocityChange);

        player.GetComponent<EntityCondition>().OnZeroCondition += ReturnToCustomization;

        currentSessionData = new PlayerSessionData(0, 0);
    }

    public void CreateCamera()
    {
        GameObject cam = Instantiate(cameraPrefab, GameObject.Find("Scene").transform);
        playerCamera = cam.GetComponent<Camera>();
        camera_transform = playerCamera.transform;
        cameraFixedPos = Vector3.zero;
    }

    public void StartUpdateCamera()
    {
        StartCoroutine(UpdateCamera());
    }

    private IEnumerator UpdateCamera()
    {
        yield return new WaitForFixedUpdate();

        var camPos = camera_transform.position;
        var playerPos = player_transform.position;
        if (cameraFixedPos == Vector3.zero)
        {
            camPos = Vector3.Lerp(camPos, new Vector3(camPos.x, camPos.y, playerPos.z), 0.1f);
            camera_transform.position = camPos;
        }
        else
        {
            camPos = Vector3.Lerp(camPos, cameraFixedPos, 5f);
            //camera_transform.LookAt(cameraFixedPos + Vector3.down * 2f);
            camera_transform.position = camPos;
        }

        yield return StartCoroutine(UpdateCamera());
    }

    public static void IncreaseDefeatedEnemiesOnThisSession()
    {
        currentSessionData.defeatedEnemies++;
        Debug.Log($"<color=green> Enemies count = {currentSessionData.defeatedEnemies} </color>");
    }

    public static void IncreaseTraveledDistance(int traveledDistance)
    {
        currentSessionData.traveledDistance += traveledDistance;
        Debug.Log($"<color=green> Traveled distance = {currentSessionData.traveledDistance} </color>");
    }

    private void ReturnToCustomization()
    {
        player.GetComponent<EntityCondition>().OnZeroCondition -= ReturnToCustomization;
        playerInstance.StopListenTargetCondition();
        PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, currentSessionData);

        StartCoroutine(LoadCustomiz());
    }

    private IEnumerator LoadCustomiz()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerStaticRunTimeData.LoadData();
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Customization");
    }
}
