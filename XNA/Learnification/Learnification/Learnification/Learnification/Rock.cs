using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Learnification
{
	class Rock
	{
		public Texture2D Sprite { get; set; }
		public Point Size { get; set; }
		public SpriteEffects Direction { get; set; }
		public bool IsAirborn { get; set; }
		public int ThrowMultiplier { get; set; }
		public bool IsFalling { get; set; }
	}
}
