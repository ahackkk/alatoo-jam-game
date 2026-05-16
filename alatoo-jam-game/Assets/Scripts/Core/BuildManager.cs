using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public PartSlot cpuSlot;
    public PartSlot coolerSlot;
    public PartSlot ramSlot;
    public PartSlot gpuSlot;

    public GameObject[] activateOnPlace;

    private PartSlot currentSlot;

    void Start()
    {
        ActivateSlot(cpuSlot);
    }

    // ---------------- TRY PLACE ----------------

    public void TryPlace(PartSlot slot, PCpart part, Rigidbody rb)
    {
        if (slot != currentSlot)
            return;

        if (slot.acceptedType != part.partType)
        {
            Debug.Log("Wrong part!");
            return;
        }

        slot.PlacePart(part, rb);

        ActivateNext(slot);
    }

    // ---------------- SLOT CONTROL ----------------

    void ActivateSlot(PartSlot slot)
    {
        currentSlot = slot;

        if (slot != null)
            slot.gameObject.SetActive(true);
    }

    void DeactivateSlot(PartSlot slot)
    {
        if (slot != null)
            slot.gameObject.SetActive(false);
    }

    // ---------------- NEXT STEP ----------------

    public void ActivateNext(PartSlot last)
    {
        if (last == cpuSlot)
        {
            DeactivateSlot(cpuSlot);
            ActivateSlot(coolerSlot);
        }
        else if (last == coolerSlot)
        {
            DeactivateSlot(coolerSlot);
            ActivateSlot(ramSlot);
        }
        else if (last == ramSlot)
        {
            DeactivateSlot(ramSlot);
            ActivateSlot(gpuSlot);
        }
        else if (last == gpuSlot)
        {
            DeactivateSlot(gpuSlot);
            currentSlot = null;
        }

        // 🔥 глобальные объекты включения
        if (activateOnPlace != null)
        {
            foreach (GameObject obj in activateOnPlace)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }
}