using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TicTacToe.GameObjects;
using TicTacToe.AI;
using System.Collections.Generic;

namespace ui
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D oTexture;
        private Texture2D xTexture;
        private Texture2D humanTexture;
        private Texture2D cpuTexture;
        private Texture2D menuTexture;
        private Texture2D lineTexture;
        private SpriteFont font;

        private GameBoard board;

        private MouseState previousMouseState;
        private KeyboardState previousKeyboardState;

        private const float scale = 1;
        private Vector2 spriteSize;
        private Vector2 cellSize;
        private Vector2 boardSize;

        private Vector2 spriteOrigin;
        private Vector2 boardOrigin;

        public enum GameState
        {
            MainMenu,
            MachineTurn,
            HumanTurn,
            GameOver
        }

        private GameState state;
        private GameState gameEntryState;
        private double timer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth  = 800;
            graphics.PreferredBackBufferHeight = 600;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            xTexture = Content.Load<Texture2D>("TicTacToeX");
            oTexture = Content.Load<Texture2D>("TicTacToeO");
            menuTexture = Content.Load<Texture2D>("menu");

            lineTexture = new Texture2D(GraphicsDevice, 1, 1);
            lineTexture.SetData<Color>(new Color[] { Color.White });

            font = Content.Load<SpriteFont>("Arial");

            spriteSize = new Vector2(xTexture.Width, xTexture.Height) * scale;
            cellSize = new Vector2(spriteSize.X * scale, spriteSize.Y * scale);
            boardSize = cellSize * 3;

            spriteOrigin = new Vector2(0.0f, 0.0f);
            boardOrigin = new Vector2((graphics.PreferredBackBufferWidth / 2) - (boardSize.X / 2),
                                      (graphics.PreferredBackBufferHeight / 2) - (boardSize.Y / 2));

            gameEntryState = GameState.HumanTurn;
            humanTexture = xTexture;
            cpuTexture = oTexture;
            EnterState(GameState.MainMenu);
        }

        protected override void UnloadContent()
        {
        }

        private Player GetCurrentPlayer(GameState state)
        {
            Player current;
            switch (state)
            {
                case GameState.MachineTurn:
                    current = Player.CPU;
                    break;
                case GameState.HumanTurn:
                    current = Player.Human;
                    break;
                default:
                    current = Player.None;
                    break;
            }

            return current;
        }
        #region State machine
        private void EnterState(GameState state)
        {
            this.state = state;
            switch (state)
            {
                case GameState.MainMenu:
                    state = GameState.MainMenu;
                    board = new GameBoard();
                    break;
                case GameState.MachineTurn:
                    timer = 1.0f;
                    break;
                case GameState.HumanTurn:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }
        }

        private void LeaveState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    break;
                case GameState.MachineTurn:
                    break;
                case GameState.HumanTurn:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }
        }

        private int Between(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private bool IsValidMouseClick(MouseState mouseState)
        {
            bool isXValid = mouseState.Position.X >= 0 && mouseState.Position.X <= graphics.GraphicsDevice.Viewport.Width;
            bool isYValid = mouseState.Position.Y >= 0 && mouseState.Position.Y <= graphics.GraphicsDevice.Viewport.Height;
            return isXValid && isYValid;
        }

        private bool KeyWasPressed(KeyboardState previousKeyboardState, Keys key)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            return previousKeyboardState != null && !previousKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key);
        }

        private void UpdateState(GameTime gameTime)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    if (KeyWasPressed(previousKeyboardState, Keys.NumPad1))
                    {
                        Minimax.easy = true;
                    }
                    else if (KeyWasPressed(previousKeyboardState, Keys.NumPad2))
                    {
                        Minimax.easy = false;
                    }
                    if (KeyWasPressed(previousKeyboardState, Keys.NumPad4))
                    {
                        gameEntryState = GameState.HumanTurn;
                    } 
                    else if (KeyWasPressed(previousKeyboardState, Keys.NumPad5))
                    {
                        gameEntryState = GameState.MachineTurn;
                    }
                    if (KeyWasPressed(previousKeyboardState, Keys.NumPad7))
                    {
                        humanTexture = xTexture;
                        cpuTexture = oTexture;
                    }
                    else if (KeyWasPressed(previousKeyboardState, Keys.NumPad8))
                    {
                        humanTexture = oTexture;
                        cpuTexture = xTexture;
                    }
                    if (KeyWasPressed(previousKeyboardState, Keys.Enter))
                    {
                        NextTurn(gameEntryState);
                    }
                    break;
                case GameState.MachineTurn:
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer <= 0)
                    {
                        LeaveState(state);
                        board = Minimax.Play(board, GetCurrentPlayer(state));
                        NextTurn(GameState.HumanTurn);
                    }
                    break;
                case GameState.HumanTurn:
                    MouseState currentMouseState = Mouse.GetState();
                    if (IsValidMouseClick(currentMouseState) &&
                        currentMouseState.LeftButton == ButtonState.Pressed && 
                        previousMouseState != null && 
                        previousMouseState.LeftButton == ButtonState.Released)
                    {
                        LeaveState(state);
                        Vector2 mouseVector = currentMouseState.Position.ToVector2();
                        Vector2 estimatedPosition = (mouseVector - boardOrigin) / cellSize;
                        int x = Between((int)estimatedPosition.X, 0, board.Width - 1);
                        int y = Between((int)estimatedPosition.Y, 0, board.Height - 1);
                        board.Play(x, y, GetCurrentPlayer(state));
                        NextTurn(GameState.MachineTurn);
                    }
                    break;
                case GameState.GameOver:
                    if (KeyWasPressed(previousKeyboardState, Keys.Enter))
                    {
                        LeaveState(state);
                        EnterState(GameState.MainMenu);
                    }
                    break;
                default:
                    break;
            }
            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();
        }

        private void NextTurn(GameState nextState)
        {
            if (board.IsOver())
            {
                EnterState(GameState.GameOver);
            }
            else
            {
                EnterState(nextState);
            }
        }

        private void DrawState(GameTime gameTime)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(menuTexture, new Rectangle(250, 100, 300, 400), Color.White);
                    break;
                case GameState.MachineTurn:
                    spriteBatch.DrawString(font, "CPU pensando...", new Vector2(10.0f, 10.0f), Color.White);
                    DrawGrid();
                    DrawBoardContent();
                    break;
                case GameState.HumanTurn:
                    spriteBatch.DrawString(font, "Sua vez!", new Vector2(10.0f, 10.0f), Color.White);
                    DrawGrid();
                    DrawBoardContent();
                    break;
                case GameState.GameOver:
                    spriteBatch.DrawString(font, "Ganhador: " + board.GetWinner().ToString(), new Vector2(10.0f, 10.0f), Color.White);
                    spriteBatch.DrawString(font, "Pressione ENTER para iniciar novo jogo", new Vector2(10.0f, 50.0f), Color.White);
                    DrawGrid();
                    DrawBoardContent();
                    break;
                default:
                    break;
            }
        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateState(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                                BlendState.AlphaBlend,
                                SamplerState.PointClamp);
            DrawState(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBoardContent()
        {
            for (int line = 0; line < board.Height; line += 1)
            {
                for (int column = 0; column < board.Width; column += 1)
                {
                    Texture2D textureToUse;
                    switch (board[line, column])
                    {
                        case Player.Human:
                            textureToUse = humanTexture;
                            break;
                        case Player.CPU:
                            textureToUse = cpuTexture;
                            break;
                        default:
                            continue;
                    }
                    spriteBatch.Draw(
                        textureToUse,
                        boardOrigin + (cellSize * new Vector2(line, column)), //position
                        null,                                         //dest rectangle
                        null,                                         //source rectange,
                        spriteOrigin,                                 //origin
                        0.0f,                                         //rotation
                        Vector2.One * scale,                          //scale
                        new Color(1.0f, 1.0f, 1.0f, 1.0f),            //color
                        SpriteEffects.None,
                        0.0f);                                        //layer depth
                }
            }
        }

        private void DrawGrid()
        {
            DrawLine(spriteBatch, boardOrigin + new Vector2(cellSize.X, 0), boardOrigin + new Vector2(cellSize.X, boardSize.Y));
            DrawLine(spriteBatch, boardOrigin + new Vector2(cellSize.X * 2, 0), boardOrigin + new Vector2(cellSize.X * 2, boardSize.Y));
            DrawLine(spriteBatch, boardOrigin + new Vector2(0, cellSize.Y), boardOrigin + new Vector2(boardSize.X, cellSize.Y));
            DrawLine(spriteBatch, boardOrigin + new Vector2(0, cellSize.Y * 2), boardOrigin + new Vector2(boardSize.X, cellSize.Y * 2));
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(lineTexture,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                new Color(1.0f, 1.0f, 1.0f, 1.0f), //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}
