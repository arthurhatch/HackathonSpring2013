using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Learnification
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // BEGIN GAME LOL
        GraphicsDeviceManager graphics;

        SpriteEffects malDirection = SpriteEffects.None;
        SpriteBatch spriteBatch;

        Texture2D learningTexture;
        Texture2D learningSprite;
	    Texture2D enemySprite;
        Texture2D background;

        Vector2 malPos = Vector2.Zero;
        Vector2 malMaxRight = Vector2.Zero;

		SpriteEffects enemyDirection = SpriteEffects.None;
		Vector2 enemyPos = Vector2.Zero;
		Vector2 enemyMaxRight = Vector2.Zero;
		Point enemySize = new Point(45, 52);
		Point enemySheetSize = new Point(4,3);
	    int moveEnemy = 1;
		Point currentEnemyFrame = new Point(0, 0);

        Point frameSize = new Point(38, 41);
        Point currentFrame = new Point(0, 0);
        Point sheetSize = new Point(4, 6);

        int isRunning = 0;
		int isJumping = 0;
	    int jumpFrame = 0;
	    float jumpPower = 0.5f;
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame = 125;
	    float heightIncrement = 0;
	    
		enum Direction
		{
			Left = -1,
			Right = 1,
		}

		Direction direction = Direction.Right;

        float malSpeed = 3f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
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
            learningTexture = Content.Load<Texture2D>(@"Images/mal_sprite_test1");
            learningSprite = Content.Load<Texture2D>(@"Images/Sprites/hobbes");
			enemySprite = Content.Load<Texture2D>(@"Images/Sprites/garfield");

            // Set up some defaults needed for default sprite locations / movement boundaries
            malPos = new Vector2(0, (Window.ClientBounds.Height - frameSize.Y));
            malMaxRight = new Vector2((Window.ClientBounds.Width - frameSize.X), 0);

			// Set up enemy defaults
			enemyPos = new Vector2((Window.ClientBounds.Width / 2), (Window.ClientBounds.Height - enemySize.Y));
			enemyMaxRight = new Vector2((Window.ClientBounds.Width - enemySize.X), 0);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // Logic for movement
			if (keyboardState.IsKeyDown(Keys.Left) && isJumping == 0)
            {
                isRunning = 1;
				isJumping = 0;
	            direction = Direction.Left;
                malDirection = SpriteEffects.FlipHorizontally;
                malPos.X -= malSpeed;
                if (malPos.X < 0)
                    malPos.X = 0;

            }

			if (keyboardState.IsKeyDown(Keys.Right) && isJumping == 0)
            {
                isRunning = 1;
				isJumping = 0;
				direction = Direction.Right;
                malDirection = SpriteEffects.None;
                malPos.X += malSpeed;
                if (malPos.X > malMaxRight.X)
                    malPos.X = malMaxRight.X;
            }

			if (keyboardState.IsKeyDown(Keys.A) && isJumping == 0)
			{
				isJumping = 1;
				jumpFrame = 1;
				jumpPower = isRunning == 1 ? 1.0f : 0.6f;
				malDirection = (int)direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				//malPos.Y -= malSpeed;
				//if (malPos.X > malMaxRight.X)
					//malPos.X = malMaxRight.X;
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

				if (Math.Abs(malPos.X - enemyPos.X) < 10)
				{
					moveEnemy = 0;
					currentEnemyFrame.X = 0;
					currentEnemyFrame.Y = 2;
				}

				if (jumpFrame > 60)
				{
					jumpFrame = 0;
					isJumping = 0;
					malPos.Y = Window.ClientBounds.Height - frameSize.Y;
					jumpPower = 0.5f;
				}
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
	            if (moveEnemy == 1)
	            {
		            ++currentEnemyFrame.X;
		            if (currentEnemyFrame.X >= enemySheetSize.X)
		            {
			            currentEnemyFrame.X = 0;
			            ++currentEnemyFrame.Y;
			            if (currentEnemyFrame.Y >= 2)
			            {
				            currentEnemyFrame.Y = 0;
			            }
		            }

		            if (enemyDirection == SpriteEffects.None)
		            {
			            enemyPos.X += malSpeed;
			            if (enemyPos.X > enemyMaxRight.X)
			            {
				            enemyPos.X = enemyMaxRight.X;
				            enemyDirection = SpriteEffects.FlipHorizontally;
			            }

		            }
		            else
		            {
			            enemyPos.X -= malSpeed;
			            if (enemyPos.X < 0)
			            {
				            enemyPos.X = 0;
			            }
		            }
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
			spriteBatch.Draw(enemySprite, enemyPos, new Rectangle(currentEnemyFrame.X * enemySize.X, currentEnemyFrame.Y * enemySize.Y, enemySize.X, enemySize.Y), Color.White, 0, Vector2.Zero, 1, enemyDirection, 1);
			
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
