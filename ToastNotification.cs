using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Windows;
using ToastNotifications.Core;

namespace LibDefender
{
    public static class NotificationManager
    {
        public static void Initialize()
        {
            Notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 20,
                    offsetY: 20);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(10));

                cfg.DisplayOptions.Width = 400;

                 

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public static Notifier? Notifier { get; private set; }
    }

}
