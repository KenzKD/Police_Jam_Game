using System;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class Video
{
    public string name;
    [SerializeField] public string videoName;
}

public class VideoManager : MonoBehaviour
{
    // Singleton instance for easy access
    public static VideoManager Instance;

    // Array of available videos
    public Video[] videos;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Find video by name and play it if found
    public void PlayVideo(string name)
    {
        Video v = Array.Find(videos, x => x.name == name);
        if (v == null)
        {
            Debug.Log($"Video '{name}' not found!");
        }
        else
        {
            // Play video using VideoPlayer component
            if (TryGetComponent<VideoPlayer>(out var player))
            {
                // Get video path from streaming assets
                string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, v.videoName);
                Debug.Log($"Playing video: {videoPath}");
                player.url = videoPath;
                player.Play();
            }
        }
    }
}
