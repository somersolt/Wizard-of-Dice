using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public Button startGame;
    public Button option;
    public Button endGame;
    public Toggle skipToggle;
    public GameObject optionPanel;
    public GameObject quitGamePanel;
    public Button yes;
    public Button no;

    public Button returnTitle;

    private void Awake()
    {
        startGame.onClick.AddListener(() => { 
            SceneManager.LoadScene("Main");
            PlayerPrefs.SetFloat("BGM", BGM.Instance.masterVolume);
            PlayerPrefs.SetFloat("SFX", BGM.Instance.SFXsound);
        });
        endGame.onClick.AddListener(() => { quitGamePanel.gameObject.SetActive(true); });
        option.onClick.AddListener(() => { optionPanel.gameObject.SetActive(true); });
        yes.onClick.AddListener(() => { QuitGame(); });
        no.onClick.AddListener(() => { quitGamePanel.gameObject.SetActive(false); });
        returnTitle.onClick.AddListener(() => { optionPanel.gameObject.SetActive(false); });
        skipToggle.isOn = PlayerPrefs.GetInt("Tutorial", 0) == 1;
        skipToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("Tutorial", 0);
            PlayerPrefs.Save();
        }
    }


    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
