
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public List<GameObject> Enemies;
    public List<GameObject> EnemiesToSpawn;

    private float minBoundaryX = -25, maxBoundaryX = 11.72f;
    private float maxBoundaryY = 23.78f, minBoundaryY = -9.36f;

    public GameObject spawningParticle;

    private float waitTime, spawnAmount;

    public int killCount, recordedKillCount;

    public float rotation;

    public float intensity = 0.37f, strength = 0.35f;

    public GameObject clock;

    public Light2D[] globalLights;
    private FieldInfo fallOfFields = typeof(Light2D).GetField("m_FalloffIntensity", BindingFlags.NonPublic | BindingFlags.Instance);

    public int limit = 15;

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
                limit += 1;
            }

            if (killCount % 18 == 0)
            {
                spawnAmount++;
            }

            recordedKillCount = killCount;
        }
    }


    public bool DoesContainKrampus()
    {
        foreach (GameObject gameObject in Enemies)
        {
            if (gameObject != null && gameObject.name.Contains("Krampus")) return true;
        }
        return false;
    }

    public void incrimentDeath()
    {
        killCount++;
        rotation += 0.55f;
        intensity -= 0.0005f;
        strength += 0.001f;
        clock.transform.rotation = Quaternion.Euler(0,0,rotation);
        if (globalLights[0].intensity <= 0.1f) return;
        for(int i = 0; i < globalLights.Length; i++)
        {
            globalLights[i].intensity = intensity;
            fallOfFields.SetValue(globalLights[i], strength);
        }
    }

    private IEnumerator spawning()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitTime);
            if (EnemyManager.instance.Enemies.Count >= 15) continue;
            for(int i = 0; i <= spawnAmount; i++)
            {
                int toSpawn = UnityEngine.Random.Range(0, EnemiesToSpawn.Count - 1);
                float randomX = UnityEngine.Random.Range(minBoundaryX, maxBoundaryX);
                float randomY = UnityEngine.Random.Range(minBoundaryY, maxBoundaryY);
                GameObject spawned = EnemiesToSpawn[toSpawn];
                if (spawned.name == "Krampus" && DoesContainKrampus() || Enemies.Count == limit) continue;
                if(DoesContainKrampus())
                {
                    print("hockey: " + spawned.name);
                }
                Instantiate(spawningParticle, new Vector2(randomX, randomY), Quaternion.identity);
                yield return new WaitForSeconds(0.12f);
                Instantiate(spawned, new Vector2(randomX, randomY), Quaternion.identity);
            }
        }
    }
}
