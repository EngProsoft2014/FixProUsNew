using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace FixProUs.Controls
{
    public class CustomsPage : ContentPage

    {
        public CustomsPage() 
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public Func<object, object, Task> Action { get; internal set; }
    }
}
