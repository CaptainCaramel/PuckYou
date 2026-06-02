using UnityEngine;

public class BerserkerPuckScript : PuckScript
{
    [SerializeField] private bool spinMode = false;

    protected override void Awake()
    {
        base.Awake();
        if (sendOnAwake && !spinMode) rb.AddForce(speed * transform.right);
    }

    private void puckParry()
    {
        returnObj = FindClosestObject(EnemyManager.instance.Enemies.ToArray());
        returnMode = true;
        print(returnObj);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.CompareTag("berserkerDash"))
        {
            //print(collision.transform.root.GetComponent<Rigidbody2D>().linearVelocity.magnitude / 0.5f);
            currReturnSpeed *= collision.transform.root.GetComponent<Rigidbody2D>().linearVelocity.magnitude * 0.1f;
            puckParry();
        }
    }

}
