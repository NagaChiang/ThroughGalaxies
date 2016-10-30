using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Advertisements;

[System.Serializable]
public struct Stage
{
    public Wave[] waves;
}

public class GameManager : MonoBehaviour {

    [Header("Player")]
    public GameObject player;

    [Header("Stage")]
    public float stageInterval;
    public Stage[] galaxies;

    [Header("Shuttle")]
    public GameObject objShuttle;
    public float probShuttleSpawnPerWave;
    public float posZShuttle;
    public Limit posXShuttle;

    [Header("GameObjects")]
    public ExpCrystal[] expCrystals;
    public HealCrystal[] healCrystals;
    public GameObject[] objSupplies;

    [Header("UI")]
    public Text guiTextScore;
    public int numScoreDigit;
    public BackgroundScroller BgScroller;
    public GameObject UiMenu;
    public GameObject UiHowToPlay;
    public GameObject UiOption;
    public GameObject UiHud;
    public GameObject UiGameover;
    public GameObject UiHighscoreFromMenu;
    public GameObject UiHighscoreFromGameover;
    public GameObject UiSubmitHighscore;
    public InputField UiInputName;
    public Text UiTextDisplay;
    public GameObject UiBossStatus;
    public GameObject UiPause;
    public GameObject UiTutorialButton;
    public GameObject UiVirtualController;
    public GameObject UiMenuQuitButton;
    public GameObject UiPauseResumeButton;
    public GameObject UiPauseQuitButton;
    public GameObject UiGameoverQuitButton;
    public GameObject UiExtraLife;
    public UiScoreResizer UiScoreResizer;

    [Header("Misc")]
    public CameraShaker Camera;
    public HerokuDatabase Database;
    public AudioManager AudioManager;
    public UnityAdDisplayer AdDisplayer;

    public bool IsMobile { get; private set; }

    private GameObject _player;
    private Coroutine RoutineWaveSpawn;
    private bool IsGameover;
    private int _score;
    private float _difficultyFactor;
    private int _stage;
    private bool _enabledEnterRestart;
    private int _remainingLife;

    private bool isPaused;
    private bool EnabledPause;

    void Start ()
    {
        // For mobile
#if UNITY_ANDROID || UNITY_IOS
        IsMobile = true;
        UiTutorialButton.SetActive(false);
        UiVirtualController.SetActive(true);
        UiMenuQuitButton.SetActive(true);
        UiPauseResumeButton.SetActive(true);
        UiPauseQuitButton.SetActive(true);
        UiGameoverQuitButton.SetActive(true);
#endif

        // show main menu
        showMainMenu();

        // BGM
        AudioManager.PlayBgm(AudioManager.Clip_Bgm);
    }

    void Update()
    {
        // Quit on menu or gameover

        // let player press enter to start
        if (_enabledEnterRestart && Input.GetButtonDown("Submit"))
        {
            PlaySfxButtonClick();
            restart();
        }

        // Toggle pause
        if(EnabledPause && (Input.GetButtonDown("Pause") || Input.GetButtonDown("Cancel")))
        {
            TogglePause();
        }

        // Submit highscore
        if(UiInputName.IsActive()
            && UiInputName.text != ""
            && Input.GetButtonDown("Submit"))
        {
            // Submit
            SubmitHighscore();
            PlaySfxButtonClick();
        }
    }

    public void restart()
    {
        if (_player == null)
        {
            // disable enter
            _enabledEnterRestart = false;

            // Enable pause
            EnabledPause = true;

            // clear previous remains
            clearRemainingGameObjects();

            // disable other UI and enable HUD
            UiMenu.SetActive(false);
            UiHud.SetActive(true);
            UiGameover.SetActive(false);
            UiBossStatus.SetActive(false);

            // stop last wave spawning
            if (RoutineWaveSpawn != null)
                StopCoroutine(RoutineWaveSpawn);

            // initial properties
            IsGameover = false;
            _score = 0;
            _difficultyFactor = 1.0f;
            _stage = 1;

            // update score UI
            updateScoreUI(_score);

            // spawn player
            _player = Instantiate(player);

            // spawn waves
            RoutineWaveSpawn = StartCoroutine(spawnWaves());
        }
    }

