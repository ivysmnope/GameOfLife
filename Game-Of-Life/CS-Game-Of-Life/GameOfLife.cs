#region Imports
#region Monogame imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion
#region Generic imports
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Threading;
#endregion
#region CS_Game_Of_Life imports
using CS_Game_Of_Life.Handlers;
using CS_Game_Of_Life.Entities;
using CS_Game_Of_Life.Managers;
#endregion
#endregion

namespace CS_Game_Of_Life {
    internal enum GameState {
        Playing,
        Paused,
    }

    public class GameOfLife : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Camera camera;

        private GameState game_state = GameState.Paused;

        private double last_update = 0;
        private int wait_time = 1000;

        public GameOfLife() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            FullScreen();
            camera = new Camera(limits: new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

            GameManager.Setup(_graphics.PreferredBackBufferWidth / 8, _graphics.PreferredBackBufferHeight / 8);
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsManager.LoadFont(Content);
        }

        protected override void UnloadContent() {
            base.UnloadContent();
            _spriteBatch.Dispose();

            GameManager.Dispose();
            GraphicsManager.Dispose();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputHandler.HandleCameraMovement(gameTime, ref camera, _graphics, GetScreenScale());

            InputHandler.HandleKeyboard(gameTime, ref game_state, ref wait_time, GraphicsDevice);

            if (game_state == GameState.Playing)
                UpdateGameOfLife(gameTime);
            else
                InputHandler.UpdateCursorLogic(camera, GraphicsDevice);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            Vector3 screen_scale = GetScreenScale();
            Matrix view_matrix = camera.GetTransform();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, view_matrix * Matrix.CreateScale(screen_scale));
            GameManager.Draw(_spriteBatch);
            InputHandler.DrawMouseRectangle(GraphicsDevice, _spriteBatch, camera);
            _spriteBatch.End();

            _spriteBatch.Begin();
            if (game_state == GameState.Paused) {
                GraphicsManager.Write(_spriteBatch, FontSize.Medium, "game-paused", new Vector2(_graphics.PreferredBackBufferWidth - 200, _graphics.PreferredBackBufferHeight - 50));
                GraphicsManager.Write(_spriteBatch, FontSize.ExtraSmall, "left click to add entity, right click to destroy", new Vector2(_graphics.PreferredBackBufferWidth - 400, _graphics.PreferredBackBufferHeight - 70));
                GraphicsManager.Write(_spriteBatch, FontSize.ExtraSmall, "C to clear, R to create a new generation", new Vector2(_graphics.PreferredBackBufferWidth - 340, _graphics.PreferredBackBufferHeight - 90));
            }
            GraphicsManager.Write(_spriteBatch, FontSize.Small, $"generation: {GameManager.GenerationCount}", new Vector2(Entity.Width, Entity.Height));
            GraphicsManager.Write(_spriteBatch, FontSize.ExtraSmall, $"number of alive entities: {GameManager.AliveBacteria}", new Vector2(Entity.Width, Entity.Height + 30));
            GraphicsManager.Write(_spriteBatch, FontSize.ExtraSmall, "space to pause", new Vector2(Entity.Width, Entity.Height + 50));

            GraphicsManager.Write(_spriteBatch, FontSize.ExtraSmall, $"update delay: {wait_time} ms", new Vector2(_graphics.PreferredBackBufferWidth - 192, Entity.Height * 2));
            GraphicsManager.Write(_spriteBatch, FontSize.ExtraSmall, "I => +, O => -", new Vector2(_graphics.PreferredBackBufferWidth - 120, Entity.Height + 30));

            GraphicsManager.Write(_spriteBatch, FontSize.ExtraSmall, "WASD to move camera, JK to zoom", new Vector2(Entity.Width, _graphics.PreferredBackBufferHeight - Entity.Height - 30));
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void FullScreen() {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges();
        }

        private Vector3 GetScreenScale() {
            float scaleX = (float)GraphicsDevice.Viewport.Width / (float)_graphics.PreferredBackBufferWidth;
            float scaleY = (float)GraphicsDevice.Viewport.Height / (float)_graphics.PreferredBackBufferHeight;
            return new Vector3(scaleX, scaleY, 1.0f);
        }

        private void UpdateGameOfLife(GameTime gameTime) {
            if (gameTime.TotalGameTime.TotalMilliseconds - last_update >= wait_time) {
                GameManager.NewGeneration(GraphicsDevice);
                last_update = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }
    }
}