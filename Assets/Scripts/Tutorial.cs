using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameMgr;

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
    public Button skipButton;
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

    private List<Coroutine> tutoCorutines = new List<Coroutine>(); // �ڷ�ƾ������

    bool eventTrigger;
    public int eventCount = 0;
    private TutorialStep currentStep;

    private enum TutorialStep
    {
        None = -1,
        Welcome = 0,
        Introduction,
        ShowDice,
        RollDicePrompt,
        RollDice,
        ShowMagicPower,
        RerollDicePrompt,
        LockDicePrompt,
        LockAndRerollDice,
        ShowTwoPairs,
        ShowMagicDescription,
        ShowMagicCombination,
        OnePairDescription,
        MagicUpgradeDescription,
        ShowPlayerAttack,
        ShowEnemyAttack,
        ShowEnemyDamageCalculation,
        ShowAttackOrder,
        ExplainMagicVariations,
        ShowMagicCombinationExplanation,
        FinalAttackPrompt,
        EndTutorial
    }

    private void Awake()
    {
        skipButton.onClick.AddListener(() => { skipPanel.gameObject.SetActive(true); });
        skipYes.onClick.AddListener(() =>  TutorialSkip());
        skipNo.onClick.AddListener(() => { skipPanel.gameObject.SetActive(false); });
        nextButton.onClick.AddListener(() => NextStep());
    }

    public void NextStep()
    {
        textCount++;
        currentStep = (TutorialStep)textCount;
        eventTrigger = false; 
        eventCount = 0;
    }

    private void Update()
    {
        switch (currentStep)
        {
            case TutorialStep.Welcome:
                if (!eventTrigger)
                {
                    DiceMgr.Instance.onDiceRoll = true;
                    eventTrigger = true;
                }
                tutorialText.text = "Wizard Of Dice�� ���Ű��� \n ȯ���մϴ�!";
                break;
            case TutorialStep.Introduction:
                tutorialText.text = "Wizard Of Dice�� �ֻ����� ������ \n�������� ������ ���յ� ������ �����\n ������ Ž���ϴ� �����Դϴ�.";
                break;
            case TutorialStep.ShowDice:
                tutorialText.text = "�̰��� ����� �ֻ����Դϴ�";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(DiceColorCycle());
                    tutoCorutines.Add(coroutine);
                }
                break;
            case TutorialStep.RollDicePrompt:
                tutorialText.text = "�ֻ����� ���������?";
                DiceMgr.Instance.onDiceRoll = true;
                break;
            case TutorialStep.RollDice:
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
            case TutorialStep.ShowMagicPower:
                tutorialText.text = "����� �������� �ֻ��� ������ \n �������� �����Ǿ� �̰��� ǥ�õ˴ϴ�! \n 4�� 6�� �հ��� 10�� ���̽ó���?";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(DamageColorCycle());
                    tutoCorutines.Add(coroutine);

                }
                break;
            case TutorialStep.RerollDicePrompt:
                if (!eventTrigger)
                {
                    damage.CrossFadeColor(Color.white, 1, true, true);
                }
                tutorialText.text = "�ֻ��� ���ڰ� ������ �ȵ�Ŵٰ��? \n ������̳׿�! �ֻ����� \'�籼��\' \n��ư���� �ٽ� ���� �� �ֽ��ϴ�.";
                break;
            case TutorialStep.LockDicePrompt:
                tutorialText.text = "���� ������ ���� �ֻ����� �����ø� \n �ֻ����� \'����\' �� �� �ִ�ϴ�! �ٸ� \n �ѹ� ������ �ֻ����� �ٽ� ������ ���ؿ�.";
                break;
            case TutorialStep.LockAndRerollDice:

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
            case TutorialStep.ShowTwoPairs:
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
            case TutorialStep.ShowMagicDescription:
                tutorialText.text = "���� �Ʒ��� \'�� ���\'�� ���̽ó���? \n �װ��� ����� ���� \'����\'�Դϴ�!";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(RanksColorCycle());
                    tutoCorutines.Add(coroutine);
                }
                break;
            case TutorialStep.ShowMagicCombination:
                tutorialText.text = "�ֻ��� ���� Ư�� ������ ���ǿ� ������ \n ����� �������� ��ȭ�˴ϴ�!";
                if (!eventTrigger)
                {
                    ranks.CrossFadeColor(Color.white, 1, true, true);
                }
                break;
            case TutorialStep.OnePairDescription:
                tutorialText.text = "'�� ���' �� ���� �ֻ��� ���� \n �� ���϶� Ȱ��ȭ �Ǵ� �����Դϴ�.";
                break;
            case TutorialStep.MagicUpgradeDescription:
                tutorialText.text = "������ ���������� Ŭ���� �Ҷ�����\n �߰��� ȹ���� ����, ���� ������ \n ���׷��̵� �� ���� �ֽ��ϴ�.";
                break;
            case TutorialStep.ShowPlayerAttack:
                nextButton.interactable = false;
                tutorialText.text = "�׷� ���� ������ �غ����? \n '����' ��ư�� ����������";
                DiceMgr.Instance.tutorialControl = true;
                DiceMgr.Instance.tutorialControlMode = 2;
                break;
            case TutorialStep.ShowEnemyAttack:
                if (!eventTrigger)
                {
                    tutorialText.text = " ";
                    eventTrigger = true;
                    tutorialPanel.gameObject.transform.position += new Vector3(0, -1000, 0);
                    GameMgr.Instance.tutorialMode = true;
                }
                if (eventCount == 1)
                {
                    tutorialText.text = "���̳���? �������� ������ ������ \n ���鵵 �ֻ����� ������ �����ؿ´�ϴ�.";
                    nextButton.interactable = true;
                    DiceMgr.Instance.tutorialControlMode = 0;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case TutorialStep.ShowEnemyDamageCalculation:
                tutorialText.text = "���� �Ӹ� ���� �ִ� �ֻ��� ����, \n ü�¹� ������ ���ݷ��� ���Ѱ��� \n ���� ������ �Դϴ�.";
                break;
            case TutorialStep.ShowAttackOrder:
                tutorialText.text = "�������� ������ �����ϴ� ������, \n ������ �����ϴ� ������ �׻� \n '����'���� ���˴ϴ�. \n �� �� �� �������ּ���!";
                break;
            case TutorialStep.ExplainMagicVariations:
                tutorialText.text = "'�� ���'�� �ϳ��� ���ۿ� \n ������ �� ������, \n ���������� Ŭ���� �� ������ ���� �� \n �ִ� ���� �����鵵 �����մϴ�";
                break;
            case TutorialStep.ShowMagicCombinationExplanation:
                tutorialText.text = "�������� ���� ������� �ֻ����� \n ������ �´ٸ�, ������ ���� �������ϴ�! \n �������� ���� �������°��� �����̰�, \n ���� ������ �� ���� ������.";
                break;
            case TutorialStep.FinalAttackPrompt:
                tutorialText.text = "��, �׷� ���� ������ ���� ����Ʈ�� ������! \n ���� ������ ��ٸ��� �ִ�ϴ�.";
                break;
            case TutorialStep.EndTutorial:
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    DiceMgr.Instance.tutorialControl = false;
                    GameMgr.Instance.tutorialMode = false;
                    tutorialPanel.gameObject.SetActive(false);
                    DiceMgr.Instance.DiceTwo();
                    DiceMgr.Instance.DiceRoll(true);
                }
                break;

        }

    }

    public IEnumerator DiceColorCycle()
    {
        while (currentStep == TutorialStep.ShowDice)
        {
            dices.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            dices.CrossFadeColor(UsedColor.usedColor, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public IEnumerator DamageColorCycle()
    {
        while (currentStep == TutorialStep.ShowMagicPower)
        {
            damage.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            damage.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }
    public IEnumerator RanksColorCycle()
    {
        while (currentStep == TutorialStep.ShowMagicDescription)
        {
            ranks.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            ranks.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    private void TutorialSkip()
    {
        currentStep = TutorialStep.None;
        textCount = -1;
        eventTrigger = false;
        eventCount = 0;
        DiceMgr.Instance.StopAllActiveCoroutines();

        dices.color = UsedColor.usedColor;
        damage.color = UsedColor.whiteColor;
        ranks.color = UsedColor.whiteColor;

        DiceMgr.Instance.tutorialControl = false;
        Instance.tutorialMode = false;
        //DiceMgr.Instance.onDiceRoll = false;
        tutorialPanel.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        skipPanel.gameObject.SetActive(false);

        DiceMgr.Instance.TutorialButtonControl(false);
        DiceMgr.Instance.manipulList[1] = 0;
        //DiceMgr.Instance.selectedDice.Clear();

        Destroy(StageMgr.Instance.enemies[0].gameObject);
        StageMgr.Instance.enemies.Clear();
        //DiceMgr.Instance.InfoClear();
        //Instance.currentDiceCount = DiceCount.three;
        //DiceMgr.Instance.DiceThree();
        //DiceMgr.Instance.DiceRoll();
        Instance.ui.GetDice();

    }

}
