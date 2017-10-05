//Author: Steven Seung
//This class handles gaming
//BUGS: catch when fail to spin and not enter into score submission<
//successive follow up spins should add to spin/time not start over<
//debug hit box<*
//change ease for more realistic spinning action* - catch swipe thru not only swipe into as success - would unease animattion look more realistic
//high score problems< used nullable float
//toggle sprite broken on android<* something with the cocossharp scheduler with accessing surface from different threads (i think)?
//background doesnt fill all the way<* broke aspect ratio set on gameview< noborder

using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace HandSpinner
{
	public class GameLayer : CCLayerColor
	{
		//CCSprite sprite;
		Random rng = new Random();
		CCLabel l;
		bool reset { get; set; }
		const string score_key = "HIGHSCORE";
		const double period = 0.05;
		const string zero = "00:00";
		const float pi = 3.14f;
		const float scale = 0.25f;
		const float a = 2;
		int length;
		float total;
		float swipe_elapsed;
		float spin_elapsed;
		bool updateClock;
		bool was_spin;
		public float? score; 
		public event EventHandler RaiseCustomEvent;
		//string[] sprite_names = new string[] { "Fidget1", "Fidget2", "Fidget3" };
		CCSprite[] sprites = new CCSprite[3];
		int spriteIndex = 1;
		CCPoint center = new CCPoint(50, 70);

		float curl;
		bool first_swipe = true;

		public GameLayer()
		{
			Color = new CCColor3B(CCColor4B.AliceBlue);
			Opacity = 255;

			l = new CCLabel(zero, "AzerWeb.ttf", 80);
			l.Scale = scale;
			l.Color = new CCColor3B(CCColor4B.Black);
			l.PositionX = 50;
			l.PositionY = 130;
			AddChild(l);
			reset = true;
			sprites[0] = new CCSprite("Fidget1") { Position = center, Scale = scale };
			sprites[1] = new CCSprite("Fidget2") { Position = center, Scale = scale };
			sprites[2] = new CCSprite("Fidget3") { Position = center, Scale = scale };
			AddChild(sprites[spriteIndex%3]);
			Device.StartTimer(TimeSpan.FromSeconds(period), HandleTime);
			//this.Schedule(HandleSchedule, 0.05f);
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			var touchListener = new CCEventListenerTouchAllAtOnce();
			touchListener.OnTouchesEnded = OnTouchesEnded;
			touchListener.OnTouchesBegan = OnTouchesBegan;
			touchListener.OnTouchesMoved = OnTouchesMoved;
			AddEventListener(touchListener, this);
		}

		void OnTouchesEnded(List<CCTouch> touches, CCEvent e)
		{
			swipe_elapsed = total - swipe_elapsed;
			if (touches.Count > 0 && sprites[spriteIndex%3].BoundingBoxTransformedToParent.ContainsPoint(touches[0].Location) && length > 0)
			{
				HandleSpin(touches[0]);
			}
		}
		void OnTouchesBegan(List<CCTouch> touches, CCEvent e)
		{
			swipe_elapsed = total;
			length = 0;
			if (touches.Count > 0 && sprites[spriteIndex%3].BoundingBoxTransformedToParent.ContainsPoint(touches[0].Location))
			{
				sprites[spriteIndex%3].StopAllActions();
			}
		}
		void OnTouchesMoved(List<CCTouch> touches, CCEvent e)
		{
			if (touches.Count > 0)
			{
				length = 0;
				foreach (CCTouch t in touches)
				{
					if (!sprites[spriteIndex%3].BoundingBoxTransformedToParent.ContainsPoint(t.StartLocation))
					{
						var tmp = (t.Location - t.StartLocation).Length;
						if (tmp > length)
						{
							length = (int)tmp;
						}
					}
					else
					{
						length = 0;
						break;
					}
				}
			}
		}
		void HandleSpin(CCTouch t)
		{
			was_spin = true;
			reset = true;
			int angle;
			//physics
			var v = length / swipe_elapsed;
			var d = (v * v) / (2 * a);
			var theta = 360 * d / (pi * sprites[spriteIndex%3].BoundingBoxTransformedToParent.Size.Height);
			var deltat = (-v - Math.Sqrt((v * v + 4 * a * d))) / (-2 * a);

			CCPoint scenter = sprites[spriteIndex%3].BoundingBoxTransformedToParent.Center;
			CCPoint to = t.StartLocation - scenter;
			CCPoint tf = t.Location - scenter;
			CCPoint P = tf - to;

			var x = CCPoint.CrossProduct(P, tf);

			if (x < 0) 
			{ 
				angle = -5*((int)theta + rng.Next(360)); 
			}
			else 
			{
				angle = 5*((int)theta + rng.Next(360)); 
			}
			if (first_swipe)
			{
				curl = x;
				first_swipe = false;
				spin_elapsed = total;
			}

			CCFiniteTimeAction coreAction = null;
			coreAction = new CCRotateBy((float)deltat, angle);
			CCAction ease = new CCEaseExponentialOut(coreAction);
			//CCAction ease = new CCEaseOut(coreAction, 10f);
			//CCAction ease = new CCEaseRateAction(coreAction, 10f);

			if (curl > 0 && x > 0)
			{
				sprites[spriteIndex%3].AddAction(ease);
			}
			else if (curl < 0 && x < 0)
			{
				sprites[spriteIndex%3].AddAction(ease);
			}
			else 
			{
				sprites[spriteIndex%3].StopAllActions();
				first_swipe = true;
			}

			//CCFiniteTimeAction coreAction = null;
			//coreAction = new CCRotateBy((float)deltat, angle);
			//CCAction ease = new CCEaseExponentialOut(coreAction);
			//sprite.AddAction(ease);
			updateClock = true;
			//spin_elapsed = total;

			//m.Text = string.Format("{0:F2}, {1:F2}, {2:F2}", deltat, sprite.NumberOfRunningActions, angle);
			//solutions to bugs probably needs to occur in this method

		}

		void HandleSchedule(float obj)
		{
			total += obj;
			if (updateClock)
			{
				var time = total - spin_elapsed;
				l.Text = string.Format("{0:F2}", time);
				score = time;
			}
			if (sprites[spriteIndex%3].NumberOfRunningActions == 0 && was_spin )
			{
				if (Application.Current.Properties.ContainsKey(score_key))
				{
					var temp = Application.Current.Properties[score_key] as float?;
					if (temp < score)
					{
						Application.Current.Properties[score_key] = score;
						OnRaiseCustomEvent(new EventArgs());
						//Debug.WriteLine("entered scoring");
					}
				}
				updateClock = false;
				was_spin = false;
				spin_elapsed = total;
				first_swipe = true;
			}
		}

		protected virtual void OnRaiseCustomEvent(EventArgs e)
		{
			EventHandler handler = RaiseCustomEvent;
			if (handler != null)
			{
				// Use the () operator to raise the event.
				handler(this, e);
			}
		}
		public void toggle_sprite()
		{
			RemoveChild(sprites[spriteIndex % 3]);
			spriteIndex++;
			//sprite = new CCSprite(sprite_names[spriteIndex % 3]) { Position = center, Scale = scale };
            AddChild(sprites[spriteIndex%3]);
		}

		private void ResetAll()
		{
			reset = false; swipe_elapsed = total; spin_elapsed = total; first_swipe = true; length = 0; l.Text = zero; sprites[spriteIndex%3].StopAllActions();
		}
		public void test()
		{
			//Debug.WriteLine(Application.Current.Properties.ContainsKey("score_key"));
			//Debug.WriteLine(Application.Current.Properties["score_key"]);
			Debug.WriteLine(sprites[spriteIndex%3].NumberOfRunningActions);
		}
		bool HandleTime()
		{
			total += (float)period;
			if (updateClock)
			{
				var time = total - spin_elapsed;
				l.Text = string.Format("{0:F2}", time);
				score = time;
			}
			if (sprites[spriteIndex%3].NumberOfRunningActions == 0 && was_spin)
			{
				if (Application.Current.Properties.ContainsKey(score_key))
				{
					var temp = Application.Current.Properties[score_key] as float?;
					if (temp < score)
					{
						Application.Current.Properties[score_key] = score;
						OnRaiseCustomEvent(new EventArgs());
						//Debug.WriteLine("entered scoring");
					}
				}
				updateClock = false;
				was_spin = false;
				spin_elapsed = total;
				first_swipe = true;
			}
			if (!reset) { return false; }
			else { return true; }
		}
	}
}
