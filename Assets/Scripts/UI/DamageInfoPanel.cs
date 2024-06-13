using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DamageInfoPanel : Panel
{

    public TextMeshProUGUI[] damages = new TextMeshProUGUI[5];
    [SerializeField]
    private Button damageExitButton;

    public override void Init()
    {
        base.Init();

        damageExitButton.onClick.AddListener(() => { ClosePanel(); });
    }

    public void DamageInfoUpdate(int totalvalue, int curruntBonusStat, int sum, int multiple, int result)
    {
        damages[0].text = totalvalue.ToString();
        damages[1].text = curruntBonusStat.ToString();
        damages[2].text = sum.ToString();
        damages[3].text = multiple.ToString() + '%';
        damages[4].text = result.ToString();
    }


}
