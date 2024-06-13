using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Mediator mediator;
    public GameObject[] sides = new GameObject[3];

    private void Awake()
    {
        mediator = FindObjectOfType<Mediator>();
    }

    public void Spawn(Enemy enemy, int num, MonsterData data = null)
    {
        var Enemy = Instantiate(enemy.gameObject, sides[num].transform.position, enemy.gameObject.transform.rotation).GetComponent<Enemy>();
        if (data != null)
        {
            Enemy.Set(data);
        }
        if (data ==  null)
        {
            Enemy.TutoSet();
        }
        mediator.stageMgr.enemies.Add(Enemy);
    }
}
