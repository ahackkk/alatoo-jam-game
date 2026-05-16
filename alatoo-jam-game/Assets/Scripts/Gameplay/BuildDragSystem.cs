using UnityEngine;

public class BuildDragSystem : MonoBehaviour
{
    public Camera cam;
    public float dragHeight = 2f;
    public float moveSpeed = 20f;

    private PCpart selectedPart;
    private Rigidbody selectedRb;

    void Update()
    {
        HandleClick();
        Drag();
        Release();
    }

    void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PCpart part = hit.collider.GetComponentInParent<PCpart>();

            if (part == null)
                return;

            // ❗ ВСЕ КРОМЕ КОРПУСА
            if (part.partType == PartType.Case)
                return;

            selectedPart = part;
            selectedRb = part.GetComponent<Rigidbody>();

            if (selectedRb != null)
            {
                selectedRb.useGravity = false;
                selectedRb.linearDamping = 10f;
            }
        }
    }

    void Drag()
    {
        if (selectedRb == null)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 target = ray.GetPoint(dragHeight);

        Vector3 dir = target - selectedRb.position;
        selectedRb.linearVelocity = dir * moveSpeed;
    }

    void Release()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        if (selectedRb != null)
        {
            selectedRb.useGravity = true;
            selectedRb.linearDamping = 5f;

            TrySnap();

            selectedRb = null;
            selectedPart = null;
        }
    }

    void TrySnap()
    {
        if (selectedPart == null || selectedRb == null)
            return;

        Collider[] cols = Physics.OverlapSphere(selectedRb.position, 1.5f);

        foreach (var col in cols)
        {
            PartSlot slot = col.GetComponent<PartSlot>();

            if (slot == null)
                continue;

            if (slot.currentPart != null)
                continue;

            if (slot.acceptedType != selectedPart.partType)
                continue;

            selectedRb.position = slot.snapPoint.position;
            selectedRb.rotation = slot.snapPoint.rotation;

            selectedRb.linearVelocity = Vector3.zero;
            selectedRb.angularVelocity = Vector3.zero;

            selectedRb.useGravity = false;

            slot.currentPart = selectedPart;
            selectedPart.currentSlot = slot;

            return;
        }
    }
}