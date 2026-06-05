using System.Collections.Generic;
using UnityEngine;

public class DavidPuck : PuckScript
{
    [SerializeField] private float explosionRadius = 4.5f;
    [SerializeField] private ContactFilter2D explosionLayer;
    [SerializeField] private GameObject particles;

    [SerializeField] private AudioClip parrySound;

    protected override void Awake()
    {
        base.Awake();
        if (sendOnAwake) rb.AddForce(speed * transform.right);
    }

    private void explosion()
    {
        audioManager.instance.playAudio(parrySound, 0.5f, 1, transform, audioManager.instance.sfx);

        Instantiate(particles, transform.position, transform.rotation);
        List<Collider2D> l = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, explosionRadius, explosionLayer, l);

        foreach (Collider2D c in l)
        {
            EnemyScript e = c.GetComponent<EnemyScript>();
            if (e != null) e.damage(damage);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.CompareTag("frostbiteDash"))
        {
            explosion();
        }
    }

}
