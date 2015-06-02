using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTacToe.AI;
using TicTacToe.GameObjects;

namespace ui
{
    public enum GameState
    {
        MainMenu,
        MachineTurn,
        HumanTurn,
        GameOver
    }

    public class StateMachine
    {
        private Vector2 boardOrigin;
        private Vector2 cellSize;
        private Viewport viewport;
        private GameState currentState;
        private GameState gameEntryState;
        private GameBoard board;
        private double timer;

        public GameState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public GameBoard Board
        {
            get { return board; }
            set { board = value; }
        }

        public GameState GameEntryState
        {
            get { return gameEntryState; }
            set { gameEntryState = value; }
        }

        public char PlayerToken { get; set; }

        public StateMachine(Viewport viewport, Vector2 boardOrigin, Vector2 cellSize)
        {
            this.currentState = GameState.MainMenu;
            this.gameEntryState = GameState.HumanTurn;
            this.viewport = viewport;
            this.boardOrigin = boardOrigin;
            this.cellSize = cellSize;
            this.PlayerToken = 'X';
        }

        public void EnterState(GameState state)
        {
            currentState = state;
            switch (currentState)
            {
                case GameState.MainMenu:
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

        public void LeaveState(GameState state)
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
            bool isXValid = mouseState.Position.X >= 0 && mouseState.Position.X <= viewport.Width;
            bool isYValid = mouseState.Position.Y >= 0 && mouseState.Position.Y <= viewport.Height;
            return isXValid && isYValid;
        }

        private bool KeyWasPressed(KeyboardState previousKeyboardState, Keys key)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            return previousKeyboardState != null && !previousKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key);
        }

        public void UpdateState(GameTime gameTime, KeyboardState previousKeyboardState, MouseState previousMouseState)
        {
            switch (currentState)
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
                        PlayerToken = 'X';
                    }
                    else if (KeyWasPressed(previousKeyboardState, Keys.NumPad8))
                    {
                        PlayerToken = 'O';
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
                        LeaveState(currentState);
                        board = Minimax.Play(board, Player.CPU);
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
                        LeaveState(currentState);
                        Vector2 mouseVector = currentMouseState.Position.ToVector2();
                        Vector2 estimatedPosition = (mouseVector - boardOrigin) / cellSize;
                        int x = Between((int)estimatedPosition.X, 0, board.Width - 1);
                        int y = Between((int)estimatedPosition.Y, 0, board.Height - 1);
                        board.Play(x, y, Player.Human);
                        NextTurn(GameState.MachineTurn);
                    }
                    break;
                case GameState.GameOver:
                    if (KeyWasPressed(previousKeyboardState, Keys.Enter))
                    {
                        LeaveState(currentState);
                        EnterState(GameState.MainMenu);
                    }
                    break;
                default:
                    break;
            }
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
    }
}
