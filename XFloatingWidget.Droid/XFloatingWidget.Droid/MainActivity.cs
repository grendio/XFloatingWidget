using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using XFloatingWidget.Droid.Services;

namespace XFloatingWidget.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            StartService(new Intent(this, typeof(FloatingWidgetService)));
            Finish();
        }
    }
}