using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateMap : MonoBehaviour
{
    private int xTileMap;
    private int yTileMap;
    private int entranceMapTileNumber = 30; // remove these numbers to randomize the tiles
    private int exitMapTileNumber = 97;
    private int checkpointMapTileNumber = 86;
    private int waypointCounter;

    private List<int> tileList;
    private List<int> edgeOfMapTileList;
    private List<int> groundMapTileList;

    [Space(20), SerializeField] public List<GameObject> waypointsList;

    private float distanceFromPathFinderToRightSideEdge;
    private float distanceFromPathFinderToLeftSideEdge;
    private float pathFinderMoveSpeed;

    private string rayCastDirectionLeftOrRight;
    private enum pathFinderForwardDirectionToGlobal { Forward, Down, Right, Left };
    private pathFinderForwardDirectionToGlobal pathFinderForwardDir;

    private GameObject mapTileGO;
    [Space(20), SerializeField] public GameObject entranceMapTileGO;
    [SerializeField] private GameObject exitMapTileGO;
    private GameObject edgeMapTileGO;
    private GameObject PathFinderMapTileGO;
    private GameObject walkPathCheckpointMapTileGO;
    private GameObject parentHolderForMapTiles;

    private bool isMapEntranceSpawned = false;
    private bool isMapExitSpawned = false;
    private bool isMapWalkPathCheckpointsSpawned = false;
    private bool isPathFinderInitialSpawnCompleted = false;
    private bool isFirstCheckpointReached = false;
    private bool isExitOrCheckpointInSightForPathFinder = false;
    private bool isBridgeCreated = false;
    private bool isBridgeCurrentlyBeingCreated = false;

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

    // initially called on start().. this sequence controls how the map is created
    private void CreateMapSequentially()
    {
        GenerateEntranceAndExitAndCheckpoints_MapTileNumbers(() =>
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
                GenerateMapTileMaterial(mapTileGO, "LavaRock");

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
                    GenerateMapTileMaterial(entranceMapTileGO, "BlackRock");

                    GameObject _entranceGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _entranceGO.name = "Entrance temp";
                    _entranceGO.transform.SetParent(entranceMapTileGO.transform);
                    _entranceGO.transform.localScale = Vector3.one * 0.75f;
                    _entranceGO.transform.position = entranceMapTileGO.transform.position + Vector3.up * 0.5f;

                    GenerateMapTileMaterial(_entranceGO, "Entrance_Exit");

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
                    GenerateMapTileMaterial(exitMapTileGO, "BlackRock");

                    GameObject _exitGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _exitGO.name = "Exit temp";
                    _exitGO.transform.SetParent(exitMapTileGO.transform);
                    _exitGO.transform.localScale = Vector3.one * 0.75f;
                    _exitGO.transform.position = exitMapTileGO.transform.position + Vector3.up * 0.5f;

                    GenerateMapTileMaterial(_exitGO, "Entrance_Exit");

                    // remove or disable edge tile in exitmaptileGO parent
                    Destroy(exitMapTileGO.transform.GetChild(0).gameObject);

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
            //GenerateMapTileMaterial(edgeMapTileGO, "LavaRock");

            GameObject _entranceInvis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _entranceInvis.name = "Edge temp";
            _entranceInvis.transform.SetParent(edgeMapTileGO.transform);
            _entranceInvis.transform.localScale = Vector3.one * 0.5f;
            _entranceInvis.transform.position = edgeMapTileGO.transform.position + Vector3.up * 0.5f;

            TurnOffMeshRenderer(_entranceInvis);
        }
    }

    // invoked on start()
    private void FindAndCreateMapWalkPathCheckpoints()
    {
        // randomly generate checkpoints on ground map ( 1 - 4 )
        if (!isMapWalkPathCheckpointsSpawned)
        {
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

                    GenerateMapTileMaterial(mapTileGO, "GrassGround");
                    isMapWalkPathCheckpointsSpawned = true;
                }
            }
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
                    GenerateMapTileMaterial(collider.gameObject, "GrassGround");
                    collider.gameObject.name = "Map tile walk path";

                    PathFinderMapTileGO.transform.position = new Vector3(collider.transform.position.x, 1f, collider.transform.position.z);
                    //Debug.Log("@INIT POSITION OF PATHFINDER");
                    onComplete?.Invoke();
                    InvokeRepeating("ContinueMapWalkPathCreation", 0.5f, pathFinderMoveSpeed); // wait (0.5f) time - for the initial spawn of pathfinder gameobject to rotate correctly
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
                GenerateMapTileMaterial(collider.gameObject, "GrassGround");
                collider.name = "Map tile walk path";
            }
            else if (collider.name.ToLower().Contains("checkpoint"))
            {
                if (!isFirstCheckpointReached)
                {
                    GameObject tempCheckpoint = GameObject.Find("Checkpoint temp");
                    Destroy(tempCheckpoint);
                    //Debug.Log($"Reached first checkpoint.. Colliding with: {collider.gameObject.name}");
                    isFirstCheckpointReached = true;
                    CancelInvoke();
                    Invoke("CheckpointReachedCustomWaitTime", 0.2f);
                    return;
                }
            }
            else if (collider.name.ToLower().Contains("exit") || collider.name.ToLower().Contains("edge"))
            {
                Debug.Log("END OF THE LINE.. Colliding with: " + collider.gameObject.name);
                CancelInvoke();
                CreateWaypoint();
                return;
            }
        }

        MovePathFinderMapTileGO();

        // make initial collider contact with overlap path
        Collider[] pathCollidersOther = Physics.OverlapBox(PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.forward / 2, pathBounds.extents);
        foreach (Collider collider in pathCollidersOther)
        {
            if (collider.name.ToLower().Contains("walk") && PathFinderMapTileGO != collider.gameObject)
            {
                if (!isBridgeCreated)
                {
                    CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder(1f, 0f, 1f);
                    StartCoroutine(CreateBridge());
                    CancelInvoke();
                    isBridgeCurrentlyBeingCreated = true;
                    //Debug.Log($"pathfinder colliding with (WALK path).. NAME: {collider.name}..");
                    return;
                }
            }
        }

        return;
    }

    private IEnumerator CreateBridge()
    {
        GameObject bPositionForPathFinder = new GameObject(); // temp gameobject .. destroy pathfinder reaches it.
        bPositionForPathFinder.name = "Position B";
        bPositionForPathFinder.transform.position = PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.forward * 2f;

        float slerpMove = 0; // initial "time stamp"
        float slerpTime = 0.1f; // increase this variable to speed up the bridge arc movement

        //find center between 'a' and 'b' .. this allows the gameobject to arc between positions
        Vector3 center = (PathFinderMapTileGO.transform.position + bPositionForPathFinder.transform.position) * 0.5f;
        center -= new Vector3(0, 1, 0);
        Vector3 pathFinderReletiveCenter = PathFinderMapTileGO.transform.position - center;
        Vector3 bPositionReletiveCenter = bPositionForPathFinder.transform.position - center;

        // create X amount of waypoints to spawned when creating the bridge (arc)
        float[] wayPointTimes = new float[10];
        for (int i = 0; i < wayPointTimes.Length; i++)
        {
            wayPointTimes[i] = slerpTime * i * 8;
        }
        int wayPointIndex = 0;

        while (slerpMove < slerpTime)
        {
            if (wayPointIndex < wayPointTimes.Length && slerpMove >= wayPointTimes[wayPointIndex] / 100f)
            {
                CreateWaypoint(0f, 0.5f, 0f);
                //Debug.Log($"Create way point.. wayPointIndex: {wayPointIndex} + slerpMove: {slerpMove}");
                if (wayPointIndex == 0 || wayPointIndex == (wayPointTimes.Length / 2) + 1)
                {
                    GenerateBridgeTiles();
                }
                wayPointIndex++;
            }

            slerpMove += Time.deltaTime;
            float perc = Mathf.Clamp01(slerpMove / slerpTime);
            PathFinderMapTileGO.transform.position = Vector3.Slerp(pathFinderReletiveCenter, bPositionReletiveCenter, perc);
            PathFinderMapTileGO.transform.position += center;

            yield return new WaitForSeconds(slerpMove);
        }

        // after the pathfinder reaches its next position
        yield return null;
        Destroy(bPositionForPathFinder);
        CreateWaypoint(0f, 0.5f, 0f);
        GenerateBridgeTiles();
        isBridgeCreated = true;
        Invoke("CheckpointReachedCustomWaitTime", 0.1f);
        CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder(-1f, 0f, -1f);
        isBridgeCurrentlyBeingCreated = false;
    }

    private int bridgeCount = 0;
    private void GenerateBridgeTiles()
    {
        GameObject bridgeTileGO = GameObject.CreatePrimitive(PrimitiveType.Quad);

        string pathFinderYAxisDecimalPlace = PathFinderMapTileGO.transform.position.y.ToString("F2"); // decimal place 1.4
        float.TryParse(pathFinderYAxisDecimalPlace, out float yResult);

        bridgeTileGO.transform.position = new Vector3
            (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult, Mathf.Round(PathFinderMapTileGO.transform.position.z));

        bridgeTileGO.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // middle bridge tile base

        if (bridgeCount != 1)
            bridgeTileGO.transform.localScale = new Vector3(1f, 1.3f, 1f);

        if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Forward)
        {
            if (bridgeCount == 0)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
            }
            if (bridgeCount == 2)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 180f, 0f);
            }
        }
        else if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Down)
        {
            if (bridgeCount == 0)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 180f, 0f);
            }
            if (bridgeCount == 2)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
            }
        }
        else if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Right)
        {
            if (bridgeCount == 0)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 90f, 0f);
            }
            if (bridgeCount == 2)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, -90f, 0f);
            }
        }
        else if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Left)
        {
            if (bridgeCount == 0)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, -90f, 0f);
            }
            if (bridgeCount == 2)
            {
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 90f, 0f);
            }
        }
        bridgeCount++;
        bridgeTileGO.name = $"Bridge tile_{bridgeCount}";
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Application.isPlaying)
            Gizmos.DrawWireCube(PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.forward / 2, PathFinderMapTileGO.GetComponent<BoxCollider>().bounds.extents);
    }

    private void CheckpointReachedCustomWaitTime()
    {
        //Debug.Log("@Continue path creation after checkpoint..");
        InvokeRepeating("ContinueMapWalkPathCreation", 0.2f, pathFinderMoveSpeed);
    }

    // called within ContinueMapWalkPathCreation()
    private void MovePathFinderMapTileGO()
    {
        // move the pathfinder GO in the forwards direction * by 1
        PathFinderMapTileGO.transform.Translate(transform.forward * 1f, Space.Self);
    }

    // called within CreateMapSequentially() function sequence
    private (int, int, int) GenerateEntranceAndExitAndCheckpoints_MapTileNumbers(Action onComplete)
    {
        if (entranceMapTileNumber == 0)
        {
            // Setup initial random tile number
            int pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
            entranceMapTileNumber = edgeOfMapTileList[pickEntranceRandomNumInList];

            // ENTRANCE -->
            var negatedEntranceMapTileNumbers = new[] { 1, 2, 9, 10, 11, 20, 81, 90, 91, 92, 99, 100, exitMapTileNumber + 90, exitMapTileNumber + 9, exitMapTileNumber - 90,
            exitMapTileNumber - 9, exitMapTileNumber + 10, exitMapTileNumber - 10, exitMapTileNumber - 1, exitMapTileNumber + 1, entranceMapTileNumber = exitMapTileNumber };

            if (negatedEntranceMapTileNumbers.Contains(entranceMapTileNumber)
                || negatedEntranceMapTileNumbers.Contains(entranceMapTileNumber) && negatedEntranceMapTileNumbers.Contains(exitMapTileNumber))
            {
                pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
                //Debug.Log("Need to randomize entrance tile number again.. " + entranceMapTileNumber);
                while (negatedEntranceMapTileNumbers.Contains(edgeOfMapTileList[pickEntranceRandomNumInList])/* || pickEntranceRandomNumInList == pickExitRandomNumInList*/)
                {
                    pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
                    if (!negatedEntranceMapTileNumbers.Contains(edgeOfMapTileList[pickEntranceRandomNumInList]))
                    {
                        break;
                    }
                }
            }
            entranceMapTileNumber = edgeOfMapTileList[pickEntranceRandomNumInList];
        }

        if (exitMapTileNumber == 0)
        {
            // Setup initial random tile number
            int pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
            exitMapTileNumber = edgeOfMapTileList[pickExitRandomNumInList];

            // EXIT -->
            var negatedExitMapTileNumbers = new[] { 1, 2, 9, 10, 11, 20, 81, 90, 91, 92, 99, 100, entranceMapTileNumber + 90, entranceMapTileNumber + 9, entranceMapTileNumber - 90,
            entranceMapTileNumber - 9, entranceMapTileNumber + 10, entranceMapTileNumber - 10, entranceMapTileNumber - 1, entranceMapTileNumber + 1, exitMapTileNumber = entranceMapTileNumber };

            if (negatedExitMapTileNumbers.Contains(exitMapTileNumber)
                || negatedExitMapTileNumbers.Contains(exitMapTileNumber) && negatedExitMapTileNumbers.Contains(entranceMapTileNumber))
            {
                pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
                //Debug.Log("Need to randomize exit tile number again.. " + exitMapTileNumber);
                while (negatedExitMapTileNumbers.Contains(edgeOfMapTileList[pickExitRandomNumInList])/* || pickExitRandomNumInList == pickEntranceRandomNumInList*/)
                {
                    pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
                    if (!negatedExitMapTileNumbers.Contains(edgeOfMapTileList[pickExitRandomNumInList]))
                    {
                        break;
                    }
                }
            }
            exitMapTileNumber = edgeOfMapTileList[pickExitRandomNumInList];
        }

        if (checkpointMapTileNumber == 0)
        {
            // CHECKPOINT -->
            /// TEMP FIX
            /// Create a way to put the checkpoint in the middle of the map (specified tiles) 
            /// Issue with applying the random number to the correct tile..
            var specificCheckPointNums = new[] { 45, 46, 56, 56 };
            System.Random rd = new System.Random();
            int random = rd.Next(specificCheckPointNums.Length);
            int createRandomCheckpointNum = specificCheckPointNums[random];
            checkpointMapTileNumber = groundMapTileList[createRandomCheckpointNum] - 23;
        }

        // return correct tile numbers && complete Invoke()
        Debug.Log($"Entrance number: {entranceMapTileNumber} && Exit number: {exitMapTileNumber} && Checkpoint number: {checkpointMapTileNumber}");
        onComplete.Invoke();
        return (entranceMapTileNumber, exitMapTileNumber, checkpointMapTileNumber);
    }

    // called on SpawnMapTiles()
    private void GenerateMapTileBoxCollider()
    {
        mapTileGO.AddComponent<BoxCollider>();
    }

    // generates specific color to identify visuals
    private void GenerateMapTileMaterial(GameObject tileGO, string resourceName)
    {
        Renderer rend = tileGO.GetComponent<Renderer>();
        rend.material = Resources.Load($"Materials/{resourceName}") as Material;
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
        GenerateMapTileMaterial(PathFinderMapTileGO, "PathFinder");
        pathFinderMoveSpeed = 0.25f;
    }

    // this function is called whenever the pathfinder rotates and turns a different direction
    // this will allow waypoints to be created so as the characters will be able to follow these waypoints accordingaly along the path
    private void CreateWaypoint(float optionalOffsetX = default, float optionalOffsetY = default, float optionalOffsetZ = default)
    {
        waypointCounter++;
        GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        waypoint.name = "Waypoint_" + waypointCounter;
        waypoint.transform.localScale = Vector3.one * 0.25f;
        waypoint.transform.position = new Vector3(PathFinderMapTileGO.transform.position.x + optionalOffsetX,
            PathFinderMapTileGO.transform.position.y + optionalOffsetY, PathFinderMapTileGO.transform.position.z + optionalOffsetZ);
        GenerateMapTileMaterial(waypoint, "PathFinder");
        waypointsList.Add(waypoint);
    }

    private void CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder
        (float optionalOffsetX = default, float optionalOffsetY = default, float optionalOffsetZ = default)
    {
        if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Forward)
            CreateWaypoint(0f, optionalOffsetY, -optionalOffsetZ);
        else if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Down)
            CreateWaypoint(0f, optionalOffsetY, optionalOffsetZ);
        else if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Right)
            CreateWaypoint(-optionalOffsetX, optionalOffsetY, 0f);
        else if (pathFinderForwardDir == pathFinderForwardDirectionToGlobal.Left)
            CreateWaypoint(optionalOffsetX, optionalOffsetY, 0f);
    }

    #region Path finder physics updater
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

        // raycasting backwards --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, back, out hit, 10f, layerMask))
        {
            InitialPathFinderGameObjectRotation(new Vector3(0f, 0f, 0f), hit, "0f backwards");
        }

        // raycasting to the right --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, right, out hit, 10f, layerMask))
        {
            PathFinderRotateTowardsExitAndCheckpoint(new Vector3(0f, 90f, 0f), hit, "Rotate right");

            CalculateDistanceFromPathFinderToRightSideEdge(hit);

            // when pathfinder gameobject spawns on the RIGHT side of the map, rotate '-90f' degrees so it is facing forward with it's back to the entrance
            InitialPathFinderGameObjectRotation(new Vector3(0f, -90f, 0f), hit, "-90f right");
        }

        // raycasting to the left --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, left, out hit, 10f, layerMask))
        {
            PathFinderRotateTowardsExitAndCheckpoint(new Vector3(0f, -90f, 0f), hit, "Rotate left");

            CalculateDistanceFromPathFinderToLeftSideEdge(hit);

            // when pathfinder gameobject spawns on the LEFT side of the map, rotate '90f' degrees so it is facing forward with it's back to the entrance
            InitialPathFinderGameObjectRotation(new Vector3(0f, 90f, 0f), hit, "90f left");
        }

        // raycasting forwards --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, fwd, out hit, 10f, layerMask))
        {
            if (!isExitOrCheckpointInSightForPathFinder)
            {
                // when pathfinder gameobject spawns on the TOP side of the map, rotate '180f' degrees so it is facing forward with it's back to the entrance
                InitialPathFinderGameObjectRotation(new Vector3(0f, 180f, 0f), hit, "180f forward");
                PathFinderCheckForCollisionOfEdge(hit);
            }
            else
            {
                InitialPathFinderGameObjectRotation(new Vector3(0f, 0f, 0f), hit, "0f forward");
            }
        }

        FindTheLongestDistanceBetweenLeftAndRightDirection();
        ReturnForwardDirectionPathFinderIsFacing();
    }

    // called in PathFinderPhysics() to rotate the GO initially on spawn
    private void InitialPathFinderGameObjectRotation(Vector3 rotation, RaycastHit _hit, string debug)
    {
        if (_hit.collider.name.ToLower().Contains("entrance") && !isPathFinderInitialSpawnCompleted)
        {
            var disBetweenEntranceAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, entranceMapTileGO.transform.position);
            if (disBetweenEntranceAndPathFinder < 1.2f)
            {
                isPathFinderInitialSpawnCompleted = true;
                PathFinderMapTileGO.transform.rotation = Quaternion.Euler(rotation);
                Debug.Log($"Pathfinder set dir:{debug}.. Raw rot:{rotation}..");
            }
        }
    }

    // called within PathFinderPhysics() - rotate the pathfinder in the correct direction
    private void PathFinderRotateTowardsExitAndCheckpoint(Vector3 rotation, RaycastHit _hit, string log)
    {
        if (_hit.collider.name.ToLower().Contains("exit") || _hit.collider.name.ToLower().Contains("checkpoint"))
        {
            isExitOrCheckpointInSightForPathFinder = true;
            //Debug.Log($"isExitOrCheckpointInSightForPathFinder: {isExitOrCheckpointInSightForPathFinder}.. @isFirstCheckpointReached: {isFirstCheckpointReached}..");
        }
        else
        {
            isExitOrCheckpointInSightForPathFinder = false;
        }

        if (_hit.collider.name.ToLower().Contains("exit") && isPathFinderInitialSpawnCompleted && isFirstCheckpointReached && isExitOrCheckpointInSightForPathFinder) // --> find exit
        {
            PathFinderMapTileGO.transform.Rotate(rotation, Space.Self);
            if (!isBridgeCurrentlyBeingCreated) CreateWaypoint();
            Debug.Log($"Exit is in sight: {log}");
        }
        if (_hit.collider.name.ToLower().Contains("checkpoint") && isPathFinderInitialSpawnCompleted && !isFirstCheckpointReached) // --> find first checkpoint
        {
            PathFinderMapTileGO.transform.Rotate(rotation, Space.Self);
            CreateWaypoint();
            Debug.Log($"Checkpoint is in sight: {log}");
        }
    }

    private void PathFinderCheckForCollisionOfEdge(RaycastHit _hit)
    {
        // if the first checkpoint has been reached EXTRA condition check -->
        if (_hit.collider.name.ToLower().Contains("edge") && isPathFinderInitialSpawnCompleted && isFirstCheckpointReached)
        {
            var disBetweenEdgeAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, _hit.transform.position);
            if (disBetweenEdgeAndPathFinder < 1.2f)
            {
                //Debug.Log($"first checkpoint reached... 1.2f distance away from an edge tile... we need to rotate left or right..");
                if (ReturnRayCastDirectionLeftOrRight() == "right")
                {
                    PathFinderMapTileGO.transform.Rotate(new Vector3(0f, 90f, 0f), Space.Self);
                    if (!isBridgeCurrentlyBeingCreated) CreateWaypoint();
                    Debug.Log("Rotate right .. collision to edge...");
                }
                else if (ReturnRayCastDirectionLeftOrRight() == "left")
                {
                    PathFinderMapTileGO.transform.Rotate(new Vector3(0f, -90f, 0f), Space.Self);
                    if (!isBridgeCurrentlyBeingCreated) CreateWaypoint();
                    Debug.Log("Rotate left .. collision to edge...");
                }
            }
        }
    }

    private (float, string) CalculateDistanceFromPathFinderToRightSideEdge(RaycastHit _hit)
    {
        if (_hit.collider.name.ToLower().Contains("edge"))
        {
            distanceFromPathFinderToRightSideEdge = Vector3.Distance(PathFinderMapTileGO.transform.position, _hit.collider.transform.position);
        }
        //Debug.Log("right side distance: " +  distanceFromPathFinderToRightSideEdge);
        return (distanceFromPathFinderToRightSideEdge, rayCastDirectionLeftOrRight);
    }

    private (float, string) CalculateDistanceFromPathFinderToLeftSideEdge(RaycastHit _hit)
    {
        if (_hit.collider.name.ToLower().Contains("edge"))
        {
            distanceFromPathFinderToLeftSideEdge = Vector3.Distance(PathFinderMapTileGO.transform.position, _hit.collider.transform.position);
        }
        //Debug.Log("left side distance: " +  distanceFromPathFinderToLeftSideEdge);
        return (distanceFromPathFinderToLeftSideEdge, rayCastDirectionLeftOrRight);
    }

    private float FindTheLongestDistanceBetweenLeftAndRightDirection()
    {
        float longestDistance = Mathf.Max(distanceFromPathFinderToRightSideEdge, distanceFromPathFinderToLeftSideEdge);

        if (distanceFromPathFinderToRightSideEdge > distanceFromPathFinderToLeftSideEdge)
        {
            rayCastDirectionLeftOrRight = "right";
        }
        else if (distanceFromPathFinderToLeftSideEdge > distanceFromPathFinderToRightSideEdge)
        {
            rayCastDirectionLeftOrRight = "left";
        }

        ReturnRayCastDirectionLeftOrRight();

        return longestDistance;
    }

    private string ReturnRayCastDirectionLeftOrRight()
    {
        return rayCastDirectionLeftOrRight;
    }

    private void ReturnForwardDirectionPathFinderIsFacing()
    {
        Quaternion forwardDir = PathFinderMapTileGO.transform.rotation;
        var rotation = Math.Round(forwardDir.eulerAngles.y);
        if (rotation == 0f) // forward
        {
            pathFinderForwardDir = pathFinderForwardDirectionToGlobal.Forward;
        }
        else if (rotation == 180f || rotation == -180f)
        {
            pathFinderForwardDir = pathFinderForwardDirectionToGlobal.Down;
        }
        else if (rotation == 90f)
        {
            pathFinderForwardDir = pathFinderForwardDirectionToGlobal.Right;
        }
        else if (rotation == -90f || rotation == 270f)
        {
            pathFinderForwardDir = pathFinderForwardDirectionToGlobal.Left;
        }
        //Debug.Log($"pathFinderForwardDirectionToGlobal: {pathFinderForwardDir}.. rotation: {rotation}");
    }
    #endregion

    #region ParentHolder for maptiles
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
    #endregion

    private void TurnOffMeshRenderer(GameObject _mr)
    {
        try
        {
            _mr.GetComponent<MeshRenderer>().enabled = false;
        }
        catch
        {
            Debug.Log($"There is no meshrenderer to disable on {_mr.name} gameobject");
        }
    }
}