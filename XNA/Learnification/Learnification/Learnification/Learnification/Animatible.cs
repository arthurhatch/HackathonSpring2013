namespace Learnification
{
	using Microsoft.Xna.Framework.Graphics;

	public class Animatible
	{
		public Texture2D Sprite { get; set; }
		public AnimatibleFramePositions[] FramePositions { get; set; }

		public void Animate(int index)
		{
			var framePosition = FramePositions[index];
			
		}
	}
}
