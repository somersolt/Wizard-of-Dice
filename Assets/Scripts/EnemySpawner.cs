using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] Pos = new GameObject[3];

    public void Spawn(Enemy enemy, int num)
    {
      var Enemy = Instantiate(enemy.gameObject, Pos[num].transform.position, Quaternion.Euler(0, 180, 0)).GetComponent<Enemy>();
      StageMgr.Instance.enemies.Add(Enemy);
    }
}
