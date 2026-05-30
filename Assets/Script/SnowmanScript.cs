using System.Collections;
using UnityEngine;

public class SnowmanScript : EnemyScript
{
    public Transform projectileSpawnLocation;
    public GameObject projectile;
    public float projectileSpeed = 5;
    public float cooldown = 1.3f;
    protected override void Attack()
    {
        StartCoroutine(AttackCrt());
    }

    IEnumerator AttackCrt()
    {
        rb.linearVelocity = Vector2.zero;
        GameObject shotProjectile = Instantiate(projectile, projectileSpawnLocation.position, transform.rotation);
        Rigidbody2D shotProjectileRb = shotProjectile.GetComponent<Rigidbody2D>();
        shotProjectileRb.linearVelocity = dir(player) * projectileSpeed;
        isAttacking = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}







