using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public AudioClip[] voices;
    public float textSpeed = 0.05f;

    [Header("Player")]
    public MonoBehaviour movementScript;
    public MonoBehaviour lookScript;

    [Header("Camera Focus")]
    public Transform playerCamera;
    public Transform lookTarget;

    [Header("Choice System")]
    public int choiceIndex = 7;

    public GameObject[] activateOnChoice1;
    public GameObject[] deactivateOnChoice1;

    public GameObject[] activateOnChoice2;
    public GameObject[] deactivateOnChoice2;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingClip;
    public AudioClip screamerClip;

    [Header("Objects")]
    public GameObject[] objectsToActivateOnStart;
    public GameObject[] objectsToActivateOnEnd;
    public GameObject[] objectsToDeactivateOnEnd;

    private int index;
    private Coroutine typingCoroutine;
    private bool waitingChoice = false;

    void Start()
    {
        textComponent.text = "";
        StartDialogue();
    }

    void Update()
    {
        // 🔥 ФИКС КАМЕРЫ НА ОБЪЕКТ
        if (lookTarget != null && movementScript != null && movementScript.enabled == false)
        {
            playerCamera.LookAt(lookTarget);
        }

        // 🔥 ВЫБОР
        if (waitingChoice)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                ApplyChoice1();

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                ApplyChoice2();

            return;
        }

        // 🔥 ПРОПУСК ДИАЛОГА
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (textComponent.text == lines[index])
                NextLine();
            else
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                textComponent.text = lines[index];
            }
        }
    }

    // ---------------- START ----------------

    void StartDialogue()
    {
        if (movementScript != null)
            movementScript.enabled = false;

        if (lookScript != null)
            lookScript.enabled = false;

        if (screamerClip != null && audioSource != null)
            audioSource.PlayOneShot(screamerClip);

        foreach (GameObject obj in objectsToActivateOnStart)
            if (obj) obj.SetActive(true);

        index = 0;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    // ---------------- TYPE ----------------

    IEnumerator TypeLine()
    {
        textComponent.text = "";

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (voices != null && index < voices.Length && voices[index] != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(voices[index]);
        }

        float lastSoundTime = 0f;
        float soundCooldown = 0.03f;

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            if (c != ' ' && c != '.' && c != ',' &&
                Time.time - lastSoundTime > soundCooldown)
            {
                if (typingClip != null && audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.3f, 1.1f);
                    audioSource.PlayOneShot(typingClip);
                    lastSoundTime = Time.time;
                }
            }

            yield return new WaitForSeconds(textSpeed);
        }

        typingCoroutine = null;
    }

    // ---------------- NEXT ----------------

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;

            textComponent.text = "";

            typingCoroutine = StartCoroutine(TypeLine());

            if (index == choiceIndex)
            {
                waitingChoice = true;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    // ---------------- CHOICE 1 ----------------

    void ApplyChoice1()
    {
        foreach (var obj in activateOnChoice1)
            if (obj) obj.SetActive(true);

        foreach (var obj in deactivateOnChoice1)
            if (obj) obj.SetActive(false);

        waitingChoice = false;

        index++;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    // ---------------- CHOICE 2 ----------------

    void ApplyChoice2()
    {
        foreach (var obj in activateOnChoice2)
            if (obj) obj.SetActive(true);

        foreach (var obj in deactivateOnChoice2)
            if (obj) obj.SetActive(false);

        waitingChoice = false;

        index++;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    // ---------------- END ----------------

    void EndDialogue()
    {
        if (movementScript != null)
            movementScript.enabled = true;

        if (lookScript != null)
            lookScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (GameObject obj in objectsToActivateOnEnd)
            if (obj) obj.SetActive(true);

        foreach (GameObject obj in objectsToDeactivateOnEnd)
            if (obj) obj.SetActive(false);

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        gameObject.SetActive(false);
    }
}