using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

#region Brendan Walsh
//Brendan Walsh
#endregion

namespace SpaceShoot
{
    public class GameClient : Microsoft.Xna.Framework.Game
    {
        //NOTE: Graphics device stuff
    
        //     // Instance the super-helpful graphics manager  
        //    graphics = new GraphicsDeviceManager(this);  
 
        //    // Set vertical trace with the back buffer  
        //    graphics.SynchronizeWithVerticalRetrace = false;  
 
        //    // Use multi-sampling to smooth corners of objects  
        //    graphics.PreferMultiSampling = true;  
 
        //    // Set the update to run as fast as it can go or  
        //    // with a target elapsed time between updates  
        //    IsFixedTimeStep = false;  
 
        //    // Make the mouse appear  
        //    IsMouseVisible = true;  
 
        //    // Set back buffer resolution  
        //    graphics.PreferredBackBufferWidth = 1280;  
        //    graphics.PreferredBackBufferHeight = 720;  
 
        //    // Make full screen  
        //    graphics.ToggleFullScreen();  
 
        //    // Assign content project subfolder  
        //    Content.RootDirectory = "Content";  

        #region Variable Declarations
        // Enums
            enum boonTypes { money, material, bonus, none }

        // Object Tracking
            static Random r;
            GraphicsDeviceManager graphics;
            SpriteBatch spriteBatch;
            KeyboardState oldState;
            Background proceduralStarBackground; 
            PlayerShip Ship;
            SpriteFont font;
            dustructablesDictionary destructableDict;
            Queue<BulletObject> Bullets;
            Queue<DestructibleObject> Destructibles;
            Queue<BoonObject> Boons;

        // Textures
            Texture2D bulletTexture;
            Texture2D otherBulletTexture;
            Texture2D enemy1;
            Texture2D background;
            Texture2D bgStar;
            Texture2D[] boonTextures;
            List<Texture2D> enemyEmitTextures = new List<Texture2D>();

        // Internal Game Properties
            bool gameOver;
            int score;
            float gameSpeed;

        // Game Settings
            int MaxX;
            int MaxY;

        // Temporary - These have not been fully organized/implemented
            float bulletVelocity;
            float fireCooldown;
            BoundingSphere bullet1 = new BoundingSphere(new Vector3(new Vector2(0, 0), 0), 7f);
            int Money;
            int Material;
            int Bonuses;
            int numOfFrames = 0;
            double FPS = 0;

        // private ScrollingBackground myBackground;
        #endregion

        ObjectManager destructibleObjects;

        Song gameplayMusic;

        public GameClient()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            r = new Random();

            #region reference
            graphics.PreferredBackBufferWidth = 1280; // 1920 1280
            graphics.PreferredBackBufferHeight = 720; // 1080 720
            #endregion
        }

        protected override void Initialize()
        {
            base.Initialize();

            #region Assign Game Settings
            MaxX = graphics.GraphicsDevice.Viewport.Width;
            MaxY = graphics.GraphicsDevice.Viewport.Height;
            int MaxTitleX = graphics.GraphicsDevice.Viewport.TitleSafeArea.Width;
            int MaxTitleY = graphics.GraphicsDevice.Viewport.TitleSafeArea.Height;
            #endregion

            #region Assign Game Properties
            gameSpeed = 1f;
            gameOver = false;
            score = 0;
            #endregion

            #region Assign Game Objects
            proceduralStarBackground = new Background(MaxX, MaxY, bgStar);
            oldState = Keyboard.GetState();
            destructableDict = new dustructablesDictionary();
            Destructibles = new Queue<DestructibleObject>();
            Bullets = new Queue<BulletObject>();
            Boons = new Queue<BoonObject>();
            #endregion

            #region Initialize Player
            Vector2 shipSpeed = new Vector2(7, 5); // 7 is horizontal speed, 5 is vertical speed
            Vector2 shipPos = new Vector2(MaxTitleX / 2 - 10, MaxTitleY - 50);
            int shipHealth = 100;
            float bonusAttraction = 300f;
            float bonusGrabRadius = 35f;
            Ship = new PlayerShip(shipPos, shipHealth, shipSpeed, bonusAttraction, bonusGrabRadius);
            List<Texture2D> shipEmitTextures = new List<Texture2D>() 
            {   // Put textures for ship particles here
                Content.Load<Texture2D>("pEngine/pStar") 
            };
            Ship.Initialize(new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Height), 
                            new AnimatedSprite(Content.Load<Texture2D>("BshipAtlas"), 1, 3), 
                            new ParticleEngine(shipEmitTextures, new Vector2(400, 240)));
            #endregion

