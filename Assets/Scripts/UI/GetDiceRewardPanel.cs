using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetDiceRewardPanel : Panel
{
    [SerializeField]
    private Image getDiceImage;
    [SerializeField]
    private Button diceRewardConfirm;

    public override void Init()
    {
        base.Init();

        diceRewardConfirm.onClick.AddListener(() =>
        {
            ClosePanel();
            mediator.gameMgr.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
            mediator.stageMgr.NextStage();
        });
    }

    public void GetDice()
    {
        RewardSound(1);

        switch (mediator.gameMgr.currentDiceCount)
        {
            case GameMgr.DiceCount.two:
                getDiceImage.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Got_Dice_2to3"));
                break;
            case GameMgr.DiceCount.four:
                getDiceImage.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Got_Dice_3to4"));
                break;
            case GameMgr.DiceCount.five:
                getDiceImage.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Got_Dice_4to5"));
                break;
        }

        SlideOpenPanel();
    }
}