    public void showMainMenu()
    {
        // disable other UI and enable main menu
        UiMenu.SetActive(true);
        UiHowToPlay.SetActive(false);
        UiHud.SetActive(false);
        UiGameover.SetActive(false);
        UiHighscoreFromMenu.SetActive(false);
        _enabledEnterRestart = true;
        UiOption.SetActive(false);
    }
    public void showHowToPlay()
    {
        // Disable enter
        _enabledEnterRestart = false;

        // enable how to play
        UiMenu.SetActive(false);
        UiHowToPlay.SetActive(true);
    }

    public void showOptions()
    {
        UiMenu.SetActive(false);
        UiOption.SetActive(true);
    }

    public void showHighscoreFromMainmenu()
    {
        // Disable enter
        _enabledEnterRestart = false;

        // Enable ui
        UiMenu.SetActive(false);
        UiHighscoreFromMenu.SetActive(true);

        // Update highscore
        List<NameScoreData> data = new List<NameScoreData>();
        StartCoroutine(Database.GetHighscoreData(data));

        // Pass to highscore to wait for coroutine finishing
        Highscore highscore = UiHighscoreFromMenu.GetComponent<Highscore>();
        if (highscore && highscore.isActiveAndEnabled)
            highscore.StartCoroutine(highscore.UpdateScores(data));
    }

    public void showHighscoreFromGameover()
    {
        // Disable enter
        _enabledEnterRestart = false;

        // Enable ui
        UiGameover.SetActive(false);
        UiHighscoreFromGameover.SetActive(true);

        // Update highscore
        List<NameScoreData> data = new List<NameScoreData>();
        StartCoroutine(Database.GetHighscoreData(data));

        // Pass to highscore to wait for coroutine finishing
        Highscore highscore = UiHighscoreFromGameover.GetComponent<Highscore>();
        if(highscore && highscore.isActiveAndEnabled)
            highscore.StartCoroutine(highscore.UpdateScores(data));
    }

    public void showSubmitHighscore()
    {
        UiSubmitHighscore.SetActive(true);
        UiInputName.ActivateInputField();
    }

    public void showGameover()
    {
        // set the gameover menu active
        UiHighscoreFromGameover.SetActive(false);
        UiTextDisplay.gameObject.SetActive(false);
        UiGameover.SetActive(true);
        _enabledEnterRestart = true;
    }

    public void PlaySfxButtonClick()
    {
        AudioManager.PlaySfx(AudioManager.Clip_ButtonClick);
    }

