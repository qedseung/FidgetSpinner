using System;
using CocosSharp;

namespace HandSpinner
{
	public class GameScene : CCScene
	{
		GameLayer layer;
		public GameScene(CCGameView gameview) : base(gameview)
		{
			layer = new GameLayer(); //CCLayerColor();
			this.AddLayer(layer);
		}
	}
}
