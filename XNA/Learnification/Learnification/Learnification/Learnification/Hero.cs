using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Learnification
{
	class Hero : Animatible
	{
		public const float RunningSpeed = 3f;

		//public Texture2D Sprite { get; set; }
		public int IsRunning { get; set; }
		public int IsJumping { get; set; }
		public int IsDying { get; set; }
		public float JumpPower { get; set; }
		public Vector2 MaxRight { get; set; }
		public SpriteEffects Direction { get; set; }
		public Point Frame { get; set; }
		public float Speed {
			get
			{
				return RunningSpeed;
			}
		}
	}
}
