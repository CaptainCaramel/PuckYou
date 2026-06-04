using System.Collections;
using UnityEngine;

public class CampFireScript : MonoBehaviour
{
    public GameObject campfire;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawning());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator spawning()
    {
        while (true)
        {
            float x;
            float y;
            int rotation = 0;
            float waitTime = UnityEngine.Random.Range(10, 20);
            int up = UnityEngine.Random.Range(0, 2);
            if (up == 1)
            {
                x = UnityEngine.Random.Range(-26.59f, 14.98f);
                y = 31.28f;
                rotation = 270;
            }
            else
            {
                x = -32.95f;
                y = UnityEngine.Random.Range(-11.78f, 25.32f);
            }
            yield return new WaitForSeconds(waitTime);
            GameObject spawned = Instantiate(campfire, new Vector2(x, y), Quaternion.Euler(0, 0, rotation));
            spawned.GetComponent<Rigidbody2D>().linearVelocity = spawned.transform.right * 5;
            yield return new WaitForSeconds(1f);
        }
    }
}
