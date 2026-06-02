using UnityEngine;

public class puckTrackerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject puck;
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Berserker");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = player.transform.position;
        puck = GameObject.Find("BerserkerPuck(Clone)");
        if (puck != null)
        {
            Vector2 direction = (puck.transform.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; ;
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, 360000 * Time.deltaTime);
            rb.rotation = newAngle;
        }
        else
        {
            rb.rotation = player.transform.eulerAngles.z;
        }
    }
}
