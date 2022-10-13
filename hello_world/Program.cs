using System;


namespace hello_world {

    class Debug {
        public static void debug() {
            Console.SetCursorPosition(0, 16);
            Console.Write("                                   ");
            Console.SetCursorPosition(0, 16);
            Console.Write(msg);
        }
        public static string msg;
    }

    enum Event {
        Nothing,
        Collection,
        Step
    }

    class Ceil {
        public enum Content {
            None,
            Block,
            Resource,
            Character
        }

        public char GetTexture() {
            switch (_content) {
                case Content.Block:
                    return '#';
                case Content.Character:
                    return '@';
                case Content.Resource:
                    return '%';
                case Content.None:
                    return ' ';
            }
            return ' ';
        }

        public Ceil(Content content) {
            _content = content;
        }

        public void SetContent(Content content) {
            _content = content;
        }

        public Content GetContent() {
            return _content;
        }

        private Content _content = Content.None;
    }

    class Map {

        public Map(int n, int m) {
            _ceils = new Ceil[n, m];
            for (int i = 0; i < n; ++i) {
                for (int j = 0; j < m; ++j) {
                    _ceils[i, j] = new Ceil(Ceil.Content.None);
                }
            }
            _ceils[2, 5].SetContent(Ceil.Content.Resource);
            _ceils[6, 9].SetContent(Ceil.Content.Resource);
            _ceils[1, 14].SetContent(Ceil.Content.Resource);
            _ceils[12, 2].SetContent(Ceil.Content.Resource);

            for (int i = 0; i < n; ++i) {
                _ceils[i, 0].SetContent(Ceil.Content.Block);
                _ceils[i, m - 1].SetContent(Ceil.Content.Block);
            }
            for (int i = 0; i < m; ++i) {
                _ceils[0, i].SetContent(Ceil.Content.Block);
                _ceils[n - 1, i].SetContent(Ceil.Content.Block);
            }
        }

        public void Draw() {
            Console.Clear();
            for (int i = 0; i < _ceils.GetLength(0); ++i) {
                for (int j = 0; j < _ceils.GetLength(1); ++j) {
                    Console.SetCursorPosition(i, j);
                    Console.Write(_ceils[i, j].GetTexture());
                }
            }
            Debug.debug();
        }

        public bool Place(int n, int m) {
            if (canStep(n, m)) {
                _ceils[n, m].SetContent(Ceil.Content.Block);
                return true;
            }
            return false;
        }

        public Event Step(int n, int m) {
            if (canStep(n, m)) {
                Event e = onStep(n, m);
                placeChar(n, m);
                return e;
            }
            return Event.Nothing;
        }

        private bool canStep(int n, int m) {
            return n > 0 && m > 0 && n < (_ceils.GetLength(0) - 1) && m < (_ceils.GetLength(1) - 1) && _ceils[n, m].GetContent() != Ceil.Content.Block;        
        }

        private void placeChar(int n, int m) {
            if (_charCeil != null) {
                _charCeil.SetContent(Ceil.Content.None);
            }
            _charCeil = _ceils[n, m];
            _charCeil.SetContent(Ceil.Content.Character);
        }

        private Event onStep(int n, int m) {
            switch (_ceils[n, m].GetContent()) {
                case Ceil.Content.Resource:
                    _ceils[n, m].SetContent(Ceil.Content.None);
                    return Event.Collection;
                case Ceil.Content.None:
                    return Event.Step;
            }
            return Event.Nothing;
        }

        private Ceil[,] _ceils = {};

        private Ceil _charCeil = null;
    }

    class Char {
        public enum Direction {
            up,
            right,
            down,
            left,
            none
        }

        public void Step(Direction d, Map map) {
            Event e = Event.Nothing;
            int new_x = _x, new_y = _y;

            switch (d) {
                case Direction.up:
                    new_y = _y - 1;
                    break;
                case Direction.right:
                    new_x = _x + 1;
                    break;
                case Direction.down:
                    new_y = _y + 1;
                    break;
                case Direction.left:
                    new_x = _x - 1;
                    break;
                case Direction.none:
                    break;
            }

            e = map.Step(new_x, new_y);

            switch (e) {
                case Event.Collection:
                    Debug.msg = "Collected a resource";
                    _resources++;
                    goto case Event.Step;
                case Event.Step:
                    _x = new_x;
                    _y = new_y;
                    break;
            }
        }

        public bool Put(Direction d, Map map) {
            if (_resources <= 0) {
                _resources = 0;
                return false;
            }
            _resources--;
            switch (d) {
                case Direction.up:
                    return map.Place(_x, _y - 1);
                case Direction.right:
                    return map.Place(_x + 1, _y);
                case Direction.down:
                    return map.Place(_x, _y + 1);
                case Direction.left:
                    return map.Place(_x - 1, _y);
            }
            return false;
        }

        public Char(int x, int y) {
            _x = x;
            _y = y;
        }

        private int _x, _y;
        private int _resources = 0;

    }

    class Game {

        public Game(Map map, Char character, int x = 1, int y = 1) {
            _map = map;
            _character = character;
            map.Step(x, y);
        }

        Char.Direction keyToDirection(ConsoleKey k) {
            switch (k) {
                case ConsoleKey.W:
                    return Char.Direction.up;
                case ConsoleKey.D:
                    return Char.Direction.right;
                case ConsoleKey.S:
                    return Char.Direction.down;
                case ConsoleKey.A:
                    return Char.Direction.left;
            }
            return Char.Direction.none;
        }

        public ConsoleKey WaitInput() {
            ConsoleKey k = Console.ReadKey(true).Key;
            if (k != ConsoleKey.P) {
                Debug.msg = k.ToString();
                _character.Step(keyToDirection(k), _map);
                _map.Draw();
                return k;
            }
            Debug.msg = "Chose direction to put: WASD";
            _map.Draw();
            ConsoleKey pd = Console.ReadKey(true).Key;
            _character.Put(keyToDirection(pd), _map);
            _map.Draw();
            return pd;
        }


        private Map _map;
        private Char _character;
    }

    class MainClass {
        public static void Main(string[] args) {
            Debug.msg = "new game";
            Map map = new Map(16, 16);
            Char c = new Char(7, 7);
            Game game = new Game(map, c, 7, 7);
            while (game.WaitInput() != ConsoleKey.Escape) {

            }
        }
    }
}
