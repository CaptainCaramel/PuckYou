
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public List<GameObject> Enemies;
    public List<GameObject> EnemiesToSpawn;

    private float minBoundaryX = -25, maxBoundaryX = 11.72f;
    private float maxBoundaryY = 23.78f, minBoundaryY = -9.36f;

    public GameObject spawningParticle;

    private float waitTime, spawnAmount;

    public int killCount, recordedKillCount, rotation;

    public GameObject clock;

    private void Awake()
    {
        instance = this;
        Enemies = new List<GameObject>();

        waitTime = 2f;
        spawnAmount = 1f;
    }

    private void Start()
    {
        StartCoroutine(spawning());
    }

    private void FixedUpdate()
    {
        if (killCount != recordedKillCount)
        {
            if (killCount % 5 == 0)
            {
                waitTime -= 0.25f;
            }

            if (killCount % 25 == 0)
            {
                spawnAmount++;
            }

            recordedKillCount = killCount;
        }
    }

    public void incrimentDeath()
    {
        killCount++;
        rotation += 4;
        //clock.transform.rotation = Quaternion.Euler(0,0,rotation);
    }

    private IEnumerator spawning()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitTime);
            if (EnemyManager.instance.Enemies.Count >= 15) continue;
            for(int i = 0; i <= spawnAmount; i++)
            {
                int toSpawn = Random.Range(0, EnemiesToSpawn.Count - 1);
                float randomX = Random.Range(minBoundaryX, maxBoundaryX);
                float randomY = Random.Range(minBoundaryY, maxBoundaryY);
                GameObject spawned = EnemiesToSpawn[toSpawn];
                Instantiate(spawningParticle, new Vector2(randomX, randomY), Quaternion.identity);
                yield return new WaitForSeconds(0.12f);
                Instantiate(spawned, new Vector2(randomX, randomY), Quaternion.identity);
            }
        }
    }
}
