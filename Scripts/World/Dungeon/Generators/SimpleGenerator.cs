using Godot;
using System;
using System.Collections.Generic;

namespace World.Dungeon.Generators
{

    public class SimpleGenerator
    {
        List<Room> _dungeonRooms;
        List<SimpleCorridor> _corridors;

        readonly int SIZE_MIN = 2;
        readonly int SIZE_MAX = 6;

        readonly int WALL_SPACE = 1;

        public int PosX_max = 60;
        public int PosY_max = 32;

        public int PosX_min = 1;
        public int PosY_min = 1;

        public SimpleGenerator()
        {
            this._corridors = new List<SimpleCorridor>();
            this._dungeonRooms = new List<Room>();

            //primero creamos las habitaciones
            this.CreateRooms(8);


            //ahora conectamos

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

        private void CreateRooms(in int rooms)
        {
            RandomNumberGenerator r = new RandomNumberGenerator();
            r.Randomize();
            int width;
            int height;
            int topX;
            int topY;

            Room room;

            for (int i = 0; i < rooms; i++)
            {
                width = r.RandiRange(SIZE_MIN, SIZE_MAX);
                height = r.RandiRange(SIZE_MIN, SIZE_MAX);
                topX = r.RandiRange(PosX_min, PosX_max);
                topY = r.RandiRange(PosY_min, PosY_max - SIZE_MAX);

                //creamos
                room = new Room(topX - WALL_SPACE, topY - WALL_SPACE, topX + width + WALL_SPACE, topY + height + WALL_SPACE);

                //chekeamos, aún no 
                if(IsRoomColliding(room)){
                    i--;
                    continue;
                }

                //añadimos
                _dungeonRooms.Add(room);
            }
        }

        private bool IsRoomColliding(in Room room)
        {
            for (int i = 0; i < _dungeonRooms.Count; i++)
            {
                if (room.TopLeftX < _dungeonRooms[i].BottomRightX &&
                    room.BottomRightX > _dungeonRooms[i].TopLeftX &&
                    room.TopLeftY < _dungeonRooms[i].BottomRightY &&
                    room.BottomRightY > _dungeonRooms[i].TopLeftY)
                {
                    Messages.Print("Simple Generator", "Collision detected");
                    return true;
                }
            }

            //para la primera
            return false;
        }






    }

}