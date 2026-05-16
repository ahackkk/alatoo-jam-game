using UnityEngine;

public class PartSlot : MonoBehaviour
{
    public BuildManager buildManager;
    public PartType acceptedType;
    public Transform snapPoint;

    [HideInInspector]
    public PCpart currentPart;

    public bool isLocked = false;

    public void PlacePart(PCpart part, Rigidbody rb)
    {
        currentPart = part;
        part.currentSlot = this;

        rb.position = snapPoint.position;
        rb.rotation = snapPoint.rotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = false;
        rb.isKinematic = false;
    }

    public void RemovePart(Rigidbody rb)
    {
        currentPart = null;

        rb.isKinematic = false;
        rb.useGravity = true;
    }
}