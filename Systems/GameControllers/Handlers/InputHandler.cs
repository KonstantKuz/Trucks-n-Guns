using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoCached
{
    public LayerMask damageableMask;
    public LayerMask groundMask;

    public Transform targetPoint;

    public RectTransform moveController;
    public RectTransform fireArea;
    public Button forwardBoost, backwardBoost, parkingBrake, changeCameraState, restart;
    private Camera mainCamera;
    private Vector3 seekPointPosition = Vector3.zero;


    private Player player;
    public float steeringForce { get; private set; }
    public bool doubleTap { get; private set; }
    
    public void FindControlsUI()
    {
        forwardBoost.onClick.AddListener(() => player.ForwardBoost());
        backwardBoost.onClick.AddListener(() => player.BackwardBoost());
        parkingBrake.onClick.AddListener(() => player.StopPlayerTruck());
        changeCameraState.onClick.AddListener(() => ChangeCameraState());
        //restart.onClick.AddListener(() => Restart());
    }
    public void FindCamera()
    {
        mainCamera = Camera.main;
    }
    
    public void ChangeCameraState()
    {
        PlayerHandler.staticCamera = !PlayerHandler.staticCamera;
    }

    public void StartUpdateInputs()
    {
        player = PlayerHandler.playerInstance;
        StartCoroutine(UpdateInputs());
    }
    private IEnumerator UpdateInputs()
    {
        yield return new WaitForEndOfFrame();
        if(!ReferenceEquals(player.FirstTrackingGroupsTarget, null) && !ReferenceEquals(player.FirstTrackingGroupsTarget.target_rigidbody, null))
        {
            targetPoint.position = player.FirstTrackingGroupsTarget.target_rigidbody.position;
        }

        if (!ReferenceEquals(player.seekPoint, null))
        {
            seekPointPosition.z = player.truck._transform.position.z + 30f;
            player.seekPoint.position = seekPointPosition;
            player.MovePlayerTruck();
        }
        #region unityAndroid
#if UNITY_ANDROID

        foreach (Touch touch in Input.touches)
        {
            Vector2 moveAreaTouchPosition = moveController.InverseTransformPoint(touch.position);
            if(moveController.rect.Contains(moveAreaTouchPosition))
            {
                SetSeekPointPosition(touch);
            }
            Vector2 fireAreaTouchPosition = fireArea.InverseTransformPoint(touch.position);
            if (fireArea.rect.Contains(fireAreaTouchPosition))
            {
                SetUpPlayersTarget(touch, GameEnums.TrackingGroup.FirstTrackingGroup);
                //if (touch.tapCount == 1)
                //{
                //    SetUpPlayersTarget(touch, GameEnums.TrackingGroup.FirstTrackingGroup);
                //}
                //if (touch.tapCount == 2)
                //{
                //    SetUpPlayersTarget(touch, GameEnums.TrackingGroup.SecondTrackingGroup);
                //}
            }
        }
#endif
        #endregion
        #region unityEditor
#if UNITY_EDITOR
        Vector2 moveAreaMousePosition = moveController.InverseTransformPoint(Input.mousePosition);
        if (moveController.rect.Contains(moveAreaMousePosition))
        {
            SetSeekPointPosition(Input.mousePosition);
        }
        Vector2 fireAreaMousePosition = fireArea.InverseTransformPoint(Input.mousePosition);
        if (fireArea.rect.Contains(fireAreaMousePosition))
        {
            SetUpPlayersTarget(Input.mousePosition, GameEnums.TrackingGroup.FirstTrackingGroup);

            //if (Input.GetMouseButton(0))
            //{
            //    SetUpPlayersTarget(Input.mousePosition, GameEnums.TrackingGroup.FirstTrackingGroup);
            //}
            //if (Input.GetMouseButton(1))
            //{
            //    SetUpPlayersTarget(Input.mousePosition, GameEnums.TrackingGroup.SecondTrackingGroup);
            //}
        }
#endif
        yield return StartCoroutine(UpdateInputs());
        #endregion
    }
    private void SetUpPlayersTarget(Vector3 mousePosition, GameEnums.TrackingGroup trackingGroup)
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
    private void SetUpPlayersTarget(Touch touch, GameEnums.TrackingGroup trackingGroup)
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
    
    private void SetSeekPointPosition(Touch touch)
    {
        Ray ray = mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, groundMask))
        {
            seekPointPosition.x = hit.point.x;
        }
    }
    private void SetSeekPointPosition(Vector3 mousePosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, groundMask))
        {
            seekPointPosition.x = hit.point.x;
        }
    }
}
