using Godot;
using System;
using System.Collections.Generic;
using World;

using Base;

namespace World.Dungeon.Generators
{
    /// <summary>
    /// A simple dungeon generator. 
    /// </summary>
    /// <remarks>
    /// The algorithms works but is not the fastest I can think. It needs some polishment. It spawns a fixed
    /// amount of rooms an connects them with an "L" shaped corridor. There's a problem with the door but, instead
    /// of trying to fix it's better to use another generator. TODO: reference to generator.
    /// <para>
    /// The best way to use this generators is to cramp a lot of rooms, intead of creating a dungeon with long corridors and
    /// differents rooms, I think this looks best with all narrow and, well, doesn't need rooms this way.
    /// </para>
    /// </remarks>
    public class SimpleGenerator : BaseGenerator
    {
        /// <summary>
        /// A list of the dungeons <see cref="Room"/>
        /// </summary>
        List<Room> _dungeonRooms;

        /// <summary>
        /// A list of the dungeons <see cref="SimpleCorridor"/>
        /// </summary>
        List<SimpleCorridor> _corridors;

        /// <summary>
        /// Tile wall space
        /// </summary>
        readonly int WALL_SPACE = 1;

        /// <summary>
        /// The max number of rooms
        /// </summary>
        readonly int WALL_NUMBER;

        readonly int WALL_MAX = 8;
        readonly int WALL_MIN = 4;

        /// <summary>
        /// The limits of the roomsize
        /// </summary>
        private (int min, int max) _roomSize;

        /// <summary>
        /// the mapsize
        /// </summary>
        private MyPoint _mapSize;

        /// <summary>
        /// the min pos of the map.
        /// Selected 1-1 for the tile wall.
        /// </summary>
        private MyPoint _minPos = new MyPoint(1, 1);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minSizeRoom">Rooms minimum size</param>
        /// <param name="maxSizeRoom">Rooms max sie</param>
        /// <param name="maxRoom">Max number of rooms</param>
        /// <param name="mapSize">The size of the map</param>
        public SimpleGenerator(in int minSizeRoom, in int maxSizeRoom, in int maxRoom, in MyPoint mapSize)
        {
            this._roomSize = (minSizeRoom, maxSizeRoom);
            this._mapSize = mapSize;
            this.Tiles = new Tile?[mapSize.X, mapSize.Y];
            this.WALL_NUMBER = maxRoom;
        }

        public override ref Tile?[,] GetTiles(out Vector2 pos)
        {
            RandomNumberGenerator r = new RandomNumberGenerator();

            //primero creamos las habitaciones
            this.CreateRooms(WALL_NUMBER, r);

            //ahora conectamos
            this.CreateCorridors(r);
            pos = this.GetInitPos();

            return ref this.Tiles;
        }

        public override (Tile?[,] tiles, Vector2 playerPos, Dictionary<MyPoint, EnitityType> enemies) GetWholePack()
        {
            Vector2 playerPos;
            Tile?[,] tiles = this.GetTiles(out playerPos);
            Dictionary<MyPoint, EnitityType> enemies = this.SpawnEnemies(new RandomNumberGenerator());

            return (tiles, playerPos, enemies);
        }

        public Vector2 GetInitPos()
        {
            return new Vector2(_dungeonRooms[0].CenterX, _dungeonRooms[0].CenterY);
        }


        public SimpleGenerator()
        {
            //default setting to test
            this._roomSize = (4, 8);
            _mapSize = new MyPoint(61, 30);

            this._corridors = new List<SimpleCorridor>();
            this._dungeonRooms = new List<Room>();
            Tiles = new Tile?[_mapSize.X, _mapSize.Y];
            RandomNumberGenerator gen = new RandomNumberGenerator();
            WALL_NUMBER = gen.RandiRange(WALL_MIN, WALL_MAX);
        }

