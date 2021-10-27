using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace INFGame
{
    public class Game1 : Game
    {
        //variable declaring
        //player one variables
        private Vector3 playerOnePos;
        private Matrix playerOne;
        private Model modelOne;
        private GamePadState gamePadOne;
        private float pOneGPadX;
        private float pOneGPadY;
        //player two variables
        private Vector3 playerTwoPos;
        private Matrix playerTwo;
        private Model modelTwo;
        private GamePadState gamePadTwo;
        private float pTwoGPadX;
        private float pTwoGPadY;
        //camera variables
        private Matrix view;
        private Matrix projection;
        //other variables
        private float playerSpeed;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spritebatch;


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
            spritebatch = new SpriteBatch(GraphicsDevice);
            playerOnePos = new Vector3(-5, 0, 0);
            playerTwoPos = new Vector3(5, 0, 0);
            view = Matrix.CreateLookAt(new Vector3(0, 5, 20), new Vector3(0, 5, 0), Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1920f / 1080f, 0.1f, 100f);
            playerSpeed = 0.4f;
        }

        //load assets
        protected override void LoadContent()
        {
            modelOne = Content.Load<Model>("TestCube");
            modelTwo = Content.Load<Model>("TestCube");


        }

        //game update loop
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //saving controller state
            gamePadOne = GamePad.GetState(PlayerIndex.One);
            gamePadTwo = GamePad.GetState(PlayerIndex.Two);
            playerOnePos += new Vector3(gamePadOne.ThumbSticks.Left.X * playerSpeed, gamePadOne.ThumbSticks.Left.Y * playerSpeed, 0);
            playerOne = Matrix.CreateTranslation(playerOnePos);
            playerTwoPos += new Vector3(gamePadTwo.ThumbSticks.Left.X * playerSpeed, gamePadTwo.ThumbSticks.Left.Y * playerSpeed, 0);
            playerTwo = Matrix.CreateTranslation(playerTwoPos);
            base.Update(gameTime);
        }

        //game render loop
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            DrawModel(modelOne, playerOne, view, projection);
            DrawModel(modelTwo, playerTwo, view, projection);



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
