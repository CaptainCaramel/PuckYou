using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ShakeCamScript : MonoBehaviour
{
    public static ShakeCamScript instance;

    private void Start()
    {
        instance = this;
    }



}
