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

        // player
        protected Player player;

        // map
        protected Map level;

        // mouse states, used for looking around 
        MouseState mState;

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
            if(mouseDisplacement > 0)
            {
                double oldDirX = player.direction.X;
                player.direction.X = (float) (player.direction.X * Math.Cos(-rotSpeed * mouseDisplacement) - player.direction.Y * Math.Sin(-rotSpeed * mouseDisplacement));
                player.direction.Y = (float)(oldDirX * Math.Sin(-rotSpeed * mouseDisplacement) + player.direction.Y * Math.Cos(-rotSpeed * mouseDisplacement));
                double oldPlaneX = player.cameraPlane.X;
                player.cameraPlane.X = (float)(player.cameraPlane.X * Math.Cos(-rotSpeed * mouseDisplacement) - player.cameraPlane.Y * Math.Sin(-rotSpeed * mouseDisplacement));
                player.cameraPlane.Y = (float)(oldPlaneX * Math.Sin(-rotSpeed * mouseDisplacement) + player.cameraPlane.Y * Math.Cos(-rotSpeed * mouseDisplacement));
            }
            // rotate left
            else if(mouseDisplacement < 0)
            {
                double oldDirX = player.direction.X;
                player.direction.X = (float)(player.direction.X * Math.Cos(-rotSpeed * mouseDisplacement) - player.direction.Y * Math.Sin(-rotSpeed * mouseDisplacement));
                player.direction.Y = (float)(oldDirX * Math.Sin(-rotSpeed * mouseDisplacement) + player.direction.Y * Math.Cos(-rotSpeed * mouseDisplacement));
                double oldPlaneX = player.cameraPlane.X;
                player.cameraPlane.X = (float)(player.cameraPlane.X * Math.Cos(-rotSpeed * mouseDisplacement) - player.cameraPlane.Y * Math.Sin(-rotSpeed * mouseDisplacement));
                player.cameraPlane.Y = (float)(oldPlaneX * Math.Sin(-rotSpeed * mouseDisplacement) + player.cameraPlane.Y * Math.Cos(-rotSpeed * mouseDisplacement));
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

            spriteBatch.Begin();
            Raycast(player, level, spriteBatch);  // draw geometry
            // spriteBatch.DrawString()
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
                if (drawStart < 0)
                    drawStart = 0;
                int drawEnd = lineHeight / 2 + SCREEN_HEIGHT / 2;
                if (drawEnd >= SCREEN_HEIGHT)
                    drawEnd = SCREEN_HEIGHT - 1;

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

                // draw the pixels of the stripe as a vertical line
                b.Draw(pix, new Rectangle(x, drawStart, 1, drawEnd - drawStart), col);
            }
        }
    }
}
