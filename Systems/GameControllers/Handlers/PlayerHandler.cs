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

    public static bool staticCamera = false;

    private Camera playerCamera;
    public Transform camera_transform { get; set; }
    private Vector3 newCamPos;

    public static Player playerInstance { get; private set; }
    public GameObject player { get; private set; }
    public Transform player_transform { get; private set; }
    public Rigidbody player_rigidbody { get; private set; }

    public Vector3 playerStartPosition { get; private set; }
    
    public void CreatePlayer()
    {
        GameObject seekPnt = Instantiate(playerSeekPoint);

        GameObject pl = Instantiate(playerPreafab, GameObject.Find("Scene").transform);
        player = pl;
        playerInstance = player.GetComponent<Player>();
        playerInstance.seekPoint = seekPnt.transform;
        player_transform = playerInstance.transform;

        playerStartPosition = player_transform.position;

        player_rigidbody = playerInstance.GetComponent<Rigidbody>();

        player_rigidbody.AddForce(player_rigidbody.transform.forward * playerStartingForce, ForceMode.VelocityChange);       
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
        var camPos = camera_transform.position;
        var playerPos = player_transform.position;
        newCamPos = camPos;
        if(staticCamera)
        {
            newCamPos.x = -24;
            newCamPos.y = 57;
        }
        else
        {
            newCamPos.x = Mathf.Clamp(playerPos.x - 20, -26, -18);
            newCamPos.y = 50;
        }
        newCamPos.z = playerPos.z;

        camPos = Vector3.Lerp(camPos, newCamPos, 0.1f);
        yield return new WaitForFixedUpdate();

        camera_transform.position = camPos;
        //camera_transform.LookAt(playerPos);
        yield return StartCoroutine(UpdateCamera());
    }
}