            // Temporary
            bulletVelocity = 10 * gameSpeed; // Vertical bullet speed?
            fireCooldown = 0;
            int[] ship1 = new int[3] { 50, 10, 3 }; 
            destructableDict.dict.Add("ship1", ship1);
            // /TEMPORARY

            destructibleObjects = new ObjectManager();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Lucida Sans");

            #region Load/Assign Textures
            bulletTexture = Content.Load<Texture2D>("BKbullet");
            otherBulletTexture = Content.Load<Texture2D>("Bbullet");
            enemy1 = Content.Load<Texture2D>("Benemy");
            background = Content.Load<Texture2D>("background1");
            bgStar = Content.Load<Texture2D>("star");
            boonTextures = new Texture2D[3] { Content.Load<Texture2D>("buck"),            // Texture for money
                                              Content.Load<Texture2D>("stolenMaterial"),  // Texture for Materials
                                              Content.Load<Texture2D>("PowerUp2") };    // Texture for Bonus
            enemyEmitTextures.Add(Content.Load<Texture2D>("pEngine/pEnemyTrail"));
            #endregion

            //load music
            gameplayMusic = Content.Load<Song>("sound/mainMusic");

            //start the music right away
            PlayMusic(gameplayMusic);

            #region old/reference
            // myBackground = new ScrollingBackground();
            // myBackground.Load(GraphicsDevice, background);
            #endregion
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            #region FPS COUNTER -- DEBUG/OPTIMIZATION PURPOSES
            if (gameTime.TotalGameTime.Milliseconds == 0)
            {
                FPS = numOfFrames;
                numOfFrames = 0;
            }
            #endregion

            if (fireCooldown > 0)
                fireCooldown--;

            checkQuit();
            createRandomEnemies();
            UpdateInput();
            UpdateSprites(gameTime);
            proceduralStarBackground.Update(timeElapsed * 100);

            base.Update(gameTime);

            #region old/reference
            // myBackground.Update(elapsed * 100); // Scrolling image background
            #endregion
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            //myBackground.Draw(spriteBatch);
            proceduralStarBackground.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (!gameOver)
            {
                Ship.Draw(spriteBatch);

                // Go through bullets, drawing each one
                int size = Bullets.Count;
                for (int i = 0; i < size; i++)
                {
                    BulletObject curBullet = Bullets.Dequeue();

                    if (!TryHit(curBullet))
                    {
                        curBullet.Draw(spriteBatch);
                        Bullets.Enqueue(curBullet);
                    }
                    else
                    {
                        if (Ship.Health <= 0)
                            gameOver = true;
                    }
                }

                
                //destructibleObjects.DrawAll(spriteBatch);

                // Go through destructables, drawing each one
                int destSize = Destructibles.Count;
                for (int i = 0; i < destSize; i++)
                {
                    DestructibleObject curDest = Destructibles.Dequeue();
                    curDest.Draw(spriteBatch, enemy1);
                    Destructibles.Enqueue(curDest);
                }

                // Go through boons, drawing each one
                int boonSize = Boons.Count;
                for (int i = 0; i < boonSize; i++)
                {
                    BoonObject curBoon = Boons.Dequeue();
                    curBoon.Draw(spriteBatch, boonTextures);
                    Boons.Enqueue(curBoon);
                }

                DrawGUI();

            }
            else
            {
                DrawText(new Vector2(MaxX / 2 - 50, MaxY / 2 - 10), "GAME OVER");
                DrawText(new Vector2(MaxX / 2 - 85 - score.ToString().Length, MaxY / 2 + 10), "Final Score: " + score);
            }

            // FPS COUNTER -- DEBUG/OPTIMIZATION PURPOSES
            numOfFrames++;
            spriteBatch.DrawString(font, "FPS: " + FPS.ToString(), new Vector2(0, 40), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();

            #region Ship Movement/Firing
            if (newKeyPress(newState, Keys.A) || keyHold(newState, Keys.A))
                Ship.LeftThrust();
            if (newKeyPress(newState, Keys.D) || keyHold(newState, Keys.D))
                Ship.RightThrust();
            if (newKeyPress(newState, Keys.W) || keyHold(newState, Keys.W))
                Ship.ForwardThrust();
            if (newKeyPress(newState, Keys.S) || keyHold(newState, Keys.S))
                Ship.RearThrust();
           

            if ((newKeyPress(newState, Keys.Space) || keyHold(newState, Keys.Space)) && fireCooldown == 0)
                shipFire();
            #endregion

            oldState = newState;

            #region debug
            //Debug.WriteLine("-----------CYCLE-----------");
            //for (int i = 0; i < newState.GetPressedKeys().GetLength(0); i++)
            //    Debug.WriteLine(newState.GetPressedKeys()[i]);
            #endregion
        }

