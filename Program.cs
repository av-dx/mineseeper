using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML;
using SFML.System;

namespace Minesweeper
{
    class Program
    {
        const int boardWidth = 16;
        const int boardHeight = 16;
        const int bombCount = 40;
        static int[,] activeboard;
        static RenderWindow window;
        static int[,] board;
        static VertexArray TileArray;
        static Font arial = new Font(@"e:\users\aashwin\documents\visual studio 2017\Projects\Minesweeper\Minesweeper\arial.ttf");
        static Texture tex = new Texture(@"e:\users\aashwin\documents\visual studio 2017\Projects\Minesweeper\Minesweeper\closed.png");

        static void Main()
        {
            board = new int[boardWidth, boardHeight];
            activeboard = new int[boardWidth, boardHeight];

            GenerateBoard(ref board, boardHeight, boardWidth, bombCount);
            //PrintBoard(board);

            window = new RenderWindow(new VideoMode(boardWidth * 32, boardHeight * 32), "MineSeeper");


            
            Text time = new Text("Time: ",arial);
            Text seconds = new Text("0", arial);


            Clock clock = new Clock();

            InitializeBoard(board, out TileArray);

            window.Closed += new EventHandler(OnClose);
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(MouseButtonPressed);

            while (window.IsOpen)
            {
                seconds = new Text(" " + (int)clock.ElapsedTime.AsSeconds(), arial);
                seconds.Position = new Vector2f(100, 0);
                window.DispatchEvents();
                window.Clear();

                Vector2i mousePos = Mouse.GetPosition(window);
                Vector2i clickedTilePos = mousePos / 32;
               
                
                window.Draw(TileArray , new RenderStates(tex));
                window.Draw(time);
                window.Draw(seconds);
                window.Display();
            }
        }



        static void OpenTile(ref VertexArray TileArray, ref int[,] board, Vector2i tilePos)
        {

            int TileX = tilePos.X;
            int TileY = tilePos.Y;
            if ((TileX < 0)|| (TileY<0))
            {
                return;
            }
            if ((TileX >= boardWidth) || (TileY >= boardHeight))
            {
                return;

            }
            if (activeboard[TileX, TileY] == 1) return;
            int Tile = board[tilePos.X, tilePos.Y];

            SetTile(ref TileArray, tilePos, Tile);
            activeboard[tilePos.X, tilePos.Y] = 1;

            if (Tile == 0)
            {
                for (int nearX = -1; nearX <= 1; nearX++)
                {
                    for (int nearY = -1; nearY <= 1; nearY++)
                    {
                        if (TileX + nearX == -1) continue;
                        if (TileX + nearX == boardWidth) continue;
                        if (TileY + nearY == -1) continue;
                        if (TileY + nearY == boardHeight) continue;


                        OpenTile(ref TileArray, ref board, new Vector2i(TileX + nearX, TileY + nearY));
                    }
                }


            }
            else if (Tile == 9)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    for (int y = 0; y < boardHeight; y++)
                    {
                        if (board[x, y] == 9) SetTile(ref TileArray, new Vector2i(x, y), 9);
                    }
                }

