using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Learnification
{
	class Enemy : Animatible
	{
        public Enemy()
        {
            FramePositions = new AnimatibleFramePositions[1];
            FramePositions[0] = new AnimatibleFramePositions{ Points = new Point[8] };
            FramePositions[0].GeneratePoints(4, 2);
        }
        
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

        public void Walk()
        {
            Animate(FramePositions[0]);
        }

        public void Die()
        {
            Health = (Health <= 12) ? 0 : Health - 12;
            IsMoving = 0;
            Frame = new Point(0, 2);
        }
	}
}
