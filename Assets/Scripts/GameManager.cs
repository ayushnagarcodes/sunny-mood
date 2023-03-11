using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gem;
    
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(gem, new Vector2(Random.Range(1.8f, 30f), -.25f), Quaternion.identity);
        }
    }
    
    void Update()
    {
        
    }
}
