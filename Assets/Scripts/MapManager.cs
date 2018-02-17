﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for Map controlls :
/// </summary>
public class MapManager : MonoBehaviour
{

    [Header("Maps variables")]
    // Active map
    private List<Transform> tilesMap;
    public GameObject wall;
    #region map1
    public Transform map1; // TileList
    private List<Transform> tilesMap1;
    // Array map (lighter than storing with list of game objects)
    public int[,] map1Array = new int[8, 8] { // map of 8 line and 8 colums
        { 0, 0, 1, 1, 0, 0, 1, 0 },
        { 0, 1, 0, 0, 1, 1, 0, 0 },
        { 1, 1, 1, 0, 0, 1, 0, 1 },
        { 0, 1, 0, 1, 1, 1, 0, 0 },
        { 0, 1, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 1, 1, 1, 1 },
        { 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 0, 1, 1, 0 }
    };
    public int[,] map2Array = new int[8, 8] { // map of 8 line and 8 colums
        { 0, 0, 0, 1, 1, 0, 1, 0 },
        { 0, 1, 0, 1, 0, 1, 1, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0 },
        { 0, 1, 1, 0, 0, 0, 1, 0 },
        { 0, 0, 1, 0, 1, 1, 1, 1 },
        { 0, 0, 0, 1, 0, 0, 1, 0 },
        { 0, 1, 1, 1, 1, 1, 0, 1 },
        { 0, 1, 0, 0, 0, 1, 0, 0 }
    };
    private bool map1active;
    #endregion
    #region map2
    public Transform map2; // TileList
    private List<Transform> tilesMap2;
    private bool map2active;
    #endregion
    private bool mapSwap;


    public static MapManager mapInstance;
    //SINGLETON
    /// <summary>
    /// Initialize singleton instance
    /// </summary>
    private void Awake()
    {
        if (mapInstance != null)
        {
            Debug.LogError("More than one Map Manager in scene");
            return;
        }
        else
        {
            mapInstance = this;
        }
    }

    /// <summary>
    /// Initialize variables
    /// </summary>
    private void Start()
    {
        tilesMap1 = new List<Transform>();
        // Get child elements from map object
        for (int i = 0; i < map1Array.GetLength(0); i++)
        {
            for (int j = 0; j < map1Array.GetLength(1); j++)
            {
                if (map1Array[i, j] == 1)
                {
                    Vector3 position = new Vector3(i, 0, j);
                    // Instantiate the wall, set its position
                    GameObject wallObj = (GameObject)Instantiate(this.wall);
                    wallObj.transform.position = position;
                    wallObj.transform.parent = map1.transform;
                    tilesMap1.Add(wallObj.transform);
                }
            }
        }

        tilesMap2 = new List<Transform>();
        for (int i = 0; i < map2Array.GetLength(0); i++)
        {
            for (int j = 0; j < map2Array.GetLength(1); j++)
            {
                if (map2Array[i, j] == 1)
                {
                    Vector3 position = new Vector3(i, 0, j);
                    // Instantiate the wall, set its position
                    GameObject wallObj = (GameObject)Instantiate(this.wall);
                    wallObj.transform.position = position;
                    wallObj.transform.parent = map2.transform;
                    tilesMap2.Add(wallObj.transform);
                }
            }
        }

        map1active = true;
        map2active = false;
        map1.gameObject.SetActive(map1active);
        map2.gameObject.SetActive(map2active);
        tilesMap = tilesMap1;
        mapSwap = true;
    }

    /// <summary>
    /// Get User input for switching Map
    /// </summary>
    private void Update()
    {
        // if the player press "Space" and is not moving and not swaping maps
        if (Input.GetKeyDown(KeyCode.Space) && !MainCharacterController.characterController.animator.GetBool("isWalking") && mapSwap)
        {
            IEnumerator coroutine = SwapMaps();
            StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Function that swaps maps and play wall animation
    /// </summary>
    /// <returns>Return true if a wall with the given X and Z postion or false if not</returns>
    private IEnumerator SwapMaps()
    {
        map1active = !map1active;
        map2active = !map2active;
        mapSwap = false;
        map1.gameObject.SetActive(map1active);
        map2.gameObject.SetActive(map2active);
        if (map1.gameObject.activeInHierarchy)
            tilesMap = tilesMap1;
        else
            tilesMap = tilesMap2;
        foreach (Transform tile in tilesMap)
        {
            tile.position += new Vector3(0, 5, 0);
        }
        yield return new WaitForSeconds(1f);
        #region WaitAndDo // this will be executed only when the coroutine have finished
        mapSwap = true;
        #endregion
    }

    /// <summary>
    /// Chack if a wall exists in the active map with a given "x" and "y" position
    /// </summary>
    /// <param name = x > float position in X axis.</param>
    /// <param name = y > float position in Z axis.</param>
    /// <returns>Return true if a wall with the given X and Z postion or false if not</returns>
    public bool GetWall(float x, float z)
    {
        foreach (Transform tile in tilesMap)
        {
            TileBlock tileBlock = tile.GetComponent<TileBlock>();
            if (tileBlock != null)
            {
                if (tileBlock.x == x && tileBlock.z == z)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