                while (window.IsOpen)
                {
                    window.DispatchEvents();
                    window.Clear();
                    window.Draw(TileArray, new RenderStates(tex));
                    window.Draw(new Text("GAME OVER!", arial));
                    window.Display();
                }
                Main();

            }
            return;
        }

        static void SetTile(ref VertexArray tileArray,Vector2i tilePos,int tile)
        {
            int x = tilePos.X;
            int y = tilePos.Y;
            if ((x < 0) || (y < 0))
            {
                return;
            }
            if ((x >= boardWidth) || (y >= boardHeight))
            {
                return;
            }

            tileArray[(uint)((y * boardWidth) + x) * 4] = new Vertex(new Vector2f(32 * x, 32 * y), new Vector2f(0, tile * 32));
            tileArray[(uint)((y * boardWidth) + x) * 4 + 1] = new Vertex(new Vector2f(32 * x, 32 + (32 * y)), new Vector2f(0, tile * 32 + 32));
            tileArray[(uint)((y * boardWidth) + x) * 4 + 2] = new Vertex(new Vector2f(32 + (32 * x), 32 + (32 * y)), new Vector2f(32, tile * 32 + 32));
            tileArray[(uint)((y * boardWidth) + x) * 4 + 3] = new Vertex(new Vector2f(32 + (32 * x), (32 * y)), new Vector2f(32, tile * 32));
            return;
        }

        static void InitializeBoard(int[,] board,out VertexArray TileArray)
        {
            TileArray = new VertexArray(PrimitiveType.Quads, boardHeight * boardWidth * 4);
            for (int y = 0; y < boardHeight; y++)
            {

                for (int x = 0; x < boardWidth; x ++)
                {
                    SetTile(ref TileArray,new Vector2i(x,y), 10);

                }
            }


        }

        static void MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            Vector2i mousePos = Mouse.GetPosition(window);
            Vector2i clickedTilePos = mousePos / 32;
            

            if ((clickedTilePos.X < boardWidth) && (clickedTilePos.Y < boardHeight))
            {
                if ((clickedTilePos.X >= 0) && (clickedTilePos.Y >= 0))
                {
                    if (e.Button == Mouse.Button.Left)
                    {
                        int clickedTile = board[clickedTilePos.X, clickedTilePos.Y];
                        OpenTile(ref TileArray, ref board, clickedTilePos);
                    }
                    else if (e.Button == Mouse.Button.Right)
                    {
                        int clickedTile = board[clickedTilePos.X, clickedTilePos.Y];
                        if (activeboard[clickedTilePos.X, clickedTilePos.Y] == 0)
                        {
                            SetTile(ref TileArray, clickedTilePos, 11);
                            activeboard[clickedTilePos.X, clickedTilePos.Y] = 2;
                        }
                        else if (activeboard[clickedTilePos.X, clickedTilePos.Y] == 2)
                        {
                            SetTile(ref TileArray, clickedTilePos, 10);
                            activeboard[clickedTilePos.X, clickedTilePos.Y] = 0;
                        }


                    }
                }
            }

        }

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }


        static void GenerateBoard(ref int[,] a, int height, int width, int bomb)
        {
            Random rand = new Random();
            for (int i=1; i<=bomb; i++)
            {
                int x = rand.Next(0, width-1);
                int y = rand.Next(0, height-1);

                if (a[x, y] != 9)
                {
                    a[x, y] = 9;
                }

            }

            for (int x=0;x<width;x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (a[x,y]!=9)
                    {
                        int bombs = 0;
                        for (int nearX = -1; nearX <= 1; nearX++)
                        {
                            for (int nearY = -1; nearY <= 1; nearY++)
                            {
                                if (x + nearX == -1) continue;
                                if (x + nearX == width) continue;
                                if (y + nearY == -1) continue;
                                if (y + nearY == height) continue;

                                if (a[x+nearX,y+nearY]==9)
                                {
                                    bombs++;
                                }
                            }
                        }
                        a[x, y] = bombs;
                    }
                }
            }

        }
        static void PrintBoard(int[,] board)
        {
            
            char[,] charBoard = new char[boardWidth, boardHeight];
            string position="";  // <top-bottom>;<left-right>

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.SetWindowSize(66,36);

            for (int y = 0; y <= boardHeight; y++)

            {

                for (int x = 0; x <= boardWidth; x++)

                {
                    position = "";

                    if (y == 0) position = string.Concat(position, "top;");
                    else if (y == boardHeight) position = string.Concat(position, "bottom;");
                    else position = string.Concat(position, "center;");


                    if (x == 0) position = string.Concat(position, "left;");
                    else if (x == boardWidth) position = string.Concat(position, "right;");
                    else position = string.Concat(position, "center;");

                    string posY = position.Split(';')[0];
                    string posX = position.Split(';')[1];

                    if ((posX == "left") && (posY == "top")) Console.Write('\u250F');
                    if ((posX == "left") && (posY == "bottom")) Console.Write('\u2517');
                    if ((posX == "left") && (posY == "center")) Console.Write('\u2523');

                    if ((posX == "right") && (posY == "top")) Console.Write('\u2513');
                    if ((posX == "right") && (posY == "bottom")) Console.Write('\u251B');
                    if ((posX == "right") && (posY == "center")) Console.Write('\u252B');

                    if ((posX == "center") && (posY == "top")) Console.Write('\u2533');
                    if ((posX == "center") && (posY == "bottom")) Console.Write('\u253B');
                    if ((posX == "center") && (posY == "center")) Console.Write('\u254B');

                    if (x < boardWidth) Console.Write('\u2501');
                    if (x < boardWidth) Console.Write('\u2501');
                    if (x < boardWidth) Console.Write('\u2501');

                }

                Console.Write("\n\n");

            }

            Console.CursorTop = 1;
            Console.CursorLeft = 2;

            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    Console.CursorTop = 1+(y*2);
                    Console.CursorLeft = 2+(x*4);
                    if (board[x, y] == 0) charBoard[x, y] = ' ';
                    else if (board[x, y] == 9) charBoard[x, y] = '*';
                    else charBoard[x, y] = char.Parse(Convert.ToString(board[x, y]));

                    Console.Write(charBoard[x, y]);
                }
            }


            

        }
    }
}
