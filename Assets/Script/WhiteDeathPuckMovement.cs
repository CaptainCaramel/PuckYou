using System.Collections;
using UnityEngine;

public class WhiteDeathPuckMovement : PuckScript
{
    public bool charged = false;
    [SerializeField] private float returnTime = 3;

    protected override void Awake()
    {
        base.Awake();
        currReturnSpeed *= 3;

        if (sendOnAwake) rb.AddForce(speed * transform.right);
    }

    public IEnumerator returnCRT()
    {
        yield return new WaitForSeconds(returnTime);
        returnMode = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyScript enemyScript = collision.transform.root.GetComponent<EnemyScript>();
        PlayerMovement pm = collision.transform.root.GetComponent<PlayerMovement>();

        if (collectable && collision.transform.root.gameObject == returnObj && pm != null && !pm.isEing)
        {
            returnPuck(pm);
            return;
        }

        if (enemyScript != null)
        {
            enemyScript.damage(damage);
            returnMode = true;
        }

        currReturnSpeed += 2.5f;
        camShakerScript.StartShake(hitShake);
    }
}
