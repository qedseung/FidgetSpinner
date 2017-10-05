//Author: Steven Seung
//Thanks to Nicholas Ventimiglia for the only PCL compliant Websockets Package :)
//This class basically starts Everything

using System;
using System.Diagnostics;
using CocosSharp;
using Xamarin.Forms;

namespace HandSpinner
{
	public partial class HandSpinnerPage : ContentPage
	{
		GameLayer layer;
		CCLayer top;
		const string score_key = "HIGHSCORE";
		public HandSpinnerPage()
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			var gameview = new CocosSharpView() {
				// Notice it has the same properties as other XamarinForms Views
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				// This gets called after CocosSharp starts up:
				ViewCreated = HandleViewCreated
			};

			layout.Children.Add(gameview);
			var buttons = new StackLayout() { 
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};
			Button lb, buy, toggle;
			lb = new Button() { Text = "High Score" };
			buy = new Button() { Text = "Buy" };
			toggle = new Button() { Text = "Toggle" };
			lb.Clicked += Lb_Clicked;
			toggle.Clicked += toggle_Clicked;
			buy.Clicked += debug;

			//Button reset = new Button() { TextColor = Color.Red, Text = "Reset" };
			//reset.Clicked += Reset_Clicked;
			//buttons.Children.Add(reset);
			buttons.Children.Add(lb);
			buttons.Children.Add(buy);
			buttons.Children.Add(toggle);
			layout.Children.Add(buttons);
		}
		void HandleViewCreated(object sender, EventArgs e)
		{
			var gameview = sender as CCGameView;
			if (gameview != null)
			{
				// This sets the game "world" resolution to 100x150 (X,Y):
				gameview.DesignResolution = new CCSizeI(100, 150);
				gameview.ResolutionPolicy = CCViewResolutionPolicy.NoBorder;
				// GameScene is the root of the CocosSharp rendering hierarchy:
				var gamescene = new CCScene(gameview); //GameScene(gameview);
				layer = new GameLayer();
				top = new TopLayer();
				layer.RaiseCustomEvent += Layer_RaiseCustomEvent;
				gamescene.AddLayer(layer);
				gamescene.AddLayer(top);
				// Starts CocosSharp:
				gameview.RunWithScene(gamescene);
			}
		}

		void toggle_Clicked(object sender, EventArgs e)
		{
			layer.toggle_sprite();
		}

		void debug(object sender, EventArgs e)
		{
			//layer.test();
			Device.OpenUri(new Uri("https://www.google.com"));
		}

		void Lb_Clicked(object sender, EventArgs e)
		{
			//await Navigation.PushAsync(new LeadBoardPage());
			DisplayAlert("HIGH SCORE",Application.Current.Properties[score_key].ToString(),"Ok");
		}

		void Layer_RaiseCustomEvent(object sender, EventArgs e)
		{
			//await Navigation.PushModalAsync(new NameModal(layer.score), true);
			DisplayAlert("HIGH SCORE", Application.Current.Properties[score_key].ToString(),"Ok");
		}
	}
}
