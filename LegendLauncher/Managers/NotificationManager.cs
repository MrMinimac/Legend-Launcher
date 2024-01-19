using Microsoft.Toolkit.Uwp.Notifications;

namespace LegendLauncher.Managers
{
    public class NotificationManager
    {
        public static void Notify(string line1, string line2)
        {
            var Notify = new ToastContentBuilder();
            Notify.AddText(line1);
            Notify.AddText(line2);
            Notify.Show();
        }
    }
}
