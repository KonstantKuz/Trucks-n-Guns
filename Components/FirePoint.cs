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
    public List<GunPoint> gunsPoints;

    public Dictionary<string, GunPoint> GunPointsDictionary { get; private set; }

    public List<GunParent> StaticGroupGuns { get; set; }
    public List<GunParent> FirstTrackingGroupGuns { get; set; }
    public List<GunParent> SecondTrackingGroupGuns { get; set; }

    public Dictionary<GameEnums.TrackingGroup, List<GunParent>> TrackingGroupsDictionary { get; set; }

    private void OnEnable()
    {
        CreateFirePointsDictionaries();
    }

    public void CreateFirePointsDictionaries()
    {
        GunPointsDictionary = new Dictionary<string, GunPoint>(gunsPoints.Count);
        for (int i = 0; i < gunsPoints.Count; i++)
        {
            var gunPoint = gunsPoints[i];
            gunPoint.locationPath = gunPoint.gunsLocation.parent.name + gunPoint.gunsLocation.name;

            if(!GunPointsDictionary.ContainsKey(gunPoint.locationPath))
            {
                GunPointsDictionary.Add(gunPoint.locationPath, gunPoint);
            }
        }

        StaticGroupGuns = new List<GunParent>();
        FirstTrackingGroupGuns = new List<GunParent>();
        SecondTrackingGroupGuns = new List<GunParent>();

        TrackingGroupsDictionary = new Dictionary<GameEnums.TrackingGroup, List<GunParent>>(3);
        TrackingGroupsDictionary.Add(GameEnums.TrackingGroup.FirstTrackingGroup, FirstTrackingGroupGuns);
        TrackingGroupsDictionary.Add(GameEnums.TrackingGroup.SecondTrackingGroup, SecondTrackingGroupGuns);
        TrackingGroupsDictionary.Add(GameEnums.TrackingGroup.StaticGroup, StaticGroupGuns);
    }

    public void MergeFirePoints(FirePoint firePointToMerge)
    {
        for (int i = 0; i < firePointToMerge.gunsPoints.Count; i++)
        {
            for (int j = 0; j < gunsPoints.Count; j++)
            {
                if (gunsPoints[j].locationPath == firePointToMerge.gunsPoints[i].locationPath)
                {
                    gunsPoints.Remove(gunsPoints[j]);
                    GunPointsDictionary.Remove(firePointToMerge.gunsPoints[i].locationPath);
                }
            }
        }

        List<GunPoint> newGunPoints = new List<GunPoint>(gunsPoints.Count + firePointToMerge.gunsPoints.Count);
        for (int i = 0; i < gunsPoints.Count; i++)
        {
            newGunPoints.Add(gunsPoints[i]);
        }

        for (int i = 0; i < firePointToMerge.gunsPoints.Count; i++)
        {
            newGunPoints.Add(firePointToMerge.gunsPoints[i]);
        }
        gunsPoints = newGunPoints;

        CreateFirePointsDictionaries();
    }

    public void SetUpTargets(TargetData targetData, GameEnums.TrackingGroup trackingGroup)
    {
        for (int i = 0; i < TrackingGroupsDictionary[trackingGroup].Count; i++)
        {
            TrackingGroupsDictionary[trackingGroup][i].SetTargetData(targetData);
        }
    }

    public void RemoveGun(GunParent gunToRemove)
    {
        if(FirstTrackingGroupGuns.Contains(gunToRemove))
        {
            FirstTrackingGroupGuns.Remove(gunToRemove);
        }
        if(SecondTrackingGroupGuns.Contains(gunToRemove))
        {
            SecondTrackingGroupGuns.Remove(gunToRemove);
        }
        if (StaticGroupGuns.Contains(gunToRemove))
        {
            StaticGroupGuns.Remove(gunToRemove);
        }
    }
 
   public void StaticAttack()
   {
        for (int i = 0; i < StaticGroupGuns.Count; i++)
        {
            StaticGroupGuns[i].Fire();
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
