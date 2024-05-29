using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGM : MonoBehaviour
{
    private static BGM instance;

    public static BGM Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BGM>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("BGM");
                    instance = obj.AddComponent<BGM>();
                }
            }
            return instance;
        }

    }    // ΩÃ±€≈Ê ∆–≈œ
    [SerializeField]
    private Slider BGMsoundBar;
    public float SFXsound;
    [SerializeField]
    private Slider SFXsoundBar;

    public AudioSource currentAudio;
    public AudioSource nextAudio;
    public AudioClip[] bgm = new AudioClip[10];
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
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
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

    public AudioClip StartBgm(int i, int j)
    {
        if (i == 1)
        {
            if (j == 7)
            {
                return bgm[2];
            }
            else
            {
                return bgm[3];
            }
        }

        if (i == 2)
        {
            if (j == 7)
            {
                return bgm[5];
            }
            else
            {
                return bgm[4];
            }
        }

        if (i == 3)
        {
            if (j == 7)
            {
                return bgm[7];
            }
            else
            {
                return bgm[6];
            }
        }

        if (i == 4)
        {
            if (j == 7)
            {
                return bgm[9];
            }
            else
            {
                return bgm[8];
            }
        }

        return null;
    }

    public AudioClip ChangeBgm(int i, int j)
    {
        if (i == 1)
        {
            if (j == 7)
            {
                return bgm[2];
            }
            else if (j == 1)
            {
                return bgm[3];
            }
            else
            {
                return null;
            }
        }


        if (i == 2)
        {
            if (j == 7)
            {
                return bgm[5];

            }
            else if (j == 1)
            {
                return bgm[4];
            }
            else
            {
                return null;
            }
        }

        if (i == 3)
        {
            if (j == 7)
            {
                return bgm[7];

            }
            else if (j == 1)
            {
                return bgm[6];
            }
            else
            {
                return null;
            }
        }

        if (i == 4)
        {
            if (j == 7)
            {
                return bgm[9];
            }
            else if (j == 1)
            {
                return bgm[8];
            }
            else
            {
                return null;
            }
        }

        return null;
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