        private void CorridorToMap(in SimpleCorridor corridor, in Room origin, in Room dest)
        {
            Vector2 point;
            List<Vector2> corrPoints = new List<Vector2>();

            if (corridor.Corner.x > corridor.Start.x)
            {
                //TODO: create a method for the operations in here 
                for (int x = (int)corridor.Corner.x; x >= corridor.Start.x; x--)
                {
                    point = new Vector2(x, corridor.Corner.y);
                    Tiles[(int)point.x, (int)corridor.Corner.y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);
                }
            }
            else
            {
                for (int x = (int)corridor.Corner.x; x <= corridor.Start.x; x++)
                {
                    point = new Vector2(x, corridor.Corner.y);
                    Tiles[(int)point.x, (int)corridor.Corner.y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);
                }
            }

            if (corridor.Corner.y < corridor.End.y)
            {
                for (int y = (int)corridor.Corner.y; y <= corridor.End.y; y++)
                {
                    point = new Vector2(corridor.Corner.x, y);
                    Tiles[(int)point.x, y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);
                }
            }
            else
            {
                for (int y = (int)corridor.Corner.y; y >= corridor.End.y; y--)
                {
                    point = new Vector2(corridor.Corner.x, y);

                    Tiles[(int)point.x, y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);
                }
            }

            //ahora, una vez hemos metido todos los puntos, vamos a mirar suelo x suelo y chekear vecinos para meter paredes
            foreach (Vector2 v in corrPoints)
            {
                this.CheckNeigbour(v, corrPoints);
            }

            this.CreateDoors(corrPoints);


        }

        private void CreateDoors(in List<Vector2> corridors)
        {
            bool door;
            List<MyPoint> doorsPos = new List<MyPoint>();

            foreach (Vector2 v in corridors)
            {
                door = true;

                MyPoint p = new MyPoint((int)v.x, (int)v.y);

                //primero miramos si está entre dos bloques verticales
                if (Tiles[p.X, p.Y + 1].HasValue && Tiles[p.X, p.Y + 1].Value.MyType != Tile.TileType.FLOOR &&
                    Tiles[p.X, p.Y - 1].HasValue && Tiles[p.X, p.Y - 1].Value.MyType != Tile.TileType.FLOOR)
                {
                    //miramos si a la derecha tiene suelo
                    if (Tiles[p.X + 1, p.Y].HasValue && Tiles[p.X + 1, p.Y].Value.MyType == Tile.TileType.FLOOR)
                    {
                        door = IsTileYSegmentAType(p.X + 1, p.Y - 1, p.Y + 1, Tile.TileType.FLOOR);

                        if (door)
                            Messages.Print("Simple generator", "Rigth door is done!");
                    }
                    else
                    {
                        door = false;
                    }

                    //si no hay puerta, miramos a ver
                    if (door == false && Tiles[p.X - 1, p.Y].HasValue && Tiles[p.X - 1, p.Y].Value.MyType == Tile.TileType.FLOOR)
                    {
                        door = IsTileYSegmentAType(p.X - 1, p.Y - 1, p.Y + 1, Tile.TileType.FLOOR);

                    }
                }
                else
                {
                    door = false;
                }


                if (door == false && Tiles[p.X - 1, p.Y].HasValue && Tiles[p.X - 1, p.Y].Value.MyType != Tile.TileType.FLOOR &&
                    Tiles[p.X + 1, p.Y].HasValue && Tiles[p.X + 1, p.Y].Value.MyType != Tile.TileType.FLOOR)
                {
                    door = true;

                    //miramos si arriba
                    if (Tiles[p.X, p.Y + 1].HasValue && Tiles[p.X, p.Y + 1].Value.MyType == Tile.TileType.FLOOR)
                    {
                        door = IsTileXSegmentAType(p.Y + 1, p.X - 1, p.X + 1, Tile.TileType.FLOOR);

                        if (door)
                            Messages.Print("Simple Generator", "Down done");
                    }
                    else
                    {
                        door = false;
                    }

                    //si hemos modificado, pasamos, sino miramos a ver qué se cuenta
                    if (door == false && Tiles[p.X, p.Y - 1].HasValue && Tiles[p.X, p.Y - 1].Value.MyType == Tile.TileType.FLOOR)
                    {
                        door = IsTileXSegmentAType(p.Y - 1, p.X - 1, p.X + 1, Tile.TileType.FLOOR);

                        if (door)
                            Messages.Print("Simple Generator", "Up done");
                    }
                }

                if (door)
                {
                    Tiles[p.X, p.Y] = new Tile(Tile.TileType.DOOR, true);

                    //añadimos para comprobar
                    doorsPos.Add(p);
                }
            }

            //ok, ahora comprobamos si una puerta tiene 
            int wallS = 0;
            const int wallLimit = 5;

            MyPoint[] eightDir = { new MyPoint(1, 0), new MyPoint(-1, 0), new MyPoint(0, 1), new MyPoint(0, -1),
                                   new MyPoint(1,1), new MyPoint(-1,1), new MyPoint(1,-1), new MyPoint(-1,-1) };
            MyPoint tempPoint;
            MyPoint current;
            Tile? tile;

            for (int d = doorsPos.Count - 1; d >= 0; d--)
            {
                current = doorsPos[d];
                //check every pos
                for (int i = 0; i < eightDir.Length; i++)
                {
                    tempPoint = current + eightDir[i];

                    if(IsInsideBounds(tempPoint) == false){
                        continue;
                    }
                    tile = Tiles[tempPoint.X, tempPoint.Y];

                    if (tile.HasValue)
                    {
                        if (tile.Value.MyType == Tile.TileType.WALL)
                        {
                            wallS++;
                        }
                    }
                }

                if (wallS >= wallLimit)
                {
                    Tiles[current.X, current.Y] = new Tile(Tile.TileType.FLOOR);
                    doorsPos.RemoveAt(d);
                }
            }

            //ahora miramos por cada puerta el lado en el que tiene una pared, si la tiene, le metemos al opuesto
            //pfff, solucionar bugs es más complicado casi que hacer otra cosa.
            foreach (MyPoint p in doorsPos)
            {

                for (int i = 0; i < eightDir.Length - 3; i++)
                {
                    current = p + eightDir[i];
                    //si hay valor y es wall, el opuesto también lo será
                    if (Tiles[current.X, current.Y].HasValue && Tiles[current.X, current.Y].Value.MyType == Tile.TileType.WALL)
                    {
                        /*Messages.Print("Yoloooooooooooo");
                        Messages.Print("before pos:", current.ToString());
                        Messages.Print("center pos: ", p.ToString());*/
                        current = p + new MyPoint(eightDir[i].X * -1, eightDir[i].Y * -1);
                        //Messages.Print("after pos:", current.ToString());
                        Tiles[current.X, current.Y] = new Tile(Tile.TileType.WALL);
                        break;
                    }
                }
            }
        }

