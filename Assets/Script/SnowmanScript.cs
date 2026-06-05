using System.Collections;
using UnityEngine;

public class SnowmanScript : EnemyScript
{
    public Transform projectileSpawnLocation;
    public GameObject projectile;
    public float projectileSpeed = 5;
    public float cooldown = 1.3f;

    [SerializeField] private AudioClip[] deathSounds;
    protected override void Start()
    {
        base.Start();
        deathSound = deathSounds;
        float range = UnityEngine.Random.Range(3, 5);
        setSpeedAndRange(2, range);
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















