using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    [Header("Activate")]
    public GameObject[] objectsToActivate;

    [Header("Deactivate")]
    public GameObject[] objectsToDeactivate;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // 🔥 
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}