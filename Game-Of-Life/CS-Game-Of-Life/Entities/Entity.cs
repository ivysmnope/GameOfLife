using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Game_Of_Life.Entities {
    internal abstract class Entity {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Microsoft.Xna.Framework.Rectangle Rectangle { get; set; } = Rectangle.Empty;
        public static Texture2D Texture { get; set; }
        public static Microsoft.Xna.Framework.Color Color { get; set; } = Microsoft.Xna.Framework.Color.White;
        public static int Height { get; set; } = 8;
        public static int Width { get; set; } = 8;

        public virtual void Dispose() {
            Texture.Dispose();
        }

        public static void InitialiseTexture(GraphicsDevice _graphicsDevice) {
            Texture = new Texture2D(_graphicsDevice, 1, 1);
            Texture.SetData(new[] { Color });
        }

        public virtual void Draw(SpriteBatch _spriteBatch) {
            _spriteBatch.Draw(Texture, Rectangle, Color);
        }
    }
}
