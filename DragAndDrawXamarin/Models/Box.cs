using System;
using Android.Graphics;
using Java.IO;

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

        public float Rotation { get; set; }

        public float CenterX => (Origin.X + Current.X) / 2;

        public float CenterY => (Origin.Y + Current.Y) / 2;
    }
}
