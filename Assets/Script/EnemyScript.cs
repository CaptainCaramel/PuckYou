using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    protected Vector2 direction;
    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected float speed, rangeDistance;
    protected float angleToTarget, fov = 1, rotationSpeed = 270f;
    protected float distance;
    protected bool canAttack = true, isAttacking, shouldRotate = true, awake;
    public GameObject deathParticles;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        EnemyManager.instance.Enemies.Add(gameObject);
        snapAtTarget(player);
        StartCoroutine(wakeUp());
    }

    private void OnDestroy()
    {
        EnemyManager.instance.Enemies.Remove(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        if (!awake) return;
        updateTargetData(player);
        Movement();
    }

    public virtual void Movement()
    {
        if (shouldRotate) lookAt(angleToTarget);

        if (isAttacking || !isLookingAt())
        {
            halt();
            return;
        }


        if (distance > rangeDistance)
        {
            animator.SetBool("isIdling", true);
            rb.linearVelocity = direction * speed;
        }
        else
        {
            if (!isAttacking && canAttack)
            {
                Attack();
            }
        }
    }

    /*
    public void setDistDirAngle()
    {
        distance = getDistance();
        direction = getDirection(player);
    }
    */

    public void updateTargetData(Transform target)
    {
        direction = getDirection(target);
        distance = getDistance(target);
        angleToTarget = getAngle(direction);
    }

    public Vector2 getDirection(Transform target)
    {
        return (target.position - transform.position).normalized;
    }

    public float getDistance(Transform target)
    {
        return Vector2.Distance(transform.position, target.position); 
    }

    public float getAngle(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; ;
    }

    public void setSpeedAndRange(float speed, float rangeDistance)
    {
        this.speed = speed;
        this.rangeDistance = rangeDistance;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("puck")) death();
    }

    protected void lookAt(float targetAngle)
    {
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        rb.rotation = newAngle;
    }

    protected void snapAtTarget(Transform target)
    {
        updateTargetData(target);
        rb.rotation = angleToTarget;
    }

    protected bool isLookingAt()
    {
        float angleBeetwen = Vector2.Angle(transform.right, direction);
        return angleBeetwen < (fov / 2f);
    }

    protected IEnumerator wakeUp()
    {
        yield return new WaitForSeconds(0.2f);
        awake = true;
        yield break;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    protected virtual void Attack()
    {
        isAttacking = true;
        canAttack = false;
        animator.SetBool("isIdling", false);
        StartCoroutine(AttackCrt());
    }

    protected virtual IEnumerator AttackCrt()
    {
        yield return null;
    }

    protected virtual void death()
    {
        Instantiate(deathParticles, this.transform.position, Quaternion.identity);
        EnemyManager.instance.incrimentDeath();
        Destroy(gameObject);
    }

    public void damage(int damage)
    {
        death();
    }

    public void halt()
    {
        rb.linearVelocity = Vector2.zero;
    }
}