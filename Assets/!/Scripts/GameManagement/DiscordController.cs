using System;
using Discord;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    public long applicationID;
    [Space]
    public string details = "Made by CTRL games with love <3";
    public string state = "Current dimension: ";
    [Space]
    public string largeImage = "game_logo";
    public string largeText = "Under Control";

    private long time;

    private static bool instanceExists;
    public Discord.Discord discord;

    void Awake() 
    {
        // Transition the GameObject between scenes, destroy any duplicates
        if (!instanceExists)
        {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsByType(GetType(), FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
        }
        
    }

    void Start()
    {
        // Log in with the Application ID
        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        EventBus.SceneReadyEvent.AddListener(UpdateStatus);
        UpdateStatus();
    }

    void Update()
    {
        // Destroy the GameObject if Discord isn't running
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            Destroy(gameObject);
        }
    }


    void UpdateStatus()
    {
        Debug.Log("Updating Discord Status...");
        try
        {
            ActivityManager activityManager = discord.GetActivityManager();
            Activity activity = new()
            {
                State = state + GameManager.Instance.CurrentDimension.ToString(),
                Details = details,
                Assets = new ActivityAssets()
                {
                    LargeImage = largeImage,
                    LargeText = largeText
                },
                Timestamps = new ActivityTimestamps()
                {
                    Start = time
                }
            };
            activityManager.UpdateActivity(activity, (result) =>
            {
                if (result != Result.Ok) Debug.Log("RPC Error: " + result.ToString());
            });
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            // If updating the status fails, Destroy the GameObject
            Destroy(gameObject);
        }
    }
}
