namespace Learnification
{
	using Microsoft.Xna.Framework;

	public class ActionSequence
	{
		public Point[] Frames { get; set; }
		public int Index { get; set; }

        public void GenerateFrames(int x, int y)
        {
            var length = x * y;
            Frames = new Point[length];
            var index = 0;

            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    Frames[index++] = new Point(i, j);
                }
            }
        }

        public void GenerateFrames(int xMin, int xMax, int yMin, int yMax)
        {
            var length = (xMax - xMin + 1) * (yMin - yMax + 1);
            Frames = new Point[length];
            var index = 0;

            for (int j = yMin; j <= yMax; j++)
            {
                for (int i = xMin; i <= xMax; i++)
                {
                    Frames[index++] = new Point(i, j);
                }
            }
        }
	}
}
