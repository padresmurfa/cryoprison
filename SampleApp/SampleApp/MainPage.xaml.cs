﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SampleApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var msg = App.GetJailBreakAlertMessage();
            if (msg != null)
            {
                this.DisplayAlert("Jailbreak Detected!", "The following jailbreaks were detected: " + msg, "Close");
            }
        }
    }
}
