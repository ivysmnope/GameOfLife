
namespace CS_Game_Of_Life {
    public class CSGameOfLife {
        public static void Main(string[] args) {
            using var game = new GameOfLife();
            game.Run();
        }
    }
}