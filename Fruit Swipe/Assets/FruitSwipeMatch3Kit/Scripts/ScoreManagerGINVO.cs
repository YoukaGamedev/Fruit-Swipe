using UnityEngine;
using System.Runtime.InteropServices;

public class ScoreManagerGINVO : MonoBehaviour
{
    // Singleton instance
    public static ScoreManagerGINVO Instance { get; private set; }

    // WebGL JS interop
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SubmitScore(int score);

    [DllImport("__Internal")]
    private static extern void ShowLeaderboardModal();
#endif

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this object between scenes
    }

    /// <summary>
    /// Submit player score to JS
    /// </summary>
    public void SubmitPlayerScore(int score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SubmitScore(score);
#else
        Debug.Log($"SubmitScore called with value: {score} (Editor)");
#endif
    }

    /// <summary>
    /// Show leaderboard modal from JS
    /// </summary>
    public void ShowLeaderboard()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowLeaderboardModal();
#else
        Debug.Log("ShowLeaderboardModal called (Editor)");
#endif
    }
}