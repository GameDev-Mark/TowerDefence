using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GenerateMap generateMap;

    private float moveSpeed = 1f;

    private int CounterHowManyWaypointsHasEnemyHit = 0;

    private void Awake()
    {
        generateMap = FindObjectOfType<GenerateMap>();
    }

    private void Update()
    {
        EnemyMovement();
    }

    private void EnemyMovement() 
    {
        // increment counter every time enemy hits checkpoint
        // catch this counter so it does not go over the actual waypoint list count
        if (transform.position == generateMap.waypointsList[CounterHowManyWaypointsHasEnemyHit].transform.position)
        {
            if (CounterHowManyWaypointsHasEnemyHit < generateMap.waypointsList.Count - 1)
            {
                CounterHowManyWaypointsHasEnemyHit++;
            }
        }

        // rotate towards the waypoint first
        // then start moving towards the waypoint position
        transform.LookAt(generateMap.waypointsList[CounterHowManyWaypointsHasEnemyHit].transform.position);
        transform.position = Vector3.MoveTowards(transform.position, generateMap.waypointsList[CounterHowManyWaypointsHasEnemyHit].transform.position, moveSpeed * Time.deltaTime);
    }
}