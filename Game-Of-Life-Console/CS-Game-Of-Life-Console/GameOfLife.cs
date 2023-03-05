using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Game_Of_Life_Console {
    internal class GameOfLife {
        public bool[,] Generation;
        Random rng = new Random();
        public GameOfLife(int rows, int columns) {
            GenerateFirstGeneration(rows, columns);
        }

        private void GenerateFirstGeneration(int rows, int columns) {
            if (rows <= 0 || columns <= 0) {
                rows = 16;
                columns = 32;
            }
            Generation = new bool[rows, columns];
        }

        public void InsertRandomLives() => Iterator((row, col) => {
            Generation[row, col] = rng.Next(0, 10) == 1;
        });

        public int CountNeighbours(int row, int col) {
            int count = 0;
            count += row + 1 < Generation.GetLength(0) && Generation[row + 1, col] ? 1 : 0;
            count += col + 1 < Generation.GetLength(1) && Generation[row, col + 1] ? 1 : 0;
            count += row - 1 >= Generation.GetLowerBound(0) && Generation[row - 1, col] ? 1 : 0;
            count += col - 1 >= Generation.GetLowerBound(1) && Generation[row, col - 1] ? 1 : 0;
            count += row + 1 < Generation.GetLength(0) && col - 1 >= Generation.GetLowerBound(1) && Generation[row + 1, col - 1] ? 1 : 0;
            count += row - 1 >= Generation.GetLowerBound(0) && col - 1 >= Generation.GetLowerBound(1) && Generation[row - 1, col - 1] ? 1 : 0;
            count += row + 1 < Generation.GetLength(0) && col + 1 < Generation.GetLength(1) && Generation[row + 1, col + 1] ? 1 : 0;
            count += row - 1 >= Generation.GetLowerBound(0) && col + 1 < Generation.GetLowerBound(1) && Generation[row - 1, col + 1] ? 1 : 0;

            return count;
        }

        public void NewGeneration() {
            bool[,] new_gen = new bool[Generation.GetLength(0), Generation.GetLength(1)];

            Iterator((row, col) => {
                if (Generation[row, col]) {
                    if (CountNeighbours(row, col) < 2) {
                        new_gen[row, col] = !Generation[row, col];
                    } else if (CountNeighbours(row, col) == 2 || CountNeighbours(row, col) == 3) {
                        new_gen[row, col] = Generation[row, col];
                    } else {
                        new_gen[row, col] = !Generation[row, col];
                    }
                } else if (CountNeighbours(row, col) == 3) {
                    new_gen[row, col] = !Generation[row, col];
                }
            });
            Generation = (bool[,])new_gen.Clone();
        }

        public string DrawGeneration() {
            StringBuilder str_builder = new StringBuilder();
            Iterator((row, col) => {
                str_builder.Append(Generation[row, col] ? "#" : " ");
            }, () => {
                str_builder.AppendLine();
            });
            return str_builder.ToString();
        }

        public void Iterator(Action<int, int> statement, Action? statement_2 = null) {
            for (int row = 0; row < Generation.GetLength(0); row++) {
                for (int col = 0; col < Generation.GetLength(1); col++) {
                    statement(row, col);
                }
                if (statement_2 is not null) {
                    statement_2();
                }
            }
        }
    }
}
