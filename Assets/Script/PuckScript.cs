using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PuckScript : MonoBehaviour
{

    Rigidbody2D rb;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float returnSpeed = 14.5f;
    [SerializeField] private float returnSpeedIncrement = 3.5f;

    public int damage = 1;

    [SerializeField] private bool sendOnAwake = true;
    [SerializeField] private bool spinMode = false;

    private float currReturnSpeed;

    public GameObject returnObj;

    private bool returnMode;
    private Vector3 moveDir;

    //amp, freq, time
    private Vector3 hitShake = new Vector3(0.1f, 0.4f, 0.075f);

    private float _larpAmount;

    private CamShakerScript camShakerScript;

    private void Awake()
    {
        returnMode = false;
        rb = GetComponent<Rigidbody2D>();
        camShakerScript = GetComponent<CamShakerScript>();


        if(sendOnAwake || !spinMode) rb.AddForce(speed * transform.right);

        currReturnSpeed = returnSpeed;
    }

    private void Update()
    {
        if (returnMode && !spinMode)
        {
            Vector3 dir = (returnObj.transform.position - transform.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

        }
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

    private void returnPuck(PlayerMovement pm)
    {
        pm.StartCoroutine(pm.puckReturn());
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (returnMode && !spinMode)
        {
            rb.linearVelocity = transform.right * currReturnSpeed;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        print("collided");
        EnemyScript enemyScript = collision.transform.root.GetComponent<EnemyScript>();
        PlayerMovement pm = collision.transform.root.GetComponent<PlayerMovement>();

        if (returnMode)
        {
            if (collision.transform.root.gameObject == returnObj && pm != null)
            {
                returnPuck(pm);
                return;
            }

            else if (enemyScript != null)
            {
                returnObj = PlayerMovement.instance.gameObject;
            }
        }

        if (!spinMode)
        {
            currReturnSpeed += returnSpeedIncrement;
            returnMode = true;
        }

        if(enemyScript != null)enemyScript.damage(damage);
        camShakerScript.StartShake(hitShake);
    }
}
