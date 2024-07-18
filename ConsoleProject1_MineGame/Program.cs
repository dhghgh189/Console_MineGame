using System.Net.Sockets;
using static ConsoleProject1_MineGame.Define;

namespace ConsoleProject1_MineGame
{
    internal class Program
    {
        static GameManager Game;

        static bool bExit = false;

        static void Main(string[] args)
        {
            while (!bExit)
            {
                Start();

                while (Game.isRunning)
                {
                    Render();

                    Input();

                    Update();
                }

                End();
            }
        }

        #region Start
        static void Start()
        {
            Console.CursorVisible = false;

            Game = new GameManager();

            Game.eMode = EMode.NONE;
            Game.isRunning = true;
            Game.currentStageIndex = -1;

            // 타이틀 출력
            PrintTitle();
            // 게임설명 출력
            PrintHowToPlay();
            // 난이도 설정
            ModeSelect();

            // 난이도 설정까지 끝나면 첫 Stage 준비
            SetNextStage();
        }
        
        static void PrintTitle()
        {
            Console.Clear();
            PrintMessage("================================================\n", ConsoleColor.Yellow);
            PrintMessage("                지뢰", ConsoleColor.Red);
            PrintMessage("피하기", ConsoleColor.Green); PrintMessage(" 게임!                \n", ConsoleColor.Cyan);               
            PrintMessage("================================================\n", ConsoleColor.Yellow);
            PrintMessage("\n>>               Press Any Key!               <<");
            Console.ReadKey();
        }

        static void PrintHowToPlay()
        {
            Console.Clear();
            PrintMessage("================================================\n", ConsoleColor.Yellow);
            PrintMessage("                   게임 설명!                   \n", ConsoleColor.Cyan);
            PrintMessage("================================================\n", ConsoleColor.Yellow);
            PrintMessage("\nWASD 혹은 방향키를 통해 안전지대까지 도착하세요\n", ConsoleColor.Green);
            PrintMessage("\n이동중에 지뢰를 밟는 경우 게임오버! \n", ConsoleColor.Red);
            PrintMessage("\n1칸 이내에 지뢰가 있는 경우 힌트가 표시됩니다\n", ConsoleColor.Cyan);
            PrintMessage("(플레이어 기준 인접한 8칸에 힌트 표시)\n");
            PrintMessage("\n끝까지 살아남으세요!\n", ConsoleColor.DarkYellow);
            PrintMessage($"\n{tileImg[(int)ETile.PLAYER]} = 플레이어\n", tileColor[(int)ETile.PLAYER]);
            PrintMessage($"{tileImg[(int)ETile.GOAL]} = 안전지대\n", tileColor[(int)ETile.GOAL]);
            PrintMessage($"{tileImg[(int)ETile.MINE_HINT]} = 지뢰힌트\n", tileColor[(int)ETile.MINE_HINT]);
            PrintMessage("\n>>               Enter키로 시작               <<\n");
            Console.ReadLine();
        }

        static void ModeSelect()
        {
            Console.CursorVisible = true;
            Console.Clear();
            PrintMessage("================================================\n", ConsoleColor.Yellow);
            PrintMessage("                   난이도 선택                   \n", ConsoleColor.Cyan);
            PrintMessage("================================================\n", ConsoleColor.Yellow);
            PrintMessage("\n1. NORMAL\n");
            PrintMessage("2. EASY\n", ConsoleColor.Green);
            PrintMessage("\n※ NORMAL MODE :\n");
            PrintMessage("    동선이 표시 되지 않음\n", ConsoleColor.Yellow);
            PrintMessage("    게임 오버시에도 지뢰가 표시되지 않음\n", ConsoleColor.Yellow);
            PrintMessage("\n※ EASY MODE :\n", ConsoleColor.Green);
            PrintMessage("    게임 진행중 유저 동선을 표시\n", ConsoleColor.Yellow);
            PrintMessage("    게임 오버시 전체 지뢰를 표시\n", ConsoleColor.Yellow);

            int iInput;
            while (Game.eMode == EMode.NONE)
            {
                PrintMessage("\n>> ");
                int.TryParse(Console.ReadLine(), out iInput);
                if (Enum.IsDefined(typeof(EMode), iInput))
                {
                    Game.eMode = (EMode)iInput;
                }
                else
                {
                    PrintMessage("잘못된 입력입니다!\n", ConsoleColor.DarkRed);
                }
            }
            Console.CursorVisible = false;
        }
        #endregion

        #region Init & Set Info
        static void ResetVariables()
        {
            // 플레이어 / Goal 위치 초기화
            Game.playerPos = new Vector2Int(PLAYER_POS_X, PLAYER_POS_Y);
            Game.bWarning = false;
        }

        static void SetNextStage()
        {
            // 다음 스테이지 진행을 위한 작업들 수행
            Game.currentStageIndex++;
            ResetVariables();
            SetMapGrid(Game.currentStageIndex);

            // 시작하자 마자 지뢰가 인접한 경우를 대비해 1회 실행
            CheckExistNearMine();
        }

