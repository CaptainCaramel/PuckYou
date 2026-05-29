using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PuckScript : MonoBehaviour
{

    Rigidbody2D rb;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float returnSpeed = 14.5f;
    [SerializeField] private float returnSpeedIncrement = 3.5f;

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
        rb.AddForce(speed * transform.right);

        currReturnSpeed = returnSpeed;
    }

    private void Update()
    {
        if (returnMode)
        {
            Vector3 dir = (returnObj.transform.position - transform.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

        }
    }

    private void FixedUpdate()
    {
        if (returnMode)
        {
            rb.linearVelocity = transform.right * currReturnSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (returnMode && collision.transform.parent.TryGetComponent(out PlayerMovement pm))
        {
            pm.StartCoroutine(pm.puckReturn());
            Destroy(gameObject);
            return;
        }
        camShakerScript.StartShake(hitShake);

        currReturnSpeed += returnSpeedIncrement;
        returnMode = true;
    }
}
