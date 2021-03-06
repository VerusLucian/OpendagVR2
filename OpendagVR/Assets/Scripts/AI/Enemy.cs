﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity {

    protected static float healthMultiplier = 1.2f;
    protected static float damageMultiplier = 1.3f;
    protected static float speedMultiplier = 1.001f;
    protected static float maxSpeed = 10f;
    protected NavMeshAgent agent;
    protected GameObject[] targets;


    public Enemy(string type, float maxHealth, float damage, int level) : base(type, maxHealth, damage, level)
    {

    }

    public void MoveTo(GameObject target)
    {
        if(agent.isStopped)
        {

        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
        
        //Debug.Log(agent.speed);
        //ani.SetBool("isWalking", true);
    }

    public void StopMove()
    {
        agent.isStopped = true;
        //ani.SetBool("isWalking", false);
    }

    protected IEnumerator DieT(GameObject enemy)
    {
        if (enemy.name != "IntroSceneEnemy")
        {
            transform.SetPositionAndRotation(new Vector3(0, -20, 0), new Quaternion(0, 0, 0, 0));
            yield return new WaitForSeconds(1);
            GameObject.Find("WaveController").GetComponent<WaveController>().RemoveFromWave(enemy.name);
        }
        Points sn = GameObject.Find("Points").gameObject.GetComponent<Points>();
        sn.AddPoints("Bow");
        Destroy(enemy);
    }
    protected IEnumerator EnemyAttackTower(bool mainTowerAttack, Collider tower)
    {
        if (mainTowerAttack)
        {
            if(tower != null)
            {
                if (tower.gameObject.GetComponent<Target>().GetCurrentHealth() > 0)
                {
                    Attack(tower.gameObject);
                    yield return new WaitForSeconds(1);
                    StartCoroutine(EnemyAttackTower(mainTowerAttack, tower));
                }
            }
            else
            {
                mainTowerAttack = false;
                agent.isStopped = false;
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i].activeSelf)
                    {
                        MoveTo(targets[i]);
                        break;
                    }
                }
            }
        }
    }

    public IEnumerator TurnOnNavMeshAgent()
    {
        yield return new WaitForSeconds(0.5f);
        agent.enabled = true;
    }

    public void SetLevel(int level, NavMeshAgent pathFinder, bool isIntroWave)
    {
        if (isIntroWave)
        {
            this.level = level;
            pathFinder.speed = 0;
            maxHealth = (float)((maxHealth * healthMultiplier) * this.level);
            curHealth = maxHealth;
            damage = (float)((damage * damageMultiplier) * this.level);
        }
        else
        {
            this.level = level;
            if ((pathFinder.speed * speedMultiplier) * this.level <= maxSpeed && level > 1)
                pathFinder.speed = ((pathFinder.speed * speedMultiplier) * this.level);
            maxHealth = (float)((maxHealth * healthMultiplier) * this.level);
            curHealth = maxHealth;
            damage = (float)((damage * damageMultiplier) * this.level);
        }
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return agent;
    }
}
