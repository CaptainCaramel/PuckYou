using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class YetiScript : EnemyScript
{
    private float tooFarDistance = 6.5f;
    private float chaseSpeed = 2.0f, runawaySpeed = 2.8f;
    private bool stop;
    public Transform projectileSpawnLocation;
    public GameObject projectile;
    public float projectileSpeed = 3;
    public float cooldown = 1.3f, chargeUp = 1f;

    protected override void Start()
    {
        base.Start();
        int range = Random.Range(4, 7);
        tooFarDistance = range + 0.5f;
        setSpeedAndRange(3, range);
    }

    public override void Movement()
    {
        if(shouldRotate && isAttacking) rb.SetRotation(angleToTarget);

        if (isAttacking) return;

        float speed = runawaySpeed;

        if (distance <= rangeDistance && !stop)
        {
            float oppositeAngle = angleToTarget + 180f;
            rb.SetRotation(oppositeAngle);
            animator.SetBool("isWalking", true);
            rb.linearVelocity = transform.right * speed;
        }
        else if (distance <= tooFarDistance)
        {
            stop = false;
            halt();
            rb.SetRotation(angleToTarget);
            if (canAttack && !isAttacking)
            {
                animator.SetBool("isWalking", false);
                animator.Play("YetiAttackingAnim");
                animator.SetBool("isAttacking", true);
                StartCoroutine(AttackCrt());
            }
        }
        else
        {
            halt();
            stop = false;
            speed = chaseSpeed;
            rb.SetRotation(angleToTarget);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", true);
            rb.linearVelocity = transform.right * speed;
        }
    }

    protected override IEnumerator AttackCrt()
    {
        isAttacking = true;
        canAttack = false;
        yield return new WaitForSeconds(chargeUp);
        //shouldRotate = false;
        shouldRotate = true;
        GameObject shotProjectile = Instantiate(projectile, projectileSpawnLocation.position, transform.rotation);
        Rigidbody2D shotProjectileRb = shotProjectile.GetComponent<Rigidbody2D>();
        shotProjectileRb.linearVelocity = getDirection(player) * projectileSpeed;
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        shouldRotate = true;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("border")) halt();
        stop = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("border")) halt();
        stop = true;
    }
}
