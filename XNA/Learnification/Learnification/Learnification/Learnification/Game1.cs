using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Learnification
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        GameTime GameTime;

        SpriteBatch spriteBatch;
	    Texture2D background;

		Enemy enemy;
	    Hero hero;
		Rock rock;

        Vector2 rockPos;

        Point frameSize = new Point(38, 41);
        Point sheetSize = new Point(4, 6);

		SpriteFont font;

		int timeSinceLastFrame;

        const int millisecondsPerFrame = 125;

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
            font = Content.Load<SpriteFont>("gameFont");
            enemy = new Enemy 
            { 
                Sprite = Content.Load<Texture2D>(@"Images/Sprites/garfield")
            };
	        hero = new Hero 
            { 
                Sprite = Content.Load<Texture2D>(@"Images/Sprites/hobbes"), 
                Position = new Vector2(0, (Window.ClientBounds.Height - frameSize.Y)),
                MaxRight = Window.ClientBounds.Width - frameSize.X
            };
            rock = new Rock 
            { 
                Sprite = Content.Load<Texture2D>(@"Images/rock"),
                Position = new Vector2(-16, -16)
            };

            enemy.Position = new Vector2((Window.ClientBounds.Width / 2), (Window.ClientBounds.Height - enemy.Size.Y));
			rockPos = rock.Position;
            enemy.MaxRight = new Vector2((Window.ClientBounds.Width - enemy.Size.X), 0);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.GameTime = gameTime;
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            hero.IsRunning = 0;

            // Allows the game to exit
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Logic for movement
			if (keyboardState.IsKeyDown(Keys.Left) && hero.IsJumping == 0 && hero.IsDying == 0)
            {
                hero.SetRunState(Direction.Left);
            }

			if (keyboardState.IsKeyDown(Keys.Right) && hero.IsJumping == 0 && hero.IsDying == 0)
            {
                hero.SetRunState(Direction.Right);
            }

			if (keyboardState.IsKeyDown(Keys.A) && hero.IsJumping == 0 && hero.IsDying == 0)
			{
                hero.SetJumpState();
			}

			if (hero.IsJumping == 1)
			{
                hero.AnimateJump(Window.ClientBounds.Height - frameSize.Y);

                if (hero.IsJumping == 0)
                {
                    setEnemyDirection();
                    if (enemy.LobRocksOnHeroLands && !rock.IsAirborn && enemy.Lives > 0)
                        throwRock();
                }

				if (collisionDetected())
				{
                    enemy.Die();
				}
			}

			if (hero.IsDying == 1)
			{
                hero.AnimateDeath(Window.ClientBounds.Height - frameSize.Y);
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
                    if (enemy.LobRocksOnRevive)
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
            spriteBatch.Draw(hero.Sprite, hero.Position, new Rectangle(hero.Frame.X * frameSize.X, hero.Frame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0, Vector2.Zero, 1, hero.SpriteEffect, 1);
			spriteBatch.Draw(enemy.Sprite, enemy.Position, new Rectangle(enemy.Frame.X * enemy.Size.X, enemy.Frame.Y * enemy.Size.Y, enemy.Size.X, enemy.Size.Y), Color.White, 0, Vector2.Zero, 1, enemy.Direction, 1);
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
                + "                     Enemy Lives: " + enemy.Lives
                + "                     Game Time: " + this.GameTime.TotalGameTime.Seconds;
        }

		private bool detectRockCollision()
		{
			var xEquivilance = Math.Abs(hero.Position.X - rockPos.X) < 20;
			var yEquivalance = Math.Abs(hero.Position.Y - rockPos.Y) < 20;

			return xEquivilance && yEquivalance;
		}
		
		private bool collisionDetected()
		{
			if (enemy.IsMoving == 0)
			{
				return false;
			}

			var xEquivilance = Math.Abs(hero.Position.X - enemy.Position.X) < 10;

			if (hero.IsJumping == 0)
			{
				return xEquivilance;
			}

			var yEquivalance = Math.Abs(Window.ClientBounds.Height - hero.Position.Y - enemy.Size.Y) < 25;

			return xEquivilance && yEquivalance;
		}

		private void throwRock()
		{
			rockPos.X = enemy.Position.X;
			rockPos.Y = enemy.Position.Y;
			rock.ThrowMultiplier = (enemy.Position.X - hero.Position.X > 0) ? -1 : 1;
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
            
            if (enemy.LobRocksOnHeroJumps && hero.IsJumping == 1 && !rock.IsAirborn)
                throwRock();
		}

		private void setEnemyDirection()
		{
			if (enemy.ChaseSmart)
			{
				if (enemy.Direction == SpriteEffects.None && enemy.Position.X - hero.Position.X > 0)
				{
					enemy.Direction = SpriteEffects.FlipHorizontally;
				}
				else if (enemy.Direction == SpriteEffects.FlipHorizontally && enemy.Position.X - hero.Position.X < 0)
				{
					enemy.Direction = SpriteEffects.None;
				}
			}
		}

		private void moveEnemy()
		{
			if (enemy.Direction == SpriteEffects.None)
		    {
				enemy.Position.X += enemy.Speed;
			    if (enemy.Position.X > enemy.MaxRight.X)
			    {
				    enemy.Position.X = enemy.MaxRight.X;
				    enemy.Direction = SpriteEffects.FlipHorizontally;
			    }
		    }
		    else
		    {
				enemy.Position.X -= enemy.Speed;
			    if (enemy.Position.X < 0)
			    {
				    enemy.Position.X = 0;
				    enemy.Direction = SpriteEffects.None;
			    }
		    }
		}
    }
}