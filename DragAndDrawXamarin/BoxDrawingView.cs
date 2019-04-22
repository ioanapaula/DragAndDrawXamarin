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
        private const int InvalidPointerId = -1;

        private Box _currentBox;
        private List<Box> _boxes = new List<Box>();
        private Paint _boxPaint;
        private Paint _backgroundPaint;
        private int _ptrId1 = InvalidPointerId;
        private int _ptrId2 = InvalidPointerId;
        private PointF _initialPoint1;
        private PointF _initialPoint2;

        public BoxDrawingView(Context context) : this(context, null)
        {
            Initialize();
        }

        public BoxDrawingView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(attrs);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var current = new PointF(e.GetX(), e.GetY());
            string action = string.Empty;

            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    action = "ActionDown";
                    _currentBox = new Box(current);
                    _boxes.Add(_currentBox);
                    _ptrId1 = e.GetPointerId(e.ActionIndex);

                    break;
                case MotionEventActions.PointerDown:
                    _ptrId2 = e.GetPointerId(e.ActionIndex);
                    _initialPoint1 = new PointF(e.GetX(e.FindPointerIndex(_ptrId1)), e.GetY(e.FindPointerIndex(_ptrId1)));
                    _initialPoint2 = new PointF(e.GetX(e.FindPointerIndex(_ptrId2)), e.GetY(e.FindPointerIndex(_ptrId2)));

                    break;
                case MotionEventActions.Move:
                    action = "ActionMove";

                    if (_currentBox != null)
                    {
                        _currentBox.Current = current;
                        Invalidate();
                    }

                    if (_ptrId2 != InvalidPointerId)
                    {
                        var currentPoint1 = new PointF(e.GetX(e.FindPointerIndex(_ptrId1)), e.GetY(e.FindPointerIndex(_ptrId1)));
                        var currentPoint2 = new PointF(e.GetX(e.FindPointerIndex(_ptrId2)), e.GetY(e.FindPointerIndex(_ptrId2)));
                        _currentBox.Rotation = AngleBetweenLines(currentPoint1, currentPoint2);
                    }
                    else if (_currentBox != null)
                    {
                        _currentBox.Current = current;
                        Invalidate();
                    }

                    break;
                case MotionEventActions.Up:
                    action = "ActionUp";
                    _currentBox = null;

                    break;
                case MotionEventActions.PointerUp:
                    _ptrId2 = InvalidPointerId;

                    break;
                case MotionEventActions.Cancel:
                    action = "ActionCancel";
                    _currentBox = null;
                    _ptrId1 = InvalidPointerId;
                    _ptrId2 = InvalidPointerId;

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

                canvas.Save();
                canvas.Rotate(box.Rotation, box.CenterX, box.CenterY);
                canvas.DrawRect(left, top, right, bottom, _boxPaint);
                canvas.Restore();
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

        private void Initialize(IAttributeSet attrs = null)
        {
            _boxPaint = new Paint();

            if (attrs != null)
            {
                var typedArray = Context.ObtainStyledAttributes(attrs, Resource.Styleable.BoxDrawingView);
                var boxColor = typedArray.GetColor(Resource.Styleable.BoxDrawingView_box_color, Color.Aquamarine);
                _boxPaint.Color = boxColor;
            }
            else
            {
                _boxPaint.Color = new Color(0x22ff0000);
            }

            _backgroundPaint = new Paint();
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

        private float AngleBetweenLines(PointF currentPoint1, PointF currentPoint2)
        {
            float angle1 = (float)Math.Atan2(_initialPoint2.Y - _initialPoint1.Y, _initialPoint2.X - _initialPoint1.X);
            float angle2 = (float)Math.Atan2(currentPoint2.Y - currentPoint1.Y, currentPoint2.X - currentPoint1.X);

            float angle = ((float)Java.Lang.Math.ToDegrees(angle1 - angle2)) % 360;

            if (angle < -180f)
            {
                angle += 360.0f;
            }

            if (angle > 180f)
            {
                angle -= 360.0f;
            }

            return -angle;
        }
    }
}
