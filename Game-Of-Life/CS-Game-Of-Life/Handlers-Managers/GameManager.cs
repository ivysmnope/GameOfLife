using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CS_Game_Of_Life.Entities;
using CS_Game_Of_Life.Handlers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS_Game_Of_Life.Managers {
    internal static class GameManager {
        #region attributes
        private static Entity[,] _generation;
        private static readonly Random rng = new();

        public static int GenerationCount { get; private set; } = 0;
        public static int AliveBacteria { get; set; } = 0;

        #region get-set
        public static Entity[,] Generation { get { return _generation; } }
        #endregion
        #endregion

        #region setup methods
        public static void Setup(int width, int height) {
            Reset(width, height);
        }

        public static void Reset(int? width = null, int? height = null) {
            _generation = new Entity[width ?? _generation.GetLength(0), height ?? _generation.GetLength(1)];
            GC.Collect(1, GCCollectionMode.Forced);
            GenerationCount = 0;
            AliveBacteria = 0;
        }

        public static void Dispose() => Iterator((row, col) => {
            if (_generation[row, col] is not null)
                _generation[row, col].Dispose();
            GC.Collect(1, GCCollectionMode.Forced);
        });
        #endregion

        #region game methods
        private static int CountNeighbours(int row, int col) {
            int len0 = _generation.GetLength(0),
                len1 = _generation.GetLength(1);
            int count = 0;

            count += (row - 1 >= 0 && _generation[row - 1, col] is Bacteria) ? 1 : 0;
            count += (row + 1 < len0 && _generation[row + 1, col] is Bacteria) ? 1 : 0;
            count += (col - 1 >= 0 && _generation[row, col - 1] is Bacteria) ? 1 : 0;
            count += (col + 1 < len1 && _generation[row, col + 1] is Bacteria) ? 1 : 0;

            count += (row - 1 >= 0 && col - 1 >= 0 && _generation[row - 1, col - 1] is Bacteria) ? 1 : 0;
            count += (row - 1 >= 0 && col + 1 < len1 && _generation[row - 1, col + 1] is Bacteria) ? 1 : 0;
            count += (row + 1 < len0 && col - 1 >= 0 && _generation[row + 1, col - 1] is Bacteria) ? 1 : 0;
            count += (row + 1 < len0 && col + 1 < len1 && _generation[row + 1, col + 1] is Bacteria) ? 1 : 0;

            return count;
        }

        public static void NewGeneration(GraphicsDevice _graphicsDevice) {
            Entity[,] new_gen = new Entity[_generation.GetLength(0), _generation.GetLength(1)];
            Iterator((row, col) => {
                int neighbours = CountNeighbours(row, col);

                if (_generation[row, col] is not null) {
                    if (neighbours < 2 || neighbours > 3) {
                        new_gen[row, col] = null;
                        AliveBacteria--;
                    }
                    if (neighbours == 2 || neighbours == 3)
                        new_gen[row, col] = _generation[row, col];
                } else if (neighbours == 3) {
                    new_gen[row, col] = new Bacteria(_graphicsDevice, new Vector2(row * Entity.Width, col * Entity.Height));
                    AliveBacteria++;
                }
            });
            _generation = new_gen;
            GenerationCount++;
            GC.Collect(1, GCCollectionMode.Forced);
        }
        #endregion

        #region utility
        public static void Iterator(Action<int, int> statement_1, Action statement_2 = null) {
            int len0 = _generation.GetLength(0),
                len1 = _generation.GetLength(1);

            for (int row = 0; row < len0; row++) {
                for (int col = 0; col < len1; col++)
                    statement_1(row, col);
                if (statement_2 is not null)
                    statement_2();
            }
        }

        public static void Draw(SpriteBatch _spriteBatch) {
            Iterator((row, col) => {
                if (_generation[row, col] is not null)
                    _generation[row, col].Draw(_spriteBatch);
            });
        }

        public static void RandomStart(GraphicsDevice _graphicsDevice) {
            AliveBacteria = 0;
            Iterator((row, col) => {
                if (rng.Next(0, 10) == 1) {
                    _generation[row, col] = new Bacteria(_graphicsDevice, new Vector2(row * Entity.Width, col * Entity.Height));
                    AliveBacteria++;
                } else
                    _generation[row, col] = null;
            });
            GenerationCount = 1;
        }
        #endregion
    }
}
