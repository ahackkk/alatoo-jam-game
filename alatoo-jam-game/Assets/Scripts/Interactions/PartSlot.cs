using UnityEngine;

public class PartSlot : MonoBehaviour
{
    public PartType acceptedType;

    public Transform snapPoint;

    [HideInInspector]
    public PCpart currentPart;
}