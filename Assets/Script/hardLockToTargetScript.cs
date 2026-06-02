using UnityEngine;

public class hardLockToTargetScript : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        transform.position = target.position;
    }
}
