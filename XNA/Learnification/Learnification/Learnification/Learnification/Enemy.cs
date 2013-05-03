using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Learnification
{
	class Enemy
	{
		public Texture2D Sprite { get; set; }
		public SpriteEffects Direction { get; set; }
		public Vector2 MaxRight { get; set; }
		public Point Size { get; set; }
		public Point SheetSize { get; set; }
		public int IsMoving { get; set; }
		public int DeadCount { get; set; }
		public int Health { get; set; }
		public float Speed { get; set; }
		public float MaxSpeed { get; set; }
		public bool ChaseSmart { get; set; }
	}
}
