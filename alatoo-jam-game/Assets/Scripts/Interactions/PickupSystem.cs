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

    private GameObject currentObject;
    private MeshRenderer currentRenderer;


    void Update()
    {
        CheckLookObject();

        if (Input.GetMouseButtonDown(0))
        {
            TryPickup();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DropObject();
        }

        MoveObject();
    }


    void CheckLookObject()
    {
        if (heldObject != null)
        {
            RemoveHighlight();

            itemText.text = "";

            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(
                Screen.width/2,
                Screen.height/2
            )
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                if(currentObject != hit.collider.gameObject)
                {
                    RemoveHighlight();

                    currentObject =
                        hit.collider.gameObject;

                    currentRenderer =
                        currentObject.GetComponentInChildren<MeshRenderer>();


                    EnableOutline();

                    ItemInfo item =
                        currentObject.GetComponent<ItemInfo>();

                    itemText.text =
                        item.itemName;
                }

                return;
            }
        }

        RemoveHighlight();

        itemText.text="";
    }


    void EnableOutline()
    {
        if(currentRenderer == null)
            return;

        currentRenderer.materials[1]
            .SetFloat("_OutlineWidth", 1.05f);

        Debug.Log("Outline ON");
    }

    void RemoveHighlight()
    {
        if(currentRenderer != null)
        {
            currentRenderer.materials[1]
                .SetFloat("_OutlineWidth",1f);

            Debug.Log("Outline OFF");
        }

        currentRenderer = null;
        currentObject = null;
    }

    void TryPickup()
    {
        if (heldObject != null)
            return;

        Ray ray =
            playerCamera.ScreenPointToRay(
            new Vector3(
                Screen.width/2,
                Screen.height/2
            )
        );

        RaycastHit hit;

        if (Physics.Raycast(ray,out hit,pickupRange))
        {
            if(hit.collider.CompareTag("Pickable"))
            {
                heldObject =
                    hit.collider.GetComponent<Rigidbody>();

                heldObject.useGravity=false;

                heldObject.linearDamping=10f;
            }
        }
    }


    void MoveObject()
    {
        if (heldObject==null)
            return;

        Vector3 direction =
            holdPoint.position -
            heldObject.position;

        heldObject.linearVelocity =
            direction*moveSpeed;
    }


    void DropObject()
    {
        if (heldObject==null)
            return;

        heldObject.useGravity=true;

        heldObject.linearDamping=5f;

        heldObject=null;
    }
}