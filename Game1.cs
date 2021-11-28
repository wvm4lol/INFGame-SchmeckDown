using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace INFGame
{
    public class Game1 : Game
    {
        //variable declaring
        //camera variables
        private Matrix view;
        private Matrix projection;
        private Matrix stageMatrix;
        Vector3 cameraPos; 
       
        //other variables
        Model stage; //model used for the floor
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Vector3 arenaSize = new Vector3(20, 0, 0);
        private SpriteFont font;
        private SpriteFont gameOverFont;
        private SpriteFont gameOverSubFont;
        Player player1 = new Player();
        Player player2 = new Player();
        Attack lightAttack = new Attack();
        Attack heavyAttack = new Attack();
        Attack guardBreak = new Attack();
        Attack[] attackList;
        Animations animationList;
        Song backgroundMusic;
        


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this); //device properties
            Content.RootDirectory = "Content"; //asset location
            //window settings
            IsMouseVisible = false; 
            graphics.PreferredBackBufferWidth = 1920; 
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            
            base.Initialize();
            Window.Title = ("SchmeckDown");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //assigning default playere positions
            player1.position = new Vector3(-5, 0, 0);
            player2.position = new Vector3(5, 0, 0);
            //creating matrices for rendering
            cameraPos = new Vector3(0, 5, 20);
            view = Matrix.CreateLookAt(new Vector3(0, 8, 20), new Vector3(0, 8, 0), Vector3.UnitY);
            projection = Matrix.CreateOrthographic(40, 22.5f, 0.1f, 100);
            //assigning controllers to players and getting first state for checking connection
            player1.gamePadState = GamePad.GetState(PlayerIndex.One);
            player2.gamePadState = GamePad.GetState(PlayerIndex.Two);
            player1.contrIndex = PlayerIndex.One;
            player2.contrIndex = PlayerIndex.Two;
            //setting player directions
            player1.facing = 1;
            player2.facing = -1;
            //assigning control schemes depending on connected controllers (0 is controller, 1 is left side of keyboard, 2 is right side)
            if (!player1.gamePadState.IsConnected)
            {
                player1.contrlScheme++;
                player2.contrlScheme++;
                player2.contrlScheme++;
            } else
            {
                if (!player2.gamePadState.IsConnected)
                {
                    player2.contrlScheme++;
                }
            }
        }

        //load assets
        protected override void LoadContent()
        {
            //assigning players their default models and loading default font for text
            font = Content.Load<SpriteFont>("File");
            //loading fonts
            gameOverFont = Content.Load<SpriteFont>("GameOverFont");
            gameOverSubFont = Content.Load<SpriteFont>("GameOverFontSub");
            //setting attack values through a very dumb way, but i'm too stupid to learn how JSONs work
            lightAttack.LightAttack();
            heavyAttack.HeavyAttack();
            guardBreak.GuardBreak();
            //putting atacks in a list for easy access and giving them to player objects, probably also very inefficient
            Attack[] attackList = {lightAttack, heavyAttack, guardBreak};
            player1.attacks = attackList;
            player2.attacks = attackList;
            //loading player animations
            LoadPlayerAnimations(player1);
            LoadPlayerAnimations(player2);

            //loading music
            backgroundMusic = Content.Load<Song>("nummer1");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.05f;
            //loading stage
            stage = Content.Load<Model>("StageCube");
            stageMatrix = Matrix.CreateScale(new Vector3(20, 20, 4)) * Matrix.CreateTranslation(new Vector3(0, -20, 0));

            
            void LoadPlayerAnimations(Player player)
            {
                //loading animations, for every frame in said animation, set that place in array to that model
                for (int i = 0; i <= 1; i++)
                {
                    player.animations.idle[i] = Content.Load<Model>("idleAnim" + i.ToString());
                }
                for (int i = 0; i <= 7; i++)
                {
                    player.animations.walk[i] = Content.Load<Model>("walkAnim" + i.ToString());
                }
                for (int i = 0; i <= 5; i++)
                {
                    player.animations.punch[i] = Content.Load<Model>("punchAnim" + i.ToString());
                }
                //for (int i = 0; i <= 7; i++)
                //{
                //    player.animations.blockingWalk[i] = Content.Load<Model>("blockingWalkAnim" + i.ToString());
                //}
                for (int i = 0; i <= 2; i++)
                {
                    player.animations.blocking[i] = Content.Load<Model>("block" + i.ToString());
                }
                for (int i = 0; i <= 2; i++)
                {
                    player.animations.stunned[i] = Content.Load<Model>("stunnedanim" + i.ToString());
                }
            }
        }

        //game update loop
        protected override void Update(GameTime gameTime)
        {
            //default check to exit game on esc
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //check if players have died, if they have go to menu until button press to reset
            if (player1.health <= 0 || player2.health <= 0)
            {
                player1.GetControllerState();
                if (player1.gamePadState.Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    player1.health = 100;
                    player2.health = 100;
                    player1.position = new Vector3(-5, 0, 0);
                    player2.position = new Vector3(5, 0, 0);
                    player1.speedX = 0; player1.speedY = 0;
                    player2.speedX = 0; player2.speedY = 0;
                }
            }
            else
            {
                player1.hit = false;

                //all player updates, will probably turn this into method
                player1.GetControllerState();
                player2.GetControllerState();
                CheckPlayerOrientation(player1, player2);
                player1.PlayerMoveY();
                player2.PlayerMoveY();
                player1.PlayerMoveX();
                player2.PlayerMoveX();
                player1.Attack(player2);
                player2.Attack(player1);
                player1.PlayerCollision(arenaSize);
                player2.PlayerCollision(arenaSize);
                player1.SetPlayerHitBox();
                player2.SetPlayerHitBox();
            }


            //translating player position to matrix for rendering
            player1.matrix = Matrix.CreateRotationX(MathHelper.ToRadians(90)) * Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateScale(new Vector3(player1.facing * 0.4f, -0.4f, 0.4f)) * Matrix.CreateTranslation(player1.position);
            player2.matrix = Matrix.CreateRotationX(MathHelper.ToRadians(90)) * Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateScale(new Vector3(player2.facing * 0.4f, -0.4f, 0.4f)) * Matrix.CreateTranslation(player2.position);

            base.Update(gameTime);
        }

        //game render loop
        protected override void Draw(GameTime gameTime)
        {
            if (player1.health <= 0 || player2.health <= 0)
            {
                spriteBatch.Begin();
                if (player1.health <= 0)
                {
                    GraphicsDevice.Clear(Color.Blue);
                    spriteBatch.DrawString(gameOverFont, "Player 2 Wins", new Vector2(760, 540), Color.Black);
                } else
                {
                    GraphicsDevice.Clear(Color.Red);
                    spriteBatch.DrawString(gameOverFont, "Player 1 Wins", new Vector2(760, 540), Color.Black);

                }
                spriteBatch.DrawString(gameOverSubFont, "Press Space or A To Continue", new Vector2(680, 600), Color.Black);
                spriteBatch.End();

            } else
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                //rendering background as specific shade of blue
                GraphicsDevice.Clear(Color.CornflowerBlue);
                //rendering using method
                DrawModel(stage, stageMatrix, view, projection, new Vector3(0, 0, 0));
                DrawModel(player1.model, player1.matrix, view, projection, new Vector3(255, 0, 0));
                DrawModel(player2.model, player2.matrix, view, projection, new Vector3(0, 0, 255));


                //drawing values for development 
                spriteBatch.Begin();
                spriteBatch.DrawString(gameOverSubFont, "Health: " + player1.health.ToString() + "%", new Vector2(0, 100), Color.Black);
                spriteBatch.DrawString(gameOverSubFont, "Health: " + player2.health.ToString() + "%", new Vector2(1650, 100), Color.Black);


                spriteBatch.End();
            }



            base.Draw(gameTime);
        }
        private void CheckPlayerOrientation(Player player1, Player player2)
        {
            if (player1.position.X > player2.position.X)
            {
                player1.facing = -1;
                player2.facing = 1;
            } else
            {
                player1.facing = 1;
                player2.facing = -1;
            }
        }

        //method to render a specific model
        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Vector3 color)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EmissiveColor = color;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }
    }

}
