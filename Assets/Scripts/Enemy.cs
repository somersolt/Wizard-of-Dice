using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int MaxHp = 100;
    public int Damage = 3;
    int Hp;
    public bool onDead = false;
    [SerializeField]
    private Image HpBar;

    [SerializeField]
    private TextMeshProUGUI Life;

    private void Awake()
    {
        Hp = MaxHp;
        Life.text = Hp.ToString();

        animator = GetComponent<Animator>();
    }

    public void OnDamage(int d)
    {
        Hp -= d;
        if ( Hp <= 0 )
        {
            animator.SetTrigger("onDead");
            onDead = true;
            Hp = 0;
            HpBar.fillAmount = (float)Hp / MaxHp;
            Life.text = Hp.ToString();
            return;
        }

        animator.SetTrigger("GetHit");

        HpBar.fillAmount = (float)Hp / MaxHp;
        Life.text = Hp.ToString();
    }

    public void Die()
    {
        StageMgr.Instance.DeadEnemies.Add(this);
        StageMgr.Instance.enemies.Remove(this);

        Signal();
    }
    public void Signal()
    {
        GameMgr.Instance.monsterSignal++;
    }

}
