using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class TWDUltShotScript : MonoBehaviour
{
    [SerializeField] private float range = 5f;
    [SerializeField] private float detonateTime = 1.45f;

    [SerializeField] private ContactFilter2D explosionLayer;

    [SerializeField] private int damage = 5;

    [SerializeField] private GameObject explodeParticles;

    private CamShakerScript cShake;

    private Vector3 explShake = new Vector3(3f, 0.25f, 0.35f);

    private void Awake()
   {
        cShake = CamShakerScript.instance;

        StartCoroutine(explodeCRT());
    }

    private IEnumerator explodeCRT()
    {
        yield return new WaitForSeconds(detonateTime);

        List<Collider2D> l = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, range, explosionLayer, l);

        foreach (Collider2D c in l)
        {
            EnemyScript e = c.GetComponent<EnemyScript>();
            if (e != null) e.damage(damage);
        }

        cShake.StartShake(explShake);
        Instantiate(explodeParticles, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Color c = Color.yellow;
        c.a = 0.2f;
        Handles.color = c;

        Handles.DrawSolidDisc(transform.position, new Vector3(0, 0, 1), range);
    }
}
