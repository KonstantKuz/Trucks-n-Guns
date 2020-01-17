using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPreafab;
    [SerializeField]
    private GameObject cameraPrefab;
    [SerializeField]
    private GameObject seekPoint;
    [SerializeField]
    private GameObject targetPoint;

    [SerializeField]
    private float playerStartingForce;

    public static bool staticCamera = false;

    public static Camera Camera;
    private Transform camera_transform;
    private Vector3 newCamPos;

    public static Player PlayerInstance { get; private set; }
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Transform player_transform;
    [HideInInspector]
    public Rigidbody player_rigidbody;
    
    public void CreatePlayer()
    {
        GameObject seekPnt = Instantiate(seekPoint);
        GameObject targetPnt = Instantiate(targetPoint);

        GameObject pl = Instantiate(playerPreafab, GameObject.Find("Scene").transform);
        player = pl;
        PlayerInstance = player.GetComponent<Player>();
        PlayerInstance.seekPoint = seekPnt.transform;
        PlayerInstance.targetPoint = targetPnt.transform;

        player_transform = PlayerInstance.transform;

        player_rigidbody = PlayerInstance.GetComponent<Rigidbody>();

        player_rigidbody.AddForce(player_rigidbody.transform.forward * playerStartingForce, ForceMode.VelocityChange);       
    }

    public void CreateCamera()
    {
        GameObject cam = Instantiate(cameraPrefab, GameObject.Find("Scene").transform);
        Camera = cam.GetComponent<Camera>();
        camera_transform = Camera.transform;
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
