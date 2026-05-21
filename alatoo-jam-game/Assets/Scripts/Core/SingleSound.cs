using UnityEngine;

public class SingleSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;

    public bool playOnStart = true;
    public bool loop = false;
    public float volume = 1f;

    void Start()
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;

        if (playOnStart)
            audioSource.Play();
    }

    public void PlaySound()
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    public void StopSound()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}