using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public enum ControlMode
{ 
    JOYSTICK_FIXED,
    JOYSTICK_DYNAMIC,
    GYROSCOPE,
}

// Save best times for each level
public static class SaveData
{ 
    public const string Prefix = "HighScore_Level_";
    public const string Level1Time = Prefix + "1";
    public const string Level2Time = Prefix + "2";
    public const string Level3Time = Prefix + "3";
    public const string Level4Time = Prefix + "4";
    public const string Level5Time = Prefix + "5";
}

// Manages game state, level loading, UI panels
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager instance;

    // Variables
    private int currentLevel = 0;
    private float levelTime = 0.0f;
    private float levelScore1 = float.MaxValue;
    private float levelScore2 = float.MaxValue;
    private float levelScore3 = float.MaxValue;
    private float levelScore4 = float.MaxValue;
    private float levelScore5 = float.MaxValue;
    private bool isPaused = false;
    private bool isVictory = false;
    private bool isGirlSkin = false;
    private bool isGirlUnlocked = true;
    private bool achievement1 = false;
    private bool achievement2 = false;
    private bool achievement3 = false;
    private bool achievement4 = false;
    private bool achievement5 = false;

    public ControlMode controlMode = ControlMode.JOYSTICK_FIXED;

    // References
    private GameObject joystick;
    private GameObject pausePanel;
    private GameObject victoryPanel;
    private GameObject timer;
    private GameObject victoryTimer;
    private GameObject victoryText;
    private GameObject bestTime1;
    private GameObject bestTime2;
    private GameObject bestTime3;
    private GameObject bestTime4;
    private GameObject bestTime5;
    private GameObject LevelSelector;
    private GameObject Shop;
    private GameObject AchievementPanel;
    private GameObject lock1;
    private GameObject lock2;
    private GameObject lock3;
    private GameObject lock4;
    private GameObject lock5;
    private GameObject achievementAnnounce;
    private GameObject leaderboardAnnounce;
    private GameObject joystickFixedButton;
    private GameObject joystickDynamicButton;
    private GameObject gyroscopeButton;
    private GameObject resetGyroButton;
    private NewBehaviourScript playerSphere;

    //-----------------------------------------------------
    // Ads
    InterstitialAd m_interstitialAd;
    BannerView m_bannerView;
    RewardedAd m_rewardedAd;

    //test id
    string m_interstitialAdUnitID = "ca-app-pub-3940256099942544/1033173712";
    string m_bannerAdUnitID = "ca-app-pub-3940256099942544/6300978111";
    string m_rewardedAdUnitID = "ca-app-pub-3940256099942544/5224354917";
    // Variable to track which level to load after the rewarded ad
    private int rewardedAdNextLevelIndex = 0;

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
        levelScore1 = PlayerPrefs.GetFloat(SaveData.Level1Time, float.MaxValue);
        levelScore2 = PlayerPrefs.GetFloat(SaveData.Level2Time, float.MaxValue);
        levelScore3 = PlayerPrefs.GetFloat(SaveData.Level3Time, float.MaxValue);
        levelScore4 = PlayerPrefs.GetFloat(SaveData.Level4Time, float.MaxValue);
        levelScore5 = PlayerPrefs.GetFloat(SaveData.Level5Time, float.MaxValue);

        // Load skin settings
        LoadSkinSettings();

        // Load achievements
        LoadAchievements();

        // TEMP : set current level
        switch (SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                currentLevel = 0;
                break;
            case "SampleScene":
                currentLevel = 1;
                break;
            case "Level2":
                currentLevel = 2;
                break;
            case "Level3":
                currentLevel = 3;
                break;
            case "Level4":
                currentLevel = 4;
                break;
            case "Level5":
                currentLevel = 5;
                break;
            default:
                currentLevel = 0;
                break;
        }
    }

    void Start()
    { 
        // Set frame rate
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0; // Disable vSync

        MobileAds.Initialize(m_initStatus => {
            print("google ads are initialized");
        });

        // Create an interstitial ad
        if (m_interstitialAd != null)
        {
            m_interstitialAd.Destroy();
            m_interstitialAd = null;
        }
        print ("Creating interstitial ad.");
        MobileAds.Initialize(m_initStatus => {
            print("Google Mobile Ads initialized.");

            // Start loading the first interstitial and rewarded ads immediately after initialization
            LoadInterstitialAd();
            LoadRewardedAd();
        });

        // Load Banner Ad
        LoadBannerAd();
    }

    // Load Interstitial Ad
    void LoadInterstitialAd()
    {
        if (m_interstitialAd != null)
        {
            m_interstitialAd.Destroy();
            m_interstitialAd = null;
        }
        print("Loading interstitial ad...");
        var adRequest = new AdRequest();
        InterstitialAd.Load(m_interstitialAdUnitID, adRequest,
          (InterstitialAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  // ad failed
                  print("Interstitial ad failed to load with error: " + error);
                  return;
              }
              // ad loaded
              print("Interstitial ad loaded with response: " + ad.GetResponseInfo());
              m_interstitialAd = ad;
              AdEventHandlers(m_interstitialAd);
          });
    }

    // Show Interstitial Ad
    public void ShowInterstitialAd()
    {
        if (m_interstitialAd != null && m_interstitialAd.CanShowAd())
        {
            Debug.Log("Showing Interstitial AD");
            m_interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad not loaded yet.");
            // If the ad isn't ready, try to load a new one for the next time
            LoadInterstitialAd();
        }
    }

    // Load Banner Ad
    public void LoadBannerAd()
    {
        // Create a 320x50 banner at bottom of the screen
        if (m_bannerView != null)
        {
            m_bannerView.Destroy();
            m_bannerView = null;
        }
        m_bannerView = new BannerView(m_bannerAdUnitID, AdSize.Banner, AdPosition.Bottom);
        // Create an empty ad request
        var request = new AdRequest();
        // Load the banner with the request
        m_bannerView.LoadAd(request);
    }

    // Load Rewarded Ad
    public void LoadRewardedAd()
    {
        if (m_rewardedAd != null)
        {
            m_rewardedAd.Destroy();
            m_rewardedAd = null;
        }
        print("Loading rewarded ad...");
        var adRequest = new AdRequest();
        RewardedAd.Load(m_rewardedAdUnitID, adRequest,
          (RewardedAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  // ad failed
                  print("Rewarded ad failed to load with error: " + error);
                  return;
              }
              // ad loaded
              print("Rewarded ad loaded with response: " + ad.GetResponseInfo());
              m_rewardedAd = ad;
              AdEventHandlers(m_rewardedAd, rewardedAdNextLevelIndex);
          });
    }

    // Show Rewarded Ad
    public void ShowRewardedAd(int _levelIndex)
    {
        rewardedAdNextLevelIndex = _levelIndex;

        if (m_rewardedAd != null && m_rewardedAd.CanShowAd())
        {
            Debug.Log("Showing Rewarded AD");
            m_rewardedAd.Show((Reward _reward) =>
            {
                Debug.Log("User earned reward. Loading level: " + rewardedAdNextLevelIndex);
                LoadLevel(rewardedAdNextLevelIndex);
            });
        }
        else
        {
            Debug.Log("Rewarded ad not loaded yet.");
            LoadRewardedAd();
        }
    }

    // Ad handler
    void AdEventHandlers(InterstitialAd _ad)
    {
        // Called when the ad is shown.
        _ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad clicked.");
        };
        // Called when the ad is opened.
        _ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad opened.");
        };
        // Called when the ad is closed.
        _ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad closed.");

            // Reload the ad for next time
            LoadInterstitialAd();
        };
        // Called when the ad is error.
        _ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("Interstitial ad failed to open with error : " + error);

            // Reload the ad for next time
            LoadInterstitialAd();
        };
    }
    void AdEventHandlers(RewardedAd _ad, int _levelIndex)
    {
        // Called when the ad is shown.
        _ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad clicked.");
        };
        // Called when the ad is opened.
        _ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad opened.");
        };
        // Called when the ad is closed.
        _ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed.");

            // Reload the ad for next time
            LoadRewardedAd();
        };
        // Called when the ad is error.
        _ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("Rewarded ad failed to open with error : " + error);

            // Reload the ad for next time
            LoadRewardedAd();
        };
    }


    // Select Boy Skin
    public void SelectBoySkin()
    {
        isGirlSkin = false;
        SaveSkinSettings();
        Debug.Log("Selected Boy skin");
    }

    // Select Girl Skin
    public void SelectGirlSkin()
    {
        if (isGirlUnlocked)
        {
            isGirlSkin = true;
            Debug.Log("Selected Girl skin");
            SaveSkinSettings();
        }
        else
        {
            // TODO: Setup IAP
            isGirlUnlocked = true;
            isGirlSkin = true;
            Debug.Log("Selected Girl skin");
            SaveSkinSettings();
        }
    }

    public bool IsGirlUnlocked()
    {
        return isGirlUnlocked;
    }
    public bool IsGirlSkin()
    {
        return isGirlSkin;
    }
    public void SaveSkinSettings()
    {
        // Save the skin preference
        int girlSkinValue = isGirlSkin ? 1 : 0;
        PlayerPrefs.SetInt("GirlSkin", girlSkinValue);

        // Save the unlocked status
        int girlUnlockedValue = isGirlUnlocked ? 1 : 0;
        PlayerPrefs.SetInt("GirlUnlocked", girlUnlockedValue);

        PlayerPrefs.Save();
        Debug.Log("Skin settings saved.");
    }

    public void LoadSkinSettings()
    {
        // Load the skin preference
        int savedSkinValue = PlayerPrefs.GetInt("GirlSkin", 0);
        isGirlSkin = (savedSkinValue == 1);

        // Load the unlocked status
        int savedUnlockedValue = PlayerPrefs.GetInt("GirlUnlocked", 0);
        isGirlUnlocked = (savedUnlockedValue == 1);

        Debug.Log($"Skin settings loaded: isGirlSkin = {isGirlSkin}, isGirlUnlocked = {isGirlUnlocked}");
    }

    public void SetAchievement(int achievementIndex, bool status)
    {
        switch (achievementIndex)
        {
            case 1:
                achievement1 = status;
                break;
            case 2:
                achievement2 = status;
                break;
            case 3:
                achievement3 = status;
                break;
            case 4:
                achievement4 = status;
                break;
            case 5:
                achievement5 = status;
                break;
            default:
                Debug.Log("Invalid achievement index");
                break;
        }
    }

    public void SaveAchievements()
    {
        PlayerPrefs.SetInt("Achievement1", achievement1 ? 1 : 0);
        PlayerPrefs.SetInt("Achievement2", achievement2 ? 1 : 0);
        PlayerPrefs.SetInt("Achievement3", achievement3 ? 1 : 0);
        PlayerPrefs.SetInt("Achievement4", achievement4 ? 1 : 0);
        PlayerPrefs.SetInt("Achievement5", achievement5 ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Achievements saved.");
    }

    public void LoadAchievements()
    {
        achievement1 = PlayerPrefs.GetInt("Achievement1", 0) == 1;
        achievement2 = PlayerPrefs.GetInt("Achievement2", 0) == 1;
        achievement3 = PlayerPrefs.GetInt("Achievement3", 0) == 1;
        achievement4 = PlayerPrefs.GetInt("Achievement4", 0) == 1;
        achievement5 = PlayerPrefs.GetInt("Achievement5", 0) == 1;
        Debug.Log("Achievements loaded.");
    }


    // -----------------------------------------------------


    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            // Keep track of time
            levelTime += Time.deltaTime;

            // Update timer UI
            if (timer != null)
            {
                int minutes = Mathf.FloorToInt(levelTime / 60F);
                int seconds = Mathf.FloorToInt(levelTime - minutes * 60);
                int milliseconds = Mathf.FloorToInt((levelTime * 100) % 100);
                timer.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            }
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
                SceneManager.LoadSceneAsync("Level2");
                break;
            case 3:
                Debug.Log("Loading Level 3");
                SceneManager.LoadSceneAsync("Level3");
                break;
            case 4:
                Debug.Log("Loading Level 4");
                SceneManager.LoadSceneAsync("Level4");
                break;
            case 5:
                Debug.Log("Loading Level 5");
                SceneManager.LoadSceneAsync("Level5");
                break;
            default:
                Debug.Log("Loading Main Menu");
                SceneManager.LoadSceneAsync("MainMenu");
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
        joystick = GameObject.FindWithTag("JoyStick");
        pausePanel = GameObject.FindWithTag("PausePanel");
        victoryPanel = GameObject.FindWithTag("VictoryPanel");
        timer = GameObject.FindWithTag("Timer");
        victoryTimer = GameObject.FindWithTag("VictoryTimer");
        victoryText = GameObject.FindWithTag("VictoryText");
        bestTime1 = GameObject.Find("BestTime1");
        bestTime2 = GameObject.Find("BestTime2");
        bestTime3 = GameObject.Find("BestTime3");
        bestTime4 = GameObject.Find("BestTime4");
        bestTime5 = GameObject.Find("BestTime5");
        LevelSelector = GameObject.FindWithTag("LevelSelectorPanel");
        Shop = GameObject.FindWithTag("ShopPanel");
        AchievementPanel = GameObject.FindWithTag("AchievementPanel");
        achievementAnnounce = GameObject.FindWithTag("AchievementAnnounce");
        leaderboardAnnounce = GameObject.FindWithTag("LeaderboardAnnounce");
        lock1 = GameObject.Find("Lock_1");
        lock2 = GameObject.Find("Lock_2");
        lock3 = GameObject.Find("Lock_3");
        lock4 = GameObject.Find("Lock_4");
        lock5 = GameObject.Find("Lock_5");
        joystickFixedButton = GameObject.Find("JoyStickFixed");
        joystickDynamicButton = GameObject.Find("JoyStickDynamic");
        gyroscopeButton = GameObject.Find("Gyro");
        resetGyroButton = GameObject.Find("Reset Gyro");

        GameObject playerSphereObject = GameObject.FindWithTag("Player");
        if (playerSphereObject != null) playerSphere = playerSphereObject.GetComponent<NewBehaviourScript>();
        else playerSphere = null;

        if (LevelSelector != null)
        {
            // Disable Level Selector on startup
            LevelSelector.SetActive(false);
        }

        if (Shop != null)
        {
            // Disable Shop on startup
            Shop.SetActive(false);

        }

        if (AchievementPanel != null)
        {
            // Disable Achievement Panel on startup
            AchievementPanel.SetActive(false);
        }

        if (pausePanel == null || victoryPanel == null || timer == null || victoryTimer == null || victoryText == null)
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
            timer.SetActive(true);
            isPaused = false;

            if (controlMode == ControlMode.JOYSTICK_FIXED && joystick != null)
            {
                joystick.SetActive(true);
                joystick.transform.position = new Vector3(540, 300, 0); // Fixed position
            }
            else if (joystick != null)
            {
                joystick.SetActive(false);
            }
        }

        // If in main menu, update best time displays
        if (scene.name == "MainMenu")
        {
            // Load banner ad
            LoadBannerAd();

            Debug.Log("In Main Menu - Updating Best Times");
            // Update best time displays
            if (bestTime1 != null)
            {
                if (levelScore1 == float.MaxValue)
                {
                    bestTime1.GetComponent<TextMeshProUGUI>().text = "N/A";
                }
                else
                {
                    int minutes = Mathf.FloorToInt(levelScore1 / 60F);
                    int seconds = Mathf.FloorToInt(levelScore1 - minutes * 60);
                    int milliseconds = Mathf.FloorToInt((levelScore1 * 100) % 100);
                    bestTime1.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
                }
            }
            else
            {
                Debug.Log("BestTime1 object not found in Main Menu");
            }
            if (bestTime2 != null)
            {
                if (levelScore2 == float.MaxValue)
                {
                    bestTime2.GetComponent<TextMeshProUGUI>().text = "N/A";
                }
                else
                {
                    int minutes = Mathf.FloorToInt(levelScore2 / 60F);
                    int seconds = Mathf.FloorToInt(levelScore2 - minutes * 60);
                    int milliseconds = Mathf.FloorToInt((levelScore2 * 100) % 100);
                    bestTime2.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
                }
            }
            if (bestTime3 != null)
            {
                if (levelScore3 == float.MaxValue)
                {
                    bestTime3.GetComponent<TextMeshProUGUI>().text = "N/A";
                }
                else
                {
                    int minutes = Mathf.FloorToInt(levelScore3 / 60F);
                    int seconds = Mathf.FloorToInt(levelScore3 - minutes * 60);
                    int milliseconds = Mathf.FloorToInt((levelScore3 * 100) % 100);
                    bestTime3.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
                }
            }
            if (bestTime4 != null)
            {
                if (levelScore4 == float.MaxValue)
                {
                    bestTime4.GetComponent<TextMeshProUGUI>().text = "N/A";
                }
                else
                {
                    int minutes = Mathf.FloorToInt(levelScore4 / 60F);
                    int seconds = Mathf.FloorToInt(levelScore4 - minutes * 60);
                    int milliseconds = Mathf.FloorToInt((levelScore4 * 100) % 100);
                    bestTime4.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
                }
            }
            if (bestTime5 != null)
            {
                if (levelScore5 == float.MaxValue)
                {
                    bestTime5.GetComponent<TextMeshProUGUI>().text = "N/A";
                }
                else
                {
                    int minutes = Mathf.FloorToInt(levelScore5 / 60F);
                    int seconds = Mathf.FloorToInt(levelScore5 - minutes * 60);
                    int milliseconds = Mathf.FloorToInt((levelScore5 * 100) % 100);
                    bestTime5.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
                }
            }
        }
    }

    public void EnterShop()
    {
        ShowInterstitialAd();
    }

    public void EnterAchievementPanel()
    {
        AchievementPanel.SetActive(true);

        // Check achievements and update locks
        if (achievement1) lock1.SetActive(false);
        else lock1.SetActive(true);

        if (achievement2) lock2.SetActive(false);
        else lock2.SetActive(true);

        if (achievement3) lock3.SetActive(false);
        else lock3.SetActive(true);

        if (achievement4) lock4.SetActive(false);
        else lock4.SetActive(true);

        if (achievement5) lock5.SetActive(false);
        else lock5.SetActive(true);
    }

    // Victory function
    public void Victory()
    {
        bool isNewBestTime = false;

        if (!isVictory)
        {
            Debug.Log("Level Complete!");

            // Get the current best time for this level
            float currentTime = levelTime;
            string saveKey = SaveData.Prefix + currentLevel.ToString();
            float bestTime = PlayerPrefs.GetFloat(saveKey, float.MaxValue);

            // Check if current time beat the best time
            if (currentTime < bestTime)
            {
                isNewBestTime = true;
                Debug.Log("New Best Time!");
                // Save new best time
                PlayerPrefs.SetFloat(saveKey, currentTime);
                PlayerPrefs.Save();

                // Update victory text
                if (victoryText != null)
                {
                    victoryText.GetComponent<TextMeshProUGUI>().text = "New\nBest Time!";
                }

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
            }
            else
            {
                // Update victory text
                if (victoryText != null)
                {
                    victoryText.GetComponent<TextMeshProUGUI>().text = "Level\nCompleted!";
                }
            }



            // Show victory screen
            victoryPanel.SetActive(true);
            timer.SetActive(false); // Hide timer
            isPaused = true;
            isVictory = true;
            Time.timeScale = 0; // Pause game time
            // Show leaderboard announcement
            leaderboardAnnounce.SetActive(isNewBestTime);

            // Achievement
            switch (currentLevel)
            {
                case 1:
                    if (!achievement1)
                    { 
                        achievementAnnounce.SetActive(true);
                        achievement1 = true;
                        SaveAchievements();
                    }
                    else achievementAnnounce.SetActive(false);
                    break;
                case 2:
                    if (!achievement2)
                    {
                        achievementAnnounce.SetActive(true);
                        achievement2 = true;
                        SaveAchievements();
                    }
                    else achievementAnnounce.SetActive(false);
                    break;
                case 3:
                    if (!achievement3)
                    {
                        achievementAnnounce.SetActive(true);
                        achievement3 = true;
                        SaveAchievements();
                    }
                    else achievementAnnounce.SetActive(false);
                    break;
                case 4:
                    if (!achievement4)
                    {
                        achievementAnnounce.SetActive(true);
                        achievement4 = true;
                        SaveAchievements();
                    }
                    else achievementAnnounce.SetActive(false);
                    break;
                case 5:
                    if (!achievement5)
                    {
                        achievementAnnounce.SetActive(true);
                        achievement5 = true;
                        SaveAchievements();
                    }
                    else achievementAnnounce.SetActive(false);
                    break;
                default:
                    Debug.Log("Error: Invalid Level Index");
                    break;
            }

            // Deactivate joystick if in fixed mode
            if (joystick != null && controlMode == ControlMode.JOYSTICK_FIXED)
            {
                joystick.SetActive(false);
            }
            // Update victory timer UI
            if (victoryTimer != null)
            {
                int minutes = Mathf.FloorToInt(levelTime / 60F);
                int seconds = Mathf.FloorToInt(levelTime - minutes * 60);
                int milliseconds = Mathf.FloorToInt((levelTime * 100) % 100);
                victoryTimer.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            }
        }
    }

    // Load next level
    public void NextLevel()
    {
        int maxLevel = 5; // Set maximum level index
        int nextLevel = currentLevel + 1;
        if (nextLevel > maxLevel)
        {
            // If on last level, go to main menu
            LoadLevel(0);
            return;
        }
        LoadLevel(nextLevel);
    }

    // Pause game
    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f; // Freeze game time
            pausePanel.SetActive(true);
            // Deactivate joystick if in fixed mode
            if (joystick != null && controlMode == ControlMode.JOYSTICK_FIXED)
            {
                joystick.SetActive(false);
            }
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
        // Reactivate joystick if in fixed mode
        if (joystick != null && controlMode == ControlMode.JOYSTICK_FIXED)
        {
            joystick.SetActive(true);
        }
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

    // Change Control Mode function
    public void ChangeControlMode(int mode)
    {
        controlMode = (ControlMode)mode;
        Debug.Log("Control Mode changed to: " + controlMode.ToString());
        if (joystick != null)
        {
            if (controlMode == ControlMode.JOYSTICK_FIXED)
            {
                joystick.SetActive(true);
                joystick.transform.position = new Vector3(540, 300, 0); // Fixed position
            }
            else
            {
                joystick.SetActive(false);
            }
        }
        if (resetGyroButton != null)
        {
            if (controlMode == ControlMode.GYROSCOPE)
            {
                resetGyroButton.SetActive(true);
            }
        }
    }

    // Reset Gyroscope function
    public void ResetGyroscope()
    {
        if (playerSphere != null)
        {
            playerSphere.ResetGyroscope();
        }
    }

    // Get current control mode
    public ControlMode GetControlMode() { return controlMode; }

    // On Destroy
    private void OnDestroy()
    {
        // Clean up ad
        if (m_interstitialAd != null)
        {
            m_interstitialAd.Destroy();
            m_interstitialAd = null;
        }
        if (m_bannerView != null)
        {
            m_bannerView.Destroy();
            m_bannerView = null;
        }
        if (m_rewardedAd != null)
        {
            m_rewardedAd.Destroy();
            m_rewardedAd = null;
        }
    }

}

