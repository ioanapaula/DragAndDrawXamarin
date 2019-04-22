using System;
using Android.Graphics;

namespace DragAndDrawXamarin.Models
{
    public class Box
    {
        public Box(PointF origin)
        {
            Origin = origin;
            Current = origin;
        }

        public PointF Origin { get; }

        public PointF Current { get; set; }
    }
}
