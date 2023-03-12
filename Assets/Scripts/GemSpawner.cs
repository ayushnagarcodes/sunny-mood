using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GemSpawner : MonoBehaviour
{
    [SerializeField] GameObject gem;
    private Collider2D ground;
    private Collider2D levelEnd;
    private GameObject[] platforms;
    private Vector2 groundLocation;
    private Vector2 platformLocation;

    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("Ground").GetComponent<Collider2D>();
        levelEnd = GameObject.FindGameObjectWithTag("LevelEnd").GetComponent<Collider2D>();
        platforms = GameObject.FindGameObjectsWithTag("Platform");
        
        for (int i = 0; i < 3; i++)
        {
            Instantiate(gem, GeneratePosition(), Quaternion.identity);
        }
    }

    Vector2 GeneratePosition()
    {
        Vector2[] gemSpawnLocations = {};

        // adding Ground (before LevelEnd) as spawn location
        groundLocation = new Vector2(
            Random.Range(ground.bounds.max.x - ground.bounds.size.x + 1.5f, levelEnd.bounds.min.x - 1.5f),
            ground.bounds.max.y + .8f);
        gemSpawnLocations = gemSpawnLocations.Append(groundLocation).ToArray();

        // adding platforms as spawn locations
        // returning Ground location if there are no platforms
        if (platforms.Length == 0)
        {
            return groundLocation;
        }
        
        foreach (GameObject platform in platforms)
        {
            Bounds bound = platform.GetComponent<Collider2D>().bounds;
            platformLocation = new Vector2(Random.Range(bound.max.x - bound.size.x + .4f, bound.max.x - .4f)
                , bound.max.y + .8f);
            gemSpawnLocations = gemSpawnLocations.Append(platformLocation).ToArray();
        }
        
        return gemSpawnLocations[Random.Range(0, gemSpawnLocations.Length)];
    }
}
