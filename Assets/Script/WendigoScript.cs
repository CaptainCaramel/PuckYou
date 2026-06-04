using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WendigoScript : EnemyScript
{
    public float chargeDistance = 25f, attackDistance = 2f;
    public bool canCharge = true, canReCheck = true;
    public GameObject attHitbox;
    public float chargeUpS, chargeUpC, attackDurC, cooldownC, attackDurS, cooldownS, chargeSpeed;
    public bool isCharging, isBegin = true;

    protected override void Start()
    {
        base.Start();
        setSpeedAndRange(11, 0);
    }

    public override void Movement()
    {
        print("fov" + fov);
        print("distance: " + distance);
        if (shouldRotate) lookAt(angleToTarget);

        if (isAttacking) return;

        if(distance <= chargeDistance && canReCheck)
        {
            if(checkForCharge())
            {
                StartCoroutine(charge(true));
                return;
            }
        }
        
        if(distance <= attackDistance)
        {
            Attack();
        }
        else
        {
            animator.SetBool("isWalking", true);
            rb.linearVelocity = direction * (speed);
        }
    }

    IEnumerator charge(bool isFirst)
    {
        if (!canAttack && isFirst) yield break;
        print("Charging");
        isAttacking = true;
        canAttack = false;
        shouldRotate = false;
        Vector2 dir = direction;
        yield return new WaitForSeconds(chargeUpC);
        isCharging = true;
        //animator.Play("Charge");
        rb.linearVelocity = dir * (chargeSpeed);
    }

    bool checkForCharge()
    {
        StartCoroutine(disableChecking());
        if (canCharge)
        {
            return (Random.Range(0, 8) == 0) ? true : false;
        }
        else return false;
    }

    IEnumerator disableChecking()
    {
        canReCheck = false;
        yield return new WaitForSeconds(4f);
        canReCheck = true;
    }

    protected override void Attack()
    {
        if (!canAttack) return;

        halt();

        animator.SetBool("isWalking", false);

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
        yield return new WaitForSeconds(chargeUpS);
        attHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDurS);
        attHitbox.SetActive(false);
        isAttacking = false;
        shouldRotate = true;
        //animator.SetBool("isIdling", true);
        yield return new WaitForSeconds(cooldownS);
        canAttack = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isCharging && collision.gameObject.tag.Equals("border"))
        {
            halt();
            if(!isBegin)
            {
                //animator.Play("decharge");
                canAttack = true;
                isAttacking = false;
                shouldRotate = true;
                isBegin = true;
                return;
            }
            isBegin = false;
            snapAtTarget(player);
            StartCoroutine(charge(false));
        }
    }
}
