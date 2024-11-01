using UnityEngine;

public class BGMController : MonoBehaviour
{
    [SerializeField] private AudioClip gameOverClip, normalClip, scaredClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNormal();
    }

    public void PlayNormal()
    {
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.pitch = 1f;
        audioSource.clip = normalClip;
        audioSource.Play();
    }

    public void PlayScared()
    {
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = scaredClip;
        audioSource.pitch = 1.2f;
        audioSource.Play();
    }

    public void PlayGameOver()
    {
        if (audioSource.isPlaying) audioSource.Stop();
        // audioSource.clip = gameOverClip;
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(gameOverClip);
    }
}