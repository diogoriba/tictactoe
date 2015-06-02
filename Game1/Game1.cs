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
        private Texture2D menuTexture;
        private Texture2D menuSelectedTexture;
        private Texture2D lineTexture;
        private SpriteFont font;

        private MouseState previousMouseState;
        private KeyboardState previousKeyboardState;

        private const float scale = 1;
        private Vector2 spriteSize;
        private Vector2 cellSize;
        private Vector2 boardSize;

        private Vector2 spriteOrigin;
        private Vector2 boardOrigin;

        private StateMachine stateMachine;

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
            menuSelectedTexture = Content.Load<Texture2D>("menuselected");

            lineTexture = new Texture2D(GraphicsDevice, 1, 1);
            lineTexture.SetData<Color>(new Color[] { Color.White });

            font = Content.Load<SpriteFont>("Arial");

            spriteSize = new Vector2(xTexture.Width, xTexture.Height) * scale;
            cellSize = new Vector2(spriteSize.X * scale, spriteSize.Y * scale);
            boardSize = cellSize * 3;

            spriteOrigin = new Vector2(0.0f, 0.0f);
            boardOrigin = new Vector2((graphics.PreferredBackBufferWidth / 2) - (boardSize.X / 2),
                                        (graphics.PreferredBackBufferHeight / 2) - (boardSize.Y / 2));

            stateMachine = new StateMachine(graphics.GraphicsDevice.Viewport, boardOrigin, cellSize);
            stateMachine.EnterState(GameState.MainMenu);
        }

        protected override void UnloadContent()
        {
        }

        private void DrawState(GameTime gameTime)
        {
            switch (stateMachine.CurrentState)
            {
                case GameState.MainMenu:
                    DrawMenu();
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
                    spriteBatch.DrawString(font, "Ganhador: " + stateMachine.Board.GetWinner().ToString(), new Vector2(10.0f, 10.0f), Color.White);
                    spriteBatch.DrawString(font, "Pressione ENTER para iniciar novo jogo", new Vector2(10.0f, 50.0f), Color.White);
                    DrawGrid();
                    DrawBoardContent();
                    break;
                default:
                    break;
            }
        }

        private void DrawMenu()
        {
            spriteBatch.Draw(menuTexture, new Rectangle(250, 100, 300, 400), Color.White);
            if (stateMachine.PlayerToken == 'X')
            {
                spriteBatch.Draw(menuSelectedTexture, new Rectangle(250 + 35, 100, 100, 100), Color.White);
            }
            else
            {
                spriteBatch.Draw(menuSelectedTexture, new Rectangle(250 + 165, 100, 100, 100), Color.White);
            }

            if (stateMachine.GameEntryState == GameState.HumanTurn)
            {
                spriteBatch.Draw(menuSelectedTexture, new Rectangle(250 + 35, 100 + 113, 100, 100), Color.White);
            }
            else
            {
                spriteBatch.Draw(menuSelectedTexture, new Rectangle(250 + 165, 100 + 113, 100, 100), Color.White);
            }

            if (Minimax.easy)
            {
                spriteBatch.Draw(menuSelectedTexture, new Rectangle(250 + 35, 100 + 226, 100, 100), Color.White);
            }
            else
            {
                spriteBatch.Draw(menuSelectedTexture, new Rectangle(250 + 165, 100 + 226, 100, 100), Color.White);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            stateMachine.UpdateState(gameTime, previousKeyboardState, previousMouseState);
            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            DrawState(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Texture2D GetTexture(char token, bool human)
        {
            if ((token == 'X' && human) || (token == 'O' && !human))
            {
                return xTexture;
            }
            else if ((token == 'O' && human) || (token =='X' && !human))
            {
                return oTexture;
            }
            return null;
        }

        public void DrawBoardContent()
        {
            for (int line = 0; line < stateMachine.Board.Height; line += 1)
            {
                for (int column = 0; column < stateMachine.Board.Width; column += 1)
                {
                    Texture2D textureToUse;
                    switch (stateMachine.Board[line, column])
                    {
                        case Player.Human:
                            textureToUse = GetTexture(stateMachine.PlayerToken, true);
                            break;
                        case Player.CPU:
                            textureToUse = GetTexture(stateMachine.PlayerToken, false);
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

        public void DrawGrid()
        {
            DrawLine(spriteBatch, boardOrigin + new Vector2(cellSize.X, 0), boardOrigin + new Vector2(cellSize.X, boardSize.Y));
            DrawLine(spriteBatch, boardOrigin + new Vector2(cellSize.X * 2, 0), boardOrigin + new Vector2(cellSize.X * 2, boardSize.Y));
            DrawLine(spriteBatch, boardOrigin + new Vector2(0, cellSize.Y), boardOrigin + new Vector2(boardSize.X, cellSize.Y));
            DrawLine(spriteBatch, boardOrigin + new Vector2(0, cellSize.Y * 2), boardOrigin + new Vector2(boardSize.X, cellSize.Y * 2));
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
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
