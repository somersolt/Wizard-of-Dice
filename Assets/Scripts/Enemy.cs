using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Canvas canvas;
    public PlayerMessage damagePrefab;
    ParticleSystem particle;
    private Transform effectPos;

    public int MaxHp;
    public int Damage;
    public string Name;
    private float time;
    private float duration = 1f;

    public bool isBoss;
    public bool isimmune;

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
        effectPos = canvas.transform.Find("effectPos").GetComponent<Transform>();


    }

    private void Update()
    {
        if (Time.time > time + duration && onDead)
        {
            gameObject.SetActive(false);
        }
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
        if(isimmune)
        {
            d = 1;
        }

        Hp -= d;
        var DamageMessage = Instantiate(damagePrefab, canvas.transform);
        DamageMessage.Setup(d, Color.red, true);
        particle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Hit 1")), effectPos.transform);
        var main = particle.main;
        main.loop = false;
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
        Destroy(particle.gameObject, main.duration);

        if (Hp <= 0)
        {
            animator.SetTrigger("onDead");
            Hp = 0;
            HpBar.fillAmount = (float)Hp / MaxHp;
            Life.text = Hp.ToString();
            return;
        }

        animator.SetTrigger("GetHit");
        HpBar.fillAmount = (float)Hp / MaxHp;
        Life.text = Hp.ToString();
    }

    public void OnTicDamage(int d)
    {
        if (isimmune)
        {
            d = 1;
        }

        Hp -= d;
        var DamageMessage = Instantiate(damagePrefab, canvas.transform);
        DamageMessage.Setup(d, Color.red, true);
        particle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Hit 13")), effectPos.transform);
        var main = particle.main;
        main.loop = false;
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
        Destroy(particle.gameObject, main.duration);
        if (Hp <= 0)
        {
            animator.SetTrigger("onDead");
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
        onDead = true;
        time = Time.time;
        if (isBoss)
        {
            GameMgr.Instance.bossSignal++;
            return;
        }
        Signal();
    }
    public void Signal()
    {
        GameMgr.Instance.monsterSignal++;
    }


    public void OnHeal(int d)
    {
        Hp += d;
        if (Hp > MaxHp)
        {
            d -= Hp - MaxHp;
            Hp = MaxHp;
        }
        var HealMessage = Instantiate(damagePrefab, canvas.transform);
        HealMessage.Setup(d, Color.green, true);
        particle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Boss1_Healing")), effectPos.transform);
        var main = particle.main;
        main.loop = false;
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
        Destroy(particle.gameObject, main.duration);
        HpBar.fillAmount = (float)Hp / MaxHp;
        Life.text = Hp.ToString();
    }
}
