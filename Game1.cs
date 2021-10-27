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
            //checking if player is on floor
            player1.onFloor = player1.position.Y <= 0;
            player2.onFloor = player2.position.Y <= 0;

            //changing player speed value if player presses A while on floor (used in player collision method)
            if (player1.onFloor)
            {
                if (player1.gamePadState.Buttons.A == ButtonState.Pressed)
                {
                    player1.speed = 1;

                } else
                {
                    player1.speed = 0;
                    player1.position.Y = 0;
                }
            } else
            {
                player1.speed = player1.speed - 0.05f;
            }
            if (player2.onFloor)
            {
                if (player2.gamePadState.Buttons.A == ButtonState.Pressed)
                {
                    player2.speed = 1;

                } else
                {
                    player2.speed = 0;
                    player2.position.Y = 0;
                }
            } else
            {
                player2.speed = player2.speed - 0.05f;
            }

            player1.position = PlayerCollision(player1.position, player1.speed, player1.gamePadX, player1.onFloor, arenaSize, speedMultiplier);
            player2.position = PlayerCollision(player2.position, player2.speed, player2.gamePadX, player2.onFloor, arenaSize, speedMultiplier);

            //translatting player posttition to matrix for rendering
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
            spriteBatch.DrawString(font, player1.speed.ToString(), new Vector2(100, 120), Color.Black);
            spriteBatch.DrawString(font, player1.onFloor.ToString(), new Vector2(100, 140), Color.Black);
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
        //method that checks player collision and updates position
        private Vector3 PlayerCollision(Vector3 playerPos, float playerSpeed, float gamePadX, bool onFloor, Vector3 arena, float speed)
        {
            playerPos += new Vector3(gamePadX * speed, 0, 0);
            //player movement and collision checking
            if (gamePadX > 0 && playerPos.X > 0)
            {
                if (playerPos.X > arena.X)
                {
                    playerPos += new Vector3(arena.X - playerPos.X, 0, 0);
                }
            }
            else if (gamePadX < 0 && playerPos.X < 0)
            {
                if (playerPos.X < -arena.X)
                {
                    playerPos += new Vector3(-arena.X - playerPos.X, 0, 0);
                }
            }
            playerPos += new Vector3(0, playerSpeed, 0);
            return playerPos;
        }
    }
    //player class (2 instances created a start), holds player specific info
    public class Player
    {
        public Vector3 position;
        public float health;
        public Matrix matrix;
        public bool onFloor;
        public float speed = 0;
        public Model model;
        public GamePadState gamePadState;
        public float gamePadX;
        public float gamePadY;

    }
}
