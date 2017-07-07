using System.Collections.Generic;

namespace com.perroelectrico.demondrian.core {

    public struct MenuAction {
        public readonly string label;
        public readonly string action;

        public MenuAction(string label, string action) {
            this.label = label;
            this.action = action;
        }
    }

    public class Menu {
        public readonly Board board;
        public readonly Dictionary<Coord, MenuAction> actions;

        public static Menu MainMenu = new Menu(
            new BoardBuilder().FromTypeGrid("0000 0011 0011 0021"),
            new Dictionary<Coord, MenuAction>() {
                { new Coord(0, 2), new MenuAction("Play", "play") },
                { new Coord(3, 0), new MenuAction("Exit", "exit") }
            });

        public Menu(Board board, Dictionary<Coord, MenuAction> actions) {
            this.board = board;
            this.actions = actions;
        }
    }
}
