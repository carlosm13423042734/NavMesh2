using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public int enemiesToSpawn = 3;
    public Transform playerTransform; 

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSpawned && other.CompareTag("Player"))
        {
            hasSpawned = true;

            float spacing = 2f;
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Vector3 offset = new Vector3(i * spacing, 0, 0);
                Vector3 spawnPos = spawnPoint.position + offset;

                GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                
                Enemies enemyScript = newEnemy.GetComponent<Enemies>();
                if (enemyScript != null)
                {
                    enemyScript.player = playerTransform;
                }
            }
        }
    }
}