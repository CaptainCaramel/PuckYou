using System;
using System.Collections;
using UnityEngine;

public class WolfScript : EnemyScript
{
    public GameObject bite;
    public float cooldown = 1f;
    public float attackDur = 0.3f;
    protected override void Attack()
    {
        StartCoroutine(AttackCrt());
    }

    IEnumerator AttackCrt()
    {
        rb.linearVelocity = Vector2.zero;
        bite.SetActive(true);
        yield return new WaitForSeconds(attackDur);
        bite.SetActive(false);
        isAttacking = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}










