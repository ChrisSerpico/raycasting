using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Raycasting
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // CONSTANTS
        /////

        // Screen width and height
        public static int SCREEN_WIDTH = 512 * 2;
        public static int SCREEN_HEIGHT = 384 * 2;

        // speed modifiers (multiplicative)
        public static double SPEED_MOD = 5.0;
        public static double ROTATION_MOD = .05;

        // Texture width and height
        public static int TEXTURE_WIDTH = 72;
        public static int TEXTURE_HEIGHT = 72;

        // the number of textures used in-game
        public static int NUM_TEXTURES = 8;

        // VARIABLES
        /////

        // Graphics Stuff
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // used to calculate fps 
        double time = 0;  // time of current frame
        double oldTime = 0;  // time of previous frame
        double frameTime;

        // pixel texture used to draw lines
        Texture2D pix; 

        // player representation
        protected Player player;

        // map representation 
        protected Map level;

        // mouse states, used for looking around 
        MouseState mState;

        // texture array, each element is a separate square texture
        Texture2D[] texture = new Texture2D[NUM_TEXTURES];

        // sprite font for writing text
        protected SpriteFont font; 

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Height and Width initialization
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // initialize player and level
            player = new Player();
            level = new Map();

            // get initial mouse states
            mState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load textures
            pix = Content.Load<Texture2D>("dot");
            texture[0] = Content.Load<Texture2D>("terrain/grayvat");
            texture[1] = Content.Load<Texture2D>("terrain/redwall");
            texture[2] = Content.Load<Texture2D>("terrain/damagedgraywall");
            texture[3] = Content.Load<Texture2D>("terrain/graywall");
            texture[4] = Content.Load<Texture2D>("terrain/bluewall");
            texture[5] = Content.Load<Texture2D>("terrain/graybookshelf");
            texture[6] = Content.Load<Texture2D>("terrain/graywindow");
            texture[7] = Content.Load<Texture2D>("terrain/graywarningdoor");

            // load font
            font = Content.Load<SpriteFont>("General");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // update mouse state
            mState = Mouse.GetState();
            Mouse.SetPosition(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // timing for input and fps counters
            oldTime = time;
            time = gameTime.TotalGameTime.TotalMilliseconds;
            frameTime = (time - oldTime) / 1000.0; 

            // calculate speed modifiers
            double moveSpeed = frameTime * SPEED_MOD;
            double rotSpeed = frameTime * ROTATION_MOD;  // rotation speed 

            // TODO: split input code out into its own method
            // Input
            if(Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (level.area[(int)(player.position.X + player.direction.X * moveSpeed), (int)player.position.Y] == 0)
                    player.position.X += player.direction.X * (float) moveSpeed;
                if (level.area[(int)player.position.X, (int)(player.position.Y + player.direction.Y * moveSpeed)] == 0)
                    player.position.Y += player.direction.Y * (float) moveSpeed;

            }
            if(Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (level.area[(int)(player.position.X - player.direction.X * moveSpeed), (int)player.position.Y] == 0)
                    player.position.X -= player.direction.X * (float)moveSpeed;
                if (level.area[(int)player.position.X, (int)(player.position.Y - player.direction.Y * moveSpeed)] == 0)
                    player.position.Y -= player.direction.Y * (float)moveSpeed;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (level.area[(int)(player.position.X + player.direction.Y * moveSpeed), (int)player.position.Y] == 0)
                    player.position.X += player.direction.Y * (float)moveSpeed;
                if (level.area[(int)player.position.X, (int)(player.position.Y - player.direction.X * moveSpeed)] == 0)
                    player.position.Y -= player.direction.X * (float)moveSpeed;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (level.area[(int)(player.position.X - player.direction.Y * moveSpeed), (int)player.position.Y] == 0)
                    player.position.X -= player.direction.Y * (float)moveSpeed;
                if (level.area[(int)player.position.X, (int)(player.position.Y + player.direction.X * moveSpeed)] == 0)
                    player.position.Y += player.direction.X * (float)moveSpeed;
            }
            // both camera direction and camera plane must be rotated
            double mouseDisplacement = mState.X - SCREEN_WIDTH / 2;
            // rotate right
            double oldDirX = player.direction.X;
            player.direction.X = (float) (player.direction.X * Math.Cos(-rotSpeed * mouseDisplacement) - player.direction.Y * Math.Sin(-rotSpeed * mouseDisplacement));
            player.direction.Y = (float) (oldDirX * Math.Sin(-rotSpeed * mouseDisplacement) + player.direction.Y * Math.Cos(-rotSpeed * mouseDisplacement));
            double oldPlaneX = player.cameraPlane.X;
            player.cameraPlane.X = (float) (player.cameraPlane.X * Math.Cos(-rotSpeed * mouseDisplacement) - player.cameraPlane.Y * Math.Sin(-rotSpeed * mouseDisplacement));
            player.cameraPlane.Y = (float) (oldPlaneX * Math.Sin(-rotSpeed * mouseDisplacement) + player.cameraPlane.Y * Math.Cos(-rotSpeed * mouseDisplacement));


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // BEGIN DRAWING
            spriteBatch.Begin();

            Raycast(player, level, spriteBatch);  // draw geometry

            // calculate fps
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            spriteBatch.DrawString(font, frameRate.ToString(), new Vector2(0, 0), Color.White);
            
            // END DRAWING
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // The raycast loop
        // It goes through the entire screen and calculates the screen view in vertical slices
        // using the player's vector data
        protected void Raycast(Player p, Map m, SpriteBatch b)
        {
            // loop through the entire screen, slicing it into SCREEN_HEIGHT sized slices along the x-axis
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                // calculate ray position and direction
                double cameraX = 2 * x / ((double) SCREEN_WIDTH) - 1; // this makes the x-coordinate -1 for the far left side of the screen
                Vector2 rayPos = p.position;
                Vector2 rayDir = new Vector2((float) (p.direction.X + p.cameraPlane.X * cameraX), 
                    (float) (p.direction.Y + p.cameraPlane.Y * cameraX));

                // which box of the map we're in
                int mapX = (int) rayPos.X;
                int mapY = (int) rayPos.Y; 

                // length of ray from current position to next x or y-side
                Vector2 sideDist = new Vector2();

                // length of ray from one x or y-side to next x or y-side
                Vector2 deltaDist = new Vector2((float) Math.Sqrt(1 + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X)), 
                    (float) Math.Sqrt(1 + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y)));
                double perpWallDist;

                // what direction to step in x or y direction (either 1 or -1)
                int stepX;
                int stepY;

                int hit = 0; // have we hit a wall yet?
                int side = 0; // was it a north-south wall or an east-west wall?

                // calculate step and initial sideDist
                if (rayDir.X < 0)
                {
                    stepX = -1;
                    sideDist.X = (rayPos.X - mapX) * deltaDist.X;
                }
                else
                {
                    stepX = 1;
                    sideDist.X = (mapX + 1.0f - rayPos.X) * deltaDist.X;
                }
                if (rayDir.Y < 0)
                {
                    stepY = -1;
                    sideDist.Y = (rayPos.Y - mapY) * deltaDist.Y;
                }
                else
                {
                    stepY = 1;
                    sideDist.Y = (mapY + 1.0f - rayPos.Y) * deltaDist.Y;
                }

                // perform digital differential analysis
                while (hit == 0)
                {
                    // jump to the next map square in x-direction OR in y-direction
                    if (sideDist.X < sideDist.Y)
                    {
                        sideDist.X += deltaDist.X;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDist.Y += deltaDist.Y;
                        mapY += stepY;
                        side = 1;
                    }
                    // check to see if the ray has hit a wall yet
                    if (m.area[mapX, mapY] > 0)
                        hit = 1;
                }

                // calculate distance projected on camera plane (oblique distance gives fisheye effect)
                if (side == 0)
                    perpWallDist = Math.Abs((mapX - rayPos.X + (1 - stepX) / 2) / rayDir.X);
                else
                    perpWallDist = Math.Abs((mapY - rayPos.Y + (1 - stepY) / 2) / rayDir.Y);

                // calculate height of line to draw on screen
                int lineHeight = Math.Abs((int)(SCREEN_HEIGHT / perpWallDist));

                // calculate lowest and highest pixel to fill in current stripe 
                int drawStart = (-lineHeight) / 2 + SCREEN_HEIGHT / 2;
                //if (drawStart < 0)
                //    drawStart = 0;
                int drawEnd = lineHeight / 2 + SCREEN_HEIGHT / 2;
                //if (drawEnd >= SCREEN_HEIGHT)
                //    drawEnd = SCREEN_HEIGHT - 1;

                /* NOT USED WITH TEXTURED RAYCASTER
                // choose wall color
                Color col;

                switch(m.area[mapX, mapY])
                {
                    case 1:
                        {
                            col = Color.Red;
                            break;
                        }
                    case 2:
                        {
                            col = Color.Green;
                            break;
                        }
                    case 3:
                        {
                            col = Color.Blue;
                            break;
                        }
                    case 4:
                        {
                            col = Color.White;
                            break;
                        }
                    default:
                        {
                            col = Color.Yellow;
                            break;
                        }
                }
                
                // give x and y sides different brightnesses
                if (side == 1)
                {
                    col = Color.Multiply(col, 0.5f);
                }
                */

                // texturing calculations
                int texNum = level.area[mapX, mapY] - 1;  // subtract 1 so that the texture at texture[0] can be used

                // calculate value of wallX
                double wallX; // where exactly the wall was hit
                if (side == 1)
                    wallX = rayPos.X + ((mapY - rayPos.Y + (1 - stepY) / 2) / rayDir.Y) * rayDir.X; 
                else
                    wallX = rayPos.Y + ((mapX - rayPos.X + (1 - stepX) / 2) / rayDir.X) * rayDir.Y;
                wallX -= Math.Floor(wallX);

                // x coordinate of the texture 
                int texX = (int) (wallX * (double)(TEXTURE_WIDTH));
                if (side == 0 && rayDir.X > 0)
                    texX = TEXTURE_WIDTH - texX - 1;
                else if (side == 1 && rayDir.Y < 0)
                    texX = TEXTURE_WIDTH - texX - 1;

                // draw the pixels of the stripe as a vertical line
                b.Draw(texture[texNum], new Rectangle(x, drawStart, 1, drawEnd - drawStart), new Rectangle(texX, 0, 1, TEXTURE_HEIGHT), Color.White);
            }
        }
    }
}
