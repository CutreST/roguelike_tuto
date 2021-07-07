using Base;
using Godot;
using System;
using System.Collections.Generic;

namespace World.Dungeon.Generators
{

    public class RoomGridGenerator : BaseGenerator
    {
        RoomGrid _grid;
        private readonly int ROOM_NUMBER = 8;
        private readonly MyPoint ROOM_MIN_SIZE = new MyPoint(3, 3);
        private readonly MyPoint ROOM_MAX_SIZE = new MyPoint(6, 6);
        private readonly MyPoint GRID_CELL_SIZE = new MyPoint(10, 10);
        private readonly MyPoint GRID_SIZE = new MyPoint(5, 3);


        public RoomGridGenerator()
        {
            this.Tiles = new Tile?[GRID_SIZE.X * GRID_CELL_SIZE.X, GRID_SIZE.Y * GRID_CELL_SIZE.Y];
            this._grid = new RoomGrid(this.GRID_SIZE, this.GRID_CELL_SIZE);
        }

        public RoomGridGenerator(in MyPoint gridSize, in MyPoint cellSize, in MyPoint roomMaxSize, in MyPoint roomMinSize, in int numberOfRooms)
        {
            this._grid = new RoomGrid(gridSize, cellSize);
            this.ROOM_NUMBER = numberOfRooms;
            this.Tiles = new Tile?[gridSize.X * cellSize.X, gridSize.Y * cellSize.Y];
        }

        public override ref Tile?[,] GetTiles(out Vector2 pos)
        {
            RandomNumberGenerator gen = new RandomNumberGenerator();

            List<MyPoint> roomCoord;
            this.PutRoomsOnGrid(gen, out roomCoord);

            #region room cheker (disabled)
            /*
            Messages.Print("yeaaasdasdasdasda");
            for (int x = 0; x < _grid.GridSize.X; x++){
                for (int y = 0; y < _grid.GridSize.Y; y++){
                    MyPoint p = new MyPoint(x, y);
                    Messages.Print("Cell " + p.ToString() + " has room? ", _grid.HasRoomAt(x, y).ToString());
                }
            }*/
            #endregion

            //pintamos habitaciones
            List<Room> rooms;
            Messages.Print("Tiles size", "(" + Tiles.GetLength(0) + "," + Tiles.GetLength(1) + ")");
            rooms = this.CreateRooms(gen, roomCoord);

            //pintamos
            //TODO: esto podría ir en un punto a parte
            foreach(Room r in rooms){
                List<Vector2> coords = r.GetWallsPositions();
                
                foreach(Vector2 v in coords){
                    Tiles[(int)v.x, (int)v.y] = new Tile(Tile.TileType.WALL);
                }

                
                coords = r.GetFloorPositions();
                foreach(Vector2 v in coords){
                    Tiles[(int)v.x, (int)v.y] = new Tile(Tile.TileType.FLOOR);
                }
            }



            pos = Vector2.Zero;
            return ref this.Tiles;
        }

        private void PutRoomsOnGrid(in RandomNumberGenerator gen, out List<MyPoint> roomCoords)
        {
            gen.Randomize();
            roomCoords = new List<MyPoint>();

            MyPoint coord;
            //colocamos habitaciones en puntos random del grid
            for (int i = 0; i < this.ROOM_NUMBER; i++)
            {
                coord = new MyPoint(gen.RandiRange(0, this.GRID_SIZE.X - 1), gen.RandiRange(0, this.GRID_SIZE.Y - 1));

                if (this._grid.HasRoomAt(coord.X, coord.Y))
                {
                    i--;
                    continue;
                }
                else
                {
                    this._grid.SetRoomAt(coord.X, coord.Y);
                    Messages.Print("Roooooooooom at " + coord);
                    roomCoords.Add(coord);
                }
            }
        }

        private List<Room> CreateRooms(in RandomNumberGenerator gen, in List<MyPoint> roomCoord)
        {
            //lista de coordenadas del grid _grid que tiene una habitación.
            MyPoint minTopLeft;
            MyPoint maxTopLeft;
            MyPoint topLeft;
            MyPoint roomSize;
            Room room;

            List<Room> rooms = new List<Room>(roomCoord.Count);

            foreach (MyPoint p in roomCoord)
            {
                //para colocar la habitación hacemos grid.x * GRID_CELL_SIZE.X, grid.Y * GRID_CELL_SIZE.Y como mímimo del top left
                minTopLeft = new MyPoint(p.X * GRID_CELL_SIZE.X, p.Y * GRID_CELL_SIZE.Y);
                Messages.Print("Min top left: " + minTopLeft);

                //para el máximo pillamos eso y le restamos el tamaño máximo de la habitación
                maxTopLeft = new MyPoint(minTopLeft.X + GRID_CELL_SIZE.X - ROOM_MAX_SIZE.X - 1,
                                         minTopLeft.Y + GRID_CELL_SIZE.Y - ROOM_MAX_SIZE.Y - 1);

                //y tenemos un topleft ramndom
                topLeft = new MyPoint(gen.RandiRange(minTopLeft.X, maxTopLeft.X),
                                         gen.RandiRange(minTopLeft.Y, maxTopLeft.Y));

                //ahrioa un tamaño random
                roomSize = new MyPoint(gen.RandiRange(ROOM_MIN_SIZE.X, ROOM_MAX_SIZE.X),
                                       gen.RandiRange(ROOM_MAX_SIZE.Y, ROOM_MAX_SIZE.Y));

                //y ahor auna habitación
                room = new Room(topLeft.X, topLeft.Y, topLeft.X + roomSize.X, topLeft.Y + roomSize.Y);
                Messages.Print("Top Left: " + topLeft);
                Messages.Print("BottomR: " + room.BottomRight);
                rooms.Add(room);
            }

            return rooms;

        }

        private void CreatePassages(in RandomNumberGenerator gen){
            //esto lo dejaremos para mañana

            //la idea es hacer un BDS o como se llame por cada habitacion del _Grid (por eso hemos creado eso básicamente) y las habitaciones
            //con las que se conecta (diccionario)
            //Así tendriamos:
                    // Grid[MyPoint_1], List<MyPoint> connexiones
                    // Grid[MyPoint_2], List<MyPoint> connexiones
                    //etc
            //allí, dependiendo del max de conexiones conectaremos
            //hará falta pensar alguna cosita extra, pero la idea general es esta


        }
    }
}
