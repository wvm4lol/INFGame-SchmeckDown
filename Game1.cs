using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;

namespace INFGame
{
    public class Game1 : Game
    {
        //variable declaring
        //camera variables
        private Matrix view;
        private Matrix projection;
        //other variables
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
        Attack[] attackList;
        


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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //assigning default playere positions
            player1.position = new Vector3(-5, 0, 0);
            player2.position = new Vector3(5, 0, 0);
            //creating matrices for rendering
            view = Matrix.CreateLookAt(new Vector3(0, 5, 20), new Vector3(0, 5, 0), Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1920f / 1080f, 0.1f, 100f);
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
            player1.model = Content.Load<Model>("TestCube");
            player2.model = Content.Load<Model>("TestCube");
            font = Content.Load<SpriteFont>("File");
            gameOverFont = Content.Load<SpriteFont>("GameOverFont");
            gameOverSubFont = Content.Load<SpriteFont>("GameOverFontSub");
            //setting attack values through a very dumb way, but i'm too stupid to learn how JSONs work
            lightAttack.LightAttack();
            heavyAttack.HeavyAttack();
            //putting atacks in a list for easy access and giving them to player objects, probably also very inefficient
            Attack[] attackList = {lightAttack, heavyAttack};
            player1.attacks = attackList;
            player2.attacks = attackList;
        }

        //game update loop
        protected override void Update(GameTime gameTime)
        {
            //default check to exit game on esc
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (player1.health <= 0 || player2.health <= 0)
            {
                player1.GetControllerState();
                if (player1.gamePadState.Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    player1.health = 1000;
                    player2.health = 1000;
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
            player1.matrix = Matrix.CreateTranslation(player1.position);
            player2.matrix = Matrix.CreateTranslation(player2.position);

            base.Update(gameTime);
        }

        //game render loop
        protected override void Draw(GameTime gameTime)
        {
            if (player1.health <= 0 || player2.health <= 0)
            {
                GraphicsDevice.Clear(Color.White);
                spriteBatch.Begin();
                if (player1.health <= 0)
                {
                    spriteBatch.DrawString(gameOverFont, "Player 2 Wins", new Vector2(760, 540), Color.Black);
                } else
                {
                    spriteBatch.DrawString(gameOverFont, "Player 1 Wins", new Vector2(760, 540), Color.Black);

                }
                spriteBatch.DrawString(gameOverSubFont, "Press Space or A To Continue", new Vector2(680, 600), Color.Black);
                spriteBatch.End();

            } else
            {
                //rendering background as specific shade of blue
                GraphicsDevice.Clear(Color.CornflowerBlue);
                //rendering using method
                DrawModel(player1.model, player1.matrix, view, projection);
                DrawModel(player2.model, player2.matrix, view, projection);
                DrawModel(player1.model, player2.hitbBR, view, projection);
                DrawModel(player1.model, player2.hitbTL, view, projection);
                DrawModel(player1.model, player2.atthitbBR, view, projection);
                DrawModel(player1.model, player2.atthitbTL, view, projection);
                DrawModel(player1.model, player1.hitbBR, view, projection);
                DrawModel(player1.model, player1.hitbTL, view, projection);
                DrawModel(player1.model, player1.atthitbBR, view, projection);
                DrawModel(player1.model, player1.atthitbTL, view, projection);

                //drawing values for development 
                spriteBatch.Begin();
                spriteBatch.DrawString(font, player1.position.ToString(), new Vector2(100, 100), Color.Black);
                spriteBatch.DrawString(font, player1.speedY.ToString(), new Vector2(100, 120), Color.Black);
                spriteBatch.DrawString(font, player1.speedX.ToString(), new Vector2(100, 140), Color.Black);
                spriteBatch.DrawString(font, player1.onFloor.ToString(), new Vector2(100, 160), Color.Black);
                spriteBatch.DrawString(font, player1.facing.ToString(), new Vector2(100, 180), Color.Black);
                spriteBatch.DrawString(font, player1.contrlScheme.ToString(), new Vector2(100, 200), Color.Black);
                spriteBatch.DrawString(font, player2.contrlScheme.ToString(), new Vector2(100, 220), Color.Black);
                spriteBatch.DrawString(font, player1.health.ToString(), new Vector2(100, 240), Color.Black);

                spriteBatch.End();
            }



            base.Draw(gameTime);
        }


        //method to render a specific model
        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }
    }

}
