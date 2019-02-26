using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Sticky_Notes
{
    [Activity(Label = "CreateActivity")]
    public class CreateActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.create_note);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tb_create);
            Button btnSave =  FindViewById<Button>(Resource.Id.btn_save);
            Button btnClear = FindViewById<Button>(Resource.Id.btn_clr);
           

            btnSave.Click += BtnSave_Click;
            btnClear.Click += BtnClear_Click;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {

            View view = (View)sender;
            EditText txtNote = FindViewById<EditText>(Resource.Id.txtCreateNote);

            txtNote.Text = string.Empty;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            EditText txtNote = FindViewById<EditText>(Resource.Id.txtCreateNote);
            Intent.PutExtra("NewNote", txtNote.Text);
            SetResult(Result.Ok, Intent);
            Finish();
        }
    }
}