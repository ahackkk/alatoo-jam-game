using TMPro;
using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;

    public float pickupRange = 3f;
    public float moveSpeed = 10f;

    public TextMeshProUGUI itemText;

    private Rigidbody heldObject;
    private PCpart heldPart;

    private GameObject currentObject;
    private ItemInfo currentItem;

    private MeshRenderer currentRenderer;
    private Color originalColor;

    void Update()
    {
        CheckLookObject();

        if (Input.GetMouseButtonDown(0))
            TryPickup();

        if (Input.GetMouseButtonUp(0))
            DropObject();

        MoveObject();
    }

    // ---------------- LOOK ----------------

    void CheckLookObject()
    {
        if (heldObject != null)
        {
            ClearHover();
            itemText.text = "";
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            ItemInfo item = hit.collider.GetComponentInParent<ItemInfo>();

            if (item == null)
            {
                ClearHover();
                itemText.text = "";
                return;
            }

            if (currentItem != item)
            {
                ClearHover();

                currentItem = item;
                currentObject = item.gameObject;

                currentRenderer = currentObject.GetComponentInChildren<MeshRenderer>();

                if (currentRenderer != null)
                {
                    originalColor = currentRenderer.material.color;
                    currentRenderer.material.color = originalColor * 1.2f;
                }

                itemText.text = item.itemName;
            }

            return;
        }

        ClearHover();
        itemText.text = "";
    }

    void ClearHover()
    {
        if (currentRenderer != null)
        {
            currentRenderer.material.color = originalColor;
        }

        currentRenderer = null;
        currentObject = null;
        currentItem = null;
    }

    // ---------------- PICKUP ----------------

    void TryPickup()
    {
        if (heldObject != null)
            return;

        if (currentItem == null)
            return;

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        PCpart part = currentItem.GetComponent<PCpart>();

        if (rb == null)
            return;

        heldObject = rb;
        heldPart = part;

        if (heldPart != null && heldPart.currentSlot != null)
        {
            heldPart.currentSlot.currentPart = null;
            heldPart.currentSlot = null;
        }

        heldObject.useGravity = false;
        heldObject.linearDamping = 10f;
    }

    // ---------------- MOVE ----------------

    void MoveObject()
    {
        if (heldObject == null)
            return;

        Vector3 dir = holdPoint.position - heldObject.position;
        heldObject.linearVelocity = dir * moveSpeed;
    }

    // ---------------- DROP ----------------

    void DropObject()
    {
        if (heldObject == null)
            return;

        TrySnapToSlot();

        heldObject.useGravity = true;
        heldObject.linearDamping = 5f;

        heldObject = null;
        heldPart = null;
    }

    // ---------------- SNAP ----------------

    void TrySnapToSlot()
    {
        if (heldObject == null || heldPart == null)
            return;

        Collider[] nearby = Physics.OverlapSphere(heldObject.position, 1.5f);

        foreach (Collider col in nearby)
        {
            PartSlot slot = col.GetComponent<PartSlot>();

            if (slot == null)
                continue;

            if (slot.currentPart != null)
                continue;

            if (slot.acceptedType != heldPart.partType)
                continue;

            heldObject.position = slot.snapPoint.position;
            heldObject.rotation = slot.snapPoint.rotation;

            heldObject.linearVelocity = Vector3.zero;
            heldObject.angularVelocity = Vector3.zero;

            heldObject.useGravity = false;

            slot.currentPart = heldPart;
            heldPart.currentSlot = slot;

            return;
        }
    }

    // ---------------- FORCE DROP ----------------

    public void ForceDrop()
    {
        if (heldObject == null)
            return;

        heldObject.useGravity = true;
        heldObject.linearVelocity = Vector3.zero;
        heldObject.angularVelocity = Vector3.zero;

        heldObject = null;
        heldPart = null;
    }
}