        static void SetMapGrid(int iStageIndex)
        {
            int yLength = maps[iStageIndex].maxY;
            int xLength = maps[iStageIndex].maxX;

            // Stage 정보 설정
            Game.currentMap = maps[iStageIndex];

            // easy mode에서 유저 동선 표시를 위한 정보를 저장할 배열
            Game.currentMap.isVisited = new bool[yLength, xLength];
            // 시작 위치는 방문한 것으로 처리
            Game.currentMap.isVisited[Game.playerPos.y, Game.playerPos.x] = true;
        }

        static ETile GetTileToGrid(Vector2Int pos)
        {
            // 맵의 특정 좌표로 부터 tile을 get
            return Game.currentMap.grid[pos.y, pos.x];
        }

        static ETile GetTileToGrid(int x, int y)
        {
            return GetTileToGrid(new Vector2Int(x, y));
        }
        #endregion

        #region Render
        static void Render()
        {
            Console.Clear();

            // Print Stage Info
            PrintStage();

            // Draw Map 
            DrawGrid();

            // Print Game Comment
            PrintComment();
        }

        static void PrintStage()
        {
            PrintMessage($"< Stage{Game.currentStageIndex + 1} >", ConsoleColor.Cyan);
            if (Game.eMode == EMode.EASY)
            {
                PrintMessage(" ( EASY )", ConsoleColor.Green);
            }
            Console.WriteLine("\n");
        }

        static void PrintComment()
        {
            // 지뢰에 인접한 경우
            if (Game.bWarning)
            {
                PrintMessage("\n근처에 지뢰가 있습니다. 조심하세요!!\n", ConsoleColor.Red);
            }
            else
            {
                PrintMessage("\n안전지대로 이동하세요!\n", ConsoleColor.Yellow);
            }

            PrintMessage($"Stage{Game.currentStageIndex+1} ", ConsoleColor.Cyan);
            PrintMessage("지뢰의 개수: ");
            PrintMessage($"{Game.currentMap.mineCount}", ConsoleColor.Red);
        }

        static void DrawGrid()
        {
            int iTile = 0;
            ETile currentTile;
            Vector2Int pos = new Vector2Int();

            for (int y = 0; y < Game.currentMap.maxY; y++)
            {
                for (int x = 0; x < Game.currentMap.maxX; x++)
                {
                    pos.SetPos(x, y);

                    // 플레이어 타일 처리
                    if (pos == Game.playerPos)
                    {
                        currentTile = ETile.PLAYER;
                    }
                    else
                    {
                        // 나머지 타일 처리
                        currentTile = GetDrawTile(pos);
                    }

                    // 선정된 tile을 index로 변경
                    iTile = (int)currentTile;

                    // 선정된 tile 출력
                    PrintMessage(tileImg[iTile], tileColor[iTile]);
                }
                // Y축 Line Number 표시
                PrintMessage($"{y + 1, 2}\n");
            }

            // X축 Line Number 표시
            for (int i = 0; i < Game.currentMap.maxX; i++)
            {
                PrintMessage($"{i + 1, -2}");
            }
            Console.WriteLine();
        }

        static ETile GetDrawTile(Vector2Int pos)
        {
            ETile currentTile = Game.currentMap.grid[pos.y, pos.x];          

            if (currentTile == ETile.MINE)
            {
                // 게임도중 지뢰는 유저에게 보이면 안됨
                if (Game.isRunning)
                {
                    currentTile = ETile.FLOOR;
                }
                else
                {
                    // 게임이 끝나도 Normal 모드에서는 모든 지뢰를 보여주지 않음
                    if ((Game.eMode != EMode.EASY) && !IsVisited(pos.x, pos.y))
                    {
                        currentTile = ETile.FLOOR;
                    }
                }
            }

            // easy 모드에서 플레이어가 방문한 좌표인 경우
            if ((Game.eMode == EMode.EASY) && IsVisited(pos.x, pos.y))
            {
                if (currentTile == ETile.FLOOR)
                {
                    // 동선을 우선적으로 표시함
                    currentTile = ETile.EASY_FLOOR;
                }                
            }
            else
            {
                if (CanDrawHint(pos))
                {
                    // 지뢰에 인접한 경우 힌트 표시
                    currentTile = ETile.MINE_HINT;
                }              
            }

            return currentTile;
        }
        #endregion

        #region 입력
        static void Input()
        {
            Game.inputKey = Console.ReadKey(true).Key;
        }
        #endregion

        #region 게임 로직
        static void Update()
        {
            if (Move())
            {
                CheckState();
            }            
        }

        static bool Move()
        {
            Vector2Int direction = GetDirection(Game.inputKey);
            Vector2Int next = Game.playerPos + direction;

            // 이동 키 이외 입력으로 인해 위치 변경 안되는 경우
            if (Game.playerPos == next)
            {
                return false;
            }

            // 벽 등의 장애물에 막혀 갈수 없는 경우
            if (CanGo(next) == false)
            {
                return false;
            }

            Game.playerPos = next;
            // 방문한 곳을 기억해둔다.
            Game.currentMap.isVisited[next.y, next.x] = true;
            return true;
        }

