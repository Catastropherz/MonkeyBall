using UnityEngine;
using System.Collections;
using Unity.Notifications.Android; 

public class NotificationManager : MonoBehaviour
{
    // The ID for the Android notification channel
    private const string AndroidChannelId = "default_channel";
    private const int AchievementNotificationId = 1;

    // Singleton instance
    public static NotificationManager instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            // destroy this
            Destroy(gameObject);
        }


        //Set to not destroy on load
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // Start the routine to handle setup and scheduling
        StartCoroutine(InitializeNotifications());
        // Check for launch intent, in case the app was opened via a notification
        CheckLaunchIntent();
    }

    // Checks if the application was launched by tapping a notification
    private void CheckLaunchIntent()
    {
        // Check if the app was launched by tapping a notification
        var notificationIntent = AndroidNotificationCenter.GetLastNotificationIntent();

        if (notificationIntent != null)
        {
            var data = notificationIntent.Notification.IntentData;
            Debug.Log("App launched from notification with data: " + data);

            AndroidNotificationCenter.CancelNotification(notificationIntent.Id);
        }
    }

    private IEnumerator InitializeNotifications()
    {
        InitializeAndroidChannel();

        Debug.Log("Android Notification setup complete.");

        yield return null; 
    }

    private void InitializeAndroidChannel()
    {
        // Create the channel object
        var channel = new AndroidNotificationChannel()
        {
            Id = AndroidChannelId,
            Name = "New Achievement",
            Importance = Importance.Default,
            Description = "New Achievement unlocked!"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        Debug.Log("Android Notification Channel registered.");
    }

    public void ScheduleAchievementNotification()
    {
        // Cancel any previous notification
        AndroidNotificationCenter.CancelNotification(AchievementNotificationId);

        var title = "New Achievement";
        var body = "New Achievement unlocked!";
        var intentData = "app_launch_from_notification";

        var notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = System.DateTime.Now.AddSeconds(1), // 2 seconds from now
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            IntentData = intentData
        };

        // Send the notification using the channel ID defined above.
        AndroidNotificationCenter.SendNotification(notification, AndroidChannelId);
        Debug.Log($"Android Notification in 2 seconds. ID: {AchievementNotificationId}");
    }
}