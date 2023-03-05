using CS_Game_Of_Life.Entities;
using CS_Game_Of_Life.Managers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace CS_Game_Of_Life.Handlers {
    internal static class InputHandler {
        #region attributes
        private static double last_key_press = 0;
        private static KeyboardState kb { get => Keyboard.GetState(); }

        private static int mouse_corner_size = 2;
        #endregion

        #region Mouse
        public static void DrawMouseRectangle(GraphicsDevice _graphicsDevice, SpriteBatch _spriteBatch, Camera camera) {

            Texture2D point_texture = new(_graphicsDevice, 1, 1);
            point_texture.SetData(new[] { Color.LightBlue });

            //Point mouse_viewpoint_position = GetMousePosition().viewpoint_position;
            //_spriteBatch.Draw(point_texture, new Rectangle(mouse_viewpoint_position.X - Entity.Width / 2, mouse_viewpoint_position.Y - Entity.Height / 2, 2, 2 + Entity.Height), Color.White);
            //_spriteBatch.Draw(point_texture, new Rectangle(mouse_viewpoint_position.X - Entity.Width / 2, mouse_viewpoint_position.Y - Entity.Height / 2, Entity.Width + 2, 2), Color.White);
            //_spriteBatch.Draw(point_texture, new Rectangle(mouse_viewpoint_position.X + Entity.Width - Entity.Width / 2, mouse_viewpoint_position.Y - Entity.Height / 2, 2, 2 + Entity.Height), Color.White);
            //_spriteBatch.Draw(point_texture, new Rectangle(mouse_viewpoint_position.X - Entity.Width / 2, mouse_viewpoint_position.Y + Entity.Height - Entity.Height / 2, 2 + Entity.Width, 2), Color.White);

            Vector2 mouse_grid_position = GetMousePosition(camera).grid_position;
            int x = (int)MathF.Round(mouse_grid_position.X / Entity.Width),
                y = (int)MathF.Round(mouse_grid_position.Y / Entity.Height);

            _spriteBatch.Draw(point_texture, new Rectangle(x * Entity.Width, y * Entity.Height, mouse_corner_size + Entity.Height, mouse_corner_size), Color.White);
            _spriteBatch.Draw(point_texture, new Rectangle(x * Entity.Width, y * Entity.Height + Entity.Height, mouse_corner_size + Entity.Width, mouse_corner_size), Color.White);
            _spriteBatch.Draw(point_texture, new Rectangle(x * Entity.Width, y * Entity.Height, mouse_corner_size, mouse_corner_size + Entity.Height), Color.White);
            _spriteBatch.Draw(point_texture, new Rectangle(x * Entity.Width + Entity.Width, y * Entity.Height, mouse_corner_size, mouse_corner_size + Entity.Height), Color.White);
        }
        public static void UpdateCursorLogic(Camera camera, GraphicsDevice _graphicsDevice) {
            MouseState ms = Mouse.GetState();
            if (!(ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed))
                return;

            Vector2 mouse_grid_position = GetMousePosition(camera).grid_position;

            int x = (int)MathF.Round(mouse_grid_position.X / Entity.Width),
                y = (int)MathF.Round(mouse_grid_position.Y / Entity.Height);

            if (x < 0 || y < 0 || x >= GameManager.Generation.GetLength(0) || y >= GameManager.Generation.GetLength(1))
                return;

            if (ms.LeftButton == ButtonState.Pressed && GameManager.Generation[x, y] is null) {
                GameManager.Generation[x, y] = new Bacteria(_graphicsDevice, new Vector2(x * Entity.Width, y * Entity.Height));
                GameManager.AliveBacteria += 1;
            } else if (ms.RightButton == ButtonState.Pressed && GameManager.Generation[x, y] is not null) {
                GameManager.Generation[x, y] = null;
                GameManager.AliveBacteria -= 1;
            }
        }
        public static (Point viewpoint_position, Vector2 world_position, Vector2 grid_position) GetMousePosition(Camera camera) {
            Point viewpoint_position = Mouse.GetState().Position;
            Vector2 world_position = Vector2.Transform(new Vector2(viewpoint_position.X, viewpoint_position.Y), Matrix.Invert(camera.GetTransform()));
            //Vector2 grid_position = new Vector2(viewpoint_position.X / camera.Zoom, viewpoint_position.Y / camera.Zoom);
            Vector2 grid_position = new Vector2(MathF.Round(world_position.X) - 2, MathF.Round(world_position.Y) - 2);
            return (viewpoint_position, world_position, grid_position);
        }
        #endregion

        #region Keyboard
        public static void HandleKeyboard(GameTime gameTime, ref GameState game_state, ref int update_wait_time, GraphicsDevice _graphicsDevice) {
            if (kb.IsKeyDown(Keys.P)) {
                return;
            } else if (kb.IsKeyDown(Keys.Space) && gameTime.TotalGameTime.TotalMilliseconds - last_key_press > 500) {
                if (game_state == GameState.Playing)
                    game_state = GameState.Paused;
                else
                    game_state = GameState.Playing;
                last_key_press = gameTime.TotalGameTime.TotalMilliseconds;
            } else if (kb.IsKeyDown(Keys.O))
                update_wait_time -= 100;
            else if (kb.IsKeyDown(Keys.I))
                update_wait_time += 100;
            else if (kb.IsKeyDown(Keys.R) && gameTime.TotalGameTime.TotalMilliseconds - last_key_press > 500) {
                GameManager.RandomStart(_graphicsDevice);
                GC.Collect(2, GCCollectionMode.Forced);
                last_key_press = gameTime.TotalGameTime.TotalMilliseconds;
            } else if (kb.IsKeyDown(Keys.C) && gameTime.TotalGameTime.TotalMilliseconds - last_key_press > 500) {
                GameManager.Reset();
                last_key_press = gameTime.TotalGameTime.TotalMilliseconds;
            }

            update_wait_time = (int)MathF.Max(100, MathF.Min(update_wait_time, 1000));
        }
        #endregion

        #region Camera
        public static void HandleCameraMovement(GameTime gameTime, ref Camera camera, GraphicsDeviceManager _graphics, Vector3 screen_scale) {
            if (!camera.CanMove && gameTime.TotalGameTime.TotalMilliseconds - last_key_press > 500) {
                camera.CanMove = true;
            }

            if (!camera.CanMove)
                return;

            Vector2 camera_movement = new Vector2();

            if (kb.IsKeyDown(Keys.W))
                camera_movement.Y += camera.Speed;
            if (kb.IsKeyDown(Keys.S))
                camera_movement.Y -= camera.Speed;
            if (kb.IsKeyDown(Keys.A))
                camera_movement.X += camera.Speed;
            if (kb.IsKeyDown(Keys.D))
                camera_movement.X -= camera.Speed;

            if (kb.IsKeyDown(Keys.J))
                camera.Zoom -= camera.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kb.IsKeyDown(Keys.K))
                camera.Zoom += camera.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            camera.Zoom = MathF.Min(3, MathF.Max(1, camera.Zoom));
            camera.Move(camera_movement);
        }
        #endregion
    }
}
