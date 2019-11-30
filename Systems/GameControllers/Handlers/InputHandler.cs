using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GeneralGameStateController
{

}

public class InputHandler : MonoCached
{
    [SerializeField]
    private GameObject moveControllerPrefab, fireAreaPrefab, fwdBoost, bcwdBoost, prkngBrake;
    
    public LayerMask damageableMask;

    private Slider moveController;
    private RectTransform fireArea;
    private Button forwardBoost, backwardBoost, parkingBrake, restart;
    private Camera mainCamera;
    private Vector3 seekPointPosition = Vector3.zero;


    public Player player { get; set; }
    public float steeringForce { get; private set; }
    public bool doubleTap { get; private set; }
    
    public void CreateControlsUI()
    {
        GameObject contr = Instantiate(moveControllerPrefab, GameObject.Find("UI").transform.GetChild(0).transform);
        moveController = contr.GetComponentInChildren<Slider>();
        GameObject fireAr = Instantiate(fireAreaPrefab, moveController.transform.parent, GameObject.Find("UI").transform.GetChild(0).transform);
        fireArea = fireAr.GetComponent<RectTransform>();

        GameObject fwdbst = Instantiate(fwdBoost, GameObject.Find("UI").transform.GetChild(0).transform);
        forwardBoost = fwdBoost.GetComponent<Button>();
        GameObject bcwdbst = Instantiate(bcwdBoost, GameObject.Find("UI").transform.GetChild(0).transform);
        backwardBoost = bcwdbst.GetComponent<Button>();

        GameObject prkBrk = Instantiate(prkngBrake, GameObject.Find("UI").transform.GetChild(0).transform);
        parkingBrake = prkBrk.GetComponent<Button>();
    }
    public void FindControlsUI()
    {
        moveController = GameObject.Find("MoveCntr").GetComponent<Slider>();
        fireArea = GameObject.Find("FireArea").GetComponent<RectTransform>();
        forwardBoost = GameObject.Find("ForwardBoost").GetComponent<Button>();
        backwardBoost = GameObject.Find("BackwardBoost").GetComponent<Button>();
        parkingBrake = GameObject.Find("ParkingBrake").GetComponent<Button>();
        //restart = GameObject.Find("Restart").GetComponent<Button>();

        forwardBoost.onClick.AddListener(() => player.ForwardBoost());
        backwardBoost.onClick.AddListener(() => player.BackwardBoost());
        parkingBrake.onClick.AddListener(() => StopPlayer());
        //restart.onClick.AddListener(() => Restart());

        moveController.onValueChanged.AddListener(delegate { SteeringForceValueChange(); });
    }
    public void FindCamera()
    {
        mainCamera = Camera.main;
    }
    
    private void StopPlayer()
    {
        player.StopPlayerTruck();
    }
    private void SteeringForceValueChange()
    {
        steeringForce = moveController.value;
    }
    public void StartUpdateInputs()
    {
        StartCoroutine(UpdateInputs());
    }
    private IEnumerator UpdateInputs()
    {
        yield return new WaitForFixedUpdate();

        #region unityAndroid
#if UNITY_ANDROID
        if(!ReferenceEquals(player.seekPoint, null))
        {
            seekPointPosition.x = steeringForce * 24f;
            seekPointPosition.z = player.truck._transform.position.z + 30f;
            player.seekPoint.position = seekPointPosition;
            player.MovePlayerTruck();
        }

        foreach (Touch touch in Input.touches)
        {
            Vector2 localTouchPosition = fireArea.InverseTransformPoint(touch.position);
            if (fireArea.rect.Contains(localTouchPosition))
            {
                if(touch.tapCount == 1)
                {
                    SetUpPlayersTarget(touch, GameEnums.TrackingGroup.FirstTrackingGroup);
                }
                if (touch.tapCount == 2)
                {
                    SetUpPlayersTarget(touch, GameEnums.TrackingGroup.SecondTrackingGroup);
                }
            }

        }
#endif
        #endregion 
        #region unityEditor
#if UNITY_EDITOR
       // steeringForce = Input.GetAxis("Vertical");
        Vector2 localMousePosition = fireArea.InverseTransformPoint(Input.mousePosition);
        if (fireArea.rect.Contains(localMousePosition))
        {
            if (Input.GetMouseButton(0))
            {
                SetUpPlayersTarget(Input.mousePosition, GameEnums.TrackingGroup.FirstTrackingGroup);
            }
            if (Input.GetMouseButton(1))
            {
                SetUpPlayersTarget(Input.mousePosition, GameEnums.TrackingGroup.SecondTrackingGroup);
            }
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            player.ForwardBoost();
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            player.BackwardBoost();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            player.StopPlayerTruck();
        }

#endif

        yield return StartCoroutine(UpdateInputs());
        #endregion
    }
    void SetUpPlayersTarget(Vector3 mousePosition, GameEnums.TrackingGroup trackingGroup)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, damageableMask))
        {
            if(!ReferenceEquals(hit.rigidbody, null) && !ReferenceEquals(hit.rigidbody, player.truck._rigidbody))
            {
                player.SetUpTargets(hit.rigidbody, trackingGroup);
            }
        }
    }
    void SetUpPlayersTarget(Touch touch, GameEnums.TrackingGroup trackingGroup)
    {
        Ray ray = mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, damageableMask))
        {
            if (!ReferenceEquals(hit.rigidbody, null) && !ReferenceEquals(hit.rigidbody, player.truck._rigidbody))
            {
                player.SetUpTargets(hit.rigidbody, trackingGroup);
            }
        }
    }
}
