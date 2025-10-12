using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager instance;

    // Variables
    private int currentLevel = 0;
    private float levelTime = 0.0f;
    private float levelScore1 = 0.0f;
    private float levelScore2 = 0.0f;
    private float levelScore3 = 0.0f;
    private float levelScore4 = 0.0f;
    private float levelScore5 = 0.0f;
    private bool isPaused = false;
    private bool isVictory = false;

    // Panel references
    private GameObject pausePanel;
    private GameObject victoryPanel;

    //-----------------------------------------------------
    // Unity Methods

    private void Awake()
    {
        // If instance already exists, destroy this
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Set game manager to persistent
        DontDestroyOnLoad(this.gameObject);

        // Load save data
        // TODO: Implement save/load system for high score

        // TEMP : set current level
        switch(SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                currentLevel = 0;
                break;
            case "SampleScene":
                currentLevel = 1;
                break;
            default:
                currentLevel = 0;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            // Keep track of time
            levelTime += Time.deltaTime;
        }
        
    }

    // Load level function
    public void LoadLevel(int levelIndex)
    {
        currentLevel = levelIndex;
        levelTime = 0.0f;
        isPaused = false;
        isVictory = false;
        Time.timeScale = 1f; // Resume game time
        switch (levelIndex)
        {
            case 0:
                Debug.Log("Loading Main Menu");
                SceneManager.LoadSceneAsync("MainMenu");
                break;
            case 1:
                Debug.Log("Loading Level 1");
                SceneManager.LoadSceneAsync("SampleScene");
                break;
            case 2:
                Debug.Log("Loading Level 2");
                break;
            default:
                Debug.Log("Loading Level " + levelIndex);
                break;
        }
        
    }

    private void OnEnable()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe when the object is destroyed/disabled
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called automatically after a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // references to UI objects in the newly loaded scene
        pausePanel = GameObject.FindWithTag("PausePanel");
        victoryPanel = GameObject.FindWithTag("VictoryPanel");

        if (pausePanel == null || victoryPanel == null)
        {
            // Only show an error if we are in a game scene where these panels are expected
            if (scene.name != "MainMenu")
            {
                Debug.LogWarning("GameManager: UI Panels not found " + scene.name);
            }
        }
        else
        {
            // Ensure panels start in the correct state
            pausePanel.SetActive(false);
            victoryPanel.SetActive(false);
            isPaused = false;
        }
    }

    // Victory function
    public void Victory()
    {
        Debug.Log("Level Complete!");

        // Save level score
        switch (currentLevel)
        {
            case 1:
                levelScore1 = levelTime;
                break;
            case 2:
                levelScore2 = levelTime;
                break;
            case 3:
                levelScore3 = levelTime;
                break;
            case 4:
                levelScore4 = levelTime;
                break;
            case 5:
                levelScore5 = levelTime;
                break;
            default:
                Debug.Log("Error: Invalid Level Index");
                break;
        }
        
        // Show victory screen
        victoryPanel.SetActive(true);
        isPaused = true;
        isVictory = true;
        Time.timeScale = 0; // Resume game time
    }

    // Load next level
    public void NextLevel()
    {
        LoadLevel(currentLevel + 1);
    }

    // Pause game
    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f; // Freeze game time
            pausePanel.SetActive(true);
        }
        else if (isVictory)
        {
            return;
        }
        else
        {
            Resume();
        }
    }

    // Resume game
    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    // Restart level
    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }

    //Back to main menu
    public void BackToMainMenu()
    {
        LoadLevel(0);
    }
}
