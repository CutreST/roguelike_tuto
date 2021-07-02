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

        const int WALL_NUMBER = 6;

        public int PosX_max = 56;
        public int PosY_max = 31;

        public int PosX_min = 1;
        public int PosY_min = 1;

        public SimpleGenerator()
        {
            this._corridors = new List<SimpleCorridor>();
            this._dungeonRooms = new List<Room>();
            RandomNumberGenerator r = new RandomNumberGenerator();

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



        private void CreateRooms(in int rooms, in RandomNumberGenerator r)
        {
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
                if (IsRoomColliding(room))
                {
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


        private void CreateCorridors(in RandomNumberGenerator r)
        {
            //randomizamos el generador
            r.Randomize();

            //buscamos por cada habitación empezando por la segunda
            for (int i = 1; i < _dungeonRooms.Count; i++)
            {
                //conectamos la habitacion con la anterior.
                SimpleCorridor corridor;
                Room origin = _dungeonRooms[i];
                Room dest = _dungeonRooms[i - 1];
                Vector2 start, end, corner;


                //50% lateral O vertical
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

                Messages.Print("Origin: " + origin.TopLeftX + "/" + origin.TopLeftY);
                Messages.Print("Destination: " + dest.TopLeftX + "/" + dest.TopLeftY);
                Messages.Print("Center Origin: " + origin.CenterX);
                Messages.Print("Center Destination: " + dest.CenterX);

                _corridors.Add(new SimpleCorridor(start, end, corner));
            }
        }

        private void CreateCorridorXAxis(in Room origin, in Room dest, out Vector2 corner, out Vector2 point)
        {
            if (origin.CenterX > dest.CenterX)
            {
                point = new Vector2(origin.TopLeftX, origin.CenterY);
                corner = new Vector2(dest.CenterX, origin.CenterY);
            }
            else
            {
                point = new Vector2(origin.BottomRightX, origin.CenterY);
                corner = new Vector2(dest.CenterX, origin.CenterY);
            }
        }

        private void CreateCorridorYAxis(in Room origin, in Room dest, in Vector2 corner, out Vector2 point)
        {
            if (origin.CenterY > dest.CenterY)
            {
                point = new Vector2(corner.x, dest.BottomRightY);
            }
            else if (origin.CenterY < dest.CenterY)
            {
                point = new Vector2(corner.x, dest.TopLeftY);
            }
            else
            {
                point = new Vector2(corner.x, dest.CenterY);
            }
        }

        private bool IsPointInRoom(in Vector2 point){
            foreach(Room room in _dungeonRooms){
                if((room.TopLeftX + 1 <= point.x && room.BottomRightX - 1 >= point.x && room.TopLeftY + 1 <= point.y && room.BottomRightY - 1 >= point.y)){
                    return true;
                }
            }
            return false;
        }







    }

}