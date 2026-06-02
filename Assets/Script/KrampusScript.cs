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
        setSpeedAndRange(velocity, 7f);
    }

    protected override IEnumerator AttackCrt()
    {
        yield return new WaitForSeconds(chargeUp);
        foreach(Transform trans in spawnLocations)
        {
            Instantiate(iceMemphit, transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(attackDur);
        isAttacking = false;
        shouldRotate = true;
        animator.SetBool("isIdling", true);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
