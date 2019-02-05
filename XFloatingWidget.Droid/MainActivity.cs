using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using XFloatingWidget.Droid.Services;
using Android.Provider;

namespace XFloatingWidget.Droid
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int ACTION_MANAGE_OVERLAY_PERMISSION_REQUEST_CODE = 5469;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_main);

            AskForPermission();
        }

         void AskForPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var intent = new Intent("android.settings.action.MANAGE_OVERLAY_PERMISSION");
                intent.SetData(Uri.Parse("package:" + PackageName.ToString()));

                StartActivityForResult(intent, ACTION_MANAGE_OVERLAY_PERMISSION_REQUEST_CODE);
            }
            
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == ACTION_MANAGE_OVERLAY_PERMISSION_REQUEST_CODE)
            {
                if (!Settings.CanDrawOverlays(this))
                {
                    AskForPermission();
                }
                else
                {
                    StartService(new Intent(this, typeof(FloatingWidgetService)));
                    Finish();
                }
            }
        }
    }
}