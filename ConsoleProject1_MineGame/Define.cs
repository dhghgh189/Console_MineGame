using System.Numerics;

namespace ConsoleProject1_MineGame
{
    internal class Define
    {
        public const int MAX_X = 9;
        public const int MAX_Y = 9;
        public const int PLAYER_POS_X = 1;
        public const int PLAYER_POS_Y = 1;
        public const int GOAL_POS_X = MAX_X - 2;
        public const int GOAL_POS_Y = MAX_Y - 2;

        public struct GameManager
        {
            public EMode eMode;
            public bool isRunning;
            public int currentStageIndex;
            public Map currentMap;
            public ConsoleKey inputKey;
            public Vector2Int playerPos;
            public bool bWarning;
        }

        public enum EMode
        {
            NONE,
            NORMAL,
            EASY,
        }

        public struct Map
        {
            public int maxX;
            public int maxY;
            public int mineCount;
            public ETile[,] grid;
            public bool[,] isVisited;   // 플레이어의 동선 정보
        }

        public static Map[] maps =
        {
            new Map         // STAGE 1 (지뢰 5개)
            {
                maxX = MAX_X,
                maxY = MAX_Y,
                mineCount = 5,
                grid = new ETile[MAX_X, MAX_Y]
                {
                    { ETile.WALL, ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.WALL},
                    { ETile.WALL, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.GOAL,  ETile.WALL},
                    { ETile.WALL, ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL},
                },
            },
            new Map         // STAGE 2 (지뢰 7개)
            {
                maxX = MAX_X,
                maxY = MAX_Y,
                mineCount = 7,
                grid = new ETile[MAX_X, MAX_Y]
                {
                    { ETile.WALL, ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.GOAL,  ETile.WALL},
                    { ETile.WALL, ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL},
                }
            },
            new Map         // STAGE 3 (지뢰 10개)
            {
                maxX = MAX_X+1,
                maxY = MAX_Y+1,
                mineCount = 10,
                grid = new ETile[MAX_X+1, MAX_Y+1]
                {
                    { ETile.WALL, ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.WALL},
                    { ETile.WALL, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.FLOOR, ETile.MINE,  ETile.FLOOR, ETile.FLOOR, ETile.GOAL,  ETile.WALL},
                    { ETile.WALL, ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL,  ETile.WALL},
                },
            },
        };

        public struct Vector2Int
        {
            public int x;
            public int y;

            public Vector2Int(int x = 0, int y = 0)
            {
                this.x = x;
                this.y = y;
            }

            public void SetPos(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            // 좌표 간 덧셈 뺄셈을 편하게 하기 위한 연산자 재정의 
            public static Vector2Int operator +(Vector2Int a, Vector2Int b)
                => new Vector2Int((a.x + b.x), (a.y + b.y));
            public static Vector2Int operator -(Vector2Int a, Vector2Int b)
                => new Vector2Int((a.x - b.x), (a.y - b.y));

            // 좌표가 동일한지 비교를 편하게 하기 위한 연산자 재정의
            public static bool operator ==(Vector2Int a, Vector2Int b)
                => (a.x == b.x) && (a.y == b.y);
            public static bool operator !=(Vector2Int a, Vector2Int b)
                => (a.x != b.x) || (a.y != b.y);
        }

        public enum EDirection
        {
            UP,
            UP_RIGHT,
            RIGHT,
            DOWN_RIGHT,
            DOWN,
            DOWN_LEFT,
            LEFT,
            UP_LEFT
        }

        public static Vector2Int[] Direction =
        {
            new Vector2Int(0, -1),   // UP
            new Vector2Int(1, -1),   // UP_RIGHT
            new Vector2Int(1, 0),   // RIGHT
            new Vector2Int(1, 1),   // DOWN_RIGHT
            new Vector2Int(0, 1),   // DOWN
            new Vector2Int(-1, 1),   // DOWN_LEFT
            new Vector2Int(-1, 0),   // LEFT
            new Vector2Int(-1, -1)    // UP_LEFT
        };

        public enum ETile
        {
            WALL,
            FLOOR,
            EASY_FLOOR,
            MINE,
            MINE_HINT,
            PLAYER,
            GOAL
        }

        public static ConsoleColor[] tileColor =
        {
            ConsoleColor.DarkBlue,   // WALL
            ConsoleColor.DarkGray,   // FLOOR
            ConsoleColor.DarkGreen,  // EASY_FLOOR
            ConsoleColor.Red,        // MINE
            ConsoleColor.Red,        // MINE_HINT
            ConsoleColor.DarkYellow, // PLAYER
            ConsoleColor.Yellow,     // GOAL
        };

        public static string[] tileImg =
        {
            "■",    // WALL
            "□",    // FLOOR
            "■",    // EASY FLOOR
            "◆",    // MINE
            "□",    // MINE_HINT
            "●",    // PLAYER
            "★",    // GOAL
        };
    }
}
