using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static float volume = 0.5f;

    public List<GameObject> targetPrefabs;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI gameOverText;
    public GameObject titleScreenObj;
    public GameObject gameStatsScreenObj;
    public GameObject pauseScreenObj;
    public GameObject gameOverScreenObj;
    public Slider volumeSlider;
    public Button pauseButton;
    public bool isGameActive = false;
    public bool isGamePaused = false;
    private GameObject mainCameraObj;
    private GameObject cursorTrailObj;
    private int score;
    private int lives;
    private float spawnRate;

    void Start()
    {
        mainCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        cursorTrailObj = GameObject.Find("Cursor Trail");
        volumeSlider.value = volume;
    }

    private void Update()
    {
        if (!isGameActive || isGamePaused)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mousePos = mainCameraObj.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            cursorTrailObj.transform.position = new Vector3(mousePos.x, mousePos.y);
        }

        TrailRenderer cursorTrail = cursorTrailObj.GetComponent<TrailRenderer>();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            cursorTrail.Clear();
            cursorTrail.emitting = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            cursorTrail.emitting = false;
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        mainCameraObj.GetComponent<AudioSource>().volume = newVolume;
    }

    public void StartGame(float newSpawnRate)
    {
        spawnRate = newSpawnRate;
        isGameActive = true;
        score = 0;
        lives = 3;
        gameStatsScreenObj.SetActive(true);
        titleScreenObj.SetActive(false);
        AddScore(0);
        SubtractLives(0);
        StartCoroutine(SpawnTarget());
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int targetIndex = Random.Range(0, targetPrefabs.Count);
            GameObject targetObj = targetPrefabs[targetIndex];
            Instantiate(targetObj);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public void SubtractLives(int amount)
    {
        lives -= amount;
        livesText.text = "Lives: " + lives;
        if (lives <= 0)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        isGameActive = false;
        gameOverScreenObj.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreenObj.SetActive(true);
        isGamePaused = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        pauseScreenObj.SetActive(false);
        isGamePaused = false;
    }
}
