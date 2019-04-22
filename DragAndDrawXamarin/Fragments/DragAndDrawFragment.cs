using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace DragAndDrawXamarin.Fragments
{
    public class DragAndDrawFragment : Fragment
    {
        public static DragAndDrawFragment NewInstance()
        {
            return new DragAndDrawFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_drag_and_draw, container, false);

            return view;
        }
    }
}
