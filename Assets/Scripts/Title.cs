using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public Button startGame;
    public Button option;
    public Button achievement;
    public Button endGame;

    public GameObject quitGamePanel;
    public Button yes;
    public Button no;

    private void Awake()
    {
        startGame.onClick.AddListener(() => { 
            SceneManager.LoadScene("Main");
            PlayerPrefs.SetFloat("BGM", BGM.Instance.masterVolume);
            PlayerPrefs.SetFloat("SFX", BGM.Instance.SFXsound);
        });
        endGame.onClick.AddListener(() => { quitGamePanel.gameObject.SetActive(true); });
        yes.onClick.AddListener(() => { QuitGame(); });
        no.onClick.AddListener(() => { quitGamePanel.gameObject.SetActive(false); });
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
