using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    [SerializeField] private MapGenerator map;

    void Start()
    {
        map.SetSeed(Random.Range(0, int.MaxValue));
        map.GenerateMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            map.SetSeed(Random.Range(0, int.MaxValue));
            map.GenerateMap();
        }
    }
}
