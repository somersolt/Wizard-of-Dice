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
                tutorialText.text = "Wizard Of Dice�� ���Ű��� \n ȯ���մϴ�!";
                break;
            case 1:
                tutorialText.text = "Wizard Of Dice�� �ֻ����� ������ \n�������� ������ ���յ� ������ �����\n ������ Ž���ϴ� �����Դϴ�.";
                break;
            case 2:
                tutorialText.text = "�̰��� ����� �ֻ����Դϴ�";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    tutoCorutine = StartCoroutine(DiceColorCycle());
                }
                break;
            case 3:
                tutorialText.text = "�ֻ����� ���������?";
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
                    tutorialText.text = "4�� 6�� ���Գ׿�!";
                    nextButton.interactable = true;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case 5:
                tutorialText.text = "����� �������� �ֻ��� ������ \n �������� �����Ǿ� �̰��� ǥ�õ˴ϴ�! \n 4�� 6�� �հ��� 10�� ���̽ó���?";
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
                tutorialText.text = "�ֻ��� ���ڰ� ������ �ȵ�Ŵٰ��? \n ������̳׿�! �ֻ����� \'�籼��\' \n��ư���� �ٽ� ���� �� �ֽ��ϴ�.";
                break;
            case 7:
                tutorialText.text = "���� ������ ���� �ֻ����� �����ø� \n �ֻ����� \'����\' �� �� �ִ�ϴ�! �ٸ� \n �ѹ� ������ �ֻ����� �ٽ� ������ ���ؿ�.";
                break;
            case 8:

                DiceMgr.Instance.onDiceRoll = false;
                tutorialText.text = "�׷� 4�� �����ϰ� �籼���� �غ����?";
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
                    tutorialText.text = "4�� �ΰ��� ���Խ��ϴ�!";
                    nextButton.interactable = true;
                    DiceMgr.Instance.tutorialControlMode = 0;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case 10:
                tutorialText.text = "���� �Ʒ��� \'�� ���\'�� ���̽ó���? \n �װ��� ����� ���� \'����\'�Դϴ�!";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    tutoCorutine = StartCoroutine(RanksColorCycle());
                }
                break;
            case 11:
                tutorialText.text = "�ֻ��� ���� Ư�� ������ ���ǿ� ������ \n ����� �������� ��ȭ�˴ϴ�!";
                StopCoroutine(tutoCorutine);
                if (!eventTrigger)
                {
                    ranks.CrossFadeColor(Color.white, 1, true, true);
                }
                break;
            case 12:
                tutorialText.text = "������ ���������� Ŭ���� �Ҷ�����\n �߰��� ȹ���� ����, ���� ������ \n ���׷��̵� �� ���� �ֽ��ϴ�.";
                break;
            case 13:
                nextButton.interactable = false;
                tutorialText.text = "�׷� ���� ������ �غ����?";
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
