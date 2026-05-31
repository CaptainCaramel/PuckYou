using System;
using System.Collections;
using UnityEngine;

public class WolfScript : EnemyScript
{
    public GameObject bite;
    public float cooldown = 1f;
    public float attackDur = 0.3f;

    private void Awake()
    {
        setSpeedAndRange(3, 2);
    }

    protected override IEnumerator AttackCrt()
    {
        bite.SetActive(true);
        yield return new WaitForSeconds(attackDur);
        bite.SetActive(false);
        isAttacking = false;
        shouldRotate = true;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
















