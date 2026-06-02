using System.Collections;
using UnityEngine;

public class SnowmanScript : EnemyScript
{
    public Transform projectileSpawnLocation;
    public GameObject projectile;
    public float projectileSpeed = 5;
    public float cooldown = 1.3f;

    protected override void Start()
    {
        base.Start();
        setSpeedAndRange(2, 4f);
        fov = 15f;
    }

    protected override IEnumerator AttackCrt()
    {
        yield return new WaitForSeconds(0.7f);
        GameObject shotProjectile = Instantiate(projectile, projectileSpawnLocation.position, Quaternion.Euler(0,0,angleToTarget - 90));
        Rigidbody2D shotProjectileRb = shotProjectile.GetComponent<Rigidbody2D>();
        shotProjectileRb.linearVelocity = getDirection(player) * projectileSpeed;
        isAttacking = false;
        shouldRotate = true;
        animator.SetBool("isIdling", true);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}















