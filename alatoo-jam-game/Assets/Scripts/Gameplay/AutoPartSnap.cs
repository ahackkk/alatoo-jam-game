using UnityEngine;

public class AutoSnapSlot : MonoBehaviour
{
    public PartType acceptedType;

    public GameObject[] activateOnSnap;
    public GameObject[] deactivateOnSnap;

    private bool isOccupied = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied)
            return;

        PCpart part = other.GetComponentInParent<PCpart>();

        if (part == null)
            return;

        if (part.partType != acceptedType)
            return;

        isOccupied = true;

        // ---------------- SNAP (ПОКА ОТКЛЮЧЕН) ----------------

        /*
        Rigidbody rb = part.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = false;
        rb.isKinematic = true;

        rb.MovePosition(snapPoint.position);
        rb.MoveRotation(snapPoint.rotation);
        */

        // ---------------- ПРОСТО УДАЛЯЕМ ДЕТАЛЬ ----------------

        part.gameObject.SetActive(false);

        // ---------------- АКТИВАЦИЯ ----------------

        if (activateOnSnap != null)
        {
            foreach (GameObject obj in activateOnSnap)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }

        // ---------------- ДЕАКТИВАЦИЯ ----------------

        if (deactivateOnSnap != null)
        {
            foreach (GameObject obj in deactivateOnSnap)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}