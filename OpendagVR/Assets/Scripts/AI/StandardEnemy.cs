﻿using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class StandardEnemy : Enemy {
    public string mainTowerObjectName;
    NavMeshAgent nav;
    static string enemyType = "Standard";
    static int baseHealth = 100;
    static float baseDamage = 2f;
    static float sHealthMultiplier = healthMultiplier;
    static float sDamageMultiplier = damageMultiplier;

    static int startLevel = 1;
    static int health = (int)((baseHealth * sHealthMultiplier) * startLevel);
    static int enemyDamage = (int)((baseDamage * sDamageMultiplier) * startLevel);
	Animator animator; 
    public bool mainTowerAttack = false;
    private bool isAttacking = false;
    private GameObject mainTower;

    public StandardEnemy() : base(enemyType, health, enemyDamage, startLevel)
    {
		
    }

    private void Awake()
    {
        healthBar = this.transform.Find("HealthBarCanvas").Find("HealthBG").Find("HealthBar").GetComponent<Image>();
        agent = GetComponent<NavMeshAgent>();
		animator = this.GetComponent<Animator>(); 

    }

    private void Update()
    {
        if (mainTower == null)
            mainTower = GameObject.Find(mainTowerObjectName);


        if (!isAttacking)
        {
            if (mainTowerAttack && mainTower != null)
            {
                Attack();
                isAttacking = true;
            }
        }
        else
        {
            if (!mainTowerAttack && mainTower != null)
            {
                isAttacking = false;
            }
        }
        
		if (curHealth / maxHealth <= 0.5F)  
		{ 
			animator.SetTrigger ("Hurt"); 
		} 
        if(curHealth <= 0)
        {
            Die();
			animator.SetTrigger ("Die");
        }
        else
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                targets = GameObject.FindGameObjectsWithTag("Target");
                if (targets.Length > 0)
                {
                    MoveTo(targets[0]);
                    mainTowerObjectName = targets[0].name;
                }
            }
                


            if (!agent.isOnNavMesh)
            {
                agent.enabled = false;
                agent.enabled = true;
                NavMeshHit closesthit;
                NavMesh.SamplePosition(gameObject.transform.position, out closesthit, 500f, NavMesh.AllAreas);
                transform.position = closesthit.position;
                agent.isStopped = false;
                targets = GameObject.FindGameObjectsWithTag("Target");
                MoveTo(targets[0]);
            }
        }
    }

    private void Die()
    {
        mainTowerAttack = false;
        StartCoroutine(DieT(gameObject));

    }

    private void Attack()
    {
        StopMove();
		animator.SetTrigger("Attack"); 
        StartCoroutine(EnemyAttackTower(mainTowerAttack, mainTower.GetComponent<Collider>()));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            StopMove();
            animator.SetTrigger("Attack");
            mainTowerAttack = true;
            StartCoroutine(EnemyAttackTower(mainTowerAttack, other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            animator.SetTrigger("Walk");
            mainTowerAttack = false;
        }
    }
} 
