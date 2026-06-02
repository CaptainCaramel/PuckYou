using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraLerpScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float lerp;
    private void FixedUpdate()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        transform.position = Vector2.Lerp(player.position, worldPos, lerp);
    }
}
