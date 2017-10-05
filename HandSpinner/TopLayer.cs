using CocosSharp;
using System.Diagnostics;

namespace HandSpinner
{
	public class TopLayer : CCLayer
	{
		CCSprite sprite;
		CCPoint center = new CCPoint(50, 70);
		const float scale = 0.25f;
		public TopLayer()
		{
			sprite = new CCSprite("fidgetcenter") { Position = center, Scale = scale };
			this.AddChild(sprite);
		}
	}
}
