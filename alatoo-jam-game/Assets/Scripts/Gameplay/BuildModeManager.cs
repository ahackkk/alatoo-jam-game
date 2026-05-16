using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform buildCameraPoint;

    public BuildDragSystem buildDragSystem;
    public PickupSystem pickupSystem;

    public MonoBehaviour fpsController;
    public MonoBehaviour fpsLook;
    public CharacterController characterController;

    public TableZone tableZone;
    public GameObject hintText;

    public GameObject[] objectsToActivate;

    [Header("Settings")]
    public float enterCooldown = 1f;

    private bool inBuildMode = false;
    private float lastPressTime;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    void Update()
    {
        HandleHint();
        HandleInput();
        HandleCameraLock();
    }

    // ---------------- INPUT ----------------

    void HandleInput()
    {
        if (Time.time < lastPressTime + enterCooldown)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!inBuildMode)
            {
                if (tableZone == null || !tableZone.playerInside)
                    return;

                EnterBuildMode();
                pickupSystem.enabled = false;
                buildDragSystem.enabled = true;
                ActivateObjects();
            }
            else
            {
                ExitBuildMode();
                pickupSystem.enabled = true;
                buildDragSystem.enabled = false;
            }

            lastPressTime = Time.time;
        }
    }

    // ---------------- ENTER ----------------

    void EnterBuildMode()
    {
        inBuildMode = true;

        // сохранить камеру
        originalCamPos = playerCamera.transform.position;
        originalCamRot = playerCamera.transform.rotation;

        // выключаем управление игроком
        if (fpsController != null)
            fpsController.enabled = false;

        if (fpsLook != null)
            fpsLook.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        // сброс предмета из рук
        if (pickupSystem != null)
            pickupSystem.ForceDrop();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (hintText != null)
            hintText.SetActive(false);

        // сразу фиксируем камеру
        playerCamera.transform.position = buildCameraPoint.position;
        playerCamera.transform.rotation = buildCameraPoint.rotation;
    }

    // ---------------- EXIT ----------------

    void ExitBuildMode()
    {
        inBuildMode = false;

        // возвращаем управление
        if (fpsController != null)
            fpsController.enabled = true;

        if (fpsLook != null)
            fpsLook.enabled = true;

        if (characterController != null)
            characterController.enabled = true;

        // сброс предмета из рук
        if (pickupSystem != null)
            pickupSystem.ForceDrop();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (hintText != null)
            hintText.SetActive(true);

        // вернуть камеру назад
        playerCamera.transform.position = originalCamPos;
        playerCamera.transform.rotation = originalCamRot;
    }

    // ---------------- CAMERA LOCK ----------------

    void HandleCameraLock()
    {
        if (!inBuildMode)
            return;

        playerCamera.transform.position = buildCameraPoint.position;
        playerCamera.transform.rotation = buildCameraPoint.rotation;
    }

    // ---------------- HINT ----------------

    void HandleHint()
    {
        if (inBuildMode || hintText == null)
            return;

        hintText.SetActive(tableZone != null && tableZone.playerInside);
    }

    void ActivateObjects()
    {
        if (objectsToActivate == null)
            return;

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
}