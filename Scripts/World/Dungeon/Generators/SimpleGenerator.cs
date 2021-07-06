using Godot;
using System;
using System.Collections.Generic;
using World;

using Base;

namespace World.Dungeon.Generators
{
    //Ok, Tile, Room and Simple corridor are now structs.
    //Have to create a 2d tile? an pass it as the data.
    //The generator is responsible of the 2d, i think.
    public class SimpleGenerator
    {
        /// <summary>
        /// A list of the dungeons <see cref="Room"/>
        /// </summary>
        List<Room> _dungeonRooms;

        /// <summary>
        /// A list of the dungeons <see cref="SimpleCorridor"/>
        /// </summary>
        List<SimpleCorridor> _corridors;

        readonly int WALL_SPACE = 1;

        const int WALL_NUMBER = 5;

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

        private Tile?[,] _tiles;

        //ok, ahora creo que las tiles no las debe hacer esto, sino el controlador.
        //sin embargo, cómo sabe el controlador qué mierda hay?
        //quizás haré un diccionario tipo-lista y el controlador decida.
        //Así, por ejemplo, si queremos meter diferentes tiles en otros generadores 
        //no tenemos que ir haciendo mierdas, bueno, hay que pensarlo mu' bien.
        public SimpleGenerator(in int minSizeRoom, in int maxSizeRoom, in MyPoint mapSize)
        {
            this._roomSize = (minSizeRoom, maxSizeRoom);
            this._mapSize = mapSize;
            this._tiles = new Tile?[mapSize.X, mapSize.Y];
        }

        public ref Tile?[,] GetTiles(out Vector2 pos)
        {
            RandomNumberGenerator r = new RandomNumberGenerator();

            //primero creamos las habitaciones
            this.CreateRooms(WALL_NUMBER, r);

            //ahora conectamos
            this.CreateCorridors(r);
            pos = this.GetInitPos();

            return ref this._tiles;
        }

        public Vector2 GetInitPos(){
            return new Vector2(_dungeonRooms[0].CenterX, _dungeonRooms[0].CenterY);
        }


        public SimpleGenerator()
        {
            //default setting to test
            this._roomSize = (6, 16);
            _mapSize = new MyPoint(61, 31);

            this._corridors = new List<SimpleCorridor>();
            this._dungeonRooms = new List<Room>();
            _tiles = new Tile?[_mapSize.X, _mapSize.Y];
        }

        private void CorridorToMap(in SimpleCorridor corridor, in Room origin, in Room dest)
        {
            Vector2 point;
            List<Vector2> corrPoints = new List<Vector2>();

            if (corridor.Corner.x > corridor.Start.x)
            {
                for (int x = (int)corridor.Corner.x; x >= corridor.Start.x; x--)
                {
                    point = new Vector2(x, corridor.Corner.y);
                    _tiles[(int)point.x, (int)corridor.Corner.y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);

                    /*
                    if (IsPointInRoom(point) == false)
                    {
                        _tiles[(int)point.x, (int)corridor.Corner.y] = new Tile(Tile.TileType.FLOOR, true);
                        corrPoints.Add(point);
                    }
                    else
                    {
                        if (IsPointInRoom(point, dest)){
                            _tiles[(int)point.x, (int)corridor.Corner.y] = new Tile(Tile.TileType.DOOR, true);
                        }
                    }*/
                }
            }
            else
            {
                for (int x = (int)corridor.Corner.x; x <= corridor.Start.x; x++)
                {
                    point = new Vector2(x, corridor.Corner.y);
                    _tiles[(int)point.x, (int)corridor.Corner.y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);

                    /*if (IsPointInRoom(point) == false)
                    {
                        _tiles[(int)point.x, (int)corridor.Corner.y] = new Tile(Tile.TileType.FLOOR, true);
                        corrPoints.Add(point);
                    }*/

                }
            }

            if (corridor.Corner.y < corridor.End.y)
            {
                for (int y = (int)corridor.Corner.y; y <= corridor.End.y; y++)
                {
                    point = new Vector2(corridor.Corner.x, y);
                    _tiles[(int)point.x, y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);

                    /*if (IsPointInRoom(point) == false)
                    {
                        _tiles[(int)point.x, y] = new Tile(Tile.TileType.FLOOR, true);
                        corrPoints.Add(point);
                    }*/
                }
            }
            else
            {
                for (int y = (int)corridor.Corner.y; y >= corridor.End.y; y--)
                {
                    point = new Vector2(corridor.Corner.x, y);

                    _tiles[(int)point.x, y] = new Tile(Tile.TileType.FLOOR, true);
                    corrPoints.Add(point);

                    /*if (IsPointInRoom(point) == false)
                    {
                        _tiles[(int)point.x, y] = new Tile(Tile.TileType.FLOOR, true);
                        corrPoints.Add(point);
                    }*/
                }
            }

            //ahora, una vez hemos metido todos los puntos, vamos a mirar suelo x suelo y chekear vecinos para meter paredes
            foreach (Vector2 v in corrPoints)
            {
                this.CheckNeigbour(v, corrPoints);
            }

            //y, en teoria,
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
                    if(x == point.x && y == point.y){
                        continue;
                    }

                    //si es nullo o bien tiene suelo y pertenece a una habitación, metemos pared.
                    if (_tiles[x, y].HasValue == false || (_tiles[x, y].Value.MyType == Tile.TileType.FLOOR && (IsPointInRoom(point) && IsPointInList(point, lisPoints) == false)))
                    {
                        _tiles[x, y] = new Tile(Tile.TileType.WALL, true);
                    }
                }
            }
        }

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
                    _tiles[(int)temp[i].x, (int)temp[i].y] = new Tile(Tile.TileType.WALL, true);
                }

                //get the floor for each room and put onto the map
                temp.Clear();
                temp = r.GetFloorPositions();

                for (int i = 0; i < temp.Count; i++)
                {
                    _tiles[(int)temp[i].x, (int)temp[i].y] = new Tile(Tile.TileType.FLOOR, true);
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
                    Messages.Print("Simple Generator", "Collision detected");
                    return true;
                }
            }

            //para la primera
            return false;
        }

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
                }
                else
                {
                    this.CreateCorridorXAxis(dest, origin, out corner, out start);
                    this.CreateCorridorYAxis(dest, origin, corner, out end);
                }

                //debug messages
                Messages.Print("Origin: " + origin.TopLeft.X + "/" + origin.TopLeft.Y);
                Messages.Print("Destination: " + dest.TopLeft.X + "/" + dest.TopLeft.Y);
                Messages.Print("Center Origin: " + origin.CenterX);
                Messages.Print("Center Destination: " + dest.CenterX);

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
                if ((room.TopLeft.X + 1 <= point.x && room.BottomRight.X - 1 >= point.x && room.TopLeft.Y + 1 <= point.y && room.BottomRight.Y - 1 >= point.y))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPointInList(in Vector2 point, in List<Vector2> listPoints){

            foreach(Vector2 v in listPoints){
                if(v.x == point.x && v.y == point.y){
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







    }

}