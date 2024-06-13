using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

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

        if(isimmune && mediator.stageMgr.currentField == mediator.stageMgr.lastField)
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

    public void OnDamage(int damage)
    {
        ApplyDamage(damage, mediator.gameMgr.scrollsound >= 3 ? "Hit 4" : "Hit 1");
    }

    public void OnTicDamage(int damage)
    {
        ApplyDamage(damage, "Hit 13");
    }

    private void ApplyDamage(int damage, string particleEffectName)
    {
        if (isimmune)
        {
            damage = 1;
        }

        Hp -= damage;
        ShowDamageMessage(damage);
        PlayParticleEffect(particleEffectName);

        if (Hp <= 0)
        {
            animator.SetTrigger("onDead");
            Hp = 0;
        }
        else
        {
            animator.SetTrigger("GetHit");
        }

        UpdateHealthUI();
    }

    private void ShowDamageMessage(int damage)
    {
        var DamageMessage = Instantiate(damagePrefab, canvas.transform);
        DamageMessage.Setup(damage, Color.red, true);
    }

    private void PlayParticleEffect(string effectName)
    {
        particle = Instantiate(Resources.Load<ParticleSystem>($"VFX/magic hit/{effectName}"), effectPos.transform);
        var main = particle.main;
        main.loop = false;
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
        Destroy(particle.gameObject, main.duration);
    }

    private void UpdateHealthUI()
    {
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
        mediator.stageMgr.deadEnemies.Add(this);
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

        PlayParticleEffect("Boss1_Healing");
        HpBar.fillAmount = (float)Hp / MaxHp;
        Life.text = Hp.ToString();
    }
}