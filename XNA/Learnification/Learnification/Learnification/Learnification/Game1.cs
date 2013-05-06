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
		Rock rock;

        Vector2 heroPos = Vector2.Zero;
		Vector2 enemyPos = Vector2.Zero;
		Vector2 rockPos = new Vector2(-16, -16);

        Point frameSize = new Point(38, 41);
        Point sheetSize = new Point(4, 6);

		SpriteFont font;

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

			enemy = new Enemy();
			enemy.Sprite = Content.Load<Texture2D>(@"Images/Sprites/garfield");

	        hero = new Hero();
	        hero.Sprite = Content.Load<Texture2D>(@"Images/Sprites/hobbes");

            rock = new Rock();
	        rock.Sprite = Content.Load<Texture2D>(@"Images/rock");

			font = Content.Load<SpriteFont>("gameFont");

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
                    enemy.Die();
				}
			}

			if (hero.IsDying == 1)
			{
				animateHeroDeath();
			}
			else if (collisionDetected())
			{
				hero.Die();
			}

            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;

				if (hero.IsRunning == 1 || hero.JumpPower == 1.0f)
				{
					hero.Run();
				}
				else if (hero.IsJumping == 1)
				{
                    hero.ShortJump();
				}
                else
                {
                    hero.Idle();
                }
	            
				// Move enemy
	            if (enemy.IsMoving == 1)
	            {
                    enemy.Walk();
                    moveEnemy();
	            }
	            else
	            {
		            enemy.DeadCount++;
	            }

                if (enemy.DeadCount > 10 && enemy.Lives > 0)
				{
                    enemy.Revive();
                    throwRock();
				}

				if (rock.IsAirborn)
				{
					animateRockThrow();
					
					if (detectRockCollision())
					{
						hero.Die();
						rock.IsAirborn = false;
						rockPos.X = -16;
						rockPos.Y = -16;
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
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            
            spriteBatch.Draw(background, new Rectangle(0, 35, Window.ClientBounds.Width, Window.ClientBounds.Height - 35), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(hero.Sprite, heroPos, new Rectangle(hero.Frame.X * frameSize.X, hero.Frame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0, Vector2.Zero, 1, hero.Direction, 1);
			spriteBatch.Draw(enemy.Sprite, enemyPos, new Rectangle(enemy.Frame.X * enemy.Size.X, enemy.Frame.Y * enemy.Size.Y, enemy.Size.X, enemy.Size.Y), Color.White, 0, Vector2.Zero, 1, enemy.Direction, 1);
            spriteBatch.Draw(rock.Sprite, rockPos, new Rectangle(0, 0, rock.Size.X, rock.Size.Y), Color.White, 0, Vector2.Zero, 1, rock.Direction, 1);
            spriteBatch.DrawString(font, GenerateMessage(), new Vector2(5, 5), Color.White);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private string GenerateMessage()
        {
            if (hero.Lives == 0) return "Game Over (hit escape)";
            if (enemy.Lives == 0) return "You Win! (hit escape)";

            return "Hero Lives: " + hero.Lives 
                + "                                                                 " 
                + "Enemy Lives: " + enemy.Lives;
        }
		private bool detectRockCollision()
		{
			var xEquivilance = Math.Abs(heroPos.X - rockPos.X) < 20;
			var yEquivalance = Math.Abs(heroPos.Y - rockPos.Y) < 20;

			return xEquivilance && yEquivalance;
		}
		
		private bool collisionDetected()
		{
			if (enemy.IsMoving == 0)
			{
				return false;
			}

			var xEquivilance = Math.Abs(heroPos.X - enemyPos.X) < 10;

			if (hero.IsJumping == 0)
			{
				return xEquivilance;
			}

			var yEquivalance = Math.Abs(Window.ClientBounds.Height - heroPos.Y - enemy.Size.Y) < 25;

			return xEquivilance && yEquivalance;
		}

		private void animateHeroDeath()
		{
			heroPos.Y = (dieFrame <= 15) ? heroPos.Y + -1 * hero.Speed : heroPos.Y + hero.Speed;

			if (++dieFrame > 90 && hero.Lives > 0)
			{
				heroPos.Y = Window.ClientBounds.Height - frameSize.Y;
                hero.Revive();
				
                jumpFrame = 0;
				dieFrame = 0;
			}
		}

		private void throwRock()
		{
			rockPos.X = enemyPos.X;
			rockPos.Y = enemyPos.Y;
			rock.ThrowMultiplier = (enemyPos.X - heroPos.X > 0) ? -1 : 1;
			rock.IsAirborn = true;
			rock.IsFalling = false;
		}

	    private void animateRockThrow()
	    {
		    rockPos.X += rock.ThrowMultiplier * 10;
			rockPos.Y += rock.IsFalling ? 8 : -8;

		    if (!rock.IsFalling)
		    {
				rock.IsFalling = rockPos.Y < 370;
		    }

		    rock.IsAirborn = rockPos.Y < Window.ClientBounds.Height;
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

		private void moveEnemy()
		{
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