        static Vector2Int GetDirection(ConsoleKey key)
        {
            Vector2Int direction = new Vector2Int();

            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    direction = Direction[(int)EDirection.UP];
                    break;

                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    direction = Direction[(int)EDirection.DOWN];
                    break;

                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    direction = Direction[(int)EDirection.LEFT];
                    break;

                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    direction = Direction[(int)EDirection.RIGHT];
                    break;

                default:
                    return new Vector2Int(0, 0);
            }

            return direction;
        }

        static bool IsVisited(int x, int y)
        {
            return Game.currentMap.isVisited[y, x];
        }

        static void CheckState()
        {
            // 플레이어가 지뢰의 위치에 닿은 경우
            if (GetTileToGrid(Game.playerPos) == ETile.MINE)
            {
                Console.Clear();

                PrintMessage("지뢰를 밟았습니다..\n\n", ConsoleColor.Red);
                Game.isRunning = false;
                Game.playerPos.SetPos(-1, -1);
                return;
            }

            // 플레이어가 안전지대의 위치에 닿은 경우
            if (GetTileToGrid(Game.playerPos) == ETile.GOAL)
            {
                Console.Clear();

                // 마지막 스테이지 였으면
                if (Game.currentStageIndex == maps.Length-1)
                {
                    // 게임 끝
                    Game.isRunning = false;
                    PrintMessage("ALL Clear!!\n\n", ConsoleColor.Green);
                    return;
                }

                PrintMessage($"Stage{Game.currentStageIndex + 1} Clear!\n", ConsoleColor.Green);
                PrintMessage("\n계속 진행하려면 아무 키나 누르세요", ConsoleColor.Yellow);
                Console.ReadKey();

                SetNextStage();
                return;
            }

            // 지뢰가 인접한지 확인
            CheckExistNearMine();
        }

        static void CheckExistNearMine()
        {
            bool bResult = false;
            Vector2Int checkPos;

            // 플레이어 위치로부터 시계방향으로 한칸 씩 지뢰 확인
            for (int i = 0; i < Direction.Length; i++)
            {
                checkPos = Game.playerPos + Direction[i];
                if (GetTileToGrid(checkPos) == ETile.MINE)
                {
                    bResult = true;
                }
            }
            // Flag set
            Game.bWarning = bResult;
        }

        // 좌표를 전달받아 갈 수 있는 곳인지 확인
        static bool CanGo(Vector2Int pos)
        {
            if (Game.currentMap.grid[pos.y, pos.x] == ETile.WALL)
                return false;

            return true;
        }

        static int GetDistance(Vector2Int dest, Vector2Int pos)
        {
            // 두 좌표간의 거리를 구한다.
            // 유니티에서 dest - pos 후 magnitude를 구하는 방식을 떠올려 적용하였음 
            Vector2Int vector = dest - pos;

            // 소수를 그대로 사용하는 경우 1.4 처럼 1보다 큰 경우가 생겨 int로 강제 형변환 진행
            return (int)Math.Sqrt((vector.x * vector.x) + (vector.y * vector.y));
        }

        static bool IsPointNearByPlayer(Vector2Int pos)
        {
            int distance = GetDistance(Game.playerPos, pos);
            return distance <= 1;
        }

        static bool CanDrawHint(Vector2Int checkPos)
        {
            // 플레이어가 지뢰에 인접했는지 확인
            if (Game.bWarning == false)
            {
                return false;
            }

            // 체크할 맵상 좌표가 플레이어와 인접한지
            if (IsPointNearByPlayer(checkPos) == false)
            {
                return false;
            }

            // 체크할 맵상 좌표가 안전지대인지
            if (GetTileToGrid(checkPos) == ETile.GOAL)
            {
                return false;
            }

            // 체크할 맵상 좌표가 이동가능한 곳인지
            if (CanGo(checkPos) == false)
            {
                return false;
            }

            // 위 조건들을 통과하면 해당 좌표에 힌트를 출력해야 함
            return true;
        }
        #endregion

        #region Etc
        static void PrintMessage(string msg, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ResetColor();
        }
        #endregion

        #region End
        static void End()
        {
            Game.bWarning = false;

            DrawGrid();

            PrintMessage("\nGAME OVER...\n", ConsoleColor.DarkRed);

            // 게임 종료 의사를 확인
            CheckExit();
        }

        static void CheckExit()
        {
            Console.CursorVisible = true;
            int iInput;
            while (true)
            {
                PrintMessage("\n1. 타이틀로 돌아간다\n", ConsoleColor.Cyan);
                PrintMessage("2. 게임 종료\n", ConsoleColor.Cyan);
                PrintMessage(">> ");
                int.TryParse(Console.ReadLine(), out iInput);

                switch (iInput)
                {
                    case 1: return;
                    case 2: bExit = true; return;
                }
            }
        }
        #endregion
    }
}
