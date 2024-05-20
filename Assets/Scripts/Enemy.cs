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
    public int MaxHp;
    public int Damage;
    public string Name;
    int Hp;
    public bool onDead = false;
    [SerializeField]
    private Image HpBar;

    [SerializeField]
    private TextMeshProUGUI Life;

    [SerializeField]
    private TextMeshProUGUI NameInfo;

    [SerializeField]
    private TextMeshProUGUI DamageInfo;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 10;
    }
    
    // TO-DO 테이블에 추가 요청

    public void TutoSet()
    {
        Hp = MaxHp;
        Life.text = Hp.ToString();
        DamageInfo.text = Damage.ToString();
    }
    

    public void Set(MonsterData data)
    {
        Name = data.GetName;
        Damage = data.DAMAGE;
        MaxHp = data.HP;

        Hp = MaxHp;
        NameInfo.text = Name;
        Life.text = Hp.ToString();
        DamageInfo.text = Damage.ToString();
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
