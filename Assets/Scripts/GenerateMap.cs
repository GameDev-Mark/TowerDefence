using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private GameObject PathFinderMapTileGO;
    private GameObject walkPathCheckpointMapTileGO;
    private GameObject parentHolderForMapTiles;

    private bool isMapEntranceSpawned = false;
    private bool isMapExitSpawned = false;
    private bool isMapWalkPathCheckpointsSpawned = false;
    private bool isPathFinderAimingAtcheckpoint = false;
    private bool isPathFinderInitialSpawnCompleted = false;

    private void Start()
    {
        CreatePathFinder();
        PopulateMapLists();
        MapTileSize();
        ParentMapTileHolderCreation();
        CreateMapSequentially();
    }

    private void FixedUpdate()
    {
        if (isMapWalkPathCheckpointsSpawned) PathFinderPhysics();
    }

    private void MapTileSize()
    {
        xTileMap = 10;
        yTileMap = 10;
    }

    // initially called on start().. this sequence controls the how the map is created
    private void CreateMapSequentially()
    {
        Random_Entrance_Exit_WalkPathCheckpoints_MapTileNumbers(() =>
        {
            SpawnMapTiles(() =>
            {
                FindAndCreateMapWalkPath(() =>
                {
                    Debug.Log("@map completion done!");
                });
            });
        });

    }

    // populate map lists - called on start()
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

    // called within CreateMapSequentially() function sequence
    private void SpawnMapTiles(Action onComplete)
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

                if (j == yTileMap - 1 && i == xTileMap - 1)
                    onComplete?.Invoke();
            }
        }
    }

    // called on SpawnMapTiles()
    private void GenerateMapTileGameObject(int xTile, int yTile)
    {
        mapTileGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mapTileGO.transform.position = new Vector3(xTile, 0.5f, yTile);
        mapTileGO.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        mapTileGO.name = "Map tile ground";

        if (mapTileGO.transform.parent != parentHolderForMapTiles.transform)
        {
            mapTileGO.transform.SetParent(parentHolderForMapTiles.transform);
            //Debug.Log("parent holder add the map tiles GO..");
        }
    }

    // called on SpawnMapTiles()
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
                    _entranceInvis.transform.localScale = Vector3.one * 0.75f;
                    _entranceInvis.transform.position = entranceMapTileGO.transform.position + Vector3.up * 0.5f;

                    isMapEntranceSpawned = true;
                }
            }
        }
    }

    // called on SpawnMapTiles()
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

    // called on SpawnMapTiles()
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
        }
    }

    // invoked on start()
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
    }

    // called within CreateMapSequentially() function sequence
    private void FindAndCreateMapWalkPath(Action onComplete)
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

                    PathFinderMapTileGO.transform.position = new Vector3(collider.transform.position.x, 1f, collider.transform.position.z);
                    Debug.Log("@INIT POSITION OF PATHFINDER");

                    onComplete?.Invoke();

                    InvokeRepeating("ContinueMapWalkPathCreation", 0.25f, 0.25f);
                    return;
                }
            }
        }
    }

    // InvokedRepeating on FindAndCreateMapWalkPath()
    private void ContinueMapWalkPathCreation()
    {
        BoxCollider pathColl = PathFinderMapTileGO.GetComponent<BoxCollider>();
        // Get the bounds of the collider
        Bounds pathBounds = pathColl.bounds;
        // Check for colliders within the bounds of the collider
        Collider[] pathColliders = Physics.OverlapBox(pathBounds.center, pathBounds.extents);

        foreach (Collider collider in pathColliders)
        {
            if (collider.name.ToLower().Contains("ground") && PathFinderMapTileGO != collider.gameObject)
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

        MovePathFinderMapTileGO();
        return;
    }

    // called within ContinueMapWalkPathCreation()
    private void MovePathFinderMapTileGO()
    {
        // move the pathfinder GO in the forwards direction * by 1
        PathFinderMapTileGO.transform.Translate(transform.forward * 1f, Space.Self);
    }

    // called within CreateMapSequentially() function sequence
    private (int, int, int) Random_Entrance_Exit_WalkPathCheckpoints_MapTileNumbers(Action onComplete)
    {
        int pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
        entranceMapTileNumber = edgeOfMapTileList[pickEntranceRandomNumInList];

        int pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
        exitMapTileNumber = edgeOfMapTileList[pickExitRandomNumInList];

        int createRandomCheckpointNum = Random.Range(12, groundMapTileList.Count);
        checkpointMapTileNumber = groundMapTileList[createRandomCheckpointNum];

        var negatedMapTileNumbers = new[] { 1, 10, 91, 100 };

        if (negatedMapTileNumbers.Contains(entranceMapTileNumber) || entranceMapTileNumber == exitMapTileNumber || negatedMapTileNumbers.Contains(entranceMapTileNumber) && negatedMapTileNumbers.Contains(exitMapTileNumber))
        {
            pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);

            while (negatedMapTileNumbers.Contains(edgeOfMapTileList[pickEntranceRandomNumInList]))
            {
                pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);

                if (!negatedMapTileNumbers.Contains(edgeOfMapTileList[pickEntranceRandomNumInList]))
                {
                    break;
                }
            }
        }

        if (negatedMapTileNumbers.Contains(exitMapTileNumber) || exitMapTileNumber == entranceMapTileNumber || negatedMapTileNumbers.Contains(exitMapTileNumber) && negatedMapTileNumbers.Contains(entranceMapTileNumber))
        {
            pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);

            while (negatedMapTileNumbers.Contains(edgeOfMapTileList[pickExitRandomNumInList]))
            {
                pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);

                if (!negatedMapTileNumbers.Contains(edgeOfMapTileList[pickExitRandomNumInList]))
                {
                    break;
                }
            }
        }

        entranceMapTileNumber = edgeOfMapTileList[pickEntranceRandomNumInList];
        exitMapTileNumber = edgeOfMapTileList[pickExitRandomNumInList];

        Debug.Log($"Entrance number: {entranceMapTileNumber} && Exit number: {exitMapTileNumber} && Checkpoint number: {checkpointMapTileNumber}");
        onComplete.Invoke();
        return (entranceMapTileNumber, exitMapTileNumber, checkpointMapTileNumber);
    }

    // called on SpawnMapTiles()
    private void GenerateMapTileBoxCollider()
    {
        BoxCollider coll = mapTileGO.AddComponent<BoxCollider>();
    }

    // generates specific color to identify visuals
    private void GenerateMapTileMaterial(GameObject tileGO, Color matColor)
    {
        Renderer rend = tileGO.GetComponent<Renderer>();
        rend.material.color = matColor;
    }

    // called on start() - creates GO 
    private void CreatePathFinder()
    {
        // create GO - give it specifics
        PathFinderMapTileGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        PathFinderMapTileGO.name = "PathFinder";
        PathFinderMapTileGO.layer = 8;
        PathFinderMapTileGO.AddComponent<BoxCollider>();
        PathFinderMapTileGO.transform.localScale = Vector3.one * 0.5f;
        GenerateMapTileMaterial(PathFinderMapTileGO, Color.blue);
    }

    // called on FixedUpdate() - once boolean "isMapWalkPathCheckpointsSpawned" is true this function will proceed
    private void PathFinderPhysics()
    {
        // ignore layer ( 8 == Pathfinder )
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        // Add raycasts in 4 directions ( forward, backwards, right, left )
        Vector3 fwd = PathFinderMapTileGO.transform.TransformDirection(Vector3.forward);
        Vector3 back = PathFinderMapTileGO.transform.TransformDirection(Vector3.back);
        Vector3 right = PathFinderMapTileGO.transform.TransformDirection(Vector3.right);
        Vector3 left = PathFinderMapTileGO.transform.TransformDirection(Vector3.left);

        RaycastHit hit;

        // raycasting forwards
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, fwd, out hit, 10f, layerMask))
        {
            if (hit.collider.name.ToLower().Contains("checkpoint") && isPathFinderInitialSpawnCompleted)
            {
                isPathFinderAimingAtcheckpoint = true;
                //Debug.DrawRay(continuedPathMapTileGO.transform.position, continuedPathMapTileGO.transform.TransformDirection(Vector3.forward) * 10f, Color.yellow);
            }
            else
            {
                isPathFinderAimingAtcheckpoint = false;
            }

            // when pathfinder gameobject spawns on the TOP side of the map, rotate '180f' degrees so it is facing forward with it's back to the entrance
            if (hit.collider.name.ToLower().Contains("entrance"))
            {
                var disBetweenEntranceAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, entranceMapTileGO.transform.position);
                if (disBetweenEntranceAndPathFinder < 1.2f && !isPathFinderInitialSpawnCompleted)
                {
                    isPathFinderInitialSpawnCompleted = true;
                    PathFinderMapTileGO.transform.Rotate(0f, 180f, 0f, Space.Self);
                    Debug.Log("Initial spawn, 180f this bitch");
                }
            }

            /////// TODO : When pathfinder reaches an edge (1 tile away) figure out which way it must turn if it does not have a checkpoint in reach
            /// NEED TO FIX : when pathfinder reaches an edge (1 tile away) it rotates towards the checkpoint AND also the other direction that we tell
            /// NEED TO prioritize checkpoint over edge movement away 

            //if (hit.collider.name.ToLower().Contains("edge"))
            //{
            //    var disBetweenPathFinderAndEdgeTile = Vector3.Distance(continuedPathMapTileGO.transform.position, hit.transform.position);
            //    if (disBetweenPathFinderAndEdgeTile < 1.2f && !isPathFinderAimingAtcheckpoint)
            //    {
            //        // TODO: when spawning at the top of the map - we should rotate forward? why are we rotating 90 instead of 180?
            //        continuedPathMapTileGO.transform.Rotate(0f, 180f, 0f, Space.Self);
            //        Debug.Log("@Hitting edge tile rotate 180 degrees...");
            //    }
            //}
        }

        // raycasting backwards
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, back, out hit, 10f, layerMask))
        {
            if (hit.collider.name.ToLower().Contains("checkpoint") && isPathFinderInitialSpawnCompleted)
            {
                PathFinderMapTileGO.transform.Rotate(0f, 180f, 0f, Space.Self);
                Debug.Log("@Rotate backwards 180f");
            }

            if (hit.collider.name.ToLower().Contains("entrance"))
            {
                var disBetweenEntranceAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, entranceMapTileGO.transform.position);
                if (disBetweenEntranceAndPathFinder < 1.2f && !isPathFinderInitialSpawnCompleted)
                {
                    isPathFinderInitialSpawnCompleted = true;
                    Debug.Log("Initial entrance : spawned at back : no need to rotate");
                }
            }
        }

        // raycasting to the right
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, right, out hit, 10f, layerMask))
        {
            if (hit.collider.name.ToLower().Contains("checkpoint") && isPathFinderInitialSpawnCompleted)
            {
                //Debug.DrawRay(continuedPathMapTileGO.transform.position, continuedPathMapTileGO.transform.TransformDirection(Vector3.right) * 10f, Color.red);
                PathFinderMapTileGO.transform.Rotate(0f, 90f, 0f, Space.Self);
                Debug.Log("@Rotate right");
            }

            // when pathfinder gameobject spawns on the RIGHT side of the map, rotate '-90f' degrees so it is facing forward with it's back to the entrance
            if (hit.collider.name.ToLower().Contains("entrance"))
            {
                var disBetweenEntranceAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, entranceMapTileGO.transform.position);
                if (disBetweenEntranceAndPathFinder < 1.2f && !isPathFinderInitialSpawnCompleted)
                {
                    isPathFinderInitialSpawnCompleted = true;
                    PathFinderMapTileGO.transform.Rotate(0f, -90f, 0f, Space.Self);
                    Debug.Log("Initial entrance : Rotate right");
                }
            }
        }

        // raycasting to the left
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, left, out hit, 10f, layerMask))
        {
            if (hit.collider.name.ToLower().Contains("checkpoint") && isPathFinderInitialSpawnCompleted)
            {
                //isPathFinderAimingAtcheckpoint = true;
                //Debug.DrawRay(continuedPathMapTileGO.transform.position, continuedPathMapTileGO.transform.TransformDirection(Vector3.left) * 10f, Color.green);
                PathFinderMapTileGO.transform.Rotate(0f, -90f, 0f, Space.Self);
                Debug.Log("@Rotate left");
            }

            // when pathfinder gameobject spawns on the LEFT side of the map, rotate '90f' degrees so it is facing forward with it's back to the entrance
            if (hit.collider.name.ToLower().Contains("entrance"))
            {
                var disBetweenEntranceAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, entranceMapTileGO.transform.position);
                if (disBetweenEntranceAndPathFinder < 1.2f && !isPathFinderInitialSpawnCompleted)
                {
                    isPathFinderInitialSpawnCompleted = true;
                    PathFinderMapTileGO.transform.Rotate(0f, 90f, 0f, Space.Self);
                    Debug.Log("Initial entrance : Rotate left");
                }
            }
        }
    }

    private void ParentMapTileHolderCreation()
    {
        parentHolderForMapTiles = new GameObject();
        parentHolderForMapTiles.name = "ParentHolderForMapTiles";
    }

    private void ParentMapTileGameObjects()
    {
        GameObject groundHolder = new GameObject();
        groundHolder.name = "Ground Holder";

        GameObject edgeHolder = new GameObject();
        edgeHolder.name = "Edge Holder";

        foreach (Transform child in parentHolderForMapTiles.transform)
        {
            Debug.Log("@parent holder child yes..");
            //if (child.name.ToLower().Contains("ground"))
            //{
            //    child.SetParent(groundHolder.transform);
            //    Debug.Log("@grounded parent..");
            //}

            if (child.name.ToLower().Contains("edge"))
            {
                child.SetParent(edgeHolder.transform);
                Debug.Log("@edge parent..");
            }
        }

        groundHolder.transform.SetParent(parentHolderForMapTiles.transform);
        edgeHolder.transform.SetParent(parentHolderForMapTiles.transform);
    }
}