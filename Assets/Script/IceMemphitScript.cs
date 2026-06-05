using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class IceMemphitScript : EnemyScript
{
    bool StopFollow, Stop, shouldStart = true;
    public float velocity = 3.5f;

    [SerializeField] private AudioClip[] deathSounds;
    protected override void Start()
    {
        base.Start();
        deathSound = deathSounds;
        velocity = UnityEngine.Random.Range(4, 6);
        setSpeedAndRange(velocity, 1000000000000000000f);
        shouldStart = false;
        shouldRotate = false;
        StartCoroutine(begineer());
    }

    public override void Movement()
    {
        if (shouldRotate && !StopFollow) lookAt(angleToTarget);

        if (!shouldStart) return;
        if (distance <= 3f)
        {
            StopFollow = true;
            if (!Stop) StartCoroutine(StopTimer());
        }

        if (!StopFollow)
        {
            rb.linearVelocity = transform.right * speed;
        }
        else
        {
            rb.linearVelocity = transform.right * speed;
            StartCoroutine(DestroyTimer());
        }
    }


    private IEnumerator StopTimer()
    {
        speed = 0f;
        yield return new WaitForSeconds(0.3f);
        speed = 20f;
        Stop = true;
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private IEnumerator begineer()
    {
        yield return new WaitForSeconds(0.9f);
        snapAtTarget(player);
        animator.SetBool("begin", true);
        shouldStart = true;
        shouldRotate = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.layer == 10)
        {
            death();
        }
    }
}
