using UnityEngine;

public class BoulderScript : MonoBehaviour
{
    public GameObject particles;

    private void OnDestroy()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "puck")
        {
            Destroy(gameObject);
            OnDestroy();
        }
    }
}
