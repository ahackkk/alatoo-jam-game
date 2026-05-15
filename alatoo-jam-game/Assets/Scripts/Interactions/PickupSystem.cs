using TMPro;
using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;

    public float pickupRange = 3f;
    public float moveSpeed = 10f;

    public TextMeshProUGUI itemText;
    
    private PCpart heldPart;
    private Rigidbody heldObject;

    private GameObject currentObject;

    private MeshRenderer currentRenderer;

    private Color originalColor;


    void Update()
    {
        CheckLookObject();

        if(Input.GetMouseButtonDown(0))
        {
            TryPickup();
        }

        if(Input.GetMouseButtonUp(0))
        {
            DropObject();
        }

        MoveObject();
    }


    void CheckLookObject()
    {
        if(heldObject != null)
        {
            RemoveHighlight();

            itemText.text = "";

            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(
                Screen.width / 2,
                Screen.height / 2
            )
        );

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, pickupRange))
        {
            if(hit.collider.CompareTag("Pickable"))
            {
                if(currentObject != hit.collider.gameObject)
                {
                    RemoveHighlight();

                    currentObject =
                        hit.collider.gameObject;

                    currentRenderer =
                        currentObject.GetComponentInChildren<MeshRenderer>();


                    if(currentRenderer != null)
                    {
                        originalColor =
                            currentRenderer.material.color;

                        currentRenderer.material.color =
                            originalColor * 1.3f;
                    }

                    ItemInfo item =
                        currentObject.GetComponent<ItemInfo>();

                    itemText.text =
                        item.itemName;
                }

                return;
            }
        }

        RemoveHighlight();

        itemText.text = "";
    }


    void RemoveHighlight()
    {
        if(currentRenderer != null)
        {
            currentRenderer.material.color =
                originalColor;
        }

        currentRenderer = null;
        currentObject = null;
    }


    void TryPickup()
    {
        if(heldObject != null)
            return;

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(
                Screen.width / 2,
                Screen.height / 2
            )
        );

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, pickupRange))
        {
            if(hit.collider.CompareTag("Pickable"))
            {
                heldObject =
                    hit.collider.GetComponent<Rigidbody>();

                heldPart =
                    hit.collider.GetComponent<PCpart>();


                if(heldPart.currentSlot != null)
                {
                    heldPart.currentSlot.currentPart = null;
                    heldPart.currentSlot = null;
}

                heldObject.useGravity = false;

                heldObject.linearDamping = 10f;
            }
        }
    }


    void MoveObject()
    {
        if(heldObject == null)
            return;

        Vector3 direction =
            holdPoint.position -
            heldObject.position;

        heldObject.linearVelocity =
            direction * moveSpeed;
    }


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

    void TrySnapToSlot()
    {
        Collider[] nearbyColliders =
            Physics.OverlapSphere(
                heldObject.position,
                1.5f
            );

        foreach(Collider col in nearbyColliders)
        {
            PartSlot slot =
                col.GetComponent<PartSlot>();

            if(slot == null)
                continue;

            if(slot.currentPart != null)
                continue;

            if(slot.acceptedType != heldPart.partType)
                continue;


            heldObject.position =
                slot.snapPoint.position;

            heldObject.rotation =
                slot.snapPoint.rotation;


            heldObject.linearVelocity =
                Vector3.zero;

            heldObject.angularVelocity =
                Vector3.zero;

            heldObject.useGravity = false;


            slot.currentPart = heldPart;

            heldPart.currentSlot = slot;

            return;
        }
    }
}