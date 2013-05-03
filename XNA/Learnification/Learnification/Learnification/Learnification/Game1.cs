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

        SpriteBatch spriteBatch;
	    Texture2D background;

		Enemy enemy;
	    Hero hero;

        Vector2 heroPos = Vector2.Zero;
		Vector2 enemyPos = Vector2.Zero;
		Point enemyCurrentFrame = new Point(0, 0);

        Point frameSize = new Point(38, 41);
        Point currentFrame = new Point(0, 0);
        Point sheetSize = new Point(4, 6);

		int jumpFrame;
		int dieFrame;
	    float heightIncrement;
        int timeSinceLastFrame;

        const int millisecondsPerFrame = 125;
	   
		enum Direction { Left = -1, Right = 1 }

		Direction direction = Direction.Right;

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
	       
			enemy = new Enemy
			{
				Sprite = Content.Load<Texture2D>(@"Images/Sprites/garfield"),
				Direction = SpriteEffects.None,
				MaxRight = Vector2.Zero,
				Size = new Point(45, 52),
				SheetSize = new Point(4, 3),
				IsMoving = 1,
				DeadCount = 0,
				Speed = 5f,
				MaxSpeed = 20f,
				ChaseSmart = false
			};

	        hero = new Hero
	        {
		        Sprite = Content.Load<Texture2D>(@"Images/Sprites/hobbes"),
		        IsRunning = 0,
		        IsJumping = 0,
		        IsDying = 0,
		        JumpPower = 0.5f,
		        MaxRight = Vector2.Zero,
				Direction = SpriteEffects.None
	        };

            // Set up some defaults needed for default sprite locations / movement boundaries
            heroPos = new Vector2(0, (Window.ClientBounds.Height - frameSize.Y));
            hero.MaxRight = new Vector2((Window.ClientBounds.Width - frameSize.X), 0);

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
            hero.IsRunning = 0;

            // Allows the game to exit
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // Logic for movement
			if (keyboardState.IsKeyDown(Keys.Left) && hero.IsJumping == 0 && hero.IsDying == 0)
            {
                hero.IsRunning = 1;
				hero.IsJumping = 0;
	            direction = Direction.Left;
                hero.Direction = SpriteEffects.FlipHorizontally;
                heroPos.X -= hero.Speed;
                if (heroPos.X < 0)
                    heroPos.X = 0;
            }

			if (keyboardState.IsKeyDown(Keys.Right) && hero.IsJumping == 0 && hero.IsDying == 0)
            {
                hero.IsRunning = 1;
				hero.IsJumping = 0;
				direction = Direction.Right;
                hero.Direction = SpriteEffects.None;
                heroPos.X += hero.Speed;
                if (heroPos.X > hero.MaxRight.X)
                    heroPos.X = hero.MaxRight.X;
            }

			if (keyboardState.IsKeyDown(Keys.A) && hero.IsJumping == 0 && hero.IsDying == 0)
			{
				hero.IsJumping = 1;
				jumpFrame = 1;
				hero.JumpPower = hero.IsRunning == 1 ? 1.0f : 0.6f;
				hero.Direction = (int)direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			}

			if (hero.IsJumping == 1)
			{
				animateHeroJump();

				if (collisionDetected())
				{
					killEnemy();
				}
			}

			if (hero.IsDying == 1)
			{
				animateHeroDeath();
			}
			else if (collisionDetected())
			{
				killHero();
			}

            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;

				if (hero.IsRunning == 1 || hero.JumpPower == 1.0f)
				{
					animateHeroLongJump();
				}
				else if (hero.IsJumping == 1)
				{
					animateHeroShortJump();
				}
				else
				{
					animateHeroIdle();
				}
	            
				// Move enemy
	            if (enemy.IsMoving == 1)
	            {
		            moveEnemy();
	            }
	            else
	            {
		            enemy.DeadCount++;
	            }

				if (enemy.DeadCount > 10)
				{
					reviveEnemy();
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
            spriteBatch.Draw(hero.Sprite, heroPos, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0, Vector2.Zero, 1, hero.Direction, 1);
			spriteBatch.Draw(enemy.Sprite, enemyPos, new Rectangle(enemyCurrentFrame.X * enemy.Size.X, enemyCurrentFrame.Y * enemy.Size.Y, enemy.Size.X, enemy.Size.Y), Color.White, 0, Vector2.Zero, 1, enemy.Direction, 1);
			
            spriteBatch.End();

            base.Draw(gameTime);
        }

		private bool collisionDetected()
		{
			return Math.Abs(heroPos.X - enemyPos.X) < 10 && enemy.IsMoving == 1;
		}

		private void killHero()
		{
			hero.IsDying = 1;
			currentFrame.Y = 1;
			currentFrame.X = 1;
		}

		private void killEnemy()
		{
			enemy.IsMoving = 0;
			enemyCurrentFrame.X = 0;
			enemyCurrentFrame.Y = 2;
		}

		private void reviveEnemy()
		{
			enemy.IsMoving = 1;
			enemy.DeadCount = 0;

			if (enemy.Speed <= enemy.MaxSpeed)
			{
				enemy.Speed += 5;
			}
			else
			{
				enemy.ChaseSmart = true;
			}
		}

		private void animateHeroDeath()
		{
			heroPos.Y = (dieFrame <= 15) ? heroPos.Y + -1 * hero.Speed : heroPos.Y + hero.Speed;

			dieFrame++;

			if (dieFrame > 90)
			{
				hero.IsJumping = 0;
				hero.IsDying = 0;
				hero.JumpPower = 0.5f;
				heroPos.Y = Window.ClientBounds.Height - frameSize.Y;
				
				jumpFrame = 0;
				dieFrame = 0;
			}
		}

		private void animateHeroJump()
		{
			heroPos.X += (int)direction * hero.Speed * hero.JumpPower;

			heightIncrement = (float)(Math.Abs(30 - jumpFrame) / 18.0 * hero.Speed) * hero.JumpPower;

			heroPos.Y = (jumpFrame <= 30) ? heroPos.Y + -1 * heightIncrement : heroPos.Y + heightIncrement;

			jumpFrame++;

			if (jumpFrame > 60)
			{
				jumpFrame = 0;
				hero.IsJumping = 0;
				heroPos.Y = Window.ClientBounds.Height - frameSize.Y;
				hero.JumpPower = 0.5f;

				setEnemyDirection();
			}
		}

		private void setEnemyDirection()
		{
			if (enemy.ChaseSmart)
			{
				if (enemy.Direction == SpriteEffects.None && enemyPos.X - heroPos.X > 0)
				{
					enemy.Direction = SpriteEffects.FlipHorizontally;
				}
				else if (enemy.Direction == SpriteEffects.FlipHorizontally && enemyPos.X - heroPos.X < 0)
				{
					enemy.Direction = SpriteEffects.None;
				}
			}
		}

		private void animateHeroLongJump()
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

		private void animateHeroShortJump()
		{
			currentFrame.Y = 1;
			currentFrame.X = 1;
		}

		private void animateHeroIdle()
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

		private void moveEnemy()
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
				enemyPos.X += enemy.Speed;
			    if (enemyPos.X > enemy.MaxRight.X)
			    {
				    enemyPos.X = enemy.MaxRight.X;
				    enemy.Direction = SpriteEffects.FlipHorizontally;
			    }
		    }
		    else
		    {
				enemyPos.X -= enemy.Speed;
			    if (enemyPos.X < 0)
			    {
				    enemyPos.X = 0;
				    enemy.Direction = SpriteEffects.None;
			    }
		    }
		}
    }
}
