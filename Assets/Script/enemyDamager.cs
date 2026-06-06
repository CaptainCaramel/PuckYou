using UnityEngine;

public class enemyDamager : MonoBehaviour
{
    public CamShakerScript camShakerScript;

    private Vector3 hitShake = new Vector3(0.1f, 0.4f, 0.075f);

    [SerializeField] private int damage = 2;
    private void Awake()
    {
        camShakerScript = transform.root.GetComponent<CamShakerScript>();
        if (camShakerScript == null) camShakerScript = GetComponent<CamShakerScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("puck collided");
        if (collision.gameObject.GetComponent<PlayerMovement>() != null) return;
        EnemyScript enemyScript = collision.GetComponent<EnemyScript>();
        if (enemyScript != null) enemyScript.damage(damage);

        print(camShakerScript);
        camShakerScript.StartShake(hitShake);
    }
}
