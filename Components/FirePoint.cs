using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePoint : MonoCached
{
    [System.Serializable]
    public class GunPoint
    {
        public Transform gunsLocation;
        public GunAnglesData allowableAnglesOnPoint;
        public string locationPath { get; set; }
    }
    public GunPoint[] gunsPoints;

    public Dictionary<string, GunPoint> GunPointsDictionary { get; private set; }

    public List<GunParent> StaticGuns { get; set; }
    public List<GunParent> FirstTrackingGroupGuns { get; set; }
    public List<GunParent> SecondTrackingGroupGuns { get; set; }

    public Dictionary<GameEnums.TrackingGroup, List<GunParent>> TrackingGroupsDictionary { get; set; }
    

    public void CreateFirePointsDictionaries()
    {
        
        GunPointsDictionary = new Dictionary<string, GunPoint>(gunsPoints.Length);
        for (int i = 0; i < gunsPoints.Length; i++)
        {
            var gunPoint = gunsPoints[i];
            gunPoint.locationPath = gunPoint.gunsLocation.parent.name + gunPoint.gunsLocation.name;
            GunPointsDictionary.Add(gunPoint.locationPath, gunPoint);
        }

        StaticGuns = new List<GunParent>();
        FirstTrackingGroupGuns = new List<GunParent>();
        SecondTrackingGroupGuns = new List<GunParent>();

        TrackingGroupsDictionary = new Dictionary<GameEnums.TrackingGroup, List<GunParent>>(3);
        TrackingGroupsDictionary.Add(GameEnums.TrackingGroup.FirstTrackingGroup, FirstTrackingGroupGuns);
        TrackingGroupsDictionary.Add(GameEnums.TrackingGroup.SecondTrackingGroup, SecondTrackingGroupGuns);
        TrackingGroupsDictionary.Add(GameEnums.TrackingGroup.StaticGroup, StaticGuns);
    }

    public void SetUpTargets(TargetData targetData, GameEnums.TrackingGroup trackingGroup)
    {
        for (int i = 0; i < TrackingGroupsDictionary[trackingGroup].Count; i++)
        {
            TrackingGroupsDictionary[trackingGroup][i].SetTargetData(targetData);
        }
    }
 
   public void StaticAttack()
   {
        for (int i = 0; i < StaticGuns.Count; i++)
        {
            StaticGuns[i].Fire();
        }
   }

    public void FirstTrackingAttack()
    {
        for (int i = 0; i < FirstTrackingGroupGuns.Count; i++)
        {
            FirstTrackingGroupGuns[i].Fire();
        }
    }

    public void SecondTrackingAttack()
    {
        for (int i = 0; i < SecondTrackingGroupGuns.Count; i++)
        {
            SecondTrackingGroupGuns[i].Fire();
        }
    }
}
