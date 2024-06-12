using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Mediator mediator;

    public Animator animator;
    public Canvas canvas;
    public PlayerMessage damagePrefab;
    ParticleSystem particle;
    ParticleSystem immuneParticle;
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
        mediator = FindObjectOfType<Mediator>();
        animator = GetComponentInChildren<Animator>();
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 10;
        effectPos = canvas.transform.Find("effectPos").GetComponent<Transform>();

        if(isimmune && mediator.stageMgr.currentField == 4)
        {
            ImmuneEffect(true);
        }
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
        if (mediator.gameMgr.scrollsound >= 3)
        {
            particle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Hit 4")), effectPos.transform);
        }
        else
        {
            particle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Hit 1")), effectPos.transform);
        }
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

    public void WindEffect()
    {
        particle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Boss3_Wind")), effectPos.transform);
        var main = particle.main;
        main.loop = false;
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
        Destroy(particle.gameObject, main.duration);
    }

    public void BloodEffect()
    {
        particle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Boss2_Blood")), effectPos.transform);
        var main = particle.main;
        main.loop = false;
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
        Destroy(particle.gameObject, main.duration);
    }

    public void ImmuneEffect(bool immune)
    {
        if (immune)
        {
            immuneParticle = Instantiate(Resources.Load<ParticleSystem>(string.Format("VFX/magic hit/{0}", "Boss4_Shield")), effectPos.transform);
            var main = immuneParticle.main;
            main.loop = true;
            immuneParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            immuneParticle.Play();
        }
        else
        {
            if (immuneParticle != null)
            {
                immuneParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                Destroy(immuneParticle.gameObject);
                immuneParticle = null;
            }
        }
    }

    public void OnAttack()
    {
        int enemyDamage = mediator.gameMgr.enemyValue + Damage;
        mediator.gameMgr.PlayerOndamage(enemyDamage);
        mediator.gameMgr.monsterSignal++;
    }

    public void Die()
    {
        mediator.stageMgr.DeadEnemies.Add(this);
        mediator.stageMgr.enemies.Remove(this);
        onDead = true;
        time = Time.time;
        if (isBoss)
        {
            mediator.gameMgr.bossSignal++;
            return;
        }
        Signal();
    }
    public void Signal()
    {
        mediator.gameMgr.monsterSignal++;
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