    public IEnumerator gameover()
    {
        // Set flag
        IsGameover = true;

        // Disable pause
        EnabledPause = false;

        // Disable Ad prompt
        UiExtraLife.SetActive(false);

        // Unfreeze time
        Time.timeScale = 1.0f;

        // Prepare player data
        PlayerController playerController = _player.GetComponent<PlayerController>();
        PlayerData data;
        data.Score = _score;
        data.Stage = _stage;
        data.BoltLevel = playerController.weapons.Bolt.level;
        data.SphereLevel = playerController.weapons.Sphere.level;
        data.LaserLevel = playerController.weapons.Laser.level;
        Destroy(_player);

        // Submit data and wait for result
        int id = Database.BUSY_STATE;
        StartCoroutine(Database.SubmitPlayerData(data, value => id = value));
        while (id == Database.BUSY_STATE)
            yield return null;
        
        // Check if it's a highscore
        if (id >= 0)
        {
            // show the form of submitting score
            showSubmitHighscore();
        }
        else
        {
            // set the gameover menu active
            showGameover();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PromptForExtraLife()
    {
        // Freeze time
        Time.timeScale = 0.0f;

        // Show UI
        UiExtraLife.SetActive(true);
    }

    public void ShowAds()
    {
        // Disable Ad prompt
        UiExtraLife.SetActive(false);

        AdDisplayer.ShowAd(HandleAdResult);
    }

    public void HandleAdResult(ShowResult result)
    {
        // Handle results
        switch (result)
        {
            case ShowResult.Finished:
                Time.timeScale = 1.0f;
                _player.GetComponent<PlayerController>().addLife(1);
                _player.GetComponent<PlayerController>().DoRespawn();
                TogglePause();
                break;
            case ShowResult.Skipped:
                DoGameover();
                break;
            case ShowResult.Failed:
                DoGameover();
                break;
        }
    }

    public void SubmitHighscore()
    {
        // Diable submit highscore UI
        UiSubmitHighscore.SetActive(false);

        // Start coroutine to submit
        StartCoroutine(DoSubmitScore());
    }

    public void DoGameover()
    {
        StartCoroutine(gameover());
    }

    public void addScore(int num)
    {
        if (!IsGameover)
        {
            // add to total score (with difficulty bonus)
            _score += (int)(num * _difficultyFactor);

            // update score UI
            updateScoreUI(_score);

            // Enlarge score text
            UiScoreResizer.ChangeScale();
        }
    }

    public void dropExperience(Vector3 center, float radius, int exp)
    {
        // sort the crystals from small to big
        //Array.Sort(expCrystals); // TODO: IComparable

        // random generate the combinations of crystals
        while(exp >= expCrystals[0].experience)
        {
            // find the index of the biggest crystal suitable
            int indexMax = 0;
            for(int i = 0; i < expCrystals.Length; i++)
            {
                if(exp >= expCrystals[i].experience)
                    indexMax = i;
            }

            // random suitable crystal
            ExpCrystal crystal = expCrystals[Random.Range(0, indexMax + 1)];
            exp -= crystal.experience;

            // random position within a circle
            Vector2 posInUnitCircle = Random.insideUnitCircle * radius;
            Vector3 position = center + new Vector3(posInUnitCircle.x, 0.0f, posInUnitCircle.y);

            // instantiate game object
            Instantiate(crystal, position, crystal.transform.rotation);
        }
    }

    public void dropHealing(Vector3 center, float radius)
    {
        // non-uniform random healing drop
        List<int> listWeight = new List<int>();
        foreach (HealCrystal heal in healCrystals)
            listWeight.Add(heal.lootWeight);
        int index = getIndexOfNonUniformRandom(listWeight);

        // random position within a circle
        Vector2 posInUnitCircle = Random.insideUnitCircle;
        Vector3 position = center + new Vector3(posInUnitCircle.x, 0.0f, posInUnitCircle.y);

        // instantiate game object
        HealCrystal crystal = healCrystals[index];
        Instantiate(crystal, position, crystal.transform.rotation);
    }

    public void dropRandomSupply(Vector3 center, float radius)
    {
        // choose between armor, healing and experience
        GameObject obj = objSupplies[Random.Range(0, objSupplies.Length)];

        // random position within a circle
        Vector2 posInUnitCircle = Random.insideUnitCircle;
        Vector3 position = center + new Vector3(posInUnitCircle.x, 0.0f, posInUnitCircle.y);

        // instantiate game object
        Instantiate(obj, position, obj.transform.rotation);
    }

    private IEnumerator spawnWaves()
    {
        // While player not die
        while (true)
        {
            // for each galaxy
            foreach (Stage galaxy in galaxies)
            {
                // Sfx: intro of galaxy
                AudioManager.PlaySfx(AudioManager.Clip_GalaxyIntro);

                // stage display start
                UiTextDisplay.text = "Galaxy " + _stage.ToString();
                UiTextDisplay.gameObject.SetActive(true);
                BgScroller.isBoostEnabled = true;

                // stage display end
                yield return new WaitForSeconds(stageInterval);
                UiTextDisplay.gameObject.SetActive(false);
                BgScroller.isBoostEnabled = false;                

                // spawn waves one by one
                foreach (Wave wave in galaxy.waves)
                {
                    if (wave != null)
                    {
                        // spawn wave
                        GameObject objWave = Instantiate(wave.gameObject);
                        objWave.GetComponent<Wave>().spawn(_difficultyFactor);
                        bool isBossWave = objWave.GetComponent<Wave>().isBoss;

                        // spawn shuttle or not
                        float roll = Random.value;
                        float shuttleDelay = 0.0f;
                        if (roll <= probShuttleSpawnPerWave)
                        {
                            // spawn at random time in wave
                            shuttleDelay = Random.Range(0.0f, wave.duration);
                            yield return new WaitForSeconds(shuttleDelay);
                            Vector3 pos = new Vector3(Random.Range(posXShuttle.min, posXShuttle.max + 1), 0.0f, posZShuttle);
                            Damageable shuttle = ((GameObject)Instantiate(objShuttle, pos, objShuttle.transform.rotation))
                                                    .GetComponent<Damageable>();
                            shuttle.SetDifficulty(_stage);
                        }

                        // wait and destroy
                        if (isBossWave)
                        {
                            // Wait until the boss dies
                            while (objWave)
                                yield return null;
                        }
                        else
                        {
                            yield return new WaitForSeconds(wave.duration - shuttleDelay);
                        }
                    }
                }

                // next atage
                _stage++;
            }

            // Raise the difficulty
            if(_difficultyFactor < 3.0f)
                _difficultyFactor += 0.25f;
        }
    }

    public void TogglePause()
    {
        Time.timeScale = Time.timeScale > 0 ? 0.0f : 1.0f;

        if (Time.timeScale == 1)
        {
            // Hide pause UI
            UiPause.SetActive(false);
        }
        else if (Time.timeScale == 0)
        {
            // Show pause UI
            UiPause.SetActive(true);
        }
    }

    private void updateScoreUI(int newScore)
    {
        // compensate prefix 0s
        string strScore = newScore.ToString();
        while (numScoreDigit - strScore.Length > 0)
            strScore = "0" + strScore;

        // update to GUI
        guiTextScore.text = strScore;
    }

    private IEnumerator DoSubmitScore()
    {
        // Submit highscore
        string name = Regex.Replace(UiInputName.text, @"\s+", " "); // spaces
        name = Regex.Replace(name, @"^\s+", ""); // no leading space
        name = Regex.Replace(name, "-", ""); // no -
        bool isFinished = false;

        if (name != "")
            StartCoroutine(Database.SubmitHighscoreName(name, b => isFinished = b));
        else
        {
            // If the string is empty, skip
            isFinished = true;
        }

        // Wait for finished
        while (!isFinished)
            yield return null;

        // Gameover
        showHighscoreFromGameover();
    }

    private void clearRemainingGameObjects()
    {
        // get all the objects to be cleared
        List<GameObject[]> listObjects = new List<GameObject[]>();
        listObjects.Add(GameObject.FindGameObjectsWithTag("Wave"));
        listObjects.Add(GameObject.FindGameObjectsWithTag("Enemy"));
        listObjects.Add(GameObject.FindGameObjectsWithTag("Powerup"));
        listObjects.Add(GameObject.FindGameObjectsWithTag("Asteroid"));
        listObjects.Add(GameObject.FindGameObjectsWithTag("Bullet"));

        // clear these objects
        foreach (GameObject[] objs in listObjects)
        {
            foreach(GameObject obj in objs)
            {
                Destroy(obj);
            }
        }
    }

    private int getIndexOfNonUniformRandom(List<int> weights)
    {
        // total
        int total = 0;
        foreach (int w in weights)
            total += w;

        // roll a random number and map to the distribution
        int random = Random.Range(1, total + 1);
        int upper = 0;
        for(int i = 0; i < weights.Count; i++)
        {
            upper += weights[i];
            if (random <= upper)
                return i;
        }

        // Error
        Debug.LogError("There is something wrong with the weights.");
        return -1;
    }
}