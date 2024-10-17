using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GenerateMap generateMap;

    private float moveSpeed = 1f;

    private int counterHowManyWaypointsHasEnemyHit = 0;

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
        if (transform.position == generateMap.waypointsList[counterHowManyWaypointsHasEnemyHit].transform.position)
        {
            if (counterHowManyWaypointsHasEnemyHit < generateMap.waypointsList.Count - 1)
            {
                counterHowManyWaypointsHasEnemyHit++;
            }
        }

        // when enemy reaches exit, destroy()
        if (transform.position == generateMap.waypointsList[generateMap.waypointsList.Count - 1].transform.position)
        {
            Destroy(gameObject);
            //Debug.Log("@DESTROY ENEMY.. AT EXIT...");
        }

        // rotate towards the waypoint first
        // then start moving towards the waypoint position
        transform.LookAt(generateMap.waypointsList[counterHowManyWaypointsHasEnemyHit].transform.position);
        transform.position = Vector3.MoveTowards(transform.position, generateMap.waypointsList[counterHowManyWaypointsHasEnemyHit].transform.position, moveSpeed * Time.deltaTime);
    }
}