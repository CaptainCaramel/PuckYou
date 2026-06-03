using UnityEngine;

public class CameraFlashScript : MonoBehaviour
{
    public static CameraFlashScript instance;

    private Animator anim;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    public void callFlash()
    {
        anim.Play("CameraFlash");
    }

}
