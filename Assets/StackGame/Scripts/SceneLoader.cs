using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public enum GameMode { Levels, Endless }
    public GameMode CurrentMode { get; private set; }

    private FadeOverlay fadeOverlay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindFadeOverlay();
        if (fadeOverlay != null)
            fadeOverlay.FadeFromBlack(0.5f);
    }

    public void LoadGameplay(GameMode mode)
    {
        CurrentMode = mode;
        TransitionToScene("Gameplay");
    }

    public void LoadMainMenu()
    {
        TransitionToScene("MainMenu");
    }

    private void TransitionToScene(string sceneName)
    {
        FindFadeOverlay();
        if (fadeOverlay != null)
        {
            fadeOverlay.FadeToBlack(0.4f, () =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void FindFadeOverlay()
    {
        fadeOverlay = FindFirstObjectByType<FadeOverlay>();
    }
}
