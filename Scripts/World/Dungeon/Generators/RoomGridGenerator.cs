using Base;
using Godot;
using System;
using System.Collections.Generic;

namespace World.Dungeon.Generators
{

    public class RoomGridGenerator : BaseGenerator
    {
        RoomGrid _grid;
        private readonly int ROOM_NUMBER = 4;
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
            Dictionary<MyPoint, Room> pointsAndRooms;
            Messages.Print("Tiles size", "(" + Tiles.GetLength(0) + "," + Tiles.GetLength(1) + ")");
            pointsAndRooms = this.CreateRooms(gen, roomCoord);

            //pintamos
            //TODO: esto podría ir en un punto a parte
            foreach (Room r in pointsAndRooms.Values)
            {
                List<Vector2> coords = r.GetWallsPositions();

                foreach (Vector2 v in coords)
                {
                    Tiles[(int)v.x, (int)v.y] = new Tile(Tile.TileType.WALL);
                }


                coords = r.GetFloorPositions();
                foreach (Vector2 v in coords)
                {
                    Tiles[(int)v.x, (int)v.y] = new Tile(Tile.TileType.FLOOR);
                }
            }

            this.CreatePassages(gen, pointsAndRooms);
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
                    //Messages.Print("Roooooooooom at " + coord);
                    roomCoords.Add(coord);
                }
            }
        }

        private Dictionary<MyPoint, Room> CreateRooms(in RandomNumberGenerator gen, in List<MyPoint> roomCoord)
        {
            //lista de coordenadas del grid _grid que tiene una habitación.
            MyPoint minTopLeft;
            MyPoint maxTopLeft;
            MyPoint topLeft;
            MyPoint roomSize;
            Room room;

            //guardamos por cada punto del _grid la habitación que tiene, más fácil luego para jugar
            Dictionary<MyPoint, Room> pointAndRoom = new Dictionary<MyPoint, Room>();

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
                //Messages.Print("Top Left: " + topLeft);
                //Messages.Print("BottomR: " + room.BottomRight);

                pointAndRoom.Add(p, room);
            }

            return pointAndRoom;

        }

        private void CreatePassages(in RandomNumberGenerator gen, in Dictionary<MyPoint, Room> pointAndRoom)
        {
            //la idea es hacer un BDS o como se llame por cada habitacion del _Grid (por eso hemos creado eso básicamente) y las habitaciones

            Dictionary<Room, List<MyPoint>> roomAndConnections = new Dictionary<Room, List<MyPoint>>();
            
            foreach (MyPoint p in pointAndRoom.Keys)
            {
                //añadimos cada habitación con sus conexiones
                roomAndConnections.Add(pointAndRoom[p], this.GetColindantRooms(p));
            }
            //comprobamos
            
            string a = "";

            foreach (Room r in roomAndConnections.Keys)
            {
                a = "Room at " + new MyPoint(r.TopLeft.X / GRID_CELL_SIZE.X, r.TopLeft.Y / GRID_CELL_SIZE.Y).ToString() + " connected with: ";

                for (int i = 0; i < roomAndConnections[r].Count; i++)
                {
                    a += roomAndConnections[r][i].ToString() + ", ";
                }
                Messages.Print(a);
            }

            return;
            //allí, dependiendo del max de conexiones conectaremos
            //hará falta pensar alguna cosita extra, pero la idea general es esta

            //ahora de cada mierda de esta pillaremos las dos primeras y creamos el pasillo
            //TODO: optimiza tio que esto tiene de todo.

            int limitCon = 2;
            List<MyPoint> connections;
            foreach (Room r in roomAndConnections.Keys)
            {
                for (int i = 0; i < limitCon; i++)
                {
                    //pillamos las celdas de camino
                    connections = this.GetPathTo(new MyPoint(r.TopLeft.X / GRID_CELL_SIZE.X, r.TopLeft.Y / GRID_CELL_SIZE.Y), roomAndConnections[r][i]);

                    //pillamos punto de salida

                    //pillamos punto de entrada

                    //conectamos
                }
            }


        }

        readonly MyPoint[] directions = {new MyPoint(0,1), new MyPoint(0,-1),
                                         new MyPoint(1,0), new MyPoint(-1,0)};
        private List<MyPoint> GetColindantRooms(in MyPoint origin)
        {
            MyPoint tempPoint;
            MyPoint currentPoint;
            Queue<MyPoint> toLook = new Queue<MyPoint>();
            List<MyPoint> usedPointS = new List<MyPoint>();
            List<MyPoint> connectedRooms = new List<MyPoint>();
            toLook.Enqueue(origin);

            //buscamos 4 direccions
            while(toLook.Count > 0){
                currentPoint = toLook.Dequeue();
                usedPointS.Add(currentPoint);

                for (int i = 0; i < directions.Length; i++){
                    tempPoint = currentPoint + directions[i];

                    //miramos si está fuera de márgenes o está usao o en la cola y, si lo está, nos vamos
                    if(tempPoint.X < 0 || tempPoint.X >= _grid.GridSize.X ||
                       tempPoint.Y < 0 || tempPoint.Y >= _grid.GridSize.Y ||
                       usedPointS.Contains(tempPoint) || toLook.Contains(tempPoint)){
                        continue;
                    }

                    //si hay habitación, significa que está conectao, lo metemos en la lista
                    //de conexiones
                    if(_grid.HasRoomAt(tempPoint.X, tempPoint.Y) && connectedRooms.Contains(tempPoint) == false){
                        connectedRooms.Add(tempPoint);
                    }else{
                        //si no lo está, lo ponemos en los puntos a mirar
                        toLook.Enqueue(tempPoint);
                    }

                }
            }
            return connectedRooms;

            /*
            //búsqueda en 4 direcciones
            MyPoint tempPoint;
            List<MyPoint> connections = new List<MyPoint>();
            used.Add(origin);
            
            for (int i = 0; i < directions.Length; i++)
            {
                tempPoint = origin + directions[i];

                if (tempPoint.X < 0 || tempPoint.X >= Tiles.GetLength(0) - 1 ||
                   tempPoint.Y < 0 || tempPoint.Y >= Tiles.GetLength(1) - 1 ||
                   used.Contains(tempPoint))
                {
                    continue;
                }

                used.Add(tempPoint);

                if (_grid.HasRoomAt(tempPoint.X, tempPoint.Y))
                {
                    connections.Add(tempPoint);
                }
                else
                {
                    //si no hay conexión, buscamos en ese punto a ver qué pasa
                    connections.AddRange(this.GetColindantRooms(tempPoint, used));
                }
            }
            return connections;*/
        }

        private List<MyPoint> GetPathTo(in MyPoint origin, in MyPoint dest)
        {
            //búsqueda en 4 direcciones            
            MyPoint tempPoint;
            MyPoint parentPoint;
            List<MyPoint> connections = new List<MyPoint>();
            Queue<MyPoint> pointsToSearch = new Queue<MyPoint>();
            List<MyPoint> used = new List<MyPoint>();

            pointsToSearch.Enqueue(origin);

            //creo que lo más sencillo será crear un grid que imite el _grid con la 
            //estructura pointPAC, el grid será nulleable, así sabremos si hemos
            //visitado ya esa celda, y luego, ale, a tirar.
            //sí, creo que será lo más sencillo.
            //OJU: también lo podriamos hacern el la struct _grid.

            PointPAC?[,] tempGrid = new PointPAC?[GRID_SIZE.X, GRID_SIZE.Y];

            for (int i = 0; i < pointsToSearch.Count; i++)
            {
                parentPoint = pointsToSearch.Dequeue();
                

                for (int j = 0; j < directions.Length; j++)
                {
                    tempPoint = parentPoint + directions[j];

                    if (tempPoint.X < 0 || tempPoint.X >= tempGrid.GetLength(0) - 1 ||
                        tempPoint.Y < 0 || tempPoint.Y >= tempGrid.GetLength(1) - 1 ||
                        pointsToSearch.Contains(tempPoint) || used.Contains(tempPoint))
                    {
                        continue;
                    }

                    used.Add(parentPoint);
                    tempGrid[tempPoint.X, tempPoint.Y] = new PointPAC(parentPoint, tempPoint);

                    if (tempPoint == dest)
                    {
                        Messages.Print("Destinaaaaation");

                        MyPoint a = new MyPoint(-1, -1);

                        while (a != origin)
                        {
                            a = tempGrid[tempPoint.X, tempPoint.Y].Value.Parent;
                            connections.Add(a);
                        }

                        return connections;
                    }
                    else if (_grid.HasRoomAt(tempPoint.X, tempPoint.Y) == false)
                    {
                        pointsToSearch.Enqueue(tempPoint);
                    }
                }

            }
            return connections;

            //OJU: nuevo bug en potencia, tampoco sabemos qué habitaciones están conectadas
            //con otras habitaciones, así que, por ejemplo, si miramos Hab_A y está conectada con Hab_B y Hab_C, y luego
            //volvemos a operar y Hab_B se vuelve a conectar con Hab_A podemos crear diferentes pasillos 
            //y acabará hecho un lío.
            //quizás deberiamos darle una vuelta de tuerca




            return connections;
        }


    }

    public struct PointPAC
    {
        public MyPoint Parent;
        public MyPoint Child;


        public PointPAC(in MyPoint parent, in MyPoint child)
        {
            this.Parent = parent;
            this.Child = child;
        }
    }
}
