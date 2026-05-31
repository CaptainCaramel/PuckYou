using System.Collections;
using UnityEngine;

public class SnowmanScript : EnemyScript
{
    public Transform projectileSpawnLocation;
    public GameObject projectile;
    public float projectileSpeed = 5;
    public float cooldown = 1.3f;

    private void Awake()
    {
        setSpeedAndRange(3, 2);
    }

    protected override IEnumerator AttackCrt()
    {
        GameObject shotProjectile = Instantiate(projectile, projectileSpawnLocation.position, transform.rotation);
        Rigidbody2D shotProjectileRb = shotProjectile.GetComponent<Rigidbody2D>();
        shotProjectileRb.linearVelocity = getDirection(player) * projectileSpeed;
        isAttacking = false;
        shouldRotate = true;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}















