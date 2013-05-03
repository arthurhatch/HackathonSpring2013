using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Learnification
{
    public class Game1 : Game
    {
        // BEGIN GAME LOL
        GraphicsDeviceManager graphics;

        SpriteEffects malDirection = SpriteEffects.None;
        SpriteBatch spriteBatch;

        Texture2D learningSprite;
	    Texture2D background;

		Enemy enemy;

        Vector2 malPos = Vector2.Zero;
        Vector2 malMaxRight = Vector2.Zero;

		Vector2 enemyPos = Vector2.Zero;
		Point enemyCurrentFrame = new Point(0, 0);

        Point frameSize = new Point(38, 41);
        Point currentFrame = new Point(0, 0);
        Point sheetSize = new Point(4, 6);

        int isRunning = 0;
		int isJumping = 0;
	    int jumpFrame = 0;
		int isDying = 0;
		int dieFrame = 0;
	    float jumpPower = 0.5f;
        int timeSinceLastFrame = 0;
        const int millisecondsPerFrame = 125;
	    float heightIncrement = 0;
	    
		enum Direction
		{
			Left = -1,
			Right = 1,
		}

		Direction direction = Direction.Right;

        const float malSpeed = 3f;
	    float enemeySpeed = 3f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the images for our game into our ContentManager / Memory
            background = Content.Load<Texture2D>(@"Images/background2");
            learningSprite = Content.Load<Texture2D>(@"Images/Sprites/hobbes");
	       
			enemy = new Enemy
				{
					Sprite = Content.Load<Texture2D>(@"Images/Sprites/garfield"),
					Direction = SpriteEffects.None,
					MaxRight = Vector2.Zero,
					Size = new Point(45, 52),
					SheetSize = new Point(4, 3),
					IsMoving = 1,
					DeadCount = 0
				};

            // Set up some defaults needed for default sprite locations / movement boundaries
            malPos = new Vector2(0, (Window.ClientBounds.Height - frameSize.Y));
            malMaxRight = new Vector2((Window.ClientBounds.Width - frameSize.X), 0);

			// Set up enemy defaults
			enemyPos = new Vector2((Window.ClientBounds.Width / 2), (Window.ClientBounds.Height - enemy.Size.Y));
			enemy.MaxRight = new Vector2((Window.ClientBounds.Width - enemy.Size.X), 0);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            isRunning = 0;

            // Allows the game to exit
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // Logic for movement
			if (keyboardState.IsKeyDown(Keys.Left) && isJumping == 0 && isDying == 0)
            {
                isRunning = 1;
				isJumping = 0;
	            direction = Direction.Left;
                malDirection = SpriteEffects.FlipHorizontally;
                malPos.X -= malSpeed;
                if (malPos.X < 0)
                    malPos.X = 0;

            }

			if (keyboardState.IsKeyDown(Keys.Right) && isJumping == 0 && isDying == 0)
            {
                isRunning = 1;
				isJumping = 0;
				direction = Direction.Right;
                malDirection = SpriteEffects.None;
                malPos.X += malSpeed;
                if (malPos.X > malMaxRight.X)
                    malPos.X = malMaxRight.X;
            }

			if (keyboardState.IsKeyDown(Keys.A) && isJumping == 0 && isDying == 0)
			{
				isJumping = 1;
				jumpFrame = 1;
				jumpPower = isRunning == 1 ? 1.0f : 0.6f;
				malDirection = (int)direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			}

			if (isJumping == 1)
			{
				malPos.X += (int)direction * malSpeed * jumpPower;

				heightIncrement = (float)(Math.Abs(30 - jumpFrame) / 18.0 * malSpeed) * jumpPower;

				if (jumpFrame <= 30)
				{
					malPos.Y -= heightIncrement;
				}
				else
				{
					malPos.Y += heightIncrement;
				}

				jumpFrame++;

				if (this.CollisionDetected())
				{
					enemy.IsMoving = 0;
					enemyCurrentFrame.X = 0;
					enemyCurrentFrame.Y = 2;
				}

				if (jumpFrame > 60)
				{
					jumpFrame = 0;
					isJumping = 0;
					malPos.Y = Window.ClientBounds.Height - frameSize.Y;
					jumpPower = 0.5f;
				}
			}
			if (isDying == 1)
			{
				if (dieFrame <= 15)
				{
					malPos.Y -= malSpeed;
				}
				else
				{
					malPos.Y += malSpeed;
				}

				dieFrame++;

				if(dieFrame > 90)
				{
					jumpFrame = 0;
					isJumping = 0;
					isDying = 0;
					dieFrame = 0;
					malPos.Y = Window.ClientBounds.Height - frameSize.Y;
					jumpPower = 0.5f;
				}
			}
			else if (this.CollisionDetected())
			{
				isDying = 1;
				currentFrame.Y = 1;
				currentFrame.X = 1;
			}

            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;

				if (isRunning == 1 || jumpPower == 1.0f)
                {
                    if (currentFrame.Y == 0)
                    {
                        currentFrame.Y = 1;
                    }
                    ++currentFrame.X;
                    if (currentFrame.X >= sheetSize.X)
                    {
                        currentFrame.X = 0;
                        ++currentFrame.Y;
                        if (currentFrame.Y >= 3)
                        {
                            currentFrame.Y = 1;
                        }
                    }
                }
				else if (isJumping == 1)
				{
					currentFrame.Y = 1;
					currentFrame.X = 1;
				}
				else // Idle Animation Logic
				{
					if (currentFrame.Y > 0)
					{
						currentFrame.Y = 0;
					}
					++currentFrame.X;
					if (currentFrame.X >= sheetSize.X)
					{
						currentFrame.X = 0;
					}
				}
	            
				// Move enemy
	            if (enemy.IsMoving == 1)
	            {
		            ++enemyCurrentFrame.X;
		            if (enemyCurrentFrame.X >= enemy.SheetSize.X)
		            {
			            enemyCurrentFrame.X = 0;
			            ++enemyCurrentFrame.Y;
			            if (enemyCurrentFrame.Y >= 2)
			            {
				            enemyCurrentFrame.Y = 0;
			            }
		            }

		            if (enemy.Direction == SpriteEffects.None)
		            {
						enemyPos.X += enemeySpeed;
			            if (enemyPos.X > enemy.MaxRight.X)
			            {
				            enemyPos.X = enemy.MaxRight.X;
				            enemy.Direction = SpriteEffects.FlipHorizontally;
			            }

		            }
		            else
		            {
						enemyPos.X -= enemeySpeed;
			            if (enemyPos.X < 0)
			            {
				            enemyPos.X = 0;
				            enemy.Direction = SpriteEffects.None;
			            }
		            }
	            }
	            else
	            {
		            enemy.DeadCount++;
	            }

				if (enemy.DeadCount > 10)
				{
					enemy.IsMoving = 1;
					enemy.DeadCount = 0;
					enemeySpeed += 8;
				}
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            spriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(learningSprite, malPos, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0, Vector2.Zero, 1, malDirection, 1);
			spriteBatch.Draw(enemy.Sprite, enemyPos, new Rectangle(enemyCurrentFrame.X * enemy.Size.X, enemyCurrentFrame.Y * enemy.Size.Y, enemy.Size.X, enemy.Size.Y), Color.White, 0, Vector2.Zero, 1, enemy.Direction, 1);
			
            spriteBatch.End();

            base.Draw(gameTime);
        }

		private bool CollisionDetected()
		{
			return Math.Abs(malPos.X - enemyPos.X) < 10 && enemy.IsMoving == 1;
		}
    }
}
