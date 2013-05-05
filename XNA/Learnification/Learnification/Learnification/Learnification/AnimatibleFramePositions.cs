namespace Learnification
{
	using Microsoft.Xna.Framework;

	public class AnimatibleFramePositions
	{
		public Point[] Points { get; set; }
		public int Index { get; set; }

        public void GeneratePoints(int x, int y)
        {
            var length = x * y;
            Points = new Point[length];
            var index = 0;

            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    Points[index++] = new Point(i, j);
                }
            }
        }

        public void GeneratePoints(int xMin, int xMax, int yMin, int yMax)
        {
            var length = (xMax - xMin + 1) * (yMin - yMax + 1);
            Points = new Point[length];
            var index = 0;

            for (int j = yMin; j <= yMax; j++)
            {
                for (int i = xMin; i <= xMax; i++)
                {
                    Points[index++] = new Point(i, j);
                }
            }
        }
	}
}
