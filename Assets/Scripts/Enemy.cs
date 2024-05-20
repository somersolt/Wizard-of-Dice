using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Canvas canvas;
    public Message damagePrefab;
    public int MaxHp = 100;
    public int Damage = 3;
    int Hp;
    public bool onDead = false;
    [SerializeField]
    private Image HpBar;

    [SerializeField]
    private TextMeshProUGUI Life;

    [SerializeField]
    private TextMeshProUGUI DamageInfo;

    private void Awake()
    {
        Hp = MaxHp;
        Life.text = Hp.ToString();
        DamageInfo.text = Damage.ToString();

        animator = GetComponentInChildren<Animator>();
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 10;
    }

    public void OnDamage(int d)
    {
        Hp -= d;
        var DamageMessage = Instantiate(damagePrefab, canvas.transform);
        DamageMessage.Setup(GameMgr.Instance.currentDamage, Color.red, true);

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


    public void OnAttack()
    {
        int enemyDamage = GameMgr.Instance.enemyValue + Damage;
        GameMgr.Instance.PlayerOndamage(enemyDamage);
        GameMgr.Instance.monsterSignal++;
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
