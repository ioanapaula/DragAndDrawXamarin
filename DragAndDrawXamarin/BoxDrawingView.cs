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

        private void Initialize()
        {
            _boxPaint = new Paint();
            _backgroundPaint = new Paint();
            _boxPaint.Color = new Color(0x22ff0000);
            _backgroundPaint.Color = new Color(0xf8efe0);
        }
    }
}
