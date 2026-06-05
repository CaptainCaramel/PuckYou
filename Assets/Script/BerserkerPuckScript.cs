using UnityEngine;

public class BerserkerPuckScript : PuckScript
{
    [SerializeField] private bool spinMode = false;

    [SerializeField] private AudioClip parrySound;

    protected override void Awake()
    {
        base.Awake();
        if (sendOnAwake && !spinMode) rb.AddForce(speed * transform.right);
    }

    private void puckParry()
    {
        audioManager.instance.playAudio(parrySound, 0.5f, 1, transform, audioManager.instance.sfx);

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
