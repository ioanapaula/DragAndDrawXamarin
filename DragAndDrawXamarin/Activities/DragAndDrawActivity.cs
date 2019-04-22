using Android.App;
using DragAndDrawXamarin.Fragments;

namespace DragAndDrawXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class DragAndDrawActivity : SingleFragmentActivity
    {
        protected override Android.Support.V4.App.Fragment CreateFragment()
        {
            return DragAndDrawFragment.NewInstance();
        }
    }
}