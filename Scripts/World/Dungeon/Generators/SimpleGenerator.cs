using Godot;
using System;
using System.Collections.Generic;

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

        const int WALL_NUMBER = 6;

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


        public SimpleGenerator(in int minSizeRoom, in int maxSizeRoom, in MyPoint mapSize){
            this._roomSize = (minSizeRoom, maxSizeRoom);
            this._mapSize = mapSize;
        }   

        public SimpleGenerator()
        {
            //default setting to test
            this._roomSize = (2, 6);
            _mapSize = new MyPoint(61, 31);

            this._corridors = new List<SimpleCorridor>();
            this._dungeonRooms = new List<Room>();
            RandomNumberGenerator r = new RandomNumberGenerator();

            //oju!!! esto irá en otro lado.
            
            //primero creamos las habitaciones
            this.CreateRooms(WALL_NUMBER, r);

            //ahora conectamos
            this.CreateCorridors(r);

            //pasamos una lista con las cosas

            //TODO: hacer diferentes tipos de algo
        }

        public List<Vector2> GetWalls()
        {
            List<Vector2> walls = new List<Vector2>();
            foreach (Room r in _dungeonRooms)
            {
                walls.AddRange(r.GetWallsPositions());

            }

            return walls;
        }

        public List<Vector2> GetFloors()
        {
            List<Vector2> floor = new List<Vector2>();
            foreach (Room r in _dungeonRooms)
            {
                floor.AddRange(r.GetFloorPositions());
            }



            return floor;
        }

        public List<Vector2> GetCorridors()
        {

            List<Vector2> corridors = new List<Vector2>();
            Vector2 point;
            //lo ponemos aquí, habrá que mejorarlo
            foreach (SimpleCorridor c in _corridors)
            {                
                if (c.Corner.x > c.Start.x)
                {
                    for (int x = (int)c.Corner.x; x >= c.Start.x; x--)
                    {
                        point = new Vector2(x, c.Corner.y);    
                        if(IsPointInRoom(point) == false){
                            corridors.Add(point);
                        }                             
                    }
                }
                else
                {
                    for (int x = (int)c.Corner.x; x <= c.Start.x; x++)
                    {
                        point = new Vector2(x, c.Corner.y);
                        if(IsPointInRoom(point) == false){
                            corridors.Add(point);
                        }                       
                    }
                }

                if (c.Corner.y < c.End.y)
                {
                    for (int y = (int)c.Corner.y; y <= c.End.y; y++)
                    {
                        point = new Vector2(c.Corner.x, y);
                        if(IsPointInRoom(point) == false){
                            corridors.Add(point);
                        }                        
                    }
                }
                else
                {
                    for (int y = (int)c.Corner.y; y >= c.End.y; y--)
                    {
                        point = new Vector2(c.Corner.x, y);
                        if(IsPointInRoom(point) == false){
                            corridors.Add(point);
                        }
                        
                    }
                }                
            }
            return corridors;
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
                topX = r.RandiRange(PosX_min, _mapSize.X - _roomSize.max);
                topY = r.RandiRange(PosY_min, _mapSize.Y - _roomSize.max);

                //create the data
                room = new Room(topX - WALL_SPACE, topY - WALL_SPACE, topX + width + WALL_SPACE, topY + height + WALL_SPACE);

                //if there's collision with another room, pass and get another
                if (IsRoomColliding(room))
                {
                    i--;
                    continue;
                }

                //add to the list
                _dungeonRooms.Add(room);
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
                    room.BottomRight.X > _dungeonRooms[i].TopLeft.Y &&
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

        private bool IsPointInRoom(in Vector2 point){
            foreach(Room room in _dungeonRooms){
                if((room.TopLeft.X + 1 <= point.x && room.BottomRight.X - 1 >= point.x && room.TopLeft.Y + 1 <= point.y && room.BottomRight.Y - 1 >= point.y)){
                    return true;
                }
            }
            return false;
        }







    }

}