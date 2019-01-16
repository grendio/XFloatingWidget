using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Views.View;

namespace XFloatingWidget.Droid.Services
{
    [Service]
    public class FloatingWidgetService : Service, IOnTouchListener
    {
        private IWindowManager windowManager;
        private WindowManagerLayoutParams layoutParams;
        private View floatingView;

        private int initialX;
        private int initialY;
        private float initialTouchX;
        private float initialTouchY;

        public override IBinder OnBind(Intent intent)
        {
            throw new System.NotImplementedException();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            floatingView = LayoutInflater.From(this).Inflate(Resource.Layout.floatingWidget,null);
            SetTouchListener();

            layoutParams = new WindowManagerLayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                         ViewGroup.LayoutParams.WrapContent,
                                                         WindowManagerTypes.Phone,
                                                         WindowManagerFlags.NotFocusable,
                                                         Format.Translucent)
            {
                Gravity = GravityFlags.Left | GravityFlags.Top
            };

            windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            windowManager.AddView(floatingView, layoutParams);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if(floatingView != null)
            {
                windowManager.RemoveView(floatingView);
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    initialX = layoutParams.X;
                    initialY = layoutParams.Y;

                    initialTouchX = e.RawX;
                    initialTouchY = e.RawY;
                    return true;

                case MotionEventActions.Move:
                    layoutParams.X = initialX + (int)(e.RawX - initialTouchX);
                    layoutParams.Y = initialY + (int)(e.RawY - initialTouchY);

                    windowManager.UpdateViewLayout(floatingView, layoutParams);
                    return true;
            }

            return false;
        }

        private void SetTouchListener()
        {
            var rootContainer = floatingView.FindViewById<RelativeLayout>(Resource.Id.root);
            rootContainer.SetOnTouchListener(this);
        }
    }
}