using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DragAndDrawXamarin.Models;

namespace DragAndDrawXamarin
{
    public class BoxDrawingView : View
    {
        private new const string Tag = "BoxDrawingView";
        private const string SavedInstanceStateKey = "InstanceStateKey";
        private const string SavedBoxesKey = "SavedBoxesKey";

        private Box _currentBox;
        private List<Box> _boxes = new List<Box>();
        private Paint _boxPaint;
        private Paint _backgroundPaint;

        public BoxDrawingView(Context context) : this(context, null)
        {
            Initialize();
        }

        public BoxDrawingView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var current = new PointF(e.GetX(), e.GetY());
            string action = string.Empty;

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    action = "ActionDown";
                    _currentBox = new Box(current);
                    _boxes.Add(_currentBox);

                    break;
                case MotionEventActions.Move:
                    action = "ActionMove";

                    if (_currentBox != null)
                    {
                        _currentBox.Current = current;
                        Invalidate();
                    }

                    break;
                case MotionEventActions.Up:
                    action = "ActionUp";
                    _currentBox = null;

                    break;
                case MotionEventActions.Cancel:
                    action = "ActionCancel";
                    _currentBox = null;

                    break;
            }

            Log.Info(Tag, $"{action} at x = {current.X}, y = {current.Y}");

            return true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.DrawPaint(_backgroundPaint);

            foreach (Box box in _boxes)
            {
                var left = Math.Min(box.Origin.X, box.Current.X);
                var right = Math.Max(box.Origin.X, box.Current.X);
                var top = Math.Max(box.Origin.Y, box.Current.Y);
                var bottom = Math.Min(box.Origin.Y, box.Current.Y);

                canvas.DrawRect(left, top, right, bottom, _boxPaint);
            }
        }

        protected override IParcelable OnSaveInstanceState()
        {
            Bundle state = new Bundle();
            state.PutParcelable(SavedInstanceStateKey, base.OnSaveInstanceState());
            state.PutFloatArray(SavedBoxesKey, ConvertToArray(_boxes));

            return state;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            var restoredState = (Bundle)state;
            base.OnRestoreInstanceState((IParcelable)restoredState.GetParcelable(SavedInstanceStateKey));

            var boxCoordinates = restoredState.GetFloatArray(SavedBoxesKey);

            for (int i = 0; i < boxCoordinates.Length / 4; i++)
            {
                var origin = new PointF(boxCoordinates[i * 4], boxCoordinates[(i * 4) + 1]);
                var current = new PointF(boxCoordinates[(i * 4) + 2], boxCoordinates[(i * 4) + 3]);
                var box = new Box(origin)
                {
                    Current = current
                };
                _boxes.Add(box);
            }
        }

        private void Initialize()
        {
            _boxPaint = new Paint();
            _backgroundPaint = new Paint();
            _boxPaint.Color = new Color(0x22ff0000);
            _backgroundPaint.Color = new Color(0xf8efe0);
        }

        private float[] ConvertToArray(List<Box> boxes)
        {
            List<float> boxCoordinates = new List<float>();

            foreach (Box box in boxes)
            {
                boxCoordinates.Add(box.Origin.X);
                boxCoordinates.Add(box.Origin.Y);
                boxCoordinates.Add(box.Current.X);
                boxCoordinates.Add(box.Current.Y);
            }

            return boxCoordinates.ToArray();
        }
    }
}
