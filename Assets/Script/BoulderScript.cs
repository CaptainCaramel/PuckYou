using UnityEngine;

public class BoulderScript : MonoBehaviour
{
    public GameObject particles;
    [SerializeField] private AudioClip breakSound;

    private void OnDestroy()
    {

        audioManager.instance.playAudio(breakSound, 0.5f, 1, transform, audioManager.instance.sfx);
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
