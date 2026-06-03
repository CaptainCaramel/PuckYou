using System.Collections;
using UnityEngine;

public class AfterimageLifeScript : MonoBehaviour
{
    public float liveTime;
    private float startOpacity;

    private float incr;

    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startOpacity = sr.color.a;

        incr = startOpacity/liveTime;
    }
}