        private void UpdateSprites(GameTime gameTime)
        {
            Ship.Update(gameTime);

            // THESE ARE ALL HORRIBLY REDUNDANT IN NEED TO BE SOMEHOW MERGED INTO ONE METHOD!??
            // Go through bullets, updating the position of each one
            UpdateBullets();
            // Go through destructables, updating the position of each one
            UpdateDestructibles();
            // Go through boons, updating the position of each one
            UpdateBoons();
        }

        private void UpdateBoons()
        {
            int boonsSize = Boons.Count;
            for (int i = 0; i < boonsSize; i++)
            {
                BoonObject curBoon = Boons.Dequeue();

                // Update the position of the current Destructible
                curBoon.Update(gameSpeed, Ship.Position, Ship.Attraction);

                bool boonCollected = curBoon.isCollected(Ship.Bounds, Ship.GrabRadius);
                if (boonCollected)
                {
                    if (EnumEquals(curBoon.type, boonTypes.money))
                        Money++;
                    if (EnumEquals(curBoon.type, boonTypes.material))
                        Material++;
                    if (EnumEquals(curBoon.type, boonTypes.bonus))
                        Bonuses++;
                }

                bool onScreen = !(curBoon.Position.Y > MaxY || boonCollected);
                if (onScreen) // If the Enemy is not past the bottom of the screen
                    Boons.Enqueue(curBoon);
            }
        }

        private void UpdateDestructibles()
        {

            //destructibleObjects.UpdateAll(gameSpeed);

            int destSize = Destructibles.Count;
            for (int i = 0; i < destSize; i++)
            {
                DestructibleObject curDest = Destructibles.Dequeue();

                curDest.Update(gameSpeed);

                if (curDest.canFire())
                    enemyFire(curDest);

                bool myShipIsHit = curDest.Intersects(Ship.Bounds); // Should this be here??
                if (myShipIsHit)
                {
                    Ship.Health -= 10; // Customize depending on stuff? (type of ship?)
                    if (Ship.Health <= 0)
                        gameOver = true;
                }

                bool onScreen = !(curDest.Position.Y > MaxY) && !myShipIsHit;
                if (onScreen) // If the Enemy is not past the bottom of the screen
                { 
                    Destructibles.Enqueue(curDest);
                }
            }
        }

        private void UpdateBullets()
        {
            int bulletsSize = Bullets.Count;
            for (int i = 0; i < bulletsSize; i++)
            {
                BulletObject curBullet = Bullets.Dequeue();

                curBullet.Update();

                // If the bullet is past the top of the screen (value needs tweaking)
                if (!(curBullet.Position.Y < -30))
                    Bullets.Enqueue(curBullet);

            }
        }

        private void DrawGUI()
        {
            DrawText(new Vector2(0, 0), "Score: " + score);
            DrawText(new Vector2(0, 20), "Health: " + Ship.Health);
            DrawText(new Vector2(0, 60), "Money: " + Money);
            DrawText(new Vector2(0, 80), "Material: " + Material);
            DrawText(new Vector2(0, 100), "Bonuses: " + Bonuses);
        }

        /*
        public delegate somethingSomethingSomething??
        
        public void PrototypicalRedundanyReducer(theDelegate method, queueToProces objects)
        {
            int size = objects.Count;
            for (int i = 0; i < size; i++)
            {
                theDelegate(objects);
            }
        }

        public methodForDelegateStuff1(queueToProcess????)
        {
        
        }
        
        public methodForDelegateStuff2(queueToProcess????)
        {
        
        }
        */

