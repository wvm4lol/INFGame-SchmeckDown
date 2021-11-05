using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace INFGame
{
    public class Game1 : Game
    {
        //variable declaring
        //camera variables
        private Matrix view;
        private Matrix projection;
        //other variables
        private float speedMultiplier;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Vector3 arenaSize;
        private SpriteFont font;
        Player player1 = new Player();
        Player player2 = new Player();
        private Model scene;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this); //device properties
            Content.RootDirectory = "Content"; //asset location
            //window settings
            IsMouseVisible = false; 
            graphics.PreferredBackBufferWidth = 1920; 
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player1.position = new Vector3(-5, 0, 0);
            player2.position = new Vector3(5, 0, 0);
            view = Matrix.CreateLookAt(new Vector3(0, 5, 20), new Vector3(0, 5, 0), Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1920f / 1080f, 0.1f, 100f);
            speedMultiplier = 0.4f;
            arenaSize = new Vector3(20, 0, 0);
        }

        //load assets
        protected override void LoadContent()
        {
            player1.model = Content.Load<Model>("TestCube");
            player2.model = Content.Load<Model>("TestCube");
            font = Content.Load<SpriteFont>("File");

        }

        //game update loop
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //saving controller state
            player1.gamePadState = GamePad.GetState(PlayerIndex.One);
            player2.gamePadState = GamePad.GetState(PlayerIndex.Two);
            player1.gamePadX = player1.gamePadState.ThumbSticks.Left.X;
            player1.gamePadY = player1.gamePadState.ThumbSticks.Left.Y;
            player2.gamePadX = player2.gamePadState.ThumbSticks.Left.X;
            player2.gamePadY = player2.gamePadState.ThumbSticks.Left.Y;
            player1.OnFloor();
            player2.OnFloor(); 



            player1.PlayerSpeedX(); //method to calculate player speed
            player1.PlayerCollision(arenaSize); //method to update player position
            player1.SetPlayerHitBox(); //Object method that sets 2 vectors for the 2 corners of the hitbox
            player2.PlayerSpeedX();
            player2.PlayerCollision(arenaSize);
            player2.SetPlayerHitBox(); 

            //translating player position to matrix for rendering
            player1.matrix = Matrix.CreateTranslation(player1.position);
            player2.matrix = Matrix.CreateTranslation(player2.position);

            base.Update(gameTime);
        }

        //game render loop
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            //rendering using method
            DrawModel(player1.model, player1.matrix, view, projection);
            DrawModel(player2.model, player2.matrix, view, projection);
            //drawing position, speed and onFloor bool to screen
            spriteBatch.Begin();
            spriteBatch.DrawString(font, player1.position.ToString(), new Vector2(100, 100), Color.Black);
            spriteBatch.DrawString(font, player1.speedY.ToString(), new Vector2(100, 120), Color.Black);
            spriteBatch.DrawString(font, player1.speedX.ToString(), new Vector2(100, 140), Color.Black);
            spriteBatch.DrawString(font, player1.onFloor.ToString(), new Vector2(100, 160), Color.Black);
            spriteBatch.End();


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
