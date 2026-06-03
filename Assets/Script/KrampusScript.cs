using System.Collections;
using UnityEngine;

public class KrampusScript : EnemyScript
{
    public GameObject iceMemphit;
    public float cooldown, chargeUp, attackDur;
    public Transform[] spawnLocations;
    public float velocity = 2f;

    protected override void Start()
    {
        base.Start();
        float range = UnityEngine.Random.Range(8, 10);
        setSpeedAndRange(velocity, range);
        fov = 180;
    }

    protected override IEnumerator AttackCrt()
    {
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(chargeUp);
        foreach(Transform trans in spawnLocations)
        {
            Instantiate(iceMemphit, trans.position, Quaternion.Euler(0,0, 90));
        }
        yield return new WaitForSeconds(attackDur);
        isAttacking = false;
        shouldRotate = true;
        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