        private bool IsTileYSegmentAType(in int xConst, in int yOrigin, in int yDest, in Tile.TileType type)
        {

            for (int y = yOrigin; y <= yDest; y++)
            {
                if (Tiles[xConst, y].HasValue && Tiles[xConst, y].Value.MyType != type)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsTileXSegmentAType(in int yConst, in int xOrigin, in int xDest, in Tile.TileType type)
        {

            for (int x = xOrigin; x <= xDest; x++)
            {
                if (Tiles[x, yConst].HasValue && Tiles[x, yConst].Value.MyType != type)
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckNeigbour(in Vector2 point, in List<Vector2> lisPoints)
        {
            //primero miramos los 8 puntos 
            const int xNeig = 1;
            const int yNeig = 1;

            for (int x = (int)point.x - xNeig; x <= (int)point.x + xNeig; x++)
            {
                for (int y = (int)point.y - yNeig; y <= (int)point.y + yNeig; y++)
                {
                    if ((x == point.x && y == point.y) || this.IsInsideBounds(x,y) == false)
                    {
                        continue;
                    }

                    //si es nullo o bien tiene suelo y pertenece a una habitación, metemos pared.
                    if (Tiles[x, y].HasValue == false || (Tiles[x, y].Value.MyType == Tile.TileType.FLOOR && (IsPointInRoom(point) && IsPointInList(point, lisPoints) == false)))
                    {
                        Tiles[x, y] = new Tile(Tile.TileType.WALL, true);
                    }
                }
            }
        }

        public bool IsInsideBounds(in MyPoint point)
        {
            return this.IsInsideBounds(point.X, point.Y);
        }

        public bool IsInsideBounds(in int x, in int y)
        {
            return !(x < 0 || x > _mapSize.X || y < 0 || y >= _mapSize.Y);
        }

        #region room methods

        /// <summary>
        /// Creates x rooms in a simple way. We span a room in a random position between <see cref="PosX_Min"/> / <see cref="PosY_Min"/> & <see cref="PosX_Max"/> / <see cref="PosY_Max"/>
        /// with a random size of <see cref="SIZE_MIN"/> & <see cref="SIZE_MAX"/> and, if the room collides with anothes, pass to the other
        /// </summary>
        /// <param name="rooms">the number of rooms to create</param>
        /// <param name="r">the random generator</param>
        private void CreateRooms(in int roomsNumber, in RandomNumberGenerator r)
        {
            //randomize the generator
            r.Randomize();

            //we need to create a **** point class with int
            int width;
            int height;
            int topX;
            int topY;

            Room room;

            for (int i = 0; i < roomsNumber; i++)
            {
                //random measurements
                width = r.RandiRange(_roomSize.min, _roomSize.max);
                height = r.RandiRange(_roomSize.min, _roomSize.max);
                topX = r.RandiRange(0, _mapSize.X - _roomSize.max - 1);
                topY = r.RandiRange(0, _mapSize.Y - _roomSize.max - 1);

                //create the data
                room = new Room(topX, topY, topX + width, topY + height);

                //if there's collision with another room, pass and get another
                if (IsRoomColliding(room))
                {
                    i--;
                    continue;
                }

                //add to the list
                _dungeonRooms.Add(room);
            }

            this.RoomToMap(_dungeonRooms);
        }

        private void RoomToMap(List<Room> dungeonRooms)
        {
            List<Vector2> temp;

            foreach (Room r in dungeonRooms)
            {
                //get the walls for each room and put onto the map
                temp = r.GetWallsPositions();

                for (int i = 0; i < temp.Count; i++)
                {
                    Tiles[(int)temp[i].x, (int)temp[i].y] = new Tile(Tile.TileType.WALL, true);
                }

                //get the floor for each room and put onto the map
                temp.Clear();
                temp = r.GetFloorPositions();

                for (int i = 0; i < temp.Count; i++)
                {
                    Tiles[(int)temp[i].x, (int)temp[i].y] = new Tile(Tile.TileType.FLOOR, true);
                }

            }
        }

        /// <summary>
        /// Check's if a single room collides with another room in the list <see cref="_dungeonRooms"/>
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private bool IsRoomColliding(in Room room)
        {
            for (int i = 0; i < _dungeonRooms.Count; i++)
            {
                if (room.TopLeft.X < _dungeonRooms[i].BottomRight.X &&
                    room.BottomRight.X > _dungeonRooms[i].TopLeft.X &&
                    room.TopLeft.Y < _dungeonRooms[i].BottomRight.Y &&
                    room.BottomRight.Y > _dungeonRooms[i].TopLeft.Y)
                {
                    //Messages.Print("Simple Generator", "Collision detected");
                    return true;
                }
            }

            //para la primera
            return false;
        }

        #endregion

        #region corridor
        /// <summary>
        /// Creates the corridors to join diferents <see cref="Room"/>. It uses a simple "L" shaped form
        /// <remarks>
        /// So, the algorithm is very simple. We check for the center of the origin and destination room, choose an axis
        /// to begin to connect the origin room to destination and find a corner that is the center of the oposite axis. 
        /// Doing this way assures an "L" shape.
        /// 
        ///   ##### 
        ///   #...#
        ///   #...OOOOOOOO -> corner
        ///   #...#      O
        ///   #####      O
        ///              O
        ///              O 
        ///           ###O##
        ///           #....#
        ///           #....#
        ///           ######
        ///           
        /// </remarks>
        /// </summary>
        /// <param name="r">random generator</param>
        private void CreateCorridors(in RandomNumberGenerator r)
        {
            //randomize the generator
            r.Randomize();
            Room origin;
            Room dest;

            //every room connects with the previous room
            for (int i = 1; i < _dungeonRooms.Count; i++)
            {
                //get the origin and dest, easier to operate                
                origin = _dungeonRooms[i];
                dest = _dungeonRooms[i - 1];
                Vector2 start, end, corner;

                //50% chances to begin the corridor in horizontal, vertical

                if (r.RandiRange(0, 100) < 50)
                {
                    this.CreateCorridorXAxis(origin, dest, out corner, out start);
                    this.CreateCorridorYAxis(origin, dest, corner, out end);
                    //Messages.Print("Chances says: ", "chosen first");
                }
                else
                {
                    this.CreateCorridorXAxis(dest, origin, out corner, out start);
                    this.CreateCorridorYAxis(dest, origin, corner, out end);
                    //Messages.Print("Chances says: ", "chosen second");
                }

                //debug messages
                /*Messages.Print("Origin: " + origin.TopLeft.X + "/" + origin.TopLeft.Y);
                Messages.Print("Destination: " + dest.TopLeft.X + "/" + dest.TopLeft.Y);
                Messages.Print("Center Origin: (" + origin.CenterX + "," + origin.CenterY + ")");
                Messages.Print("Center Destination: (" + dest.CenterX + "," + dest.CenterY + ")");*/

                SimpleCorridor s = new SimpleCorridor(start, end, corner);
                this.CorridorToMap(s, origin, dest);

                //add the corridor to the list
                _corridors.Add(new SimpleCorridor(start, end, corner));
            }
        }

        /// <summary>
        /// OJU! Change name!
        /// </summary>
        /// <param name="origin">The room that is the origin</param>
        /// <param name="dest">The room that is the destination</param>
        /// <param name="corner">The corner between the two rooms. OUT parameter</param>
        /// <param name="point">The beggining point in the origin room. OUT parameter</param>
        private void CreateCorridorXAxis(in Room origin, in Room dest, out Vector2 corner, out Vector2 point)
        {
            if (origin.CenterX > dest.CenterX)
            {
                point = new Vector2(origin.TopLeft.X, origin.CenterY);
                corner = new Vector2(dest.CenterX, origin.CenterY);
            }
            else
            {
                point = new Vector2(origin.BottomRight.X, origin.CenterY);
                corner = new Vector2(dest.CenterX, origin.CenterY);
            }
        }

        /// <summary>
        /// OJU! Cambiar el nombre
        /// </summary>
        /// <param name="origin">The origin room</param>
        /// <param name="dest">The destination room</param>
        /// <param name="corner">The corner vbetween the two rooms</param>
        /// <param name="point">The point in the origin room</param>
        private void CreateCorridorYAxis(in Room origin, in Room dest, in Vector2 corner, out Vector2 point)
        {
            if (origin.CenterY > dest.CenterY)
            {
                point = new Vector2(corner.x, dest.BottomRight.Y);
            }
            else if (origin.CenterY < dest.CenterY)
            {
                point = new Vector2(corner.x, dest.TopLeft.Y);
            }
            else
            {
                point = new Vector2(corner.x, dest.CenterY);
            }
        }

        #endregion

        #region points method

        /// <summary>
        /// Checks if a point is inside any room of the <see cref="_dungeonRooms"/>.
        /// OJU! It looks for each room of the dungeon.
        /// </summary>
        /// <param name="point">The point that we want to check</param>
        /// <returns>Is the point in any room?</returns>
        private bool IsPointInRoom(in Vector2 point)
        {
            foreach (Room room in _dungeonRooms)
            {
                if ((room.TopLeft.X <= point.x && room.BottomRight.X >= point.x && room.TopLeft.Y <= point.y && room.BottomRight.Y >= point.y))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPointInList(in Vector2 point, in List<Vector2> listPoints)
        {

            foreach (Vector2 v in listPoints)
            {
                if (v.x == point.x && v.y == point.y)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPointInRoom(in Vector2 point, in Room room)
        {
            if ((room.TopLeft.X + 1 <= point.x && room.BottomRight.X - 1 >= point.x && room.TopLeft.Y + 1 <= point.y && room.BottomRight.Y - 1 >= point.y))
            {
                return true;
            }

            return false;
        }
        #endregion

        //esto irá en otra parte, creo
        const int ENEMY_COUNT_MIN = 0;
        const int ENEMY_COUNT_MAX = 2;
        const int PROB_ORC = 80;
        const int PROB_TROLL = 20;

        public override Dictionary<MyPoint, EnitityType> SpawnEnemies(in RandomNumberGenerator r)
        {
            Dictionary<MyPoint, EnitityType> enemies = new Dictionary<MyPoint, EnitityType>();
            r.Randomize();
            int result;
            for (int i = 1; i < _dungeonRooms.Count; i++)
            {
                result = r.RandiRange(ENEMY_COUNT_MIN, ENEMY_COUNT_MAX);

                if (result > 0)
                {
                    int prob = r.RandiRange(1, 100);
                    EnitityType enemy = EnitityType.EMPTY;

                    if (prob <= PROB_TROLL)
                    {
                        enemy = EnitityType.TROLL;
                    }
                    else
                    {
                        enemy = EnitityType.ORC;
                    }

                    MyPoint pos;
                    bool inRoom = false;
                    List<Vector2> roomPos = _dungeonRooms[i].GetFloorPositions();

                    while (inRoom == false)
                    {
                        result = r.RandiRange(0, roomPos.Count - 1);
                        pos = (MyPoint)roomPos[result];

                        if (enemies.ContainsKey(pos) == false)
                        {
                            inRoom = true;
                            enemies.Add(pos, enemy);
                        }
                    }
                }

            }

            return enemies;
        }
    }

}