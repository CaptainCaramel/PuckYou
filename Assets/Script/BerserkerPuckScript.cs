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
        print(returnObj);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.CompareTag("berserkerDash"))
        {
            puckParry();
        }
    }
}
