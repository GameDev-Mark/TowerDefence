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
    /// Testing purposes : first bridge tile -->
    // entranceMapTileNumber = 30, exitMapTileNumber = 97, checkpointMapTileNumber = 86 --> rotating from the forward dir .. rotate right
    // entranceMapTileNumber = 80, exitMapTileNumber = 7, checkpointMapTileNumber = 16 --> rotating from the forward dir .. rotate left
    // entranceMapTileNumber = 98, exitMapTileNumber = 61, checkpointMapTileNumber = 29 --> rotating from the right dir .. rotate down
    // entranceMapTileNumber = 8, exitMapTileNumber = 21, checkpointMapTileNumber = 47 --> rotating from the left dir .. rotate down
    // entranceMapTileNumber = 93, exitMapTileNumber = 70, checkpointMapTileNumber = 22 --> rotating from the right dir .. rotate forward
    // entranceMapTileNumber = 3, exitMapTileNumber = 30, checkpointMapTileNumber = 72 --> rotating from the left dir .. rotate forward
    /// Testing purposes : Last bridge tile -->
    // entranceMapTileNumber = 97, exitMapTileNumber = 8, checkpointMapTileNumber = 44 (With exit on last tile when turning)
    // entranceMapTileNumber = 71, exitMapTileNumber = 7, checkpointMapTileNumber = 56 (With edge on last tile when turning)
    /// Testing purposes : checkpoint rotating to exit -->
    // entranceMapTileNumber = 70, exitMapTileNumber = 50, checkpointMapTileNumber = 43
    /// Testing purposes : exit turning quick
    // Entrance number: 30 && Exit number: 51 && Checkpoint number: 43 Up to Down entrance and exit
    // Entrance number: 94 && Exit number: 80 && Checkpoint number: 85 right to up entrance and exit
    // Entrance number: 94 && Exit number: 51 && Checkpoint number: 43 right to Down entrance and exit
    /// Testing purposes : exit turning quick AND false bridge creation
    // Entrance number: 3 && Exit number: 41 && Checkpoint number: 55 
    private int entranceMapTileNumber = 0; // remove these numbers to randomize the tiles
    private int exitMapTileNumber = 0;
    private int checkpointMapTileNumber = 0;
    private int waypointCounter;

    private List<int> tileList;
    private List<int> edgeOfMapTileList;
    private List<int> checkpointGroundMapTileList;

    [Space(20), SerializeField] public List<GameObject> waypointsList;

    private float distanceFromPathFinderToRightSideEdge;
    private float distanceFromPathFinderToLeftSideEdge;
    private float pathFinderMoveSpeed;

    private string rayCastDirectionLeftOrRight;
    private enum PathFinderForwardDirectionToGlobal { Forward, Down, Right, Left };
    private PathFinderForwardDirectionToGlobal currentPathFinderForwardDir;
    private PathFinderForwardDirectionToGlobal previousPathFinderForwardDir;
    private PathFinderForwardDirectionToGlobal pathFinderNewDirectionChange;
    private List<string> pathFinderDirectionHistory = new List<string>();

    private GameObject mapTileGO;
    [Space(20), SerializeField] public GameObject entranceMapTileGO;
    [SerializeField] private GameObject exitMapTileGO;
    private GameObject edgeMapTileGO;
    private GameObject PathFinderMapTileGO;
    private GameObject walkPathCheckpointMapTileGO;
    private GameObject parentHolderForMapTiles;
    private GameObject mapTileInfrontOfEntrance;

    private bool isMapEntranceSpawned = false;
    private bool isMapExitSpawned = false;
    private bool isMapWalkPathCheckpointsSpawned = false;
    private bool isPathFinderInitialSpawnCompleted = false;
    private bool isFirstCheckpointReached = false;
    private bool isExitOrCheckpointInSightForPathFinder = false;
    private bool isBridgeCreated = false;
    private bool isBridgeCurrentlyBeingCreated = false;
    private bool isPathFinderRotatingToExit = false;
    private bool hasPathFindReachedExit = false;
    private bool isMapCompleted = false;

    // almost all variables for renaming tiles, etc
    private string mapTileWalkPathName = "Map tile walk path", mapTileGroundName = "Map tile ground", mapTileEntranceName = "Entrance temp",
        mapTileExitName = "Exit temp", mapTileEdgeName = "Edge temp", mapTileCheckpointName = "Checkpoint",
        towerTileName = "Tower tile", bridgeTileName = "Bridge tile", waypointName = "Waypoint", waypointExitTurnName = "Waypoint_ExitTurn",
        lastMapTileWalkPathName = "Last map tile walk path", mapTileOppcupied = "Occupied tile";

    [SerializeField] LayerMask LayerMask;

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
        if (isMapCompleted && !hasPathFindReachedExit) PathFinderPhysics();
    }
    private void Update()
    {
        if (!hasPathFindReachedExit)
        {
            ReturnCurrentPathFinderForwardDirection();
            ReturnPreviousPathFinderForwardDirection();
            CoolDownTimerForDirectionChange();
        }
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
                    isMapCompleted = true;
                    SetLastWalkTileToLastBeforeExit();
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

        checkpointGroundMapTileList = new List<int>()
        { 45, 46, 55, 56 };
        //{ 12, 13, 14, 15, 16, 17, 18, 19,
        //22, 23, 24, 25, 26, 27, 28, 28, 29,
        //32, 33, 34, 35, 36, 37, 38, 39,
        //42, 43, 44, 45, 46, 47, 48, 49,
        //52, 53, 54, 55, 56, 57, 58, 59,
        //62, 63, 64, 65, 66, 67, 68, 69,
        //72, 73, 74, 75, 76, 77, 78, 79,
        //82, 83, 84, 85, 86, 87, 88, 89
        //};
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
        SetGameObjectName(mapTileGO, mapTileGroundName);
        mapTileGO.transform.SetParent(parentHolderForMapTiles.transform.GetChild(0)); // ground holder
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
                    SetGameObjectName(entranceMapTileGO, mapTileEntranceName);
                    entranceMapTileGO.transform.SetParent(parentHolderForMapTiles.transform);
                    GenerateMapTileMaterial(entranceMapTileGO, "BlackRock");

                    GameObject _entranceGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _entranceGO.name = "Entrance temp";
                    _entranceGO.transform.SetParent(entranceMapTileGO.transform);
                    _entranceGO.transform.localScale = Vector3.one * 0.75f;
                    _entranceGO.transform.position = entranceMapTileGO.transform.position + Vector3.up * 0.5f;

                    TurnOffMeshRenderer(_entranceGO);

                    //GenerateMapTileMaterial(_entranceGO, "Entrance_Exit");

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
                    SetGameObjectName(exitMapTileGO, mapTileExitName);
                    exitMapTileGO.transform.SetParent(parentHolderForMapTiles.transform);
                    GenerateMapTileMaterial(exitMapTileGO, "BlackRock");

                    GameObject _exitGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _exitGO.name = "Exit temp";
                    _exitGO.transform.SetParent(exitMapTileGO.transform);
                    _exitGO.transform.localScale = Vector3.one * 0.75f;
                    _exitGO.transform.position = exitMapTileGO.transform.position + Vector3.up * 0.5f;

                    TurnOffMeshRenderer(_exitGO);

                    //GenerateMapTileMaterial(_exitGO, "Entrance_Exit");

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
            SetGameObjectName(edgeMapTileGO, mapTileEdgeName);
            edgeMapTileGO.transform.SetParent(parentHolderForMapTiles.transform.GetChild(1)); // edge holder
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
        if (!isMapWalkPathCheckpointsSpawned)
        {
            for (int i = 0; i <= tileList.Count; i++)
            {
                if (i == checkpointMapTileNumber)
                {
                    walkPathCheckpointMapTileGO = mapTileGO;
                    SetGameObjectName(walkPathCheckpointMapTileGO, mapTileCheckpointName);
                    walkPathCheckpointMapTileGO.transform.SetParent(parentHolderForMapTiles.transform);

                    GameObject _checkpointInvis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _checkpointInvis.name = "Checkpoint temp";
                    _checkpointInvis.transform.SetParent(walkPathCheckpointMapTileGO.transform);
                    _checkpointInvis.transform.localScale = Vector3.one * 0.5f;
                    _checkpointInvis.transform.position = walkPathCheckpointMapTileGO.transform.position + Vector3.up * 0.5f;

                    TurnOffMeshRenderer(_checkpointInvis);
                    GenerateMapTileMaterial(mapTileGO, "GroundTile_OffGreen_CobbleStone");
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
            if (collider.name != mapTileEdgeName && collider.name != mapTileEntranceName)
            {
                var dis = Vector3.Distance(entranceMapTileGO.transform.position,
                    collider.transform.position);

                if (collider.name == mapTileGroundName && dis == 1)
                {
                    mapTileInfrontOfEntrance = collider.gameObject;
                    GenerateMapTileMaterial(collider.gameObject, "GroundTile_OffGreen_CobbleStone");
                    SetGameObjectName(collider.gameObject, mapTileWalkPathName);
                    collider.transform.SetParent(parentHolderForMapTiles.transform.GetChild(2)); // walk path holder

                    InitialPositionOfPathFinder(collider); // initial pathfinder.transform.position

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
            if (collider.name == mapTileGroundName && PathFinderMapTileGO != collider.gameObject && !isBridgeCurrentlyBeingCreated ||
                collider.name == mapTileOppcupied && PathFinderMapTileGO != collider.gameObject && !isBridgeCurrentlyBeingCreated)
            {
                GenerateMapTileMaterial(collider.gameObject, "GroundTile_OffGreen_CobbleStone");
                SetGameObjectName(collider.gameObject, mapTileWalkPathName);
                collider.transform.SetParent(parentHolderForMapTiles.transform.GetChild(2)); // walk path holder
                CreateMapTileWalls(collider.gameObject);
            }
            else if (collider.name == lastMapTileWalkPathName && PathFinderMapTileGO != collider.gameObject && !isBridgeCurrentlyBeingCreated)
            {
                GenerateMapTileMaterial(collider.gameObject, "GroundTile_OffGreen_CobbleStone");
                collider.transform.SetParent(parentHolderForMapTiles.transform.GetChild(2)); // walk path holder
                CreateMapTileWalls(collider.gameObject);
            }
            else if (collider.name.ToLower().Contains("checkpoint"))
            {
                if (!isFirstCheckpointReached)
                {
                    Debug.Log($"Reached first checkpoint.. Colliding with: {collider.gameObject.name}");
                    isFirstCheckpointReached = true;
                    GameObject tempCheckpoint = GameObject.Find("Checkpoint temp");
                    Destroy(tempCheckpoint);
                    CancelInvoke();
                    Invoke("CheckpointReachedCustomWaitTime", 0.01f);
                    return;
                }

                if (collider.name.ToLower().Contains("temp"))
                    CreateMapTileWalls(collider.gameObject.transform.parent.gameObject);
                else
                    CreateMapTileWalls(collider.gameObject);
            }
            else if (collider.name.ToLower().Contains("exit temp"))
            {
                Debug.Log("END OF THE LINE.. Colliding with: " + collider.gameObject.name);
                CancelInvoke();
                CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder(1f, 0f, 1f); // spawn waypoint 1 tile before exit
                CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder(); // spawn waypoint at exit tile
                return;
            }
            if (collider.name.ToLower().Contains("tower") && PathFinderMapTileGO != collider.gameObject && !isBridgeCurrentlyBeingCreated)
            {
                Destroy(collider.gameObject);
            }
        }

        MovePathFinderMapTileGO();

        // check when overlappying with path on the right or left hand side of the pathfinderGO and build the bridge accordingly and also generate tower tiles
        Collider[] OverlapWithPathCollidersToRight = Physics.OverlapBox(PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.right / 2, pathBounds.extents);
        Collider[] OverlapWithPathCollidersToLeft = Physics.OverlapBox(PathFinderMapTileGO.transform.position + -PathFinderMapTileGO.transform.right / 2, pathBounds.extents);
        foreach (Collider collider in OverlapWithPathCollidersToRight.Union(OverlapWithPathCollidersToLeft))
        {
            if (collider.name == mapTileGroundName && PathFinderMapTileGO != collider.gameObject && !isBridgeCurrentlyBeingCreated) // tower tiles creation
            {
                GenerateAndCreateTowerTiles(collider.gameObject);
            }
            if (collider.name == mapTileWalkPathName && PathFinderMapTileGO != collider.gameObject && isPathFinderRotatingToExit) // bridge creation
            {
                float disBetweenColliderAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, collider.transform.position);
                if (!isBridgeCreated && disBetweenColliderAndPathFinder == 0.5f)
                {
                    isBridgeCurrentlyBeingCreated = true;
                    OffsetTheExitTurnWaypoint(); // offset waypoint position
                    StartCoroutine(CreateBridge(-PathFinderMapTileGO.transform.forward, 1.5f, 0f, 1f));
                    isPathFinderRotatingToExit = false;
                    CancelInvoke();
                    Debug.Log($"Creating bridge... overlap sides");
                    Destroy(collider.gameObject);
                    return;
                }
            }
        }

        // make initial collider contact with overlap path.. FORWARD overlap collision
        Collider[] OverlapWithPathCollidersInFront = Physics.OverlapBox(PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.forward / 2, pathBounds.extents);
        foreach (Collider collider in OverlapWithPathCollidersInFront)
        {
            if (collider.name == mapTileWalkPathName && PathFinderMapTileGO != collider.gameObject)
            {
                if (!isBridgeCreated)
                {
                    isBridgeCurrentlyBeingCreated = true;
                    CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder(1f, 0f, 1f);
                    StartCoroutine(CreateBridge(Vector3.zero, 1.5f, 1.5f, 1f));
                    CancelInvoke();
                    Debug.Log($"Creating bridge... overlap front");
                    return;
                }
            }
            if (collider.name == mapTileExitName && PathFinderMapTileGO != collider.gameObject)
            {
                if (!hasPathFindReachedExit)
                {
                    InstantiatePrefab_MapEntranceAndExit("Exit Arch Way", exitMapTileGO);
                    hasPathFindReachedExit = true;
                    return;
                }
            }
        }
        return;
    }

    // called when waypoints are being spawned in the IENumerator CreateBridge() - if pathfinder collides with a "tower" tile then destory it
    private void RemoveTowerTileOnBridgeTile()
    {
        Collider pathFinderColl = PathFinderMapTileGO.transform.GetComponent<Collider>();
        Collider[] pathFinderColls = Physics.OverlapBox(PathFinderMapTileGO.transform.position + -PathFinderMapTileGO.transform.up, pathFinderColl.bounds.extents);
        foreach (Collider coll in pathFinderColls)
        {
            if (coll.name.ToLower().Contains("tower") && PathFinderMapTileGO != coll.gameObject)
            {
                Destroy(coll.gameObject);
            }
        }
    }

    // called within ContinueMapWalkPathCreation() when left and right colliders collide with a ground tile triggers this function
    // set amount of random chance of tower spawning.. then generates and creates tower tiles
    private int maxAmountOfTowerTiles = 20;
    private int currentAmountOfTowerTiles = 0;
    private int fixedNumberForChanceOfTowerTileSpawning = 5; // between minRandomNumberForTowerTileSpawning && maxRandomNumberForTowerTileSpawning
    private int minRandomNumberForTowerTileSpawning = 0; // min
    private int maxRandomNumberForTowerTileSpawning = 10; // max
    private int currentRandomNumberChanceOfTowerTileSpawning = 0;
    private void GenerateAndCreateTowerTiles(GameObject _tileLeftOrRightOfPathFinder)
    {
        currentRandomNumberChanceOfTowerTileSpawning = Random.Range(minRandomNumberForTowerTileSpawning, maxRandomNumberForTowerTileSpawning); // random number
        {
            if (currentRandomNumberChanceOfTowerTileSpawning > fixedNumberForChanceOfTowerTileSpawning && currentAmountOfTowerTiles <= maxAmountOfTowerTiles) // chance of tower spawning on that tile
            {
                currentAmountOfTowerTiles++;
                CreateTowerTile(_tileLeftOrRightOfPathFinder, currentAmountOfTowerTiles);
                SetGameObjectName(_tileLeftOrRightOfPathFinder, $"{mapTileOppcupied}");
            }
        }
    }

    // called within GenerateAndCreateTowerTiles() when the appropriate measures are met
    // this creates a tower and sets the correct information
    private void CreateTowerTile(GameObject _tileGO, int _towerTileCount)
    {
        GameObject _towerTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _towerTile.layer = 9; // layer == "Tower"
        SetGameObjectName(_towerTile, $"{towerTileName}_{_towerTileCount}");
        _towerTile.transform.SetParent(parentHolderForMapTiles.transform.GetChild(4)); // tower holder
        _towerTile.transform.position = _tileGO.transform.position;
        GenerateMapTileMaterial(_towerTile, "RockCliff_Layered");
        _towerTile.AddComponent<TowerStatsAndInfo>();
    }

    // called within ContinueMapWalkPathCreation() creates tile walls based on pathfinder forward direction
    private void CreateMapTileWalls(GameObject _tile)
    {
        GameObject _wallOne = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject _wallTwo = GameObject.CreatePrimitive(PrimitiveType.Cube);

        GenerateMapTileMaterial(_wallOne, "BlackRock");
        GenerateMapTileMaterial(_wallTwo, "BlackRock");

        SetGameObjectName(_wallOne, "Wall One");
        SetGameObjectName(_wallTwo, "Wall Two");

        _wallOne.transform.localScale = new Vector3(0.1f, 0.5f, 1f);
        _wallTwo.transform.localScale = new Vector3(0.1f, 0.5f, 1f);

        _wallOne.transform.SetParent(_tile.transform, true);
        _wallTwo.transform.SetParent(_tile.transform, true);

        if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Forward)
        {
            if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
            {
                _wallOne.transform.position = _tile.transform.position + -_tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + _tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
            {
                _wallOne.transform.position = _tile.transform.position + -_tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + -_tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else
            {
                _wallOne.transform.position = _tile.transform.position + _tile.transform.right / 2;
                _wallTwo.transform.position = _tile.transform.position + -_tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 0f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
        }
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Down)
        {
            if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
            {
                _wallOne.transform.position = _tile.transform.position + _tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + _tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
            {
                _wallOne.transform.position = _tile.transform.position + _tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + -_tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else
            {
                _wallOne.transform.position = _tile.transform.position + _tile.transform.right / 2;
                _wallTwo.transform.position = _tile.transform.position + -_tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 0f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
        }
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
        {
            if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Forward)
            {
                _wallOne.transform.position = _tile.transform.position + _tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + -_tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Down)
            {
                _wallOne.transform.position = _tile.transform.position + -_tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + -_tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else
            {
                _wallOne.transform.position = _tile.transform.position + _tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + -_tile.transform.up / 2;

                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 90f, 0f);
            }
        }
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
        {
            if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Forward)
            {
                _wallOne.transform.position = _tile.transform.position + _tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + _tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Down)
            {
                _wallOne.transform.position = _tile.transform.position + -_tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + _tile.transform.right / 2;
                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 0f, 0f);
            }
            else
            {
                _wallOne.transform.position = _tile.transform.position + -_tile.transform.up / 2;
                _wallTwo.transform.position = _tile.transform.position + _tile.transform.up / 2;

                _wallOne.transform.rotation = Quaternion.Euler(0, 90f, 0f);
                _wallTwo.transform.rotation = Quaternion.Euler(0, 90f, 0f);
            }
        }
    }

    // called from ContinueMapWalkPathCreation() function
    // Creates a bridge like slerp motion that spawns waypoints from 'a' to 'b' position (3 tiles - up, across, down)
    private IEnumerator CreateBridge(Vector3 optionalOffsetForAPos = default,
        float optionalOffsetForFirstWaypointSpawnYAxis = default, float optionalOffsetForLastWaypointSpawnYAxis = default, float optionalOffsetAddOntoCenterCurveSlerp = default)
    {
        Vector3 positionA = PathFinderMapTileGO.transform.position + optionalOffsetForAPos; // initialize position A of the slerp
        if (optionalOffsetForFirstWaypointSpawnYAxis != default)
            positionA.y = optionalOffsetForFirstWaypointSpawnYAxis;

        GameObject positionB = new GameObject(); // initialize position B for slerp.. temp gameobject.. destroy positionB gameobject when pathfinder reaches its destination.
        positionB.name = "Position B"; // tempoary gameobject.. removing 
        Vector3 postionB = PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.forward * 2f;
        if (optionalOffsetForLastWaypointSpawnYAxis != default)
            postionB.y = optionalOffsetForLastWaypointSpawnYAxis;
        positionB.transform.position = postionB;

        float slerpMove = 0; // initial "time stamp"
        float slerpTime = 0.1f; // increase this variable to speed up the bridge arc movement

        //find center between 'a' and 'b' .. this allows the gameobject to arc between positions
        Vector3 center = (positionA + postionB) * 0.5f;
        center -= new Vector3(0, 1f + optionalOffsetAddOntoCenterCurveSlerp, 0);
        Vector3 pathFinderReletiveCenter = positionA - center;
        Vector3 bPositionReletiveCenter = postionB - center;

        // create X amount of waypoints to spawned when creating the bridge (arc)
        float[] wayPointTimes = new float[5];
        for (int i = 0; i < wayPointTimes.Length; i++)
        {
            wayPointTimes[i] = slerpTime * i * 0.1f;
        }
        int wayPointIndex = 0;

        while (slerpMove < slerpTime)
        {
            if (wayPointIndex < wayPointTimes.Length && slerpMove >= wayPointTimes[wayPointIndex])
            {
                if (wayPointIndex == 0)
                {
                    if (optionalOffsetForAPos != default)
                        GenerateBridgeTiles(1f, 0f, 1f); // first bridge tile
                    else
                        GenerateBridgeTiles(0f, 0f, 0f);
                }
                if (wayPointIndex == (wayPointTimes.Length / 2) + 1)
                {
                    GenerateBridgeTiles(); // middle bridge tile 
                }

                RemoveTowerTileOnBridgeTile();
                wayPointIndex++;
            }

            slerpMove += Time.deltaTime;
            float perc = Mathf.Clamp01(slerpMove / slerpTime);
            PathFinderMapTileGO.transform.position = Vector3.Slerp(pathFinderReletiveCenter, bPositionReletiveCenter, perc);
            PathFinderMapTileGO.transform.position += center;

            CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder();

            yield return new WaitForSeconds(slerpMove);
        }

        // after the pathfinder reaches its next position
        yield return null;
        if (PathFinderMapTileGO.transform.position != new Vector3(PathFinderMapTileGO.transform.position.x, 1f, PathFinderMapTileGO.transform.position.z))
            PathFinderMapTileGO.transform.position = new Vector3(PathFinderMapTileGO.transform.position.x, 1f, PathFinderMapTileGO.transform.position.z);
        yield return new WaitForSeconds(0.1f);
        Destroy(positionB);
        if (optionalOffsetForAPos != default)
            GenerateBridgeTiles(1f, 0f, 1f); // last bridge tile
        else
            GenerateBridgeTiles();
        isBridgeCreated = true;
        isBridgeCurrentlyBeingCreated = false;
        Invoke("CheckpointReachedCustomWaitTime", 0.1f);
        CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder(-1f, 0f, -1f);
        yield return new WaitForSeconds(pathFinderMoveSpeed * 2f);
    }

    // _local bridge counter to keep track of the bridge tile amount
    // Called from the CreateBridge() function.. spawns bridge tiles based on the position of the pathfinder
    private int _localBridgeCount = 0;
    private void GenerateBridgeTiles(float offsetPosX = default, float offsetPosY = default, float offsetPosZ = default)
    {
        GameObject bridgeTileGO = GameObject.CreatePrimitive(PrimitiveType.Quad);

        GenerateMapTileMaterial(bridgeTileGO, "BridgeTile_CobbleStone");

        string pathFinderYAxisDecimalPlace = PathFinderMapTileGO.transform.position.y.ToString("F2"); // decimal place 1.4
        float.TryParse(pathFinderYAxisDecimalPlace, out float yResult); // PathFinderMapTileGO posistion Y axis 

        bridgeTileGO.transform.position = new Vector3
        (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult, Mathf.Round(PathFinderMapTileGO.transform.position.z)); // initial position catch

        if (_localBridgeCount == 1) // middle bridge tile
        {
            bridgeTileGO.transform.position = new Vector3
           (Mathf.Round(PathFinderMapTileGO.transform.position.x), 1.4f, Mathf.Round(PathFinderMapTileGO.transform.position.z));
            bridgeTileGO.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // middle bridge tile base
        }

        if (_localBridgeCount != 1) bridgeTileGO.transform.localScale = new Vector3(1f, 1.3f, 1f); // middle bridge tile

        if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Forward)
        {
            if (_localBridgeCount == 0)
            {
                if (offsetPosZ != default && ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z + -offsetPosZ));
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 90f, 0f);
                }
                else if (offsetPosZ != default && ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z + -offsetPosZ));
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, -90f, 0f);
                }
                else
                {
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
                }
            }
            if (_localBridgeCount == 2)
            {
                if (offsetPosZ != default)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z + -offsetPosZ));
                }
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 180f, 0f);
            }
        }
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Down)
        {
            if (_localBridgeCount == 0)
            {
                if (offsetPosZ != default && ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z + offsetPosZ));
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 90f, 0f);
                }
                else if (offsetPosZ != default && ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z + offsetPosZ));
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, -90f, 0f);
                }
                else
                {
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 180f, 0f);
                }
            }
            if (_localBridgeCount == 2)
            {
                if (offsetPosZ != default)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z + offsetPosZ));
                }
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
            }
        }
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
        {
            if (_localBridgeCount == 0)
            {
                if (offsetPosX != default)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x + -offsetPosX), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z));
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
                }
                else
                {
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 90f, 0f);
                }
            }
            if (_localBridgeCount == 2)
            {
                if (offsetPosX != default)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x + -offsetPosX), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z));
                }
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, -90f, 0f);
            }
        }
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
        {
            if (_localBridgeCount == 0)
            {
                if (offsetPosX != default)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x + offsetPosX), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z));
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
                }
                else
                {
                    bridgeTileGO.transform.rotation = Quaternion.Euler(50f, -90f, 0f);
                }
            }
            if (_localBridgeCount == 2)
            {
                if (offsetPosX != default)
                {
                    bridgeTileGO.transform.position = new Vector3
                    (Mathf.Round(PathFinderMapTileGO.transform.position.x + offsetPosX), yResult + offsetPosY, Mathf.Round(PathFinderMapTileGO.transform.position.z));
                }
                bridgeTileGO.transform.rotation = Quaternion.Euler(50f, 90f, 0f);
            }
        }
        _localBridgeCount++;
        SetGameObjectName(bridgeTileGO, $"{bridgeTileName}_{_localBridgeCount}");
        bridgeTileGO.transform.SetParent(parentHolderForMapTiles.transform.GetChild(5)); // bridge holder
    }

    // invoked from ContinueMapWalkPathCreation() && IEnumerator CreateBridge() functions
    // Custom wait time - this is called after an invoke has been canceled.. to resume creation of the path
    private void CheckpointReachedCustomWaitTime()
    {
        InvokeRepeating("ContinueMapWalkPathCreation", 0.2f, pathFinderMoveSpeed);
    }

    // called within ContinueMapWalkPathCreation()
    private void MovePathFinderMapTileGO()
    {
        // move the pathfinder GO in the forwards direction * by 1
        PathFinderMapTileGO.transform.Translate(transform.forward * 1f, Space.Self);
    }

    // initial positioning pathfinder.transform.position on spawn .. called within FindAndCreateMapWalkPath()
    private void InitialPositionOfPathFinder(Collider coll)
    {
        PathFinderMapTileGO.transform.position = new Vector3(coll.transform.position.x, 1f, coll.transform.position.z);
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
            int randomNum = Random.Range(0, checkpointGroundMapTileList.Count);
            checkpointMapTileNumber = checkpointGroundMapTileList[randomNum];
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
        SetGameObjectName(PathFinderMapTileGO, "PathFinder");
        PathFinderMapTileGO.layer = 8;
        PathFinderMapTileGO.AddComponent<BoxCollider>();
        PathFinderMapTileGO.transform.localScale = Vector3.one * 0.5f;
        //GenerateMapTileMaterial(PathFinderMapTileGO, "PathFinder");
        pathFinderMoveSpeed = 0.15f;
        TurnOffMeshRenderer(PathFinderMapTileGO);
    }

    // this function is called whenever the pathfinder rotates and turns a different direction
    // this will allow waypoints to be created so as the characters will be able to follow these waypoints accordingaly along the path
    private void CreateWaypoint(float optionalOffsetX = default, float optionalOffsetY = default, float optionalOffsetZ = default, Transform moveableWaypoint = default)
    {
        if (moveableWaypoint == default)
        {
            waypointCounter++;
            GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            SetGameObjectName(waypoint, $"{waypointName}_{waypointCounter}");
            waypoint.transform.SetParent(parentHolderForMapTiles.transform.GetChild(3)); // waypoint holder
            waypoint.transform.localScale = Vector3.one * 0.25f;
            waypoint.transform.position = new Vector3(PathFinderMapTileGO.transform.position.x + optionalOffsetX,
                PathFinderMapTileGO.transform.position.y + optionalOffsetY, PathFinderMapTileGO.transform.position.z + optionalOffsetZ);
            waypoint.transform.rotation = PathFinderMapTileGO.transform.rotation;
            //GenerateMapTileMaterial(waypoint, "PathFinder");
            TurnOffMeshRenderer(waypoint);
            waypointsList.Add(waypoint);
        }
        else // if you want to move a waypoint that has already been created
        {
            moveableWaypoint.position = new Vector3(PathFinderMapTileGO.transform.position.x + optionalOffsetX,
                PathFinderMapTileGO.transform.position.y + optionalOffsetY, PathFinderMapTileGO.transform.position.z + optionalOffsetZ);
        }
    }

    private void InstantiatePrefab_MapEntranceAndExit(string _GOName, GameObject _GOParent, Vector3 _optionalRotation = default)
    {
        GameObject _archWay = Instantiate(Resources.Load("Prefabs/ArchWithPillars") as GameObject);
        SetGameObjectName(_archWay, _GOName);
        _archWay.transform.SetParent(_GOParent.transform);
        _archWay.transform.position = _GOParent.transform.position;
        if (_optionalRotation == default)
        {
            if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Forward)
            {
                _archWay.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Down)
            {
                _archWay.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
            {
                _archWay.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
            {
                _archWay.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
        }
        else
        {
            _archWay.transform.rotation = Quaternion.Euler(_optionalRotation.x, _optionalRotation.y, _optionalRotation.z);
        }
    }

    // This function calls CreateWaypoint() after it offsets the waypoints accordingly
    private void CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder
        (float optionalOffsetX = default, float optionalOffsetY = default, float optionalOffsetZ = default, Transform moveableWaypoint = default)
    {
        if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Forward)
            CreateWaypoint(0f, optionalOffsetY, -optionalOffsetZ, moveableWaypoint);
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Down)
            CreateWaypoint(0f, optionalOffsetY, optionalOffsetZ, moveableWaypoint);
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
            CreateWaypoint(-optionalOffsetX, optionalOffsetY, 0f, moveableWaypoint);
        else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
            CreateWaypoint(optionalOffsetX, optionalOffsetY, 0f, moveableWaypoint);
    }

    // called from ContinueMapWalkPathCreation() .. only called when collider check is triggered from the 'right' or 'left' triggers when rotating towards exit
    // checks for exitturn waypoint and positions the waypoint were should intend to be depending on the forward direction of the pathfinder
    private void OffsetTheExitTurnWaypoint()
    {
        GameObject waypointExitTurn = waypointsList.Last();
        if (waypointExitTurn.name == waypointExitTurnName)
        {
            if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Forward)
            {
                if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right) // working
                    CreateWaypoint(-1f, 0f, -1f, waypointExitTurn.transform);
                else if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left) // working
                    CreateWaypoint(1, 0f, -1f, waypointExitTurn.transform);
            }
            else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Down)
            {
                if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right) // working
                    CreateWaypoint(-1, 0f, 1f, waypointExitTurn.transform);
                else if (ReturnPreviousPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left) // working
                    CreateWaypoint(1, 0f, 1f, waypointExitTurn.transform);
            }
            else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Right)
                CreateWaypoint(-1f, 0f, -1f, waypointExitTurn.transform);
            else if (ReturnCurrentPathFinderForwardDirection() == PathFinderForwardDirectionToGlobal.Left)
                CreateWaypoint(1f, 0f, -1f, waypointExitTurn.transform);
        }
    }

    #region Path finder physics updater
    // called on FixedUpdate() - once boolean "isMapWalkPathCheckpointsSpawned" is true this function will proceed
    private void PathFinderPhysics()
    {
        // Add raycasts in 4 directions ( forward, backwards, right, left )
        Vector3 fwd = PathFinderMapTileGO.transform.TransformDirection(Vector3.forward);
        Vector3 back = PathFinderMapTileGO.transform.TransformDirection(Vector3.back);
        Vector3 right = PathFinderMapTileGO.transform.TransformDirection(Vector3.right);
        Vector3 left = PathFinderMapTileGO.transform.TransformDirection(Vector3.left);

        RaycastHit hit;

        // raycasting backwards --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, back, out hit, 10f, ~LayerMask))
        {
            InitialPathFinderGameObjectRotation(new Vector3(0f, 0f, 0f), hit, "0f backwards");
        }

        // raycasting to the right --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, right, out hit, 10f, ~LayerMask))
        {
            PathFinderRotateTowardsExitAndCheckpoint(new Vector3(0f, 90f, 0f), hit, "Rotate right");

            CalculateDistanceFromPathFinderToRightSideEdge(hit);

            // when pathfinder gameobject spawns on the RIGHT side of the map, rotate '-90f' degrees so it is facing forward with it's back to the entrance
            InitialPathFinderGameObjectRotation(new Vector3(0f, -90f, 0f), hit, "-90f right");
        }

        // raycasting to the left --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, left, out hit, 10f, ~LayerMask))
        {
            PathFinderRotateTowardsExitAndCheckpoint(new Vector3(0f, -90f, 0f), hit, "Rotate left");

            CalculateDistanceFromPathFinderToLeftSideEdge(hit);

            // when pathfinder gameobject spawns on the LEFT side of the map, rotate '90f' degrees so it is facing forward with it's back to the entrance
            InitialPathFinderGameObjectRotation(new Vector3(0f, 90f, 0f), hit, "90f left");
        }

        // raycasting forwards --->
        if (Physics.Raycast(PathFinderMapTileGO.transform.position, fwd, out hit, 10f, ~LayerMask))
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
    }

    // called in PathFinderPhysics() to rotate the GO initially on spawn
    private void InitialPathFinderGameObjectRotation(Vector3 rotation, RaycastHit _hit, string debug)
    {
        if (_hit.collider.name == mapTileEntranceName && !isPathFinderInitialSpawnCompleted)
        {
            var disBetweenEntranceAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, entranceMapTileGO.transform.position);
            if (disBetweenEntranceAndPathFinder < 1.2f)
            {
                isPathFinderInitialSpawnCompleted = true;
                PathFinderMapTileGO.transform.rotation = Quaternion.Euler(rotation);
                previousPathFinderForwardDir = ReturnCurrentPathFinderForwardDirection();
                pathFinderNewDirectionChange = ReturnCurrentPathFinderForwardDirection();
                InstantiatePrefab_MapEntranceAndExit("Entrance Arch Way", entranceMapTileGO, rotation);
                CreateMapTileWalls(mapTileInfrontOfEntrance);
            }
        }
    }

    // called within PathFinderPhysics() - rotate the pathfinder in the correct direction
    private void PathFinderRotateTowardsExitAndCheckpoint(Vector3 rotation, RaycastHit _hit, string log)
    {
        if (_hit.collider.name == mapTileExitName || _hit.collider.name.ToLower().Contains("checkpoint"))
        {
            isExitOrCheckpointInSightForPathFinder = true;
            //Debug.Log($"isExitOrCheckpointInSightForPathFinder: {isExitOrCheckpointInSightForPathFinder}.. @isFirstCheckpointReached: {isFirstCheckpointReached}..");
        }
        else
        {
            isExitOrCheckpointInSightForPathFinder = false;
        }

        if (_hit.collider.name == mapTileExitName && isPathFinderInitialSpawnCompleted && isFirstCheckpointReached && isExitOrCheckpointInSightForPathFinder) // --> find exit
        {
            Debug.Log($"Exit is in sight: {log}");
            CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder();
            isPathFinderRotatingToExit = true;
            GameObject exitTurnWaypoint = waypointsList.Last(); // get the last waypoint and rename it so we can differentiate it
            SetGameObjectName(exitTurnWaypoint, $"{waypointExitTurnName}_{waypointCounter}");
            PathFinderMapTileGO.transform.Rotate(rotation, Space.Self);
        }

        if (_hit.collider.name.ToLower().Contains("checkpoint") && isPathFinderInitialSpawnCompleted && !isFirstCheckpointReached) // --> find first checkpoint
        {
            PathFinderMapTileGO.transform.Rotate(rotation, Space.Self);
            CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder();
            Debug.Log($"Checkpoint is in sight: {log}");
        }
    }

    private void PathFinderCheckForCollisionOfEdge(RaycastHit _hit)
    {
        // if the first checkpoint has been reached EXTRA condition check -->
        if (_hit.collider.name == mapTileEdgeName && isPathFinderInitialSpawnCompleted && isFirstCheckpointReached)
        {
            var disBetweenEdgeAndPathFinder = Vector3.Distance(PathFinderMapTileGO.transform.position, _hit.transform.position);
            if (disBetweenEdgeAndPathFinder < 1.2f)
            {
                //Debug.Log($"first checkpoint reached... 1.2f distance away from an edge tile... we need to rotate left or right..");
                if (ReturnRayCastDirectionLeftOrRight() == "right")
                {
                    PathFinderMapTileGO.transform.Rotate(new Vector3(0f, 90f, 0f), Space.Self);
                    if (!isBridgeCurrentlyBeingCreated) CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder();
                    Debug.Log("Rotate right .. collision to edge...");
                }
                else if (ReturnRayCastDirectionLeftOrRight() == "left")
                {
                    PathFinderMapTileGO.transform.Rotate(new Vector3(0f, -90f, 0f), Space.Self);
                    if (!isBridgeCurrentlyBeingCreated) CreateWaypointWithOffsetToTheForwardDirectionOfPathFinder();
                    Debug.Log("Rotate left .. collision to edge...");
                }
            }
        }
    }

    private (float, string) CalculateDistanceFromPathFinderToRightSideEdge(RaycastHit _hit)
    {
        if (_hit.collider.name == mapTileEdgeName)
        {
            distanceFromPathFinderToRightSideEdge = Vector3.Distance(PathFinderMapTileGO.transform.position, _hit.collider.transform.position);
        }
        //Debug.Log("right side distance: " +  distanceFromPathFinderToRightSideEdge);
        return (distanceFromPathFinderToRightSideEdge, rayCastDirectionLeftOrRight);
    }

    private (float, string) CalculateDistanceFromPathFinderToLeftSideEdge(RaycastHit _hit)
    {
        if (_hit.collider.name == mapTileEdgeName)
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
    #endregion

    #region PathFinder directions
    private PathFinderForwardDirectionToGlobal ReturnCurrentPathFinderForwardDirection()
    {
        Quaternion forwardDir = PathFinderMapTileGO.transform.rotation;
        var rotation = Math.Round(forwardDir.eulerAngles.y);
        if (rotation == 0f) // forward
        {
            currentPathFinderForwardDir = PathFinderForwardDirectionToGlobal.Forward;
        }
        else if (rotation == 180f || rotation == -180f)
        {
            currentPathFinderForwardDir = PathFinderForwardDirectionToGlobal.Down;
        }
        else if (rotation == 90f)
        {
            currentPathFinderForwardDir = PathFinderForwardDirectionToGlobal.Right;
        }
        else if (rotation == -90f || rotation == 270f)
        {
            currentPathFinderForwardDir = PathFinderForwardDirectionToGlobal.Left;
        }
        return currentPathFinderForwardDir;
    }

    float directionChangeCoolDownTimerAmount = 0.2f;
    float directionChangeCoolDownTimerCurrent = 0;
    bool canActivateCoolDownTimerForDirectionChange;
    // apply previous path finder gameobject position .. with a basic timer check every X seconds
    private PathFinderForwardDirectionToGlobal ReturnPreviousPathFinderForwardDirection()
    {
        if (pathFinderNewDirectionChange != ReturnCurrentPathFinderForwardDirection())
        {
            pathFinderDirectionHistory.Add(pathFinderNewDirectionChange.ToString());
            pathFinderNewDirectionChange = ReturnCurrentPathFinderForwardDirection();
            previousPathFinderForwardDir = (PathFinderForwardDirectionToGlobal)Enum.Parse(typeof(PathFinderForwardDirectionToGlobal), pathFinderDirectionHistory[pathFinderDirectionHistory.Count - 1]);
            canActivateCoolDownTimerForDirectionChange = true;
            directionChangeCoolDownTimerCurrent = directionChangeCoolDownTimerAmount;
        }
        return previousPathFinderForwardDir;
    }

    private void CoolDownTimerForDirectionChange()
    {
        if (canActivateCoolDownTimerForDirectionChange)
        {
            if (directionChangeCoolDownTimerCurrent > 0)
            {
                directionChangeCoolDownTimerCurrent -= Time.deltaTime;
            }
            if (directionChangeCoolDownTimerCurrent < 0)
            {
                canActivateCoolDownTimerForDirectionChange = false;
                previousPathFinderForwardDir = ReturnCurrentPathFinderForwardDirection();
            }
        }
    }
    #endregion

    #region ParentHolder for maptiles and adjustments
    // initiate gameobject holders on start() 
    private void ParentMapTileHolderCreation()
    {
        parentHolderForMapTiles = new GameObject();
        SetGameObjectName(parentHolderForMapTiles, "ParentHolderForMapTiles");

        GameObject _groundHolder = new GameObject(); // child 0
        SetGameObjectName(_groundHolder, "Ground Holder");
        _groundHolder.transform.SetParent(parentHolderForMapTiles.transform);

        GameObject _edgeHolder = new GameObject(); // child 1
        SetGameObjectName(_edgeHolder, "Edge Holder");
        _edgeHolder.transform.SetParent(parentHolderForMapTiles.transform);

        GameObject _walkPathHolder = new GameObject(); // child 2 
        SetGameObjectName(_walkPathHolder, "Walk Path Holder");
        _walkPathHolder.transform.SetParent(parentHolderForMapTiles.transform);

        GameObject _wayPointsHolder = new GameObject(); // child 3 
        SetGameObjectName(_wayPointsHolder, "Waypoint Holder");
        _wayPointsHolder.transform.SetParent(parentHolderForMapTiles.transform);

        GameObject _towerHolder = new GameObject(); // child 4 
        SetGameObjectName(_towerHolder, "Tower Holder");
        _towerHolder.transform.SetParent(parentHolderForMapTiles.transform);

        GameObject _bridgeHolder = new GameObject(); // child 5 
        SetGameObjectName(_bridgeHolder, "Bridge Holder");
        _bridgeHolder.transform.SetParent(parentHolderForMapTiles.transform);
    }

    private void SetLastWalkTileToLastBeforeExit()
    {
        BoxCollider pathColl = exitMapTileGO.GetComponent<BoxCollider>();
        // Get the bounds of the collider
        Bounds pathBounds = pathColl.bounds;
        // Check for colliders within the bounds of the collider
        Collider[] overlapWithCollidersInEnd1 = Physics.OverlapBox(exitMapTileGO.transform.position + -exitMapTileGO.transform.up / 2, pathBounds.extents);
        Collider[] overlapWithCollidersInEnd2 = Physics.OverlapBox(exitMapTileGO.transform.position + exitMapTileGO.transform.up / 2, pathBounds.extents);

        foreach (Collider collider in overlapWithCollidersInEnd1.Union(overlapWithCollidersInEnd2))
        {
            float dis = Vector3.Distance(exitMapTileGO.transform.position, collider.transform.position);
            if (collider.name == mapTileGroundName && dis == 1)
            {
                SetGameObjectName(collider.gameObject, lastMapTileWalkPathName);
            }
        }
    }

    private void SetGameObjectName(GameObject _gameObject, string _name)
    {
        _gameObject.name = _name;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            Gizmos.DrawWireCube(PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.forward / 2, PathFinderMapTileGO.GetComponent<BoxCollider>().bounds.extents); // forward box
            Gizmos.DrawWireCube(PathFinderMapTileGO.transform.position + PathFinderMapTileGO.transform.right / 2, PathFinderMapTileGO.GetComponent<BoxCollider>().bounds.extents); // right box
            Gizmos.DrawWireCube(PathFinderMapTileGO.transform.position + -PathFinderMapTileGO.transform.right / 2, PathFinderMapTileGO.GetComponent<BoxCollider>().bounds.extents); // left box
            Gizmos.DrawWireCube(PathFinderMapTileGO.transform.position + -PathFinderMapTileGO.transform.up / 2, PathFinderMapTileGO.GetComponent<BoxCollider>().bounds.extents); // down box

            Gizmos.DrawWireCube(exitMapTileGO.transform.position + exitMapTileGO.transform.up / 2, exitMapTileGO.GetComponent<BoxCollider>().bounds.extents); // draw box in front of the exit tile
            Gizmos.DrawWireCube(exitMapTileGO.transform.position + -exitMapTileGO.transform.up / 2, exitMapTileGO.GetComponent<BoxCollider>().bounds.extents); // draw box in front of the exit tile
        }
    }
}