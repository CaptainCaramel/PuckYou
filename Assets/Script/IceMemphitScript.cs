using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class IceMemphitScript : EnemyScript
{
    bool StopFollow, Stop;
    public float velocity = 3.5f;

    protected override void Start()
    {
        base.Start();
        setSpeedAndRange(velocity, 1000000000000000000f);
    }

    public override void Movement()
    {
        if (distance <= 0.5f)
        {
            StopFollow = true;
            if (!Stop) StartCoroutine(StopTimer());
        }

        if (!StopFollow)
        {
            rb.linearVelocity = transform.right * speed;
            print("abc");
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
        yield return new WaitForSeconds(0.15f);
        speed = 20f;
        Stop = true;
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
