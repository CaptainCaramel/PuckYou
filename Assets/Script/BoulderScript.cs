using UnityEngine;

public class BoulderScript : MonoBehaviour
{
    public GameObject particles;

    private void OnDestroy()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
