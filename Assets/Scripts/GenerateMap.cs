using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    private int xTileMap;
    private int yTileMap;
    private int entranceMapTileNumber;
    private int exitMapTileNumber;
    private int checkpointMapTileNumber;
    private List<int> tileList;
    private List<int> edgeOfMapTileList;
    private List<int> groundMapTileList;

    private GameObject mapTileGO;
    private GameObject entranceMapTileGO;
    private GameObject exitMapTileGO;
    private GameObject edgeMapTileGO;
    private GameObject continuedPathMapTileGO;
    private GameObject walkPathCheckpointMapTileGO;

    private bool isMapEntranceSpawned = false;
    private bool isMapExitSpawned = false;
    private bool isMapWalkPathCheckpointsSpawned = false;
    private bool isPathFinderAimingAtcheckpoint = false;


    private void Start()
    {
        CreatePathFinder();

        PopulateMapLists();

        xTileMap = 10;
        yTileMap = 10;

        RandomEntranceAndExitAndWalkPathCheckpointsMapTileNumbers();
        Invoke("SpawnMapTiles", 0.2f);
        Invoke("FindAndCreateMapWalkPath", 0.5f);
    }

    private void FixedUpdate()
    {
        if (isMapWalkPathCheckpointsSpawned)
            PathFinderPhysics();
    }

    // fill map lists - called in the start function
    private void PopulateMapLists()
    {
        tileList = new List<int>();

        edgeOfMapTileList = new List<int>()
        { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 20,
            21, 30, 31, 40, 41, 50, 51, 60, 61, 70, 71, 80,
            81, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100
        };

        groundMapTileList = new List<int>()
        { 12, 13, 14, 15, 16, 17, 18, 19,
        22, 23, 24, 25, 26, 27, 28, 28, 29,
        32, 33, 34, 35, 36, 37, 38, 39,
        42, 43, 44, 45, 46, 47, 48, 49,
        52, 53, 54, 55, 56, 57, 58, 59,
        62, 63, 64, 65, 66, 67, 68, 69,
        72, 73, 74, 75, 76, 77, 78, 79,
        82, 83, 84, 85, 86, 87, 88, 89
        };
    }

    // called at the start
    private void SpawnMapTiles()
    {
        for (int i = 0; i < xTileMap; i++)
        {
            for (int j = 0; j < yTileMap; j++)
            {
                GenerateMapTileGameObject(i, j);

                GenerateMapTileBoxCollider();
                GenerateMapTileMaterial(mapTileGO, Color.green);

                tileList.Add(j);
                //Debug.Log("TileList == " + tileList.Count);

                FindAndCreateMapEdges();
                FindAndCreateMapEntrance();
                FindAndCreateMapExit();
                FindAndCreateMapWalkPathCheckpoints();
            }
        }
    }

    // called at SpawnMapTiles()
    private void GenerateMapTileGameObject(int xTile, int yTile)
    {
        mapTileGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mapTileGO.transform.position = new Vector3(xTile, 0.5f, yTile);
        mapTileGO.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        mapTileGO.name = "Map tile ground";
    }

    // called at SpawnMapTiles()
    private void FindAndCreateMapEntrance()
    {
        if (!isMapEntranceSpawned)
        {
            for (int i = 0; i <= tileList.Count; i++)
            {
                if (i == entranceMapTileNumber)
                {
                    entranceMapTileGO = mapTileGO;
                    entranceMapTileGO.name = "Map tile entrance";
                    GenerateMapTileMaterial(mapTileGO, Color.grey);

                    GameObject _entranceInvis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _entranceInvis.name = "Entrance temp";
                    _entranceInvis.transform.SetParent(entranceMapTileGO.transform);
                    _entranceInvis.transform.localScale = Vector3.one * 0.5f;
                    _entranceInvis.transform.position = entranceMapTileGO.transform.position + Vector3.up * 0.5f;

                    isMapEntranceSpawned = true;
                }
            }
        }
    }

    // called at SpawnMapTiles()
    private void FindAndCreateMapExit()
    {
        if (!isMapExitSpawned)
        {
            for (int i = 0; i <= tileList.Count; i++)
            {
                if (i == exitMapTileNumber)
                {
                    exitMapTileGO = mapTileGO;
                    exitMapTileGO.name = "Map tile exit";
                    GenerateMapTileMaterial(mapTileGO, Color.blue);

                    GameObject _entranceInvis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _entranceInvis.name = "Entrance temp";
                    _entranceInvis.transform.SetParent(exitMapTileGO.transform);
                    _entranceInvis.transform.localScale = Vector3.one * 0.5f;
                    _entranceInvis.transform.position = exitMapTileGO.transform.position + Vector3.up * 0.5f;

                    isMapExitSpawned = true;
                }
            }
        }
    }

    // called at SpawnMapTiles()
    private void FindAndCreateMapEdges()
    {
        if (edgeOfMapTileList.Contains(tileList.Count))
        {
            edgeMapTileGO = mapTileGO;
            edgeMapTileGO.name = "Map tile edge";
            GenerateMapTileMaterial(mapTileGO, Color.red);

            GameObject _entranceInvis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _entranceInvis.name = "Edge temp";
            _entranceInvis.transform.SetParent(edgeMapTileGO.transform);
            _entranceInvis.transform.localScale = Vector3.one * 0.5f;
            _entranceInvis.transform.position = edgeMapTileGO.transform.position + Vector3.up * 0.5f;

            //Debug.Log("@Map edges creation......");
        }
    }

    private void FindAndCreateMapWalkPathCheckpoints()
    {
        // randomly generate checkpoints on ground map ( 1 - 4 )
        if (!isMapWalkPathCheckpointsSpawned)
        {
            //Debug.Log("@ how many ground tiles on the map... " + groundMapTileList.Count);

            for (int i = 0; i <= tileList.Count; i++)
            {
                if (i == checkpointMapTileNumber)
                {
                    walkPathCheckpointMapTileGO = mapTileGO;
                    walkPathCheckpointMapTileGO.name = "Map tile checkpoint";

                    GameObject _checkpointInvis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _checkpointInvis.name = "Checkpoint temp";
                    _checkpointInvis.transform.SetParent(walkPathCheckpointMapTileGO.transform);
                    _checkpointInvis.transform.localScale = Vector3.one * 0.5f;
                    _checkpointInvis.transform.position = walkPathCheckpointMapTileGO.transform.position + Vector3.up * 0.5f;

                    GenerateMapTileMaterial(mapTileGO, Color.white);
                    isMapWalkPathCheckpointsSpawned = true;
                }
            }
            //entranceMapTileNumber = edgeOfMapTileList[createCheckpoint];
        }

        // last checkpoint is the exit ( 5 )
    }

    // Invoked at the start 
    private void FindAndCreateMapWalkPath()
    {
        BoxCollider entranceColl = entranceMapTileGO.GetComponent<BoxCollider>();
        // Get the bounds of the collider
        Bounds entranceBounds = entranceColl.bounds;
        // Check for colliders within the bounds of the collider
        Collider[] entranceColliders = Physics.OverlapBox(entranceBounds.center, entranceBounds.extents);

        foreach (Collider collider in entranceColliders)
        {
            if (!collider.name.ToLower().Contains("edge") &&
                !collider.name.ToLower().Contains("entrance"))
            {
                var dis = Vector3.Distance(entranceMapTileGO.transform.position,
                    collider.transform.position);

                if (collider.name.ToLower().Contains("ground") && dis == 1)
                {
                    GenerateMapTileMaterial(collider.gameObject, Color.cyan);
                    collider.gameObject.name = "Map tile walk path";

                    continuedPathMapTileGO.transform.position = new Vector3(collider.transform.position.x, 1f, collider.transform.position.z);

                    InvokeRepeating("ContinueMapWalkPathCreation", 0.25f, 0.25f);
                    return;
                }
            }
        }
    }

    // InvokedRepeating inside the FindAndCreateMapWalkPath() function
    private void ContinueMapWalkPathCreation()
    {
        BoxCollider pathColl = continuedPathMapTileGO.GetComponent<BoxCollider>();
        // Get the bounds of the collider
        Bounds pathBounds = pathColl.bounds;
        // Check for colliders within the bounds of the collider
        Collider[] pathColliders = Physics.OverlapBox(pathBounds.center, pathBounds.extents);

        foreach (Collider collider in pathColliders)
        {
            if (collider.name.ToLower().Contains("ground") && continuedPathMapTileGO != collider.gameObject)
            {
                GenerateMapTileMaterial(collider.gameObject, Color.yellow);
                collider.name = "Map tile walk path";
            }
            else if (collider.name.ToLower().Contains("exit") || collider.name.ToLower().Contains("checkpoint") || collider.name.ToLower().Contains("edge"))
            {
                Debug.Log("END OF THE LINE.. Colliding with: " + collider.gameObject.name);
                CancelInvoke();
                return;
            }
        }

        // move the pathfinder GO in the forwards direction * by 1
        continuedPathMapTileGO.transform.Translate(transform.forward * 1f, Space.Self);
        return;
    }

    // called at the start - before spawning tiles
    private (int, int, int) RandomEntranceAndExitAndWalkPathCheckpointsMapTileNumbers()
    {
        int pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
        entranceMapTileNumber = edgeOfMapTileList[pickEntranceRandomNumInList];

        int pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
        exitMapTileNumber = edgeOfMapTileList[pickExitRandomNumInList];

        int createRandomCheckpointNum = Random.Range(12, groundMapTileList.Count);
        checkpointMapTileNumber = groundMapTileList[createRandomCheckpointNum];

        if (new[] { 1, 10, 91, 100 }.Contains(entranceMapTileNumber)
            || entranceMapTileNumber == exitMapTileNumber)
        {
            int newEntrancePickRandomNumInArr = Random.Range(1, edgeOfMapTileList.Count);
            entranceMapTileNumber = edgeOfMapTileList[newEntrancePickRandomNumInArr];
            Debug.Log("DO not use corner tiles for entrance.. pick another number (this num is) == " + entranceMapTileNumber);
            return (entranceMapTileNumber, exitMapTileNumber, checkpointMapTileNumber);
        } 

        if (new[] { 1, 10, 91, 100 }.Contains(exitMapTileNumber)
            || exitMapTileNumber == entranceMapTileNumber)
        {
            int newExitPickRandomNumInArr = Random.Range(1, edgeOfMapTileList.Count);
            exitMapTileNumber = edgeOfMapTileList[newExitPickRandomNumInArr];
            Debug.Log("DO not use corner tiles for exit.. pick another number (this num is) == " + exitMapTileNumber);
            return (entranceMapTileNumber, exitMapTileNumber, checkpointMapTileNumber);
        }

        Debug.Log($"Entrance number: {entranceMapTileNumber} && Exit number: {exitMapTileNumber} && Checkpoint number: {checkpointMapTileNumber}");
        return (entranceMapTileNumber, exitMapTileNumber, checkpointMapTileNumber);
    }

    // called SpawnMapTiles() function - 
    private void GenerateMapTileBoxCollider()
    {
        BoxCollider coll = mapTileGO.AddComponent<BoxCollider>();
        //coll.size *= 1.5f;
    }

    // generates specific color to identify visuals
    private void GenerateMapTileMaterial(GameObject tileGO, Color matColor)
    {
        Renderer rend = tileGO.GetComponent<Renderer>();
        rend.material.color = matColor;
    }

    // called at the start - creates GO 
    private void CreatePathFinder()
    {
        // create GO - give it specifics
        continuedPathMapTileGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        continuedPathMapTileGO.name = "PathFinder";
        continuedPathMapTileGO.layer = 8;
        continuedPathMapTileGO.AddComponent<BoxCollider>();
        continuedPathMapTileGO.transform.localScale = Vector3.one * 0.5f;
        GenerateMapTileMaterial(continuedPathMapTileGO, Color.blue);
    }

    // called in the FixedUpdate() - once boolean "isMapWalkPathCheckpointsSpawned" is true this function will proceed
    private void PathFinderPhysics()
    {
        // ignore layer ( 8 == Pathfinder )
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        // Add raycasts in 4 directions ( forward, backwards, right, left )
        Vector3 fwd = continuedPathMapTileGO.transform.TransformDirection(Vector3.forward);
        Vector3 back = continuedPathMapTileGO.transform.TransformDirection(Vector3.back);
        Vector3 right = continuedPathMapTileGO.transform.TransformDirection(Vector3.right);
        Vector3 left = continuedPathMapTileGO.transform.TransformDirection(Vector3.left);

        RaycastHit hit;

        if (Physics.Raycast(continuedPathMapTileGO.transform.position, fwd, out hit, 10f, layerMask)) // forward
        {

            if (hit.collider.name.ToLower().Contains("checkpoint"))
            {
                Debug.Log("@raycast hit checkpoint");
                isPathFinderAimingAtcheckpoint = true;
                //Debug.DrawRay(continuedPathMapTileGO.transform.position, continuedPathMapTileGO.transform.TransformDirection(Vector3.forward) * 10f, Color.yellow);
            }
            else
            {
                isPathFinderAimingAtcheckpoint = false;

                // rotate the continuedPathMapTileGO 180f - 
                var disBetweenEntranceAndPathFinder = Vector3.Distance(continuedPathMapTileGO.transform.position, entranceMapTileGO.transform.position);
                if (hit.collider.name.ToLower().Contains("entrance") && disBetweenEntranceAndPathFinder < 1.2f)
                {
                    continuedPathMapTileGO.transform.Rotate(0f, 180f, 0f, Space.Self);
                    Debug.Log("@180f this bitch");
                }
            }

            if (hit.collider.name.ToLower().Contains("edge") && !isPathFinderAimingAtcheckpoint)
            {
                var disBetweenPathFinderAndEdgeTile = Vector3.Distance(continuedPathMapTileGO.transform.position, hit.transform.position);
                if (disBetweenPathFinderAndEdgeTile < 1.2f)
                {
                    continuedPathMapTileGO.transform.Rotate(0f, 90f, 0f, Space.Self);
                }
            }
        }
        if (Physics.Raycast(continuedPathMapTileGO.transform.position, back, out hit, 10f, layerMask)) // back
        {
            if (hit.collider.name.ToLower().Contains("checkpoint"))
            {
                Debug.Log("@raycast hit checkpoint");
                isPathFinderAimingAtcheckpoint = true;
                //Debug.DrawRay(continuedPathMapTileGO.transform.position, continuedPathMapTileGO.transform.TransformDirection(Vector3.back) * 10f, Color.blue);
            }
            else
            {
                isPathFinderAimingAtcheckpoint = false;
            }
        }
        if (Physics.Raycast(continuedPathMapTileGO.transform.position, right, out hit, 10f, layerMask)) // right
        {
            if (hit.collider.name.ToLower().Contains("checkpoint"))
            {
                Debug.Log("@raycast hit checkpoint");
                isPathFinderAimingAtcheckpoint = true;
                //Debug.DrawRay(continuedPathMapTileGO.transform.position, continuedPathMapTileGO.transform.TransformDirection(Vector3.right) * 10f, Color.red);
                continuedPathMapTileGO.transform.Rotate(0f, 90f, 0f, Space.Self);
            }
            else
            {
                isPathFinderAimingAtcheckpoint = false;
            }

            var disBetweenEntranceAndPathFinder = Vector3.Distance(continuedPathMapTileGO.transform.position, entranceMapTileGO.transform.position);
            if (hit.collider.name.ToLower().Contains("entrance") && disBetweenEntranceAndPathFinder < 1.2f)
            {
                continuedPathMapTileGO.transform.Rotate(0f, -90f, 0f, Space.Self);
            }
        }
        if (Physics.Raycast(continuedPathMapTileGO.transform.position, left, out hit, 10f, layerMask)) // left
        {
            if (hit.collider.name.ToLower().Contains("checkpoint"))
            {
                Debug.Log("@raycast hit checkpoint");
                isPathFinderAimingAtcheckpoint = true;
                //Debug.DrawRay(continuedPathMapTileGO.transform.position, continuedPathMapTileGO.transform.TransformDirection(Vector3.left) * 10f, Color.green);
                continuedPathMapTileGO.transform.Rotate(0f, -90f, 0f, Space.Self);
            }
            else
            {
                isPathFinderAimingAtcheckpoint = false;
            }

            var disBetweenEntranceAndPathFinder = Vector3.Distance(continuedPathMapTileGO.transform.position, entranceMapTileGO.transform.position);
            if (hit.collider.name.ToLower().Contains("entrance") && disBetweenEntranceAndPathFinder < 1.2f)
            {
                continuedPathMapTileGO.transform.Rotate(0f, 90f, 0f, Space.Self);
            }
        }
    }
}