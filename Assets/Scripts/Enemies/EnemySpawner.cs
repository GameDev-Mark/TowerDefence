using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GenerateMap generateMap;
    private int enemyCounter;

    private void Start()
    {
        generateMap = FindObjectOfType<GenerateMap>();
        // this we can randomize and do more complex shit with
        InvokeRepeating("SpawnEnemies", 5f, 1f);
    }

    private void SpawnEnemies()
    {
        if (enemyCounter <= 10)
        {
            // increment counter to keep track of enemies (future development)
            enemyCounter++;

            // naming each enemy
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.name = "Enemy_" + enemyCounter;

            // scale, position and rotation
            enemy.transform.localScale = Vector3.one * 0.5f;
            enemy.transform.position =
                new Vector3(generateMap.entranceMapTileGO.transform.position.x, 0.75f, generateMap.entranceMapTileGO.transform.position.z);
            enemy.transform.rotation = Quaternion.identity;

            // add component script that controls enemy behaviour
            enemy.AddComponent<EnemyBehaviour>();

            // colorizing enemies (for now)
            Renderer rend = enemy.GetComponent<Renderer>();
            rend.material.color = Color.red;
        }
    }
}