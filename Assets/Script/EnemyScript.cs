using System;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    protected Transform player;
    [SerializeField]
    private float speed = 3, rangeDistance = 2;
    protected bool canAttack = true, isAttacking;
    protected Rigidbody2D rb;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        Movement();
    }

    public void Movement()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = dir(player);

        if (distance > rangeDistance)
        {
            rb.linearVelocity = direction * speed;
        }
        else if (!isAttacking && canAttack)
        {
            isAttacking = true;
            canAttack = false;
            Attack();
        }

        if(!isAttacking)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.SetRotation(angle);
        }
    }

    public Vector2 dir(Transform target)
    {
        return (target.position - transform.position).normalized;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        print("triggered");
        if (collision.gameObject.tag.Equals("puck")) death();
    }

    /*
     public void setisAttacking(bool val)
    {
        isAttacking = val;
    }
    */

    protected virtual void Attack()
    { }

    protected virtual void death()
    {
        Destroy(gameObject);
    }
}