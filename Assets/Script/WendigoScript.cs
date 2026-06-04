using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WendigoScript : EnemyScript
{
    public float chargeDistance = 25f, attackDistance = 2f;
    public bool canCharge = true, canReCheck = true;
    public GameObject hexaHitbox, clawHitbox;
    public float chargeUpS, chargeUpC, attackDurC, cooldownC, attackDurS, cooldownS, chargeSpeed;
    public bool isCharging, isBegin = true;
    public Animator clawAnimator;
    public CamShakerScript cum;
    public Vector3 wallShake = new Vector3(2f,2f,1f);
    public bool alrHit;
    public bool start;

    protected override void Start()
    {
        base.Start();
        setSpeedAndRange(8, 0);
        rotationSpeed = 360;
        alrHit = false;
        StartCoroutine(starter());
    }

    public override void Movement()
    {
        if (!start) return;

        bool isInRange = distance <= attackDistance;
        if (shouldRotate || isAttacking && isInRange && !isCharging) lookAt(angleToTarget);

        if (isAttacking) return;

        if(distance <= chargeDistance && canReCheck)
        {
            if(checkForCharge())
            {
                StartCoroutine(charge(true));
                return;
            }
        }
        
        if(isInRange)
        {
            Attack();
        }
        else
        {
            animator.SetBool("isWalking", true);
            clawAnimator.SetBool("isAttacking", true);
            rb.linearVelocity = direction * (speed);
            alrHit = true;
        }
    }

    IEnumerator charge(bool isFirst)
    {
        if (!canAttack && isFirst) yield break;
        isAttacking = true;
        isCharging = true;
        animator.SetBool("isHit", false);
        if (isFirst)
        {
            halt();
            clawAnimator.SetBool("isCrashingOut", false);
            clawAnimator.SetBool("isAttacking", false);
            animator.SetBool("isCharging", true);
            yield return new WaitForSeconds(0.5f);
        }
        print("Charging");
        isAttacking = true;
        canAttack = false;
        if(isFirst)yield return new WaitForSeconds(chargeUpC);
        shouldRotate = false;
        Vector2 dir = direction;
        //animator.Play("Charge");
        rb.linearVelocity = dir * (chargeSpeed);
    }

    bool checkForCharge()
    {
        StartCoroutine(disableChecking());
        if (canCharge)
        {
            return (Random.Range(0, 6) == 0) ? true : false;
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
        clawAnimator.SetBool("isCrashingOut", true);

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
        GameObject attHitbox = UnityEngine.Random.Range(0, 2) == 0 ? clawHitbox : hexaHitbox;
        yield return new WaitForSeconds(chargeUpS);
        //attHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDurS);
        //Time.timeScale = 0;
        //attHitbox.SetActive(false);
        clawAnimator.SetBool("isCrashingOut", false);
        //animator.SetBool("isIdling", true);
        yield return new WaitForSeconds(cooldownS);
        //Time.timeScale = 0;
        isAttacking = false;
        shouldRotate = true;
        canAttack = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collision gameobject: " + collision.gameObject.tag);
        if(isCharging && collision.gameObject.tag.Equals("border"))
        {
            print("dashing again");
            cum.StartShake(wallShake);
            StartCoroutine(backCharge());
        }
    }

    IEnumerator backCharge()
    {
        rb.angularVelocity = 0;
        rb.linearVelocity = -transform.forward * 50;
        halt();
        if (isBegin)
        {
            isBegin = false;
            animator.SetBool("isHit", true);
            transform.Rotate(new Vector3(0, 0, 180));
            yield return new WaitForSeconds(1.5f);
            snapAtTarget(player);
            animator.SetBool("isHit", false);
            StartCoroutine(charge(false));
        }
        else
        {
            isBegin = true;
            isCharging = false;
            animator.SetBool("isStopped", true);
            yield return new WaitForSeconds(0.3f);
            animator.SetBool("isCharging", false);
            animator.SetBool("isStopped", false);
            canAttack = true;
            isAttacking = false;
            shouldRotate = true;
            yield break;
        }
    }

    public IEnumerator starter()
    {
        start = false;
        yield return new WaitForSeconds(4f);
        start = true;
    }
}
