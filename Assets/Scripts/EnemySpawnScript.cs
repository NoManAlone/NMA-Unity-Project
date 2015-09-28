using UnityEngine;
using System.Collections;
using System;


public class EnemySpawnScript : MonoBehaviour
{
    public int test1;
    public int enemyType;

    //Enemy_Orb Variables
    public int startCorner;
    public int direction;

    public void SpawnEnemy()
    {
        switch (enemyType)
        {
            case 0:
                SpawnEnemyBasic();
                break;
            case 1:
                SpawnEnemyOrb();
                break;
        }
    }

    void SpawnEnemyBasic()
    {
        PhotonNetwork.Instantiate("Enemy_Basic", gameObject.transform.position, Quaternion.identity, 0);
    }

    void SpawnEnemyOrb()
    {

        GameObject enemyOrb = PhotonNetwork.Instantiate("Enemy_Orb", gameObject.transform.position, Quaternion.identity, 0) as GameObject;
        OrbMovement o = enemyOrb.GetComponent<OrbMovement>();
        o.startCorner = startCorner;
        o.moveClockwise = direction;
    }
}

