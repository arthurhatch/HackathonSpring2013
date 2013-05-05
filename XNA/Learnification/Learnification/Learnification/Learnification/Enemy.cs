using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Learnification
{
	class Enemy : Animatible
	{
        public Enemy()
        {
            FramePositions = new AnimatibleFramePositions[1];

            //Moving
            FramePositions[0] = new AnimatibleFramePositions();
            FramePositions[0].Points = new Point[8];
                FramePositions[0].Points[0] = new Point(0, 0);
                FramePositions[0].Points[1] = new Point(1, 0);
                FramePositions[0].Points[2] = new Point(2, 0);
                FramePositions[0].Points[3] = new Point(3, 0);
                FramePositions[0].Points[4] = new Point(0, 1);
                FramePositions[0].Points[5] = new Point(1, 1);
                FramePositions[0].Points[6] = new Point(2, 1);
                FramePositions[0].Points[7] = new Point(3, 1);
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
            Health = (Health <= -12) ? 0 : Health - 12;
            IsMoving = 0;
            Frame = new Point(0, 2);
        }
	}
}
