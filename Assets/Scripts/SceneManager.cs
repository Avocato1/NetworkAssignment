using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public bool levelChanged = false;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelChanged = false;
    }

    public void LoadLevel(string levelName)
    {
        var level = SceneManager.LoadSceneAsync(levelName);
        level.allowSceneActivation = true;
        levelChanged = true;
    }


    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}