
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public List<GameObject> Enemies;

    private void Awake()
    {
        instance = this;
        Enemies = new List<GameObject>();
    }
}
