namespace Learnification
{
	using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

	public abstract class Animatible
	{
		public Texture2D Sprite { get; set; }
        public ActionSequence[] ActionSequences { get; set; }
        public Point Frame { get; set; }

        protected void Animate(ActionSequence actionSequence)
		{
            ++actionSequence.Index;
            actionSequence.Index = actionSequence.Index % actionSequence.Frames.Length;
            Frame = actionSequence.Frames[actionSequence.Index];
		}
	}
}