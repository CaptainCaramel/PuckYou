using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WendigoScript : EnemyScript
{
    public float chargeDistance = 25f, attackDistance = 10f;
    public bool canCharge = true, canReCheck;
    public GameObject attHitbox;
    public float chargeUp, attackDur, cooldown;

    public override void Movement()
    {
        if (shouldRotate) lookAt(angleToTarget);

        if (isAttacking) return;

        if(distance <= chargeDistance && canReCheck)
        {
            if(checkForCharge())
            {
                charge();
                return;
            }
        }
        
        if(distance <= attackDistance)
        {
            Attack();
        }
        else
        {
            rb.linearVelocity = direction * speed;
        }
    }

    IEnumerator charge()
    {
        if (!canAttack) yield break;

        isAttacking = true;
        canAttack = false;
    }

    bool checkForCharge()
    {
        StartCoroutine(disableChecking());
        if (canCharge)
        {
            return (Random.Range(0, 3) == 0) ? true : false;
        }
        else return false;
    }

    IEnumerator disableChecking()
    {
        canReCheck = false;
        yield return new WaitForSeconds(6f);
        canReCheck = true;
    }

    protected override void Attack()
    {
        if (!canAttack) return;

        halt();

        isAttacking = true;
        canAttack = false;

        int random = Random.Range(0, 1);

        if(random == 0)
        {
            //animator.Play("Swinging");
            StartCoroutine(swing());
        }
    }
    IEnumerator swing()
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
