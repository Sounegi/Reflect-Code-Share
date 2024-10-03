using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    private AudioSource audioSource;

    float fadeOutDuration = 1.5f;

    public static BGMManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void FadeOutMusic()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
    }
}
