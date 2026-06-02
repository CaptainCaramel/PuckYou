using System;
using System.Collections;
using UnityEngine;

public class MeleeEnemyScript : EnemyScript
{
    public GameObject attHitbox;
    public float cooldown;
    public float attackDur;
    public float chargeUp;
    public bool isWhite, isGolem;

    protected override void Start()
    {
        base.Start();
        float speed = isGolem ? 1f : (isWhite ? 4f : 3f);
        setSpeedAndRange(speed, 0.8f);
        fov = 45f;
    }

    protected override IEnumerator AttackCrt()
    {
        halt();
        shouldRotate = false;
        yield return new WaitForSeconds(chargeUp);
        attHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDur);
        attHitbox.SetActive(false);
        isAttacking = false;
        shouldRotate = true;
        animator.SetBool("isIdling", true);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
















