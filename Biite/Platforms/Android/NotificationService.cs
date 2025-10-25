using Android.App;
using Android.Content;
using Android.Runtime;
using AndroidX.Core.App;

namespace Biite.Services.PartialMethods
{
    static partial class NotificationService
    {
        private static Context Context;
        public const string CHANNEL_ID = "biite_events";

        static NotificationService()
        {
            // This constructor runs before the first notification call
            // It creates the notification channel for Android
            Context = Platform.CurrentActivity.ApplicationContext;
            var channelName = "Biite Event Reminders";
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default);

            NotificationManager notificationManager =
                Context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.CreateNotificationChannel(channel);
        }

        static partial void DoSendNotification(string title, string message, DateTime scheduleTime)
        {
            // Get the device's alarm manager
            AlarmManager alarmManager =
                Context.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();

            int id = (int)DateTime.Now.Ticks; // Unique ID for each notification

            var alarmIntent = new Intent(Context, typeof(AlarmReceiver));

            // Add notification data
            alarmIntent.PutExtra("id", id);
            alarmIntent.PutExtra("title", title);
            alarmIntent.PutExtra("message", message);

            
            DateTimeOffset dateOffsetValue = DateTimeOffset.Parse(scheduleTime.ToString());
            long millisecondsToBegin = dateOffsetValue.ToUnixTimeMilliseconds();

            PendingIntent pending = PendingIntent.GetBroadcast(
                Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            // Schedule the alarm
            alarmManager.SetExact(AlarmType.RtcWakeup, millisecondsToBegin, pending);
        }

    }

        // AlarmReceiver - handles the notification when it triggers
        [BroadcastReceiver]
        public class AlarmReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                // Get notification data
                var title = intent.GetStringExtra("title");
                var message = intent.GetStringExtra("message");
                var idString = intent.GetStringExtra("id");
                var id = Convert.ToInt32(idString);

                // Intent to launch app when notification is tapped
                Intent resultIntent = new Intent(context, typeof(MainActivity));
                resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                resultIntent.PutExtra("notification_type", "event_reminder");

                const int pendingIntentId = 0;
                PendingIntent pendingIntent = PendingIntent.GetActivity(
                    context, pendingIntentId, resultIntent,
                    PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

                    var color = new Color(255, 87, 34).ToInt(); // Orange color for Biite

            // Build the notification
            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, NotificationService.CHANNEL_ID)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetDefaults((int)(NotificationDefaults.Sound | NotificationDefaults.Vibrate))
                    .SetSmallIcon(Resource.Drawable.biite_icon) // 
                    .SetColor(color)
                    .SetContentIntent(pendingIntent)
                    .SetAutoCancel(true) // Dismiss when tapped
                    .SetPriority((int)NotificationPriority.High);

                NotificationManager notificationManager =
                    context.GetSystemService(Context.NotificationService) as NotificationManager;

                Notification notification = builder.Build();
                notificationManager.Notify(id, notification);
            }
        }
    }
