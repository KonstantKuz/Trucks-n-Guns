using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoCached
{
    public LayerMask damageableMask;
    public LayerMask groundMask;
    
    private Vector3 seekPointPosition = Vector3.zero;

    private Player player;

    private Vector2 moveAreaTouchPosition, fireAreaTouchPosition, moveAreaMousePosition, fireAreaMousePosition;

    private Camera playerCamera;

    private Controls controls;

    public void SetUpControlsUI()
    {
        controls = GeneralGameUIHolder.Instance.controls;

        //controls.forwardBoost.onClick.AddListener(() => player.ForwardBoost());
        //controls.backwardBoost.onClick.AddListener(() => player.BackwardBoost());
        //controls.parkingBrake.onClick.AddListener(() => player.StopPlayerTruck());
    }
    private bool forwardBoosted;
    private bool backwardBossted;

    public void StartForwardBoost()
    {
        forwardBoosted = true;
    }

    public void StopForwardBoost()
    {
        forwardBoosted = false;
    }

    public void StartBackwardBoost()
    {
        backwardBossted = true;
    }
    public void StopBackwardBoost()
    {
        backwardBossted = false;
    }

    public void StartUpdateInputs()
    {
        playerCamera = PlayerHandler.Camera;
        player = PlayerHandler.PlayerInstance;
        StartCoroutine(UpdateInputs());
    }
    private IEnumerator UpdateInputs()
    {
        yield return new WaitForEndOfFrame();

        if(forwardBoosted)
        {
            player.ForwardBoost();
        }
        if(backwardBossted)
        {
            player.BackwardBoost();
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
            moveAreaTouchPosition = controls.moveArea.InverseTransformPoint(touch.position);
            if(controls.moveArea.rect.Contains(moveAreaTouchPosition))
            {
                SetSeekPointPosition(touch);
            }
            fireAreaTouchPosition = controls.fireArea.InverseTransformPoint(touch.position);
            if (controls.fireArea.rect.Contains(fireAreaTouchPosition))
            {
                SetUpPlayersTarget(touch, GameEnums.TrackingGroup.FirstTrackingGroup);
            }
        }
#endif
        #endregion
        #region unityEditor
#if UNITY_EDITOR
        moveAreaMousePosition = controls.moveArea.InverseTransformPoint(Input.mousePosition);
        if (controls.moveArea.rect.Contains(moveAreaMousePosition))
        {
            SetSeekPointPosition(Input.mousePosition);
        }
        fireAreaMousePosition = controls.fireArea.InverseTransformPoint(Input.mousePosition);
        if (controls.fireArea.rect.Contains(fireAreaMousePosition))
        {
            SetUpPlayersTarget(Input.mousePosition, GameEnums.TrackingGroup.FirstTrackingGroup);
        }
#endif
        yield return StartCoroutine(UpdateInputs());
        #endregion
    }
    private void SetUpPlayersTarget(Vector3 mousePosition, GameEnums.TrackingGroup trackingGroup)
    {
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
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
        Ray ray = playerCamera.ScreenPointToRay(touch.position);
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
        Ray ray = playerCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, groundMask))
        {
            seekPointPosition.x = hit.point.x;
        }
    }
    private void SetSeekPointPosition(Vector3 mousePosition)
    {
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, groundMask))
        {
            seekPointPosition.x = hit.point.x;
        }
    }
}
