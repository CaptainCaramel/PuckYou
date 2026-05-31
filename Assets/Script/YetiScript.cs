using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class YetiScript : EnemyScript
{
    private float tooFarDistance = 6.5f;
    private float chaseSpeed = 2.0f, runawaySpeed = 2.8f;
    private bool runningAway;
    public Transform projectileSpawnLocation;
    public GameObject projectile;
    public float projectileSpeed = 3;
    public float cooldown = 1.3f;

    private void Awake()
    {
        setSpeedAndRange(1, 5.5f);
    }

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        EnemyManager.instance.Enemies.Add(gameObject);
        rb.SetRotation(getAngle());
    }

    public override void Movement()
    {
        if (isAttacking) return;

        setDistAndDir();

        float angle = getAngle();
        float speed = runawaySpeed;

        if (distance < rangeDistance)
        {
            runningAway = true;
            float oppositeAngle = angle + 180f;
            rb.SetRotation(oppositeAngle);
            rb.linearVelocity = transform.right * speed;
        }
        else if (distance >= rangeDistance && distance <= tooFarDistance)
        {
            halt();
            rb.SetRotation(angle);
            if (canAttack && !isAttacking) StartCoroutine(AttackCrt());
        }

        if(distance > tooFarDistance)
        {
            runningAway = false;
            speed = chaseSpeed;
            rb.SetRotation(angle);
            rb.linearVelocity = transform.right * speed;
        }
    }

    protected override IEnumerator AttackCrt()
    {
        isAttacking = true;
        shouldRotate = false;
        canAttack = false;
        GameObject shotProjectile = Instantiate(projectile, projectileSpawnLocation.position, transform.rotation);
        Rigidbody2D shotProjectileRb = shotProjectile.GetComponent<Rigidbody2D>();
        shotProjectileRb.linearVelocity = getDirection(player) * projectileSpeed;
        isAttacking = false;
        shouldRotate = true;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("border")) halt();
    }
}
