using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace Sticky_Notes
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ListActivity
    {
        const int CREATE_BASE_NOTE = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //SetContentView(Resource.Layout.activity_main);
            String[] items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, items);

            ListView.TextFilterEnabled = true;

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            //FloatingActionButton fabAddNoteMenu = FindViewById<FloatingActionButton>(Resource.Id.fabAddNoteMenu);
            //fabAddNoteMenu.Click += OpenActionMenu_Click;

            FloatingActionButton fabCreateNote = FindViewById<FloatingActionButton>(Resource.Id.fabCreateNote);
            fabCreateNote.Click += delegate 
            {
                var intent = new Intent(this, typeof(CreateActivity));
                StartActivityForResult(intent, CREATE_BASE_NOTE);
            };

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == CREATE_BASE_NOTE)
            {
                if (resultCode == Result.Ok)
                {
                    ArrayAdapter<TextView> textViewAdapter = new ArrayAdapter<TextView>(this, Resource.Layout.content_main);
                    //ListView lvContentMain = FindViewById<ListView>(Resource.Id.lvNoteList);
                    TextView NewText = new TextView(this);
                    NewText.Text = data.GetStringExtra("NewNote");
                    textViewAdapter.Add(NewText);
                    textViewAdapter.NotifyDataSetChanged();
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void OpenActionMenu_Click(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Flyout Menu - Note Templates", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }


}

}