using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.volume = 0.2f;
            audioSource.Play();
        }
    }
}