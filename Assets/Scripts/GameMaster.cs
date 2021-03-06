﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Class responsible for GameOver and Success
/// Manage Time
/// </summary>
public class GameMaster : MonoBehaviour {

    public const int MAXSUBLVL = 2;
    public static int lvl = 0;

    [Header("UI Elements variables")]
    public Animator retryUI;
    public static bool retry = false; // for 1st loading, we don't want to display "retry" txt
    public GameObject finishUI;
    public List<GameObject> imgLvl;
    public List<GameObject> mapLvlIcon;

    public static float elapsedTime = 0; // we wan't to get the time spend on menu
    public TextMeshProUGUI timeFinish;

    public static GameMaster gameMasterinstance;
    //SINGLETON
    /// <summary>
    /// Initialize singleton instance and variables
    /// </summary> 
    private void Awake()
    {

        // Debug.Log("Time" + (Time.time));
        // Debug.Log("Start Time"+(Time.time - elapsedTime));
        if (gameMasterinstance != null)
        {
            Debug.LogError("More than one gameMasterinstance in scene");
            return;
        }
        else
        {
            gameMasterinstance = this;
        }
    }

    private void Start()
    {
        imgLvl[MapManager.sublvl].SetActive(true);
        mapLvlIcon[MapManager.mapInstance.GetActiveMap() - 1].SetActive(true);
    }
    /// <summary>
    /// Function called when the object becomes enabled and active
    /// </summary>
    public void OnEnable()
    {
        if(retry)
            retryUI.SetTrigger("retry"); // Activate Retry Ui, Animation will play on Entry
        MainCharacterController.OnWallCollisionEvent += Retry; // Subscribing to OnCollision event
        MainCharacterController.ReachedGoalEvent += LevelFinished;
    }

    /// <summary>
    /// Function called when the object becomes disabled and inactive
    /// </summary>
    public void OnDisable()
    {
        MainCharacterController.OnWallCollisionEvent -= Retry;
        MainCharacterController.ReachedGoalEvent -= LevelFinished;
    }
    
    /// <summary>
    /// Function called when OnCollision broadcast a signal
    /// </summary>
    public void Retry()
    {
        if(lvl>1)
            SceneManager.LoadScene(3); // Reload lvl
        else
            SceneManager.LoadScene(lvl+1); // Reload lvl
        GameMaster.retry = true; // Tell the class that we have retried (so the next scene can display retry UI)
    }

    public void ReLoadRandomGeneration()
    {
        SceneManager.LoadScene(3); // Reload lvl
        GameMaster.retry = false;
    }

    /// <summary>
    /// Function called when user swaps maps
    /// </summary>
    public void ChangeMapIcon()
    {
        foreach (GameObject icon in mapLvlIcon)
        {
            icon.SetActive(false);
        }
        mapLvlIcon[MapManager.mapInstance.GetActiveMap()-1].SetActive(true);
    }


    /// <summary>
    /// Finishing level if the player has reached goal
    /// </summary>
    public void LevelFinished()
    {
        //Debug.Log("Finish" + (Time.time - elapsedTime));

        finishUI.SetActive(true);
        float time = Mathf.Floor(Time.time - elapsedTime);
        timeFinish.text = time.ToString();
        // Send Highscore from level 1
        if(lvl>0 && lvl != 2)
            Highscores.highscoreManager.AddNewHighscore(Menu.playerName, lvl + MapManager.sublvl, (int)time); // MapManager.sublvl begins at 0

        MapManager.sublvl++;
        if (MapManager.sublvl != MAXSUBLVL && lvl != 0 && lvl != 2)
        {
            IEnumerator coroutine = WaitAndLoadScene();
            StartCoroutine(coroutine);
        }
        else // Full Lvl finished
        {
            MapManager.sublvl = 0;
            GameMaster.retry = false;
        }
    }

    /// <summary>
    /// Function that waits seconds and LoadNextScene
    /// </summary>
    /// <returns>Return true if a wall with the given X and Z postion or false if not</returns>
    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(2f);
        #region WaitAndDo // this will be executed only when the coroutine have finished
        elapsedTime = Time.time - elapsedTime; // reset timer
        SceneManager.LoadScene(lvl+1); // Reload 1st lvl
        GameMaster.retry = false;
        #endregion
    }

    /// <summary>
    /// Function that triggers when option button is cliked on game
    /// </summary>
    public void PreventControlsInOptions()
    {
        MainCharacterController.characterController.lockControls = true;
    }

    /// <summary>
    /// Function that triggers when back button is cliked on option menu in game
    /// </summary>
    public void RestoreControlsFromOptions()
    {
        MainCharacterController.characterController.lockControls = false;
    }
}
