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
using System.Threading;

#region Brendan Walsh
//Brendan Walsh
#endregion

namespace SpaceShoot
{
    public class GameClient : Microsoft.Xna.Framework.Game
    {
        //NOTE: Graphics device stuff

        //    SetFrameRate(graphics, 60);
        //
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
            Queue<EnemyShip> EnemyShips;
            Queue<BoonObject> Boons;

        // Textures
            Texture2D enemy1;
            Texture2D background;
            Texture2D bgStar;
            Texture2D[] bulletTextures;
            Texture2D[] boonTextures;
            List<Texture2D> enemyEmitTextures = new List<Texture2D>();

        // Internal Game Properties
            bool gameOver;
            int score;
            float gameSpeed;
            float delta;

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

            //IsFixedTimeStep = false;
            //SetFrameRate(graphics, 60);

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
            EnemyShips = new Queue<EnemyShip>();
            Bullets = new Queue<BulletObject>();
            Boons = new Queue<BoonObject>();
            #endregion

            #region Initialize Player
            Vector2 shipSpeed = new Vector2(7, 5); // 7 is horizontal speed, 5 is vertical speed
            Vector2 shipPos = new Vector2(MaxTitleX / 2 - 10, MaxTitleY - 50);
            int shipHealth = 100;
            float bonusAttraction = 300f;
            float bonusGrabRadius = 35f;
            List<WeaponObject> weapons = new List<WeaponObject>();
            weapons.Add(new WeaponObject(true, 0, 40, 10, 1.5, 0, 60)); // ZZZ minimum weapon fire speed is once per frame 
            weapons.Add(new WeaponObject(true, 1, 25, 10, 0.25, 0, 30)); // eg for other weapon

            Ship = new PlayerShip(shipPos, shipHealth, shipSpeed, weapons, bonusAttraction, bonusGrabRadius);
            
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
            enemy1 = Content.Load<Texture2D>("Benemy");
            background = Content.Load<Texture2D>("background1");
            bgStar = Content.Load<Texture2D>("star");
            bulletTextures = new Texture2D[2] { Content.Load<Texture2D>("Bbullet"), // basic bullet 
                                                Content.Load<Texture2D>("BKbullet")};    // nonbasic bullet
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

            #region FPS COUNTER -- DEBUG/OPTIMIZATION PURPOSES
            if (gameTime.TotalGameTime.Milliseconds == 0)
            {
                FPS = numOfFrames;
                numOfFrames = 0;
            }
            #endregion

            checkQuit();
            createRandomEnemies();
            UpdateInput();
            UpdateSprites(gameTime);
            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            proceduralStarBackground.Update(delta * 100);

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
                int destSize = EnemyShips.Count;
                for (int i = 0; i < destSize; i++)
                {
                    EnemyShip curDest = EnemyShips.Dequeue();
                    curDest.Draw(spriteBatch, enemy1);
                    EnemyShips.Enqueue(curDest);
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

            FPS = gameTime.ElapsedGameTime.Ticks;
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


            if (newKeyPress(newState, Keys.Space) || keyHold(newState, Keys.Space))
                Ship.firing = true;
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
            if (Ship.firing)
            {
                Ship.fire(Bullets, bulletTextures, r);
            } 
            
            Ship.Update(gameTime);
            
            // THESE ARE ALL HORRIBLY REDUNDANT IN NEED TO BE SOMEHOW MERGED INTO ONE METHOD!??
            // Go through bullets, updating the position of each one

            delta = gameTime.ElapsedGameTime.Milliseconds;

            UpdateBullets();
            // Go through destructables, updating the position of each one
            UpdateEnemies();
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

        private void UpdateEnemies()
        {

            //destructibleObjects.UpdateAll(gameSpeed);
            for (int i = 0; i < EnemyShips.Count; i++)
            {
                EnemyShip curEnemy = EnemyShips.Dequeue();
                curEnemy.Update(delta);
                curEnemy.fire(Bullets, bulletTextures, r);

                bool myShipIsHit = curEnemy.Intersects(Ship.Bounds); // Should this be here??
                if (myShipIsHit)
                {
                    Ship.Health -= 10 * ((int)(curEnemy.CurSpeed.Y) / 4); // Customize depending on stuff? (type of ship?)
                    if (Ship.Health <= 0)
                        gameOver = true;
                }

                bool onScreen = !(curEnemy.Position.Y > MaxY) && !myShipIsHit;
                if (onScreen) // If the Enemy is not past the bottom of the screen
                {
                    EnemyShips.Enqueue(curEnemy);
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

            if (!curBullet.isFriendly() && bullet1.Intersects(Ship.Bounds))
            {
                Ship.Health -= 10; // DO STUFF
                return true;
            } else if (curBullet.isFriendly()) {
                int destSize = EnemyShips.Count;
                // Go through destructables, check if each one is hit
                for (int i = 0; i < destSize; i++)
                {
                    EnemyShip curDest = EnemyShips.Dequeue();
                    if (curDest.Intersects(bullet1))
                    {
                        // Maybe spawn some debris here, because somethings been hit?
                        curDest.Damage(curBullet);
                        if (!curDest.IsDead())
                        {
                            EnemyShips.Enqueue(curDest);
                        } else {
                            score += 100;
                            // something's been killed, do stuff!
                            trySpawnRewards(curDest.Position);
                        }
                        return true;
                    } else {
                        EnemyShips.Enqueue(curDest);
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
            if (chanceToSpawn == 0 && EnemyShips.Count < 20) // Limit destructibles to 20
            {
                int health = 50; // Initial Health of Enemy
                int fireDamage = r.Next(50); // Firing Damage of Enemy
                int fireSpeed = r.Next(50); // Firing Speed of Enemy
                Vector2 moveSpeed = new Vector2(0, (r.Next(2) + 4)); // Movement Speed of Enemy
                int xPos = r.Next(graphics.PreferredBackBufferWidth); // Initial X Position of the enemy at the top of the screen
                Vector2 position = new Vector2(xPos, 0);
                List<WeaponObject> enemyWeapons = new List<WeaponObject>();
                int numWeps = r.Next(2) + 1;
                for (int i = 0; i < numWeps; i++)
                {
                    float FireSpeed = r.Next(100, 500);
                    enemyWeapons.Add(new WeaponObject(false, 1, (int)(0.5 * FireSpeed), -10, 0.3, 1, FireSpeed));
                }
                EnemyShip enemy = new EnemyShip(position, health, enemyWeapons, moveSpeed);

                enemy.Initialize(new EnemyParticleEngine(enemyEmitTextures, new Vector2(xPos, 0)));

                //destructibleObjects.Add(enemy);
                EnemyShips.Enqueue(enemy); // Put it in the queue
            }
        } // Randomly generate enemies at the top of the screen TEMPORARY SOLUTION

        private bool EnumEquals(Enum first, Enum second)
        {
            return ((boonTypes)first).Equals(second);
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
        
        public void SetFrameRate(GraphicsDeviceManager manager, int frames)
        {
                double dt = (double)1000 / (double)frames;
                manager.SynchronizeWithVerticalRetrace = false;
                TargetElapsedTime = TimeSpan.FromMilliseconds(dt);
                manager.ApplyChanges();
        }
    }
}
