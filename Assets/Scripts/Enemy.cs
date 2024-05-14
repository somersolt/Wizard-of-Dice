using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    int MaxHp = 100;
    public int Damage = 3;
    public int Hp;

    [SerializeField]
    private Image HpBar;

    [SerializeField]
    private TextMeshProUGUI Life;

    private void Awake()
    {
        Hp = MaxHp;
    }

    public void OnDamage(int d)
    {
        Hp -= d;

        if ( Hp <= 0 )
        {
            Hp = 0;
            StageMgr.Instance.enemies.Remove(this);

            Destroy(gameObject);
            return;
        }
        
        HpBar.fillAmount = (float)Hp / MaxHp;
        Life.text = Hp.ToString();
    }

}
