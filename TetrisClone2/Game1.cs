using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TetrisClone2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D block;
        List<int[,]> pieces = new List<int[,]>();
        Color[] tetronimoColors = {
                Color.Transparent,
                Color.Orange,
                Color.Blue,
                Color.Red,
                Color.LightSkyBlue,
                Color.Yellow,
                Color.Magenta,
                Color.LimeGreen

            };
        const int boardWidth = 10;
        const int boardHeight = 20;
        const int blockSize = 20;
        Vector2 boardLocation = Vector2.Zero; // Board location on screen
        int[,] board = new int[boardWidth, boardHeight];

        int[,] spawnedPiece;
        Vector2 spawnedPieceLocation;

        int stepTime = 300;
        int elapsedTime = 0;
        int keyboardElapsedTime = 0;


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
            block = Content.Load<Texture2D>("block");
            
            // TODO: use this.Content to load your game content here
            // Load pieces list with the seven tetrominoes
            // Piece I
            pieces.Add(new int[4, 4] {
            {0, 0, 0, 0},
            {1, 1, 1, 1 },
            {0, 0, 0, 0},
            {0, 0, 0, 0}
            });
            // Piece J
            pieces.Add(new int[3, 3] {
            {0, 0, 1},
            {1, 1, 1},
            {0, 0, 0}
            });
            // Piece O
            pieces.Add(new int[2, 2] {
            {1, 1},
            {1, 1}
            });
            // Piece S
            pieces.Add(new int[3, 3] {
            {0, 1, 1},
            {1, 1, 0},
            {0, 0, 0}
            });
            // Piece T
            pieces.Add(new int[3, 3] {
            {0, 1, 0},
            {1, 1, 1},
            {0, 0, 0}
            });
            // Piece Z
            pieces.Add(new int[3, 3] {
            {1, 1, 0},
            {0, 1, 1},
            {0, 0, 0}
            });

            // Initialize board to zero
            for (int y = 0; y < boardHeight; y++)
                for (int x = 0; x < boardWidth; x++)
                    board[x, y] = 0;

            SpawnPiece();

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            keyboardElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            KeyboardState ks = Keyboard.GetState();
            if (keyboardElapsedTime > 200)
            {
                if (ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.Right))
                {
                    // Create a new location that contains where we WANT to move the piece
                    Vector2 NewSpawnedPieceLocation = spawnedPieceLocation + new Vector2(ks.IsKeyDown(Keys.Left) ? -1 : 1, 0);
                    // Next, check to see if we can actually place the piece there
                    PlaceStates ps = CanPlace(board, spawnedPiece, (int)NewSpawnedPieceLocation.X, (int)NewSpawnedPieceLocation.Y);
                    if (ps == PlaceStates.CAN_PLACE)
                    {
                        spawnedPieceLocation = NewSpawnedPieceLocation;
                    }
                    keyboardElapsedTime = 0;
                }
                if (ks.IsKeyDown(Keys.Up))
                {
                    int[,] newSpawnedPiece = Rotate(spawnedPiece);
                    PlaceStates ps = CanPlace(board, newSpawnedPiece, (int)spawnedPieceLocation.X, (int)spawnedPieceLocation.Y);
                    if (ps == PlaceStates.CAN_PLACE)
                    {
                        spawnedPiece = newSpawnedPiece;
                    }
                    keyboardElapsedTime = 0;
                }
                if (ks.IsKeyDown(Keys.Down))
                {
                    elapsedTime = stepTime + 1;
                    keyboardElapsedTime = 175;
                }
            }

            if (elapsedTime > stepTime)
            {
                Vector2 newSpawnedPieceLocation = spawnedPieceLocation + new Vector2(0, 1);

                PlaceStates ps = CanPlace(board, spawnedPiece, (int)newSpawnedPieceLocation.X, (int)newSpawnedPieceLocation.Y);

                if (ps != PlaceStates.CAN_PLACE)
                {
                    Place(board, spawnedPiece, (int)spawnedPieceLocation.X, (int)spawnedPieceLocation.Y);
                    SpawnPiece();

                    ps = CanPlace(board, spawnedPiece, (int)spawnedPieceLocation.X, (int)spawnedPieceLocation.Y);
                    if (ps == PlaceStates.BLOCKED)
                    {
                        Exit();
                    }
                }
                else
                {
                    spawnedPieceLocation = newSpawnedPieceLocation;
                }
                elapsedTime = 0;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            // Draw gameboard
            for (int y = 0; y < boardHeight; y++)
                for (int x = 0; x < boardWidth; x++)
                {
                    Color tintColor = tetronimoColors[board[x,y]];

                    if (board[x,y] == 0)
                    {
                        tintColor = Color.FromNonPremultiplied(50, 50, 50, 50);
                    }   
                    spriteBatch.Draw(block,
                                    new Rectangle((int)boardLocation.X + x * blockSize,
                                                    (int)boardLocation.Y + y * blockSize,
                                                    blockSize, blockSize),
                                    new Rectangle(0, 0, 33, 32),
                                    tintColor);
                    
                }

            int dim = spawnedPiece.GetLength(0);
            for (int y = 0; y < dim; y++)
                for (int x = 0; x < dim; x++)
                {
                    if (spawnedPiece[x, y] != 0)
                    {
                        Color tintColor = tetronimoColors[spawnedPiece[x, y]];
                        spriteBatch.Draw(block,
                        new Rectangle((int)boardLocation.X + ((int)spawnedPieceLocation.X + x) * blockSize,
                        (int)boardLocation.Y + ((int)spawnedPieceLocation.Y + y) * blockSize,
                        blockSize,
                        blockSize),
                        new Rectangle(0, 0, 33, 32),
                        tintColor);
                    }
                }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void SpawnPiece()
        {
            Random rand = new Random();
            int colr = rand.Next(0, pieces.Count);
            spawnedPiece = (int[,])pieces[colr].Clone();
            int dim = spawnedPiece.GetLength(0);

            
            for (int x = 0; x < dim; x++)
                for (int y = 0; y<dim; y++)
                    spawnedPiece[x, y] *= (colr + 1);
                    
            spawnedPieceLocation = Vector2.Zero;
                
        }

        public PlaceStates CanPlace(int[,] board, int[,] piece, int x, int y)
        {
            int dim = piece.GetLength(0);

            for (int px =0; px < dim; px++)
                for (int py = 0; py < dim; py++)
                {
                    int coordx = x + px; // x is the offset of piece on screen
                    int coordy = y + py; // Coord x and coordy are the coordinates of a specific block of a tetromino on the screen

                    if (piece[px, py] != 0)
                    {
                        if ( coordx < 0 || coordx >= boardWidth)
                        {
                            return PlaceStates.OFF_SCREEN;
                        }

                        if ( coordy >= boardHeight || board[coordx, coordy] != 0)
                        {
                            return PlaceStates.BLOCKED;
                        }
                    }

                }

            return PlaceStates.CAN_PLACE;
        }

        public void RemoveCompletedLines(int[,] board)
        {
            for (int y = boardHeight - 1; y >= 0; y--)
            {

                bool isComplete = true;
                for (int x=0; x< boardWidth; x++)
                {
                    if (board[x,y] ==0)
                    {
                        isComplete = false;

                    }
                }

                if(isComplete)
                {
                    for (int yCopy = y; yCopy > 0; yCopy--)
                    {
                        for(int x=0; x < boardWidth; x++)
                        {
                            board[x, yCopy] = board[x, yCopy - 1];
                        }
                    }

                    y++;   
                }

            }
        }

        public void Place(int[,] board, int[,] piece, int x, int y)
        {

            int dim = piece.GetLength(0);

            for (int px = 0; px < dim; px++)
                for (int py = 0; py < dim; py++)
                {
                    int coordx = x + px; // x is the offset of piece on screen
                    int coordy = y + py; // Coord x and coordy are the coordinates of a specific block of a tetromino on the screen

                    if (piece[px, py] != 0)
                    {
                        board[coordx, coordy] = piece[px, py];
                    }

                }
            RemoveCompletedLines(board);

        }

        public int[,] Rotate(int[,] piece)
        {
            int dim = piece.GetLength(0);
            int[,] rpiece = new int[dim,dim];

            for (int x = 0; x < dim; x++)
                for (int y = 0; y < dim; y++)
                {                  
                     rpiece[y,x] = piece[dim - 1 - x, y];                    
                }

            return rpiece;
        }








    }

    public enum PlaceStates
    {
        BLOCKED,
        CAN_PLACE,
        OFF_SCREEN
    }
}
