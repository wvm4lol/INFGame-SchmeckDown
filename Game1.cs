using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace INFGame
{
    public class Game1 : Game
    {
        //variable declaring
        private GraphicsDeviceManager graphics;
        private Vector3 playerOnePos;
        private Vector3 PlayerTwoPos;
        private Matrix playerOne;
        private Model modelOne;
        private Matrix playerTwo;
        private Model modelTwo;
        private Matrix view;
        private Matrix projection;
        private float playerSpeed;
        private float gamePadXpOne;
        private float gamePadYpOne;
        private float gamePadXpTwo;
        private float gamePadYpTwo;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {

            base.Initialize();
            playerOne = Matrix.CreateTranslation(new Vector3(-5, 0, 0));
            playerTwo = Matrix.CreateTranslation(new Vector3(5, 0, 0));
            view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1920f / 1080f, 0.1f, 100f);
    }

        protected override void LoadContent()
        {
         

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            base.Draw(gameTime);
        }

        //code om een bepaald model te renderen
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
