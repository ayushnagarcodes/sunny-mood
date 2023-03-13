using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    private GameObject[] platforms;
    private Vector2 location;
    private int randIndex;
    private int[] previousIndexes = {1000};

    void Start()
    {
        platforms = GameObject.FindGameObjectsWithTag("Platform");
        
        Vector2[] spawnLocations = {};
        // adding platforms of width > 3 as spawn locations
        foreach (GameObject platform in platforms)
        {
            Bounds bound = platform.GetComponent<Collider2D>().bounds;
            if (bound.size.x > 4)
            {
                location = new Vector2(bound.max.x - 1f, bound.max.y + .9f);
                spawnLocations = spawnLocations.Append(location).ToArray();
            }
        }

        // Instantiating enemies
        for (int i = 0; i < 3; i++)
        {
            randIndex = Random.Range(0, spawnLocations.Length);
            // to prevent spawning at the same position
            while (previousIndexes.Contains(randIndex))
            {
                randIndex = Random.Range(0, spawnLocations.Length);
            }
            previousIndexes = previousIndexes.Append(randIndex).ToArray();
            
            Instantiate(enemy, spawnLocations[randIndex], Quaternion.identity);
        }
    }
}
