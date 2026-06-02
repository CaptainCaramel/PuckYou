using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PuckScript : MonoBehaviour
{

    protected Rigidbody2D rb;

    [SerializeField] protected float speed = 10f;
    [SerializeField] private float returnSpeed = 14.5f;
    [SerializeField] private float returnSpeedIncrement = 3.5f;

    public int damage = 1;

    [SerializeField] protected bool sendOnAwake = true;

    protected bool collectable = false;
    [SerializeField] private float spawnTime = 0.25f;

    protected ParticleSystem trailParticle;

    protected float currReturnSpeed;

    public GameObject returnObj;

    protected bool returnMode;
    private Vector3 moveDir;

    //amp, freq, time
    protected Vector3 hitShake = new Vector3(0.1f, 0.4f, 0.075f);

    protected CamShakerScript camShakerScript;

    protected virtual void Awake()
    {
        returnMode = false;
        collectable = false;
        rb = GetComponent<Rigidbody2D>();
        camShakerScript = GetComponent<CamShakerScript>();

        trailParticle = transform.GetChild(0).GetComponent<ParticleSystem>();


        

        currReturnSpeed = returnSpeed;

        StartCoroutine(spawnProtection());
    }

    private IEnumerator spawnProtection()
    {
        yield return new WaitForSeconds(spawnTime);
        collectable = true;
    }

    public GameObject FindClosestObject(GameObject[] targets)
    {
        GameObject closest = null;
        float minSqrDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            Vector3 directionToTarget = target.transform.position - currentPosition;

            float sqrDistance = directionToTarget.sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                closest = target;
            }
        }

        return closest;
    }

    protected void returnPuck(PlayerMovement pm)
    {
        pm.StartCoroutine(pm.puckReturn());
        trailParticle.transform.parent = null;
        trailParticle.Stop();
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (returnMode)
        {
            Vector3 dir = (returnObj.transform.position - transform.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

            rb.linearVelocity = transform.right * currReturnSpeed;         
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8) return;
        EnemyScript enemyScript = collision.transform.root.GetComponent<EnemyScript>();
        PlayerMovement pm = collision.transform.root.GetComponent<PlayerMovement>();

        if (collectable && collision.transform.root.gameObject == returnObj && pm != null && !pm.isEing)
        {
            returnPuck(pm);
            return;
        }

        if (returnMode && enemyScript != null)
        {
            returnObj = PlayerMovement.instance.gameObject;
        }


        if (enemyScript != null)
        {
            enemyScript.damage(damage);
            currReturnSpeed += returnSpeedIncrement;
            returnMode = true;
        }
        camShakerScript.StartShake(hitShake);
    }
}
