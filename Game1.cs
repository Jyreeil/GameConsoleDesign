using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BloomPostprocess;

namespace KlaxMatchGameConsoleDesign
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private const int Width = 5;
        private const int Height = 5;
        private const int MinimumMatchLength = 3;
        private const int MaximumColors = 5;  //No more than 5 unless you add to the colors array too!
        private static readonly ConsoleColor[] Colors = new[]
                                             {
                                                 ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.Green,
                                                 ConsoleColor.Blue, ConsoleColor.Magenta, ConsoleColor.Cyan
                                             };

        private static int[,] _pixelArt = new int[11, 11]
            { { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0 }, { 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0 }, { 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1 },
                { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1 }, { 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1 }, { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1 }, { 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1 },
           { 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0 }, { 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0 }, { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 } };
        private static int[,] _position = new int[Width, Height]
            { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
        private static int[,] _grid = new int[Width, Height]
            { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };

        
        private static int[,] _matches = new int[Width, Height];

        public struct ParticleData
        {
            public float StartTime;
            public float EndAge;
            public Vector2 BeginPosition;
            public Vector2 Accelaration;
            public Vector2 Direction;
            public Vector2 Position;
            public float Scaling;
            public Color ModColor;
        }

        private static readonly Random Random = new Random();

        private static int _score;
        float currentTime = 0f;
        private static int position = 1;
        private static int paddleColor = 0;
        private static int randPos = 0;
        private static int color = 0;
        private static int gameMode = 0;
        private static int _lives = 3;
        bool notStoppedYet = true;
        bool notPlayedYet = true;

        //soundeffects
        SoundEffect soundEffect1;

        SoundEffect soundEffect2;

        SoundEffect soundEffect3;

        SoundEffect soundEffect4;

        SoundEffect soundEffect5;

        SoundEffect soundEffect6;

        //variables for volume of sound effects

        float volume = 0.1f;
        float pitch = 0.0f;
        float pan = 0.0f;

        //background music

        Song backgroundMusic1;

        Song backgroundMusic2;

        Song backgroundMusic3;

        //other stuff
        SpriteFont spriteFont;

        SpriteBatch spriteBatch;

        GraphicsDeviceManager graphics;
        
        BloomComponent bloom;
        
        Texture2D dummyTexture;

        Texture2D explosionTexture;

        //models
        Model blackBrick;
        Model blueBrick;
        Model brickTable;
        Model cyanBrick;
        Model greenBrick;
        Model magentaBrick;
        Model paddle;
        Model redBrick;
        Model colorBrick;
        Model model;

        private Matrix world = Matrix.CreateTranslation(new Vector3(-15, 1, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(15, 15, 15), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);

        int bloomSettingsIndex = 0;

        bool isInverted = false;

        List<ParticleData> particleList = new List<ParticleData>();

        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();

        public Game1()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            bloom = new BloomComponent(this);

            Components.Add(bloom);
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
            MediaPlayer.IsRepeating = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("hudFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
            soundEffect1 = Content.Load<SoundEffect>("002");
            soundEffect2 = Content.Load<SoundEffect>("004");
            soundEffect3 = Content.Load<SoundEffect>("009");
            soundEffect4 = Content.Load<SoundEffect>("012");
            soundEffect5 = Content.Load<SoundEffect>("015");
            soundEffect6 = Content.Load<SoundEffect>("017");
            backgroundMusic1 = Content.Load<Song>("bensound-buddy");
            backgroundMusic2 = Content.Load<Song>("bensound-cute");
            backgroundMusic3 = Content.Load<Song>("bensound-littleidea");
            blackBrick = Content.Load<Model>("BlackBrick");
            blueBrick = Content.Load<Model>("BlueBrick");
            brickTable = Content.Load<Model>("BrickTable");
            cyanBrick = Content.Load<Model>("CyanBrick");
            greenBrick = Content.Load<Model>("GreenBrick");
            magentaBrick = Content.Load<Model>("MagentaBrick");
            paddle = Content.Load<Model>("Paddle");
            redBrick = Content.Load<Model>("RedBrick");
            colorBrick = Content.Load<Model>("ColorBrick");
            explosionTexture = Content.Load<Texture2D>("explosion");
            MediaPlayer.Play(backgroundMusic1);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            if (gameMode == 1)
            {
                currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (currentTime >= 1.5f)
                {
                    timer(gameTime);
                    currentTime = 0;
                }

                if (particleList.Count > 0)
                    UpdateParticles(gameTime);
            }
            if (_lives <= 0 && notStoppedYet == true)
            {
                gameMode = 0;

                MediaPlayer.Stop();
                MediaPlayer.Play(backgroundMusic1);
                notStoppedYet = false;
                paddleColor = 0;
                _score = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        _position[x, y] = 0;
                        _grid[x, y] = 0;
                    }
                }
            }
            if (_score >= 200 && notPlayedYet == true)
            {
                gameMode = 4;
                MediaPlayer.Stop();
                MediaPlayer.Play(backgroundMusic1);
                notPlayedYet = false;
                _score = 0;
                paddleColor = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        _position[x, y] = 0;
                        _grid[x, y] = 0;
                    }
                }
            }
            



            HandleInput();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            Viewport viewport = device.Viewport;

            bloom.BeginDraw();
            if (gameMode == 1)
            {
                if (isInverted == false)
                {
                    PrintGrid(0, 0, 0);
                }
                else
                {
                    PrintGrid(9, 7, 5);
                }
                DrawExplosion();
                DrawOverlayText();
            }
            if (gameMode == 0)
            {
                DrawMenuText();
            }
            if (gameMode == 2)
            {
                DrawCreditsText();
            }
            if (gameMode == 4)
            {
                DrawWinText();
            }


            base.Draw(gameTime);

        }

        private void HandleInput()
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                if (gameMode == 1)
                {
                    Exit();
                }
            }
            if (currentKeyboardState.IsKeyDown(Keys.A) &&
                 lastKeyboardState.IsKeyUp(Keys.A))
            {
                if (gameMode == 1)
                {
                    MovePaddle(1);
                }
                if (gameMode == 0)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Play(backgroundMusic2);
                    gameMode = 2;
                }

            }
            if (currentKeyboardState.IsKeyDown(Keys.D) &&
                 lastKeyboardState.IsKeyUp(Keys.D))
            {
                if (gameMode == 1)
                {
                    MovePaddle(2);
                }
                if (gameMode == 0)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Play(backgroundMusic3);
                    gameMode = 1;
                    _lives = 3;
                    notStoppedYet = true;
                }

            }
            if (currentKeyboardState.IsKeyDown(Keys.Space) &&
                 lastKeyboardState.IsKeyUp(Keys.Space))
            {
                if (gameMode == 1)
                {
                    DropBlock();
                }
                
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) &&
                 lastKeyboardState.IsKeyUp(Keys.S))
            {
                if (gameMode == 1)
                {
                    bloomSettingsIndex = (bloomSettingsIndex + 1) %
                                     BloomSettings.PresetSettings.Length;

                    bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                    bloom.Visible = true;
                }
                
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) &&
                 lastKeyboardState.IsKeyUp(Keys.D))
            {
                if (gameMode == 1)
                {
                    drawPixelArt();
                }
                
            }
            if (currentKeyboardState.IsKeyDown(Keys.Q) &&
                 lastKeyboardState.IsKeyUp(Keys.Q))
            {
                if (gameMode == 1)
                {
                    view = Matrix.CreateLookAt(new Vector3(-15, 15, 15), new Vector3(0, 0, 0), Vector3.UnitY);
                    isInverted = true;
                }
                
            }
            if (currentKeyboardState.IsKeyDown(Keys.E) &&
                 lastKeyboardState.IsKeyUp(Keys.E))
            {
                if (gameMode == 1)
                {
                    view = Matrix.CreateLookAt(new Vector3(15, 15, 15), new Vector3(0, 0, 0), Vector3.UnitY);
                    isInverted = false;
                }
                if (gameMode == 0)
                {
                    Exit();
                }
                if (gameMode == 2)
                {
                    gameMode = 0;
                    MediaPlayer.Stop();
                    MediaPlayer.Play(backgroundMusic1);
                }
                if (gameMode == 4)
                {
                    gameMode = 0;

                    MediaPlayer.Stop();
                    MediaPlayer.Play(backgroundMusic1);
                    notPlayedYet = true;
                }

            }
        }
        
        int count = 0;

        void timer(GameTime gameTime)
        {
            bool didJustMove = false;
            //bool letGridMove = false;
            Console.Write("works");
            Console.Write(count);
            if (count >= 3)
            {
                RandomGrid();
                count = 0;
            }
            count++;
            for (int x = 0; x <= Width - 1; x++)
            {
                for (int i = 0; i < Height; i++)
                {
                    didJustMove = false;
                    if (_position[x, i] != 0 && i != 0)
                    {
                        _position[x, i - 1] = _position[x, i];
                        _position[x, i] = 0;
                        soundEffect1.Play(volume, pitch, pan);
                        if (i <= 1)
                        {
                            didJustMove = true;
                        }
                    }
                    if (i == 0 && _position[x, i] != 0 && position != x && didJustMove == false)
                    {
                        if (_grid[x, Height - 1] == 0)
                        {
                            _grid[x, Height - 1] = _position[x, i];
                            _position[x, i] = 0;
                        }else
                        {
                            _position[x, i] = 0;
                            _lives -= 1;
                        }
                        soundEffect3.Play(volume, pitch, pan);
                        didJustMove = true;
                    }
                    if (i == 0 && _position[x, i] != 0 && x == position && didJustMove == false)
                    {
                        if (paddleColor == 0)
                        {
                            paddleColor = _position[x, i];
                            _position[x, i] = 0;
                        }else
                        {
                            _position[x, i] = 0;
                            _lives -= 1;
                        }
                        //color = 0;
                        soundEffect4.Play(volume, pitch, pan);
                    }
                }
            }
            for (int x = 0; x <= Width - 1; x++)
            {
                for (int i = 0; i < Height; i++)
                {
                    if (_grid[x, i] != 0 && i != 0 && _grid[x, i - 1] == 0)
                    {
                        _grid[x, i - 1] = _grid[x, i];
                        _grid[x, i] = 0;
                        soundEffect2.Play(volume, pitch, pan);
                    }
                }
            }
            MatchGrid(gameTime);
        }

        private void DropBlock()
        {
            if (paddleColor != 0)
            {
                if (_grid[position, 4] == 0)
                {
                    _grid[position, 4] = paddleColor;
                    //gridColor = paddleColor;
                    paddleColor = 0;
                    soundEffect5.Play(volume, pitch, pan);
                }else
                {
                    paddleColor = 0;
                    _lives -= 1;
                }
            }
            
        }

        private static void MovePaddle(int direction)
        {
            if (direction == 1 && position > 0)
            {
                position -= 1;
            }
            if (direction == 2 && position < 4)
            {
                position += 1;
            }
        }

        private void MatchGrid(GameTime gameTime)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (y <= 2 && (_grid[x, y + 1] == _grid[x, y] && _grid[x, y + 2] == _grid[x, y] && _grid[x, y] != 0 ||
                            (_grid[x, y] == 6 && _grid[x, y + 2] == _grid[x, y + 1] && _grid[x, y] != 0 && _grid[x, y + 2] != 0 && _grid[x, y + 1] != 0) ||
                            (_grid[x, y + 2] == 6 && _grid[x, y] == _grid[x, y + 1] && _grid[x, y] != 0 && _grid[x, y + 2] != 0 && _grid[x, y + 1] != 0) ||
                            (_grid[x, y] == _grid[x, y + 2] && _grid[x, y + 1] == 6 && _grid[x, y] != 0 && _grid[x, y + 2] != 0 && _grid[x, y + 1] != 0)))
                    {
                        
                        _matches[x, y + 1] += 1;
                        _matches[x, y] += 1;
                        _matches[x, y + 2] += 1;
                        if (y <= 1 && _grid[x, y + 3] == _grid[x, y])
                        {
                            _matches[x, y + 3] += 1;
                            _matches[x, y] -= 1;
                            _grid[x, y] = 6;
                            _score += 10;
                        }
                    }
                    if (y != 0)
                    {
                        if (x <= 2 && ((_grid[x + 1, y] == _grid[x, y] && _grid[x + 2, y] == _grid[x, y] && _grid[x, y] != 0) ||
                            (_grid[x, y] == 6 && _grid[x + 2, y] == _grid[x + 1, y] && _grid[x, y] != 0 && _grid[x + 2, y] != 0 && _grid[x + 1, y] != 0) ||
                            (_grid[x + 2, y] == 6 && _grid[x, y] == _grid[x + 1, y] && _grid[x, y] != 0 && _grid[x + 2, y] != 0 && _grid[x + 1, y] != 0) ||
                            (_grid[x, y] == _grid[x + 2, y] && _grid[x + 1, y] == 6 && _grid[x, y] != 0 && _grid[x + 2, y] != 0 && _grid[x + 1, y] != 0))
                            && _grid[x + 1, y - 1] != 0 && _grid[x + 2, y - 1] != 0 && _grid[x, y - 1] != 0)
                        {
                            _matches[x + 1, y] += 1;
                            _matches[x, y] += 1;
                            _matches[x + 2, y] += 1;
                            if (x <= 1 && _grid[x + 3, y] == _grid[x, y])
                            {
                                _matches[x + 3, y] += 1;
                                _matches[x, y] -= 1;
                                _grid[x, y] = 6;
                                _score += 10;
                            }
                        }
                    }
                    else if (y == 0 && x <= 2 && (_grid[x + 1, y] == _grid[x, y] && _grid[x + 2, y] == _grid[x, y] && _grid[x, y] != 0 ||
                            (_grid[x, y] == 6 && _grid[x + 2, y] == _grid[x + 1, y] && _grid[x, y] != 0 && _grid[x + 2, y] != 0 && _grid[x + 1, y] != 0) ||
                            (_grid[x + 2, y] == 6 && _grid[x, y] == _grid[x + 1, y] && _grid[x, y] != 0 && _grid[x + 2, y] != 0 && _grid[x + 1, y] != 0) ||
                            (_grid[x, y] == _grid[x + 2, y] && _grid[x + 1, y] == 6 && _grid[x, y] != 0 && _grid[x + 2, y] != 0 && _grid[x + 1, y] != 0)))
                    {
                        _matches[x + 1, y] += 1;
                        _matches[x, y] += 1;
                        _matches[x + 2, y] += 1;
                        if (x <= 1 && _grid[x + 3, y] == _grid[x, y])
                        {
                            _matches[x + 3, y] += 1;
                            _matches[x, y] -= 1;
                            _grid[x, y] = 6;
                            _score += 10;
                        }
                    }
                    if (y != 0)
                    {
                        if (x <= 2 && y <= 2 && ((_grid[x + 1, y + 1] == _grid[x, y] && _grid[x + 2, y + 2] == _grid[x, y] && _grid[x, y] != 0) ||
                            (_grid[x, y] == 6 && _grid[x + 2, y + 2] == _grid[x + 1, y + 1] && _grid[x, y] != 0 && _grid[x + 2, y + 2] != 0 && _grid[x + 1, y + 1] != 0) ||
                            (_grid[x, y] == _grid[x + 2, y + 2] && _grid[x + 1, y + 1] == 6 && _grid[x, y] != 0 && _grid[x + 2, y + 2] != 0 && _grid[x + 1, y + 1] != 0))
                            && _grid[x + 1, y] != 0 && _grid[x + 2, y + 1] != 0 && _grid[x, y - 1] != 0)
                        {
                            _matches[x + 1, y + 1] += 1;
                            _matches[x, y] += 1;
                            _matches[x + 2, y + 2] += 1;
                            if (y <= 1 && x <= 1 && _grid[x + 3, y + 3] == _grid[x, y])
                            {
                                _matches[x + 3, y + 3] += 1;
                                _matches[x, y] -= 1;
                                _grid[x, y] = 6;
                                _score += 10;
                            }
                        }
                    }
                    else if (y == 0 && x <= 2 && y <= 2 && ((_grid[x + 1, y + 1] == _grid[x, y] && _grid[x + 2, y + 2] == _grid[x, y] && _grid[x, y] != 0) ||
                           (_grid[x, y] == 6 && _grid[x + 2, y + 2] == _grid[x + 1, y + 1] && _grid[x, y] != 0 && _grid[x + 2, y + 2] != 0 && _grid[x + 1, y + 1] != 0) ||
                           (_grid[x, y] == _grid[x + 2, y + 2] && _grid[x + 1, y + 1] == 6 && _grid[x, y] != 0 && _grid[x + 2, y + 2] != 0 && _grid[x + 1, y + 1] != 0))
                           && _grid[x + 1, y] != 0 && _grid[x + 2, y + 1] != 0)
                    {
                        _matches[x + 1, y + 1] += 1;
                        _matches[x, y] += 1;
                        _matches[x + 2, y + 2] += 1;
                        if (y <= 1 && x <= 1 && _grid[x + 3, y + 3] == _grid[x, y])
                        {
                            _matches[x + 3, y + 3] += 1;
                            _matches[x, y] -= 1;
                            _grid[x, y] = 6;
                            _score += 10;
                        }
                    }

                    if (y != 0)
                    {
                        if (x >= 2 && y <= 2 && ((_grid[x - 1, y + 1] == _grid[x, y] && _grid[x - 2, y + 2] == _grid[x, y] && _grid[x, y] != 0) ||
                            (_grid[x, y] == 6 && _grid[x - 2, y + 2] == _grid[x - 1, y + 1] && _grid[x, y] != 0 && _grid[x - 2, y + 2] != 0 && _grid[x - 1, y + 1] != 0) ||
                            (_grid[x - 2, y + 2] == 6 && _grid[x, y] == _grid[x - 1, y + 1] && _grid[x, y] != 0 && _grid[x - 2, y + 2] != 0 && _grid[x - 1, y + 1] != 0) ||
                            (_grid[x, y] == _grid[x - 2, y + 2] && _grid[x - 1, y + 1] == 6 && _grid[x, y] != 0 && _grid[x - 2, y + 2] != 0 && _grid[x - 1, y + 1] != 0))
                        && _grid[x - 1, y] != 0 && _grid[x - 2, y + 1] != 0 && _grid[x, y - 1] != 0)
                        {
                            _matches[x - 1, y + 1] += 1;
                            _matches[x, y] += 1;
                            _matches[x - 2, y + 2] += 1;
                            if (y <= 1 && x >= 3 && _grid[x - 3, y + 3] == _grid[x, y])
                            {
                                _matches[x - 3, y + 3] += 1;
                                _matches[x, y] -= 1;
                                _grid[x, y] = 6;
                                _score += 10;
                            }
                        }
                    }
                    else if (y == 0 && x >= 2 && y <= 2 && ((_grid[x - 1, y + 1] == _grid[x, y] && _grid[x - 2, y + 2] == _grid[x, y] && _grid[x, y] != 0) ||
                           (_grid[x, y] == 6 && _grid[x - 2, y + 2] == _grid[x - 1, y + 1] && _grid[x, y] != 0 && _grid[x - 2, y + 2] != 0 && _grid[x - 1, y + 1] != 0) ||
                           (_grid[x - 2, y + 2] == 6 && _grid[x, y] == _grid[x - 1, y + 1] && _grid[x, y] != 0 && _grid[x - 2, y + 2] != 0 && _grid[x - 1, y + 1] != 0) ||
                           (_grid[x, y] == _grid[x - 2, y + 2] && _grid[x - 1, y + 1] == 6 && _grid[x, y] != 0 && _grid[x - 2, y + 2] != 0 && _grid[x - 1, y + 1] != 0))
                           && _grid[x - 1, y] != 0 && _grid[x - 2, y + 1] != 0)
                    {
                        _matches[x - 1, y + 1] += 1;
                        _matches[x, y] += 1;
                        _matches[x - 2, y + 2] += 1;
                        if (y <= 1 && x >= 3 && _grid[x - 3, y + 3] == _grid[x, y])
                        {
                            _matches[x - 3, y + 3] += 1;
                            _matches[x, y] -= 1;
                            _grid[x, y] = 6;
                            _score += 10;
                        }
                    }
                }
            }
            RemoveBlocks(gameTime);
        }

        private void RemoveBlocks(GameTime gameTime)
        {
            //Remove any blocks marked as matches and move the remaining ones down
            for (int x = 0; x < Width; x++)
            {
                int copyOffset = 0;

                for (int y = 0; y < Height; y++)
                {
                    if (_matches[x, y] > 0)
                    {
                        AddExplosion(new Vector2(100, 100), 10, 80.0f, 2000.0f, gameTime);
                        copyOffset++;
                        _score += 5;
                        _matches[x, y] = 0;
                    }
                    else
                    {
                        //Copy the current cell to its new location
                        _grid[x, y - copyOffset] = _grid[x, y];
                    }
                }
                //Zero out however many blocks we just moved down
                for (int y = Height - copyOffset; y < Height; y++)
                {
                    _grid[x, y] = 0;
                }
            }
        }

        private void RandomGrid()
        {
            color = Random.Next(1, MaximumColors);
            randPos = Random.Next(1, Width - 1);
            _position[randPos, Height - 1] = color;
            soundEffect6.Play(volume, pitch, pan);
        }

        private void ColorPicker(int color)
        {
            switch (color)
            {
                case 0:
                    model = blackBrick;
                    break;
                case 1:
                    model = redBrick;
                    break;
                case 2:
                    model = greenBrick;
                    break;
                case 3:
                    model = blueBrick;
                    break;
                case 4:
                    model = magentaBrick;
                    break;
                case 5:
                    model = cyanBrick;
                    break;
                case 6:
                    model = colorBrick;
                    break;
            }
        }

        private void PrintGrid(int gridMod, int paddleMod, int positionMod)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (_position[x, y] != 0)
                    {
                        
                        world = Matrix.CreateTranslation(new Vector3(x - 13 + positionMod, 2, -y));
                        ColorPicker(_position[x, y]);
                        spriteBatch.Begin();
                        DrawModel(model, world, view, projection);
                        spriteBatch.End();
                    }
                }
            }
            for (int x = 0; x < Width + 3; x++)
            {
                if (x == position)
                {
                    ColorPicker(paddleColor);
                    spriteBatch.Begin();
                    world = Matrix.CreateTranslation(new Vector3(x - 15 + paddleMod, 0, 0));
                    DrawModel(paddle, world, view, projection);
                    if(paddleColor != 0)
                    {
                        world = Matrix.CreateTranslation(new Vector3(x - 15 + paddleMod, 1, 0));
                        DrawModel(model, world, view, projection);
                    }
                    spriteBatch.End();
                }
            }

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = Width - 1; x >= 0; x--)
                {
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    ColorPicker(_grid[x, y]);
                    spriteBatch.Begin();
                    world = Matrix.CreateTranslation(new Vector3(x - 17 + gridMod, y-6, 0));
                    if (_grid[x, y] != 0)
                    {
                        DrawModel(model, world, view, projection);
                    }
                    spriteBatch.End();
                }
            }
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    world = Matrix.CreateTranslation(new Vector3(x, y, 0));
                    ColorPicker(_pixelArt[x, y]);
                    spriteBatch.Begin();
                    DrawModel(model, world, view, projection);
                    spriteBatch.End();
                }
            }
        }

        void DrawOverlayText()
        {
            string text = "Space = drop block from paddle\n" +
                          "A and D = Move Paddle\n" +
                          "S = Bloom Settings\n" +
                          "Q and E = Change Camera Position\n" +
                          "X = Exit\n" +
                          "Score:  " + _score + "\n" +
                          "Lives:  " + _lives + "\n" +
                          "Match 3 in a row horizontally, vertically, or diagonally to earn points\n" +
                          "If you get 4 in a row, a special block will appear which matches with anything\n" +
                          "5 points for each matched block, and 10 bonus points for 4 matched blocks\n";

            spriteBatch.Begin();

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(10, 255), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(10, 254), Color.White);

            spriteBatch.End();
        }

        void DrawMenuText()
        {
            string text = "A = Credits\n" +
                          "D = Play\n" +
                          "E = Exit";

            spriteBatch.Begin();

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(400, 255), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(400, 254), Color.White);

            spriteBatch.End();
        }

        void DrawCreditsText()
        {
            string text = "Creator:  Jeremy Swartley\n" +
                          "Professor:  David Horachek\n" +
                          "Music:  http://www.bensound.com/  \n" +
                          "E = Back to menu";

            spriteBatch.Begin();

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(400, 255), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(400, 254), Color.White);

            spriteBatch.End();
        }

        void DrawWinText()
        {
            string text = "You Win!\n" +
                          "E = Back to menu";

            spriteBatch.Begin();

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(400, 255), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(400, 254), Color.White);

            spriteBatch.End();
        }

        void drawPixelArt()
        {
            //Rectangle rect;
            //Color color;
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    if ((x == 0 && y == 0) || (x == 1 && y == 0) || (x == 2 && y == 0) || (x == 8 && y == 0) || (x == 9 && y == 0) || (x == 10 && y == 0) ||
                        (x == 0 && y == 1) || (x == 1 && y == 1) || (x == 4 && y == 1) || (x == 5 && y == 1) || (x == 6 && y == 1) || (x == 9 && y == 1) ||
                        (x == 10 && y == 1) || (x == 0 && y == 2) || (x == 3 && y == 2) || (x == 4 && y == 2) || (x == 5 && y == 2) || (x == 6 && y == 2) ||
                        (x == 7 && y == 2) || (x == 10 && y == 2) || (x == 2 && y == 3) || (x == 3 && y == 3) || (x == 7 && y == 3) || (x == 8 && y == 3) ||
                        (x == 1 && y == 4) || (x == 2 && y == 4) || (x == 4 && y == 4) || (x == 5 && y == 4) || (x == 6 && y == 4) ||
                        (x == 8 && y == 4) || (x == 9 && y == 4) || (x == 1 && y == 5) || (x == 2 && y == 5) || (x == 3 && y == 5) || (x == 4 && y == 5) || (x == 6 && y == 5) ||
                        (x == 7 && y == 5) || (x == 8 && y == 5) || (x == 9 && y == 5) || (x == 1 && y == 6) || (x == 2 && y == 6) || (x == 4 && y == 6) ||
                        (x == 5 && y == 6) || (x == 6 && y == 6) || (x == 8 && y == 6) || (x == 9 && y == 6) || (x == 2 && y == 7) ||
                        (x == 3 && y == 7) || (x == 4 && y == 7) || (x == 5 && y == 7) || (x == 6 && y == 7) || (x == 7 && y == 7) || (x == 8 && y == 7) || (x == 0 && y == 8) || (x == 3 && y == 8) ||
                        (x == 4 && y == 8) || (x == 5 && y == 8) || (x == 6 && y == 8) || (x == 7 && y == 8) || (x == 10 && y == 8) || (x == 0 && y == 9) ||
                        (x == 1 && y == 9) || (x == 4 && y == 9) || (x == 5 && y == 9) || (x == 6 && y == 9) || (x == 9 && y == 9) || (x == 10 && y == 9) ||
                        (x == 0 && y == 10) || (x == 1 && y == 10) || (x == 2 && y == 10) || (x == 8 && y == 10) || (x == 9 && y == 10) || (x == 10 && y == 10))
                    {
                        _pixelArt[x, y] += 1;
                        if (_pixelArt[x, y] > 5)
                        {
                            _pixelArt[x, y] = 0;
                        }
                    }
                    if ((x == 3 && y == 0) || (x == 4 && y == 0) || (x == 5 && y == 0) || (x == 6 && y == 0) || (x == 7 && y == 0) || (x == 2 && y == 1) ||
                        (x == 3 && y == 1) || (x == 7 && y == 1) || (x == 8 && y == 1) || (x == 1 && y == 2) || (x == 2 && y == 2) || (x == 8 && y == 2) ||
                        (x == 9 && y == 2) || (x == 0 && y == 3) || (x == 1 && y == 3) || (x == 4 && y == 3) || (x == 5 && y == 3) || (x == 6 && y == 3) || (x == 9 && y == 3) ||
                        (x == 10 && y == 3) || (x == 0 && y == 4) || (x == 3 && y == 4) || (x == 7 && y == 4) || (x == 10 && y == 4) || (x == 0 && y == 5) ||
                        (x == 5 && y == 5) || (x == 10 && y == 5) || (x == 0 && y == 6) || (x == 3 && y == 6) || (x == 7 && y == 6) || (x == 10 && y == 6) || (x == 0 && y == 7) ||
                        (x == 1 && y == 7) || (x == 9 && y == 7) || (x == 10 && y == 7) || (x == 1 && y == 8) ||
                        (x == 2 && y == 8) || (x == 8 && y == 8) || (x == 9 && y == 8) || (x == 2 && y == 9) || (x == 3 && y == 9) || (x == 7 && y == 9) ||
                        (x == 8 && y == 9) || (x == 3 && y == 10) || (x == 4 && y == 10) || (x == 5 && y == 10) || (x == 6 && y == 10) || (x == 7 && y == 10))
                    {
                        _pixelArt[x, y] += 2;
                        if (_pixelArt[x, y] > 5)
                        {
                            _pixelArt[x, y] -= 5;
                        }
                    }
                    //rect.X = x * 10;
                    //rect.Y = 110 - y * 10 + 100;
                    //rect.Height = 10;
                    //rect.Width = 10;
                    //color = ColorPicker(_pixelArt[x, y]);
                    //spriteBatch.Begin();
                    //spriteBatch.Draw(dummyTexture, rect, color);
                    //spriteBatch.End();
                }
            }    
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                    effect.DirectionalLight0.Direction = new Vector3(-1, -1, -1);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 0.1f, 0);
                }

                mesh.Draw();
            }
        }

        private void AddExplosion(Vector2 explosionPos, int amountOfParticles, float size, float endAge, GameTime gameTime)
        {
            for (int i = 0; i < amountOfParticles; i++)
                AddExplosionParticle(explosionPos, size, endAge, gameTime);
        }

        private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float endAge, GameTime gameTime)
        {
            ParticleData particle = new ParticleData();

            particle.BeginPosition = explosionPos;
            particle.Position = particle.BeginPosition;

            particle.StartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            particle.EndAge = endAge;
            particle.Scaling = 0.25f;
            particle.ModColor = Color.White;

            float particleDistance = (float)Random.NextDouble() * explosionSize;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(Random.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement * 2.0f;
            particle.Accelaration = -particle.Direction;

            particleList.Add(particle);
        }

        private void UpdateParticles(GameTime gameTime)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            for (int i = particleList.Count - 1; i >= 0; i--)
            {
                ParticleData particle = particleList[i];
                float timeAlive = now - particle.StartTime;

                if (timeAlive > particle.EndAge)
                {
                    particleList.RemoveAt(i);
                }
                else
                {
                    float relAge = timeAlive / particle.EndAge;
                    particle.Position = 0.5f * particle.Accelaration * relAge * relAge + particle.Direction * relAge + particle.BeginPosition;

                    float invAge = 1.0f - relAge;
                    particle.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

                    Vector2 positionFromCenter = particle.Position - particle.BeginPosition;
                    float distance = positionFromCenter.Length();
                    particle.Scaling = (50.0f + distance) / 200.0f;

                    particleList[i] = particle;
                }
            }
        }

        private void DrawExplosion()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (int i = 0; i < particleList.Count; i++)
            {
                ParticleData particle = particleList[i];
                spriteBatch.Draw(explosionTexture, particle.Position, null, particle.ModColor, i, new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
            }
            spriteBatch.End();
        }
    }//end class
}
