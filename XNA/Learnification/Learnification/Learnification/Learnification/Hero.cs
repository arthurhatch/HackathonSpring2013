using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Learnification
{
	class Hero : Animatible
	{
        public Hero()
        {
            ActionSequences = new ActionSequence[2];

            //Running or Long Jumping
            ActionSequences[0] = new ActionSequence();
            ActionSequences[0].GenerateFrames(0, 3, 1, 1);

            //Idle
            ActionSequences[1] = new ActionSequence();
            ActionSequences[1].GenerateFrames(4, 1);
           
            Lives = 3;
		    JumpPower = 0.5f;
		    MaxRight = Vector2.Zero;
			Direction = SpriteEffects.None;
        }
        
        public float Speed = 3f;
        public int IsRunning { get; set; }
		public int IsJumping { get; set; }
		public int IsDying { get; set; }
        public float JumpPower { get; set; }
		public Vector2 MaxRight { get; set; }
		public SpriteEffects Direction { get; set; }
        public int Lives;

        public void ShortJump()
        {
            Frame = new Point(1, 1);
        }

        public void Run()
        {
            Animate(ActionSequences[0]);   
        }

        public void Idle()
        {
            Animate(ActionSequences[1]);
        }

        public void Die()
        {
            --Lives;
            IsDying = 1;
            Frame = new Point(1, 1);
        }

        public void Revive()
        {
            IsJumping = 0;
            IsDying = 0;
            JumpPower = 0.5f;
				
        }
	}
}
