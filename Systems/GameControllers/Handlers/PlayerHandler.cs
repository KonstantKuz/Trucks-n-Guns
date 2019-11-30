using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Vector3 newCamPos;

    public static Player playerInstance { get; private set; }
    public GameObject player { get; private set; }
    public Transform player_transform { get; private set; }
    public Rigidbody player_rigidbody { get; private set; }

    public Vector3 playerStartPosition { get; private set; }

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
       
    }

    public void CreateCamera()
    {
        GameObject cam = Instantiate(cameraPrefab, GameObject.Find("Scene").transform);
        playerCamera = cam.GetComponent<Camera>();
        camera_transform = playerCamera.transform;
        newCamPos = Vector3.zero;
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
        newCamPos.x = Mathf.Clamp(playerPos.x-20, -26, -18);
        
        newCamPos.y = camPos.y;
        newCamPos.z = playerPos.z;

        camPos = Vector3.Lerp(camPos, newCamPos, 0.1f);
        camera_transform.position = camPos;
        //camera_transform.LookAt(playerPos);
        yield return StartCoroutine(UpdateCamera());
    }

    private void ReturnToCustomization()
    {
        player.GetComponent<EntityCondition>().OnZeroCondition -= ReturnToCustomization;

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
