using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Learnification
{
    enum Direction { Left = -1, Right = 1 }

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
		    SpriteEffect = SpriteEffects.None;
            Direction = Direction.Right;
        }
        
        const float Speed = 3f;
        public int IsRunning { get; set; }
		public int IsJumping { get; set; }
		public int IsDying { get; set; }
        public float JumpPower { get; set; }
        public int JumpFrame;
        public int DieFrame;
        public float HeightIncrement;
		public int MaxRight { get; set; }
		public SpriteEffects SpriteEffect { get; set; }
        public int Lives { get; set; }
        public Vector2 Position;
        public Direction Direction { get; set; }

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
        }

        public void Revive()
        {
            IsJumping = 0;
            IsDying = 0;
            JumpPower = 0.5f;
            JumpFrame = 0;
            DieFrame = 0;
        }

        public void SetRunState(Direction direction)
        {
            IsRunning = 1;
            IsJumping = 0;
            Direction = direction;
            SpriteEffect = (direction == Direction.Left) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Position.X += (int)direction * Speed;
        }

        public void SetJumpState()
        {
            IsJumping = 1;
            JumpPower = IsRunning == 1 ? 1.0f : 0.6f;
            SpriteEffect = Direction == Direction.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public void AnimateDeath(float groundLevel)
        {
            if (++DieFrame > 90 && Lives > 0)
            {
                Position.Y = groundLevel;
                Revive();
                return;
            }
            
            Position.Y += Speed * (DieFrame <= 15 ? -1 : 1);
        }

        public void AnimateJump(float groundLevel)
        {
            if (++JumpFrame > 60)
            {
                JumpFrame = 0;
                IsJumping = 0;
                Position.Y = groundLevel;
                JumpPower = 0.5f;
                return;
            }

            HeightIncrement = (float)(Math.Abs(30 - JumpFrame) / 18.0 * Speed) * JumpPower;
            Position.Y += HeightIncrement * (JumpFrame <= 30 ? -1 : 1);
            Position.X += (int)Direction * Speed * JumpPower;
        }
    }
}