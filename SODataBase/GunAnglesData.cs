using UnityEngine;
[CreateAssetMenu(fileName = "GunAnglesData", menuName = "Data/GunAnglesData")]
public class GunAnglesData : ScriptableObject
{
    public int HeadHolderMaxAngle;
    public int HeadMaxAngle;
    public int HeadMinAngle;
    public int StartDirectionAngle;

    public const int headHolderMaxAngle = 180;
    public const int headMaxAngle = 60;
    public const int headMinAngle = -20;
}
