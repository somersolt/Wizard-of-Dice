using TMPro;
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

    public GameObject continueGamePanel;
    public Button continueButton;
    public Button newGameButton;

    public GameObject quitGamePanel;

    public Button yes;
    public Button no;

    public GameObject storyPanel;
    public TextMeshProUGUI storyText;
    public Button nextStory;
    public TextMeshProUGUI nextButtonText;
    private int storyTextCount = 1;

    public Button returnTitle;

    public BGM titleBgm;


    private void Awake()
    {
        startGame.onClick.AddListener(StartGame);
        endGame.onClick.AddListener(() => { quitGamePanel.gameObject.SetActive(true); });
        option.onClick.AddListener(() => { optionPanel.gameObject.SetActive(true); });
        yes.onClick.AddListener(() => { QuitGame(); });
        no.onClick.AddListener(() => { quitGamePanel.gameObject.SetActive(false); });

        continueButton.onClick.AddListener(GameScene);
        newGameButton.onClick.AddListener(() => { PlayerPrefs.SetInt("Save", 0); continueGamePanel.gameObject.SetActive(false);  storyPanel.gameObject.SetActive(true); });

        returnTitle.onClick.AddListener(() => { optionPanel.gameObject.SetActive(false); });
        skipToggle.isOn = PlayerPrefs.GetInt("Tutorial", 0) == 1;
        skipToggle.onValueChanged.AddListener(OnToggleValueChanged);
        nextStory.onClick.AddListener(OnClickNext);
        storyText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(30001);

    }


    private void OnClickNext()
    {
        storyTextCount++;
        switch (storyTextCount)
        {
            case 1:
                storyText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(30001);
                break;
            case 2:
                storyText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(30002);
                break;
            case 3:
                storyText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(30003);
                break;
            case 4:
                storyText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(30004);
                nextStory.onClick.AddListener(GameScene);
                nextButtonText.text = "Ω√¿€";
                break;
        }
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

    private void StartGame()
    {

        if (PlayerPrefs.GetInt("Save",0) == 1)
        {
            continueGamePanel.gameObject.SetActive(true);
        }
        else
        {
            storyPanel.gameObject.SetActive(true);
        }

    }

    private void GameScene()
    {
        PlayerPrefs.SetFloat("BGM", titleBgm.masterVolume);
        PlayerPrefs.SetFloat("SFX", titleBgm.SFXsound);
        SceneManager.LoadScene("Main");
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
