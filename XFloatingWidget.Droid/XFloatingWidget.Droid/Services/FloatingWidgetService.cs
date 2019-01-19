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
        private WindowManagerLayoutParams closeLayoutParams;
        private View floatingView;
        private View closeFloatingView;

        private int initialX;
        private int initialY;
        private float initialTouchX;
        private float initialTouchY;

        private int centerX;
        private int centerY;

        private double[] closeCoords = { 0, 0 };
        private double[] floatingCoords = { 0, 0 };

        public override IBinder OnBind(Intent intent)
        {
            throw new System.NotImplementedException();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            floatingView = LayoutInflater.From(this).Inflate(Resource.Layout.floatingWidget,null);
            closeFloatingView = LayoutInflater.From(this).Inflate(Resource.Layout.closeWidget, null);
            closeFloatingView.Visibility = ViewStates.Invisible;

            SetTouchListener();

            closeLayoutParams = new WindowManagerLayoutParams(ViewGroup.LayoutParams.WrapContent,
                                 ViewGroup.LayoutParams.WrapContent,
                                 WindowManagerTypes.ApplicationOverlay,
                                 WindowManagerFlags.NotFocusable,
                                 Format.Translucent)
            {
                Gravity = GravityFlags.Center,
            };

            windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            windowManager.AddView(closeFloatingView, closeLayoutParams);

            layoutParams = new WindowManagerLayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                         ViewGroup.LayoutParams.WrapContent,
                                                         WindowManagerTypes.ApplicationOverlay,
                                                         WindowManagerFlags.NotFocusable,
                                                         Format.Translucent)
            {
                Gravity = GravityFlags.Left | GravityFlags.Top
            };

            windowManager.AddView(floatingView, layoutParams);

            SetCenter();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if(floatingView != null)
            {
                windowManager.RemoveViewImmediate(floatingView);
                windowManager.RemoveViewImmediate(closeFloatingView);
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Up:
                    closeFloatingView.Visibility = ViewStates.Gone;

                    initialX = layoutParams.X;
                    initialY = layoutParams.Y;

                    if (initialX + v.Width / 2 <= centerX + 100 && initialX + v.Width / 2 >= centerX - 100)
                        if (initialY + v.Height / 2 <= centerY + 100 && initialY + v.Height / 2 >= centerY - 100)
                        {
                            floatingView.Visibility = ViewStates.Gone;
                            StopSelf();
                        }
                    return true;

                case MotionEventActions.Down:
                    initialX = layoutParams.X;
                    initialY = layoutParams.Y;

                    initialTouchX = e.RawX;
                    initialTouchY = e.RawY;
                    return true;

                case MotionEventActions.Move:
                    layoutParams.X = initialX + (int)(e.RawX - initialTouchX);
                    layoutParams.Y = initialY + (int)(e.RawY - initialTouchY);

                    closeFloatingView.Visibility = ViewStates.Visible;
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

        private void SetCenter()
        {
            var display = windowManager.DefaultDisplay;
            var size = new Point();
            display.GetSize(size);

            centerX = size.X / 2;
            centerY = size.Y / 2;
        }
    }
}