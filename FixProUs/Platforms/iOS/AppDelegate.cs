using Foundation;
using UIKit;

namespace FixProUs
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            // Get the main window
            var window = UIApplication.SharedApplication.KeyWindow;

            if (window != null)
            {
                // Add a tap gesture recognizer to dismiss the keyboard
                var tapRecognizer = new UITapGestureRecognizer(() =>
                {
                    window.EndEditing(true); // Dismiss the keyboard
                })
                {
                    CancelsTouchesInView = false // Ensure other UI interactions are not blocked
                };
                window.AddGestureRecognizer(tapRecognizer);
            }

            return result;
        }
    }
}
