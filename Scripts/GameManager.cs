using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private List<string> allLevels = new List<string> { "Level 1", "Level 2", "Level 3", "Level 4", "Level 5" };
    private List<string> remainingLevels;
    private int levelsCleared = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            remainingLevels = new List<string>(allLevels);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadRandomLevel()
    {
        if (remainingLevels.Count == 0)
        {
            Debug.Log("All levels cleared. Game Over.");
            // 게임 종료 처리 로직 추가
            return;
        }

        int randomIndex = Random.Range(0, remainingLevels.Count);
        string selectedLevel = remainingLevels[randomIndex];
        remainingLevels.RemoveAt(randomIndex);
        levelsCleared++;

        SceneManager.LoadScene(selectedLevel);
    }

    public void OnLevelCleared()
    {
        if (levelsCleared >= 3)
        {
            Debug.Log("Three levels cleared. Game Over.");
            // 게임 종료 처리 로직 추가
        }
        else
        {
            LoadRandomLevel();
        }
    }
}