using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialPanel;
    [SerializeField]
    private GameObject skipPanel;
    [SerializeField]
    private TextMeshProUGUI tutorialText;

    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button skipButton;
    [SerializeField]
    private Button skipYes;
    [SerializeField]
    private Button skipNo;
    public int textCount = 0;

    [SerializeField]
    private Image dices;
    [SerializeField]
    private Image damage;
    [SerializeField]
    private Image ranks;

    private Coroutine tutoCorutine;
    bool eventTrigger;
    public int eventCount = 0;
    private void Awake()
    {
        skipButton.onClick.AddListener(() => { skipPanel.gameObject.SetActive(true); });
        //skipYes.onClick.AddListener(() => { skipPanel.gameObject.SetActive(true); });
        skipNo.onClick.AddListener(() => { skipPanel.gameObject.SetActive(false); });
        nextButton.onClick.AddListener(() => { textCount++; eventTrigger = false; eventCount = 0; });
    }

    private void Update()
    {
        switch (textCount)
        {
            case 0:
                DiceMgr.Instance.onDiceRoll = true;
                tutorialText.text = "Wizard Of Dice에 오신것을 \n 환영합니다!";
                break;
            case 1:
                tutorialText.text = "Wizard Of Dice는 주사위를 굴려서 \n마법력을 모으고 조합된 마법을 사용해\n 던전을 탐험하는 게임입니다.";
                break;
            case 2:
                tutorialText.text = "이것이 당신의 주사위입니다";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    tutoCorutine = StartCoroutine(DiceColorCycle());
                }
                break;
            case 3:
                tutorialText.text = "주사위를 굴려볼까요?";
                DiceMgr.Instance.onDiceRoll = true;
                break;
            case 4:
                StopCoroutine(tutoCorutine);
                if (!eventTrigger)
                {
                    dices.CrossFadeColor(UsedColor.usedColor, 1, true, true);
                    nextButton.interactable = false;
                    eventTrigger = true;
                    DiceMgr.Instance.DiceRoll(true, GameMode.Tutorial);
                }
                if (eventCount == 2)
                {
                    tutorialText.text = "4와 6이 나왔네요!";
                    nextButton.interactable = true;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case 5:
                tutorialText.text = "당신의 마법력은 주사위 눈금의 \n 총합으로 결정되어 이곳에 표시됩니다! \n 4와 6의 합계인 10이 보이시나요?";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    tutoCorutine = StartCoroutine(DamageColorCycle());
                }
                break;
            case 6:
                StopCoroutine(tutoCorutine);
                if (!eventTrigger)
                {
                    damage.CrossFadeColor(Color.white, 1, true, true);
                }
                tutorialText.text = "주사위 숫자가 마음에 안드신다고요? \n 욕심쟁이네요! 주사위는 \'재굴림\' \n버튼으로 다시 굴릴 수 있습니다.";
                break;
            case 7:
                tutorialText.text = "또한 굴리기 전에 주사위를 누르시면 \n 주사위를 \'고정\' 할 수 있답니다! 다만 \n 한번 고정한 주사위는 다신 굴리지 못해요.";
                break;
            case 8:

                DiceMgr.Instance.onDiceRoll = false;
                tutorialText.text = "그럼 4를 고정하고 재굴림을 해볼까요?";
                if (!eventTrigger)
                {
                    eventTrigger = true;

                    DiceMgr.Instance.ButtonSelect(0);
                    DiceMgr.Instance.tutorialControl = true;
                    DiceMgr.Instance.tutorialControlMode = 1;
                    DiceMgr.Instance.TutorialButtonControl(true);
                }
                nextButton.interactable = false;
                break;
            case 9:
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    DiceMgr.Instance.onDiceRoll = true;
                    //DiceMgr.Instance.TutorialButtonControl(false);
                    DiceMgr.Instance.manipulList[1] = 4;
                }

                if (eventCount == 1)
                {
                    tutorialText.text = "4가 두개가 나왔습니다!";
                    nextButton.interactable = true;
                    DiceMgr.Instance.tutorialControlMode = 0;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case 10:
                tutorialText.text = "왼쪽 아래에 \'원 페어\'가 보이시나요? \n 그것이 당신이 가진 \'족보\'입니다!";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    tutoCorutine = StartCoroutine(RanksColorCycle());
                }
                break;
            case 11:
                tutorialText.text = "주사위 눈이 특정 족보의 조건에 맞으면 \n 당신의 마법력이 강화됩니다!";
                StopCoroutine(tutoCorutine);
                if (!eventTrigger)
                {
                    ranks.CrossFadeColor(Color.white, 1, true, true);
                }
                break;
            case 12:
                tutorialText.text = "족보는 스테이지를 클리어 할때마다\n 추가로 획득할 수도, 기존 족보를 \n 업그레이드 할 수도 있습니다.";
                break;
            case 13:
                nextButton.interactable = false;
                tutorialText.text = "그럼 이제 공격을 해볼까요?";
                DiceMgr.Instance.tutorialControl = true;
                DiceMgr.Instance.tutorialControlMode = 2;
                break;
        }

    }

    public IEnumerator DiceColorCycle()
    {
        while (true)
        {
            dices.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            dices.CrossFadeColor(UsedColor.usedColor, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public IEnumerator DamageColorCycle()
    {
        while (true)
        {
            damage.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            damage.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }
    public IEnumerator RanksColorCycle()
    {
        while (true)
        {
            ranks.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            ranks.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
