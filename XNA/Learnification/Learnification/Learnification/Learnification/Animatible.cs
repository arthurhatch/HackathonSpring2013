namespace Learnification
{
	using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

	public abstract class Animatible
	{
		public Texture2D Sprite { get; set; }
		public AnimatibleFramePositions[] FramePositions { get; set; }
        public Point Frame { get; set; }

		protected void Animate(AnimatibleFramePositions positions)
		{
            ++positions.Index;
            positions.Index = positions.Index % positions.Points.Length;
            Frame = positions.Points[positions.Index];
		}
	}
}
