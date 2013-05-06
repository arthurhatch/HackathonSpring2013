using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Learnification
{
	class Enemy : Animatible
	{
        public Enemy()
        {
            ActionSequences = new ActionSequence[1];
            ActionSequences[0] = new ActionSequence{ Frames = new Point[8] };
            ActionSequences[0].GenerateFrames(4, 2);

            Direction = SpriteEffects.None;
			MaxRight = Vector2.Zero;
			Size = new Point(45, 52);
			SheetSize = new Point(4, 3);
			IsMoving = 1;
			Lives = 9;
			Speed = 5f;
			MaxSpeed = 20f;
			ChaseSmart = false;
        }
        
        public SpriteEffects Direction { get; set; }
		public Vector2 MaxRight { get; set; }
		public Point Size { get; set; }
		public Point SheetSize { get; set; }
		public int IsMoving { get; set; }
		public int DeadCount { get; set; }
		public int Lives { get; set; }
		public float Speed { get; set; }
		public float MaxSpeed { get; set; }
		public bool ChaseSmart { get; set; }

        public void Walk()
        {
            Animate(ActionSequences[0]);
        }

        public void Die()
        {
            --Lives;
            IsMoving = 0;
            Frame = new Point(0, 2);
        }

        public void Revive()
        {
            IsMoving = 1;
            DeadCount = 0;

            if (Speed <= MaxSpeed)
            {
                Speed += 5;
            }
            else
            {
                ChaseSmart = true;
            }
        }
	}
}
