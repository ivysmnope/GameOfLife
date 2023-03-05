using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Game_Of_Life.Managers {
    internal enum FontSize {
        ExtraSmall,
        Small,
        Medium,
        Big
    }
    internal static class GraphicsManager {
        private static SpriteFont extra_small_font;
        private static SpriteFont small_font;
        private static SpriteFont medium_font;
        private static SpriteFont big_font;
        public static Microsoft.Xna.Framework.Color Color { get; set; } = Microsoft.Xna.Framework.Color.DeepPink;

        public static void LoadFont(ContentManager content) {
            extra_small_font = content.Load<SpriteFont>("Extra_Small_Font");
            small_font = content.Load<SpriteFont>("Small_Font");
            medium_font = content.Load<SpriteFont>("Medium_Font");
            big_font = content.Load<SpriteFont>("Big_Font");
        }

        public static void Write(SpriteBatch _spriteBatch, FontSize font_size, string str, Vector2 position) => _spriteBatch.DrawString(FontFromSize(font_size), str, position, Color);

        private static SpriteFont FontFromSize(FontSize font_size) {
            switch (font_size) {
                case FontSize.ExtraSmall:
                    return extra_small_font;
                case FontSize.Small:
                    return small_font;
                case FontSize.Medium:
                    return medium_font;
                case FontSize.Big:
                    return big_font;
                default:
                    return medium_font;
            }
        }

        public static void Dispose() {

        }

    }
}
