using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    protected Transform player;
    protected float speed = 3, rangeDistance = 2;
    protected bool canAttack = true, isAttacking, shouldRotate = true;
    protected Rigidbody2D rb;
    protected float distance;
    protected Vector2 direction;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        EnemyManager.instance.Enemies.Add(gameObject);
    }

    private void OnDestroy()
    {
        EnemyManager.instance.Enemies.Remove(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        Movement();
    }

    public virtual void Movement()
    {
        if (isAttacking) return;

        setDistAndDir();

        if (shouldRotate) rb.SetRotation(getAngle());

        if (distance > rangeDistance)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            halt();

            if(!isAttacking && canAttack)
            {
                isAttacking = true;
                canAttack = false;
                shouldRotate = false;
                Attack();
            }
        }
    }

    public void setDistAndDir()
    {
        distance = getDistance();
        direction = getDirection(player);
    }

    public Vector2 getDirection(Transform target)
    {
        return (target.position - transform.position).normalized;
    }

    public float getDistance()
    {
        return Vector2.Distance(transform.position, player.position); 
    }

    public float getAngle()
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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    protected virtual void Attack()
    {
        StartCoroutine(AttackCrt());
    }

    protected virtual IEnumerator AttackCrt()
    {
        yield return null;
    }

    protected virtual void death()
    {
        Destroy(gameObject);
    }

    public void damage(int damage)
    {
        //hp -= damage
    }

    public void halt()
    {
        rb.linearVelocity = Vector2.zero;
    }
}