        private bool TryHit(BulletObject curBullet)
        {
            bullet1.Center = (new Vector3(curBullet.Position, 0f));

            if (bullet1.Intersects(Ship.Bounds))
            {
                Ship.Health -= 10; // DO STUFF
                return true;
            } else {
                int destSize = Destructibles.Count;
                // Go through destructables, check if each one is hit
                for (int i = 0; i < destSize; i++)
                {
                    DestructibleObject curDest = Destructibles.Dequeue();
                    if (curDest.Intersects(bullet1))
                    {
                        // Maybe spawn some debris here, because somethings been hit?
                        curDest.Damage(curBullet);
                        if (!curDest.IsDead())
                        {
                            Destructibles.Enqueue(curDest);
                        } else {
                            score += 100;
                            // something's been killed, do stuff!
                            trySpawnRewards(curDest.Position);
                        }
                        return true;
                    } else {
                        Destructibles.Enqueue(curDest);
                    }
                }
            }
            return false;
        }

        private void trySpawnRewards(Vector2 pos)
        {
            for (int i = 0; i < r.Next(2); i++)
            {
                spawnRandomBoon(pos, true);
            }
            //spawnRandomBoon(pos, true);
        } // NEEDS BALANCING

        private void spawnRandomBoon(Vector2 pos, bool bonusPossible)
        {
            BoonObject newBoon = new BoonObject(pos, bonusPossible, r);
            bool boonExists = !EnumEquals(newBoon.type, boonTypes.none);
            if (boonExists)
                Boons.Enqueue(newBoon);
        }

        private void checkQuit()
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

#if WINDOWS
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
#endif
        }

        private void createRandomEnemies()
        {
            // Create Enemies [RANDOMLY]
            int chanceToSpawn = r.Next(25); // Inverse chance to spawn (ie. 25 is 1/25 chance, 100, is 1/100 chance)
            if (chanceToSpawn == 0 && Destructibles.Count < 20) // Limit destructibles to 20
            {
                int health = 50; // Initial Health of Enemy
                int fireDamage = r.Next(50); // Firing Damage of Enemy
                int fireSpeed = r.Next(50); // Firing Speed of Enemy
                Vector2 moveSpeed = new Vector2(0, (r.Next(0) + 5)); // Movement Speed of Enemy
                int xPos = r.Next(graphics.PreferredBackBufferWidth); // Initial X Position of the enemy at the top of the screen
                Vector2 position = new Vector2(xPos, 0);
                DestructibleObject enemy = new DestructibleObject(position, health, fireDamage, fireSpeed, moveSpeed);

                enemy.Initialize(new EnemyParticleEngine(enemyEmitTextures, new Vector2(xPos, 0)));

                //destructibleObjects.Add(enemy);
                Destructibles.Enqueue(enemy); // Put it in the queue
            }
        } // Randomly generate enemies at the top of the screen TEMPORARY SOLUTION

        private bool EnumEquals(Enum first, Enum second)
        {
            return ((boonTypes)first).Equals(second);
        }

        private void enemyFire(DestructibleObject enemy)
        {
            enemy.Fire();
            Vector2 bulletSpeed = new Vector2(0, 0);
            Bullets.Enqueue(new BulletObject(otherBulletTexture, otherBulletTexture, new Vector2(enemy.Position.X, enemy.Position.Y + 80), bulletSpeed, -bulletVelocity, r));
        }

        private void shipFire()
        {
            Vector2 bulletSpeed = Ship.CurSpeed;
            if (Ship.atEdge)
                bulletSpeed = new Vector2(0, bulletSpeed.Y);
            Bullets.Enqueue(new BulletObject(otherBulletTexture, bulletTexture, new Vector2(Ship.Position.X, Ship.Position.Y - 5), bulletSpeed, bulletVelocity, r));
            fireCooldown = 0; // 5 * 1 / gameSpeed;
        }

        private void DrawText(Vector2 pos, String text)
        {
            spriteBatch.DrawString(font, text, pos, Color.White);
        }

        private void playHit()
        {
            // Make a hit animation for the bullet?
        }

        private Boolean newKeyPress(KeyboardState newState, Keys key)
        {
            return (newState.IsKeyDown(key) && !oldState.IsKeyDown(key));
        }

        private Boolean endKeyPress(KeyboardState newState, Keys key)
        {
            return (!newState.IsKeyDown(key) && oldState.IsKeyDown(key));
        }

        private Boolean keyHold(KeyboardState newState, Keys key)
        {
            return (newState.IsKeyDown(key) && oldState.IsKeyDown(key));
        }

        private void PlayMusic(Song song)
        {
            //due to the way the media player plays music
            //we have to catch the exception, music will play when the game is not tethered.
            try
            {
                // play music
                MediaPlayer.Play(song);

                //loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

    }
}
