using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    private int xTileMap;
    private int yTileMap;
    private int entranceMapTileNumber;
    private int ExitMapTileNumber;
    private List<int> tileList;
    private List<int> edgeOfMapTileList;

    private GameObject mapTileGO;
    private GameObject entranceMapTileGO;
    private GameObject exitMapTileGO;
    private GameObject edgeMapTileGO;
    private GameObject continuedPathMapTileGO;

    private bool isMapEntranceSpawned = false;
    private bool isMapExitSpawned = false;

    private void Start()
    {
        tileList = new List<int>();
        edgeOfMapTileList = new List<int>()
        { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 20,
            21, 30, 31, 40, 41, 50, 51, 60, 61, 70, 71, 80,
            81, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };

        xTileMap = 10;
        yTileMap = 10;

        RandomEntranceAndExitMapTileNumber();
        Invoke("SpawnMapTiles", 0.2f);
        Invoke("FindAndCreateMapWalkPath", 0.5f);
    }

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
            }
        }
    }

    private void GenerateMapTileGameObject(int xTile, int yTile)
    {
        mapTileGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mapTileGO.transform.position = new Vector3(xTile, 0.5f, yTile);
        mapTileGO.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        mapTileGO.name = "Map tile other";
    }

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
                    isMapEntranceSpawned = true;
                }
            }
        }
    }

    private void FindAndCreateMapExit()
    {
        if (!isMapExitSpawned)
        {
            for (int i = 0; i <= tileList.Count; i++)
            {
                if (i == ExitMapTileNumber)
                {
                    exitMapTileGO = mapTileGO;
                    exitMapTileGO.name = "Map tile exit";
                    GenerateMapTileMaterial(mapTileGO, Color.blue);
                    isMapExitSpawned = true;
                }
            }
        }
    }

    private void FindAndCreateMapEdges()
    {
        if (edgeOfMapTileList.Contains(tileList.Count))
        {
            edgeMapTileGO = mapTileGO;
            edgeMapTileGO.name = "Map tile Edge";
            GenerateMapTileMaterial(mapTileGO, Color.red);
        }
    }

    private void FindAndCreateMapWalkPath()
    {
        BoxCollider entranceColl = entranceMapTileGO.GetComponent<BoxCollider>();
        // Get the bounds of the collider
        Bounds entranceBounds = entranceColl.bounds;
        // Check for colliders within the bounds of the collider
        Collider[] entranceColliders = Physics.OverlapBox(entranceBounds.center, entranceBounds.extents);

        foreach (Collider collider in entranceColliders)
        {
            // Do something with the collider (this includes the colliding GameObject)
            if (!collider.name.ToLower().Contains("edge") &&
                !collider.name.ToLower().Contains("entrance"))
            {
                var dis = Vector3.Distance(entranceMapTileGO.transform.position,
                    collider.transform.position);

                if (collider.name.ToLower().Contains("other") && dis == 1)
                {
                    Debug.Log("Colliding with: " + collider.gameObject.name + "Dis:" + dis);
                    GenerateMapTileMaterial(collider.gameObject, Color.yellow);
                    continuedPathMapTileGO = collider.gameObject;
                    InvokeRepeating("ContinueMapWalkPathCreation", 1f, 1f);
                    return;
                }
            }
        }
    }

    private void ContinueMapWalkPathCreation()
    {
        BoxCollider pathColl = continuedPathMapTileGO.GetComponent<BoxCollider>();
        // Get the bounds of the collider
        Bounds pathBounds = pathColl.bounds;
        // Check for colliders within the bounds of the collider
        Collider[] pathColliders = Physics.OverlapBox(pathBounds.center, pathBounds.extents);

        foreach (Collider collider in pathColliders)
        {
            // Do something with the collider (this includes the colliding GameObject)
            if (!collider.name.ToLower().Contains("edge") &&
                !collider.name.ToLower().Contains("entrance"))
            {
                var dis = Vector3.Distance(continuedPathMapTileGO.transform.position,
                    collider.transform.position);
                Debug.Log("dis..." + dis);

                if (collider.name.ToLower().Contains("other") && dis == 1)
                {
                    Debug.Log("Colliding with: " + collider.gameObject.name /*+ "Dis:" + dis*/);
                    GenerateMapTileMaterial(collider.gameObject, Color.yellow);
                    continuedPathMapTileGO = collider.gameObject;
                    return;
                }
                if (collider.name.ToLower().Contains("exit"))
                {
                    Debug.Log("Colliding with: " + collider.gameObject.name);
                    CancelInvoke();
                }
            }
        }
    }

    private (int, int) RandomEntranceAndExitMapTileNumber()
    {
        int pickEntranceRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
        entranceMapTileNumber = edgeOfMapTileList[pickEntranceRandomNumInList];

        int pickExitRandomNumInList = Random.Range(1, edgeOfMapTileList.Count);
        ExitMapTileNumber = edgeOfMapTileList[pickExitRandomNumInList];

        if (new[] { 1, 10, 91, 100 }.Contains(entranceMapTileNumber)
            || entranceMapTileNumber == ExitMapTileNumber)
        {
            int newEntrancePickRandomNumInArr = Random.Range(1, edgeOfMapTileList.Count);
            entranceMapTileNumber = edgeOfMapTileList[newEntrancePickRandomNumInArr];
            Debug.Log("DO not use corner tiles for entrance.. pick another number");
        }

        if (new[] { 1, 10, 91, 100 }.Contains(ExitMapTileNumber)
            || ExitMapTileNumber == entranceMapTileNumber)
        {
            int newExitPickRandomNumInArr = Random.Range(1, edgeOfMapTileList.Count);
            ExitMapTileNumber = edgeOfMapTileList[newExitPickRandomNumInArr];
            Debug.Log("DO not use corner tiles for exit.. pick another number");
        }

        Debug.Log($"Entrance number: {entranceMapTileNumber} && Exit Number: {ExitMapTileNumber}");
        return (entranceMapTileNumber, ExitMapTileNumber);
    }

    private void GenerateMapTileBoxCollider()
    {
        BoxCollider coll = mapTileGO.AddComponent<BoxCollider>();
        coll.size *= 1.5f;
    }

    private void GenerateMapTileMaterial(GameObject tileGO, Color matColor)
    {
        Renderer rend = tileGO.GetComponent<Renderer>();
        rend.material.color = matColor;
    }
}