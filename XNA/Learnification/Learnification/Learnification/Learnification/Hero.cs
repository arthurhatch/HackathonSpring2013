using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Learnification
{
	class Hero : Animatible
	{
        public Hero()
        {
            FramePositions = new AnimatibleFramePositions[2];

            //Running or Long Jumping
            FramePositions[0] = new AnimatibleFramePositions();
            FramePositions[0].Points = new Point[4];
                FramePositions[0].Points[0] = new Point(0, 1);
                FramePositions[0].Points[1] = new Point(1, 1);
                FramePositions[0].Points[2] = new Point(2, 1);
                FramePositions[0].Points[3] = new Point(3, 1);

            //Idle
            FramePositions[1] = new AnimatibleFramePositions();
            FramePositions[1].GeneratePoints(4, 1);
        }
        
        public float Speed = 3f;
        public int IsRunning { get; set; }
		public int IsJumping { get; set; }
		public int IsDying { get; set; }
		public float JumpPower { get; set; }
		public Vector2 MaxRight { get; set; }
		public SpriteEffects Direction { get; set; }
        
        public void ShortJump()
        {
            Frame = new Point(1, 1);
        }

        public void Run()
        {
            Animate(FramePositions[0]);   
        }

        public void Idle()
        {
            Animate(FramePositions[1]);
        }

        public void Die()
        {
            IsDying = 1;
            Frame = new Point(1, 1);
        }
	}
}
