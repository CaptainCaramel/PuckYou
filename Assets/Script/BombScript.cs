using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] private float detonateTime = 2.5f;
    [SerializeField] private float driftForce = 10f;
    [SerializeField] private float rotationForce = 5;

    [SerializeField] private float explosionRadius = 5;
    [SerializeField] private int damage = 3;

    [SerializeField] private GameObject explodeParticles;
    [SerializeField] private ParticleSystem driftParticles;

    [SerializeField] private ContactFilter2D explosionLayer;

    private Rigidbody2D rb;

    private Vector3 explShake = new Vector3(1.5f, 0.2f, 0.1f);

    private CamShakerScript cShake;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        cShake = CamShakerScript.instance;

        rb.AddForce(transform.right * driftForce * Random.Range(0.5f, 1.5f));
        rb.AddTorque(rotationForce * Random.Range(0.5f, 1.5f));

        StartCoroutine(explodeCRT());
    }

    private IEnumerator explodeCRT()
    {
        yield return new WaitForSeconds(detonateTime);
        print("b");

        driftParticles.transform.parent = null;
        driftParticles.Stop();

        List<Collider2D> l = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, explosionRadius, explosionLayer, l);

        foreach (Collider2D c in l)
        {
            EnemyScript e = c.GetComponent<EnemyScript>();
            if (e != null) e.damage(damage);
        }


        cShake.StartShake(explShake);
        Instantiate(explodeParticles, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
