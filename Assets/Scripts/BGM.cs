using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGM : MonoBehaviour
{
    [SerializeField]
    private Slider BGMsoundBar;
    public float SFXsound;
    [SerializeField]
    private Slider SFXsoundBar;

    public AudioSource currentAudio;
    public AudioSource nextAudio;
    public AudioClip[] bgmList = new AudioClip[10];
    private bool isFading = false;
    public float masterVolume = 1f;

    private bool soundsetting;

    private void UpdateVolume()
    {
        currentAudio.volume = masterVolume;
        nextAudio.volume = masterVolume;
    }

    private void Awake()
    {
        SoundValueUpdate();
        UpdateVolume();
        soundsetting = true;
    }

    void Update()
    {
        if (soundsetting)
        {
            SFXsound = SFXsoundBar.value;
            masterVolume = BGMsoundBar.value;
            PlayerPrefs.SetFloat("BGM", masterVolume);
            PlayerPrefs.SetFloat("SFX", SFXsound);
            UpdateVolume();
        }
    }

    public AudioClip GetBgmOnGameStart(int i, int j)
    {
        if (j == 7)
        {
            return bgmList[i*2 + 1];
        }
        else
        {
            return bgmList[i * 2];
        }
    }

    public AudioClip GetBgmOnNextStage(int i, int j)
    {
        if (j == 7)
        {
            return bgmList[i * 2 + 1];
        }
        else if(j == 1)
        {
            return bgmList[i * 2];
        }
        else
        {
            return null;
        }
    }

    public void PlayBGM(AudioClip newClip, float fadeDuration)
    {
        if (isFading) return;
        if (newClip == null)
        {
            return;
        }

        StartCoroutine(FadeOutIn(newClip, fadeDuration));
    }

    private IEnumerator FadeOutIn(AudioClip newClip, float fadeDuration)
    {
        isFading = true;

        nextAudio.clip = newClip;
        nextAudio.volume = 0f;
        nextAudio.Play();
        float initialCurrentVolume = currentAudio.volume;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            currentAudio.volume = Mathf.Lerp(initialCurrentVolume, 0f, progress);
            nextAudio.volume = Mathf.Lerp(0f, masterVolume, progress);
            yield return null;
        }

        currentAudio.volume = 0f;
        nextAudio.volume = masterVolume;

        currentAudio.Stop();

        var temp = currentAudio;
        currentAudio = nextAudio;
        nextAudio = temp;

        isFading = false;
    }

    private void SoundValueUpdate()
    {
        masterVolume = PlayerPrefs.GetFloat("BGM", 1.0f);
        SFXsound = PlayerPrefs.GetFloat("SFX", 1.0f);
        BGMsoundBar.value = masterVolume;
        SFXsoundBar.value = SFXsound;
    }
}
