using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace PASS3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Random number setup
        static Random rng = new Random();

        //General storage
        //Store colours
        Color[] randomColour = new Color[9] {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet, Color.Magenta, Color.Aqua};
        //Store random colour
        int randomColourNum;

        //Store window size
        int screenWidth;
        int screenHeight;

        //Store the scale
        double scale;

        //Store keyboard state
        KeyboardState kb;
        KeyboardState prevKb;

        //Store the mouse state
        MouseState mouse;
        MouseState prevMouse;

        //Store gamestates
        const int MENU = 0;
        const int GAME = 1;
        const int PAUSE = 2;
        const int END = 3;
        int gamestate = 0;
        
        //Store arrow directions
        const int UP = 0;
        const int DOWN = 1;
        const int LEFT = 2;
        const int RIGHT = 3;
        int selectedArrow;
        
        //Store max number of balls
        const int NUMBALLS = 100;

        //Stuff related to sprites and their positions
        Texture2D blank;
        Texture2D playBtnImg;
        Texture2D exitBtnImg;
        Texture2D ballImg;
        
        //Store arrow sprites
        Texture2D[] arrow = new Texture2D [4];

        //Fonts
        SpriteFont font;
        
        //UI
        Rectangle playBtnRec;
        Rectangle exitBtnRec;
        Rectangle playAreaRec;
        
        
        //Game stuff
        Rectangle horizontalRowRec;
        Rectangle verticalRowRec;
        
        Rectangle[] ballRec = new Rectangle[NUMBALLS];
        Rectangle arrowRec;
        Rectangle centerRec;
        
        //Vector2 for fonts
        Vector2 textLoc;
        
        //Movement stuff
        Vector2[] ballPos = new Vector2[NUMBALLS];                 //Stores the ball’s true position
        float[] speed = new float [NUMBALLS];              //Stores the speed/update movement rate
        //int dirX = 1;       //Stores the x direction, which will be one of 1(right), -1(left) or 0(stopped)
        //int dirY = 0;      //Stores the y direction, which will be one of 1(down), -1(up) or 0(stopped)
        int[] dirX = new int[]  { 0, 0, 1, -1};       //Stores the x direction, which will be one of 1(right), -1(left) or 0(stopped)
        int[] dirY = new int[] { 1, -1, 0, 0};      //Stores the y direction, which will be one of 1(down), -1(up) or 0(stopped)
        
        //Store the directions and spawns for the paths
        const int TOPPATH = 0;
        const int BOTTOMPATH = 1;
        const int LEFTPATH = 2;
        const int RIGHTPATH = 3;
        Rectangle[] possibleSpawns = new Rectangle[4];
        int[] path = new int [NUMBALLS];
        //TEST
        int hits;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            //Make window borderless
            Window.IsBorderless = true;

            //Set resolution 
            this.graphics.PreferredBackBufferWidth = 500;
            this.graphics.PreferredBackBufferHeight = 500;
            this.graphics.ApplyChanges();
            Window.AllowUserResizing = true;

            //Generate random colour
            randomColourNum = rng.Next(0,randomColour.Length);
            
            //Make mouse visible
            IsMouseVisible = true;

            //Get the window size
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //Load sprites
            blank = Content.Load<Texture2D>("Sprites/Blank");
            playBtnImg = Content.Load<Texture2D>("Sprites/PlayBtn1");
            exitBtnImg = Content.Load<Texture2D>("Sprites/ExitBtn1");
            ballImg = Content.Load<Texture2D>("Sprites/ball");
            arrow[UP] = Content.Load<Texture2D>("Sprites/UpArrow");
            arrow[DOWN] = Content.Load<Texture2D>("Sprites/DownArrow");
            arrow[LEFT] = Content.Load<Texture2D>("Sprites/LeftArrow");
            arrow[RIGHT] = Content.Load<Texture2D>("Sprites/RightArrow");
            
            //Load fonts
            font = Content.Load<SpriteFont>("Fonts/Font");

            playBtnRec = new Rectangle(((screenWidth / 2) - (playBtnImg.Width / 2)), ((screenHeight / 2) - (playBtnImg.Height / 2) - 50), playBtnImg.Width, playBtnImg.Height);
            exitBtnRec = new Rectangle(((screenWidth / 2) - (exitBtnImg.Width / 2)), ((screenHeight / 2) - (exitBtnImg.Height / 2) + 50), exitBtnImg.Width, exitBtnImg.Height);
            playAreaRec = new Rectangle(((screenWidth - screenHeight) / 2), 0, screenHeight, screenHeight);
            horizontalRowRec = new Rectangle(((screenWidth - screenHeight) / 2), ((screenHeight / 2) - (screenHeight / 16)), screenHeight, (screenHeight / 8));
            verticalRowRec = new Rectangle(((screenWidth / 2) - (screenHeight / 16)), 0, (screenHeight / 8), screenHeight);
            
            //Load vector2
            textLoc = new Vector2(0, (int)(100 * scale));

            for (int i = 0; i < NUMBALLS; i++)
            {
                path[i] = rng.Next(0,4);
                speed[i] = (rng.Next(10, 31) / 10);
            }
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

            // TODO: use this.Content to load your game content here
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
            //Update keyboard states
            prevKb = kb;
            kb = Keyboard.GetState();

            //Update mouse states
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Update window size
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //Make a hitbox for the center
            centerRec = new Rectangle(Convert.ToInt32((screenHeight / 2) - (screenHeight / 24)), Convert.ToInt32((screenHeight / 2) - screenHeight / 24), screenHeight / 12, screenHeight / 12);

            //Update stuff based on gamestate
            switch (gamestate)
            {
                case MENU:

                    if (playBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
                    {
                        //Generate random colour
                        randomColourNum = rng.Next(0, randomColour.Length);
                        CalcPos();
                        gamestate = GAME;
                        
                        
                    }
                    if (exitBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
                        Exit();

                    //Change colour on right click
                    if (mouse.RightButton == ButtonState.Pressed && mouse != prevMouse)
                        randomColourNum = rng.Next(0, randomColour.Length);

                    //Logic to change window size
                    if (mouse.ScrollWheelValue > prevMouse.ScrollWheelValue)
                    {
                        this.graphics.PreferredBackBufferWidth += 20;
                        this.graphics.PreferredBackBufferHeight += 20;
                        this.graphics.ApplyChanges();
                    }
                    if (mouse.ScrollWheelValue < prevMouse.ScrollWheelValue)
                    {
                        this.graphics.PreferredBackBufferWidth -= 20;
                        this.graphics.PreferredBackBufferHeight -= 20;
                        this.graphics.ApplyChanges();
                    }
                    
                    //Update locations
                    playBtnRec = new Rectangle(((screenWidth / 2) - (playBtnImg.Width / 2)), ((screenHeight / 2) - (playBtnImg.Height / 2) - 50), playBtnImg.Width, playBtnImg.Height);
                    exitBtnRec = new Rectangle(((screenWidth / 2) - (exitBtnImg.Width / 2)), ((screenHeight / 2) - (exitBtnImg.Height / 2) + 50), exitBtnImg.Width, exitBtnImg.Height);
                    
                    break;

                case GAME:
                    //Pause game
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb)
                    {
                        gamestate = PAUSE;
                    }
                    //Change colour when pressing space
                    if (kb.IsKeyDown(Keys.Space) && kb != prevKb)
                    {
                        randomColourNum = rng.Next(0, randomColour.Length);
                        hits ++;
                    }

                        
                    
                    this.graphics.PreferredBackBufferWidth = Math.Min(screenHeight, screenWidth);
                    this.graphics.PreferredBackBufferHeight = Math.Min(screenHeight, screenWidth);
                    this.graphics.ApplyChanges();

                    if (screenWidth < 500)
                    {
                        this.graphics.PreferredBackBufferWidth = 500;
                        this.graphics.PreferredBackBufferHeight = 500;
                        this.graphics.ApplyChanges();
                    }

                    //Control arrow
                    if (kb.IsKeyDown(Keys.W))
                        selectedArrow = UP;
                    
                    if (kb.IsKeyDown(Keys.S))
                        selectedArrow = DOWN;
                    
                    if (kb.IsKeyDown(Keys.A))
                        selectedArrow = LEFT;
                    
                    if (kb.IsKeyDown(Keys.D))
                        selectedArrow = RIGHT;
                    
                    //Update ball position
                    //Calculate the speed in each direction and move the ball
                    for (int i = 0; i < NUMBALLS; i++)
                    {
                        ballPos[i].X = ballRec[i].X;
                        ballPos[i].Y = ballRec[i].Y;
                        ballPos[i].X = ballPos[i].X + (dirX[path[i]] * speed[i]);
                        ballPos[i].Y = ballPos[i].Y + (dirY[path[i]] * speed[i]);
                        ballRec[i].X = (int)ballPos[i].X;
                        ballRec[i].Y = (int)ballPos[i].Y;
                        
                    }
                    
                    /*ballPos[0].X = ballRec[0].X;
                    ballPos[0].Y = ballRec[0].Y;
                    ballPos[0].X = ballPos[0].X + (dirX[path[0]] * speed);
                    ballPos[0].Y = ballPos[0].Y + (dirY[path[0]] * speed);
                    ballRec[0].X = (int)ballPos[0].X;
                    ballRec[0].Y = (int)ballPos[0].Y;
                    
                    ballPos[1].X = ballRec[1].X;
                    ballPos[1].Y = ballRec[1].Y;
                    ballPos[1].X = ballPos[1].X + (dirX[path[1]] * speed);
                    ballPos[1].Y = ballPos[1].Y + (dirY[path[1]] * speed);
                    ballRec[1].X = (int)ballPos[1].X;
                    ballRec[1].Y = (int)ballPos[1].Y;*/


                    //Check for ball death
                    CheckBallDeath();

                    break;

                case PAUSE:
                    //unpause game
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb)
                    {
                        gamestate = GAME;
                    }
                    break;

                case END:

                    break;

            }

                    // TODO: Add your update logic here

                    base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();
            switch (gamestate)
            {
                case MENU:
                    GraphicsDevice.Clear(randomColour[randomColourNum]);
                    spriteBatch.Draw(playBtnImg, playBtnRec, Color.White);
                    spriteBatch.Draw(exitBtnImg, exitBtnRec, Color.White);
                    break;

                case GAME:
                    GraphicsDevice.Clear(randomColour[randomColourNum]);
                    spriteBatch.Draw(blank, playAreaRec, Color.Black * 0.5f);
                    spriteBatch.Draw(blank, horizontalRowRec, Color.Black * 0.5f);
                    spriteBatch.Draw(blank, verticalRowRec, Color.Black * 0.5f);
                    for (int i = 0; i < NUMBALLS; i++)
                    {
                        spriteBatch.Draw(ballImg, ballRec[i], Color.White);
                    }
                    
                    spriteBatch.Draw(blank, centerRec, Color.Black);
                    spriteBatch.Draw(arrow[selectedArrow], arrowRec, randomColour[randomColourNum]);

                    
                    //TEST
                    Console.WriteLine(hits);
                    
                    //spriteBatch.DrawString((font * scale), "test", textLoc, Color.White);
                    
                    //Draw center hitbox
                    
                    break;

                case PAUSE:
                    GraphicsDevice.Clear(Color.Black);
                    break;

                case END:

                    break;
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        void CalcPos()
        {
            //Scale all the stuff based on the window size
            //Update scale
            scale = (double)(screenHeight) / 500;

            playAreaRec = new Rectangle(((screenWidth - screenHeight) / 2), 0, screenHeight, screenHeight);
            horizontalRowRec = new Rectangle(((screenWidth - screenHeight) / 2), ((screenHeight / 2) - (screenHeight / 24)), screenHeight, (screenHeight / 12));
            verticalRowRec = new Rectangle(((screenWidth / 2) - (screenHeight / 24)), 0, (screenHeight / 12), screenHeight);

            //Balls
            possibleSpawns[0] = new Rectangle((int)((screenHeight / 2) - 8 * scale), 0, (int)(16 * scale), (int)(16 * scale));
            possibleSpawns[1] = new Rectangle((int)((screenHeight / 2) - 8 * scale), (int)(screenHeight - (16 * scale)), (int)(16 * scale), (int)(16 * scale));
            possibleSpawns[2] = new Rectangle(0, (int)((screenHeight / 2) - 8 * scale), (int)(16 * scale), (int)(16 * scale));
            possibleSpawns[3] = new Rectangle((int)(screenHeight - (16 * scale)), (int)((screenHeight / 2) - 8 * scale), (int)(16 * scale), (int)(16 * scale));
            arrowRec = new Rectangle(Convert.ToInt32((screenHeight / 2) - (screenHeight / 24)), Convert.ToInt32((screenHeight / 2) - screenHeight / 24), screenHeight / 12, screenHeight / 12);

            for (int i = 0; i < NUMBALLS; i++)
            {
                ballRec[i] = possibleSpawns[path[i]];
            }
        }

        void CheckBallDeath()
        {
            
            for (int i = 0; i < NUMBALLS; i++)
            {
                if (centerRec.Contains(ballRec[i]))
                {
                    //Regenerate random spawn
                    path[i] = rng.Next(0,4);
                    //Set ball rec to new random spawn
                    ballRec[i] = possibleSpawns[path[i]];
                    //Regenerate random colour
                    randomColourNum = rng.Next(0,randomColour.Length);
                    //Regenerate random speed
                    speed[i] = (rng.Next(10, 31) / 10);
                
                }
            }
        }

    }
}
