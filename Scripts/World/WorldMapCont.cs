using Base;
using Godot;
using MySystems;
using System;
using System.Collections.Generic;
using World.Dungeon;

namespace World
{
    /// <summary>
    /// Controller for the map. 
    /// </summary>
    /// <remarks>
    /// So, for each level the thing that changes is the <see cref="MyWorld"/>. So, the controller is persistent
    /// trought the gameplay. 
    /// </remarks>

    public class WorldMapCont : Node
    {
        //pillamos el tilemap para poder dibujar, lo hará esto 
        //TODO: OJU! esto podría ser un sistema y ale.
        /// <summary>
        /// The current tilemap
        /// </summary>
        /// <remarks>
        /// OJU! Work In Progress.
        /// Have to thing about the map creation.
        /// </remarks>
        private TileMap _tilemap;

        /// <summary>
        /// The world map.
        /// </summary>
        public WorldMap MyWorld { get; set; }

        private DungeonGenerator _generator;

        #region Map settings

        //OJU
        //This hardcoded settings are only for test. 
        //Have to remove when creating diferents maps        

        [Export]
        private readonly string WALL_NAME;

        [Export]
        private readonly string FLOOR_NAME;

        [Export]
        private readonly string CORRIDOR_NAME;

        [Export]
        private readonly string DOOR_NAME;

        [Export]
        private readonly string PLAYER_NAME;

        #endregion

        private Entities.Entity _player;

        #region Godot Methods
        public override void _EnterTree()
        {
            base._EnterTree();
            //create a new world,
            //OJU! change if changes the map


            //pains the tilemap
            //this.RunTileMap();
            _tilemap = this.TryGetFromChild_Rec<TileMap>();

            //put the controller to the gamesys
            InGameSys sys;

            System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out sys, true);
            sys.MyWorldCont = this;

            //ok, so the controller is responsible of calling the movement system 
            //and set itself as a reference.
            //The singleton only works with nodes (i can't get the root node without a node), so, it makes sense
            //i think.
            MovementSystem movementSystem;
            System_Manager.GetInstance(this).TryGetSystem<MovementSystem>(out movementSystem, true);
            movementSystem.MyWorld = this;

            //metemos generator para testear
            _generator = new DungeonGenerator(this);

            //buscamos al player
            _player = base.GetTree().Root.TryGetFromChild_Rec<Entities.Entity>(PLAYER_NAME);


            //rellenamos el diccionario
            this.SetMap();
        }



        #endregion
        const string SPACE_ACTION = "ui_select";
        /// <summary>
        /// input for creating a new map.
        /// </summary>
        /// <param name="delta"></param>
        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (Input.IsActionJustPressed(SPACE_ACTION))
            {
                //clear
                _tilemap.Clear();
                MyWorld.ClearMap();

                this.SetMap();
            }
        }
        const int OCTANE = 8;
        const int OCTANE_START = 0;
        const int FOV_DIST = 6*6;

        public void PaintFOV(in Vector2 pos)
        {
            List<MyPoint> p = new List<MyPoint>();
            Vector2 posInTile = this.WorldToTilePos(pos);
            //_tilemap.Clear();

            toPaint.Clear();
            for (int oct = OCTANE_START; oct < OCTANE; oct++)
            {
                //end.Clear();
                //p.AddRange(this.ToPaint(1, 0, posInTile, oct));
                this.CastLight(posInTile, 1, 1.0f, 0.0f, oct);
            }

            foreach (MyPoint point in toPaint)
            {
                this.PaintMap(point.X, point.Y, point.X + 1, point.Y + 1);
            }
        }

        List<MyPoint> toPaint = new List<MyPoint>();
        private void CastLight(in Vector2 posInTile, in int startCol, float leftSlope, float rightSlope, in int oct)
        {
            //if block, true
            bool previousBlock = false;

            float savedSlope = -1;
            Tile? tempTile;

            for (int currentCol = startCol; currentCol <= FOV_DIST; currentCol++)
            {
                int xCol = currentCol;

                for (int yCol = currentCol; yCol >= 0; yCol--)
                {
                    //Vector2 tilePos = WorldToTilePos(pos);
                    Vector2 octPos = TransformOctane(xCol, yCol, oct);

                    int x = (int)posInTile.x + (int)octPos.x;
                    int y = (int)posInTile.y + (int)octPos.y;

                    this.GetTileAt(out tempTile, x, y, false);

                    if (tempTile.HasValue == false)
                    {
                        continue;
                    }

                    //compute slopes
                    float leftBlockSlope = (yCol + 0.5f) / (xCol - 0.5f);
                    float rightBlockSlope = (yCol - 0.5f) / (xCol + 0.5f);

                    //check if outside area
                    if (rightBlockSlope > leftSlope)
                    {
                        // Block is above the left edge of our view area; skip.
                        continue;
                    }
                    else if (leftBlockSlope < rightSlope)
                    {
                        // Block is below the right edge of our view area; we're done.
                        break;
                    }

                    float distanceSquared = xCol * xCol + yCol * yCol;

                    if (distanceSquared <= FOV_DIST)
                    {
                        toPaint.Add(new MyPoint(x, y));
                    }

                    bool curBlocked = this.IsTileBlocked(x, y, false);

                    if (previousBlock)
                    {
                        if (curBlocked)
                        {
                            // Still traversing a column of walls.
                            savedSlope = rightBlockSlope;
                        }
                        else
                        {
                            // Found the end of the column of walls.  Set the left edge of our
                            // view area to the right corner of the last wall we saw.
                            previousBlock = false;
                            leftSlope = savedSlope;
                        }
                    }
                    else
                    {
                        if (curBlocked)
                        {
                            // Found a wall.  Split the view area, recursively pursuing the
                            // part to the left.  The leftmost corner of the wall we just found
                            // becomes the right boundary of the view area.
                            //
                            // If this is the first block in the column, the slope of the top-left
                            // corner will be greater than the initial view slope (1.0).  Handle
                            // that here.
                            if (leftBlockSlope <= leftSlope)
                            {
                                CastLight(posInTile, currentCol + 1, leftSlope, leftBlockSlope, oct);
                            }

                            // Once that's done, we keep searching to the right (down the column),
                            // looking for another opening.
                            previousBlock = true;
                            savedSlope = rightBlockSlope;
                        }
                    }

                }

                // Open areas are handled recursively, with the function continuing to search to
                // the right (down the column).  If we reach the bottom of the column without
                // finding an open cell, then the area defined by our view area is completely
                // obstructed, and we can stop working.
                if (previousBlock)
                {
                    break;
                }
            }
        }
        private List<Shadow> end = new List<Shadow>();
        private List<MyPoint> ToPaint(in int rowStart, in int colStart, in Vector2 posInTile, in int oct)
        {
            List<MyPoint> toPaint = new List<MyPoint>();
            Tile? tempTile;
            int endCol = colStart;
            bool lastWall = false;
            for (int row = rowStart; row < FOV_DIST; row++)
            {


                for (int col = colStart; col <= row; col++)
                {
                    //Vector2 tilePos = WorldToTilePos(pos);
                    Vector2 octPos = TransformOctane(row, col, oct);

                    int x = (int)posInTile.x + (int)octPos.x;
                    int y = (int)posInTile.y + (int)octPos.y;

                    this.GetTileAt(out tempTile, x, y, false);

                    if (tempTile.HasValue == false)
                    {
                        lastWall = true;
                        break;
                    }


                    //miramos si está en la lista de sombras
                    Shadow a = this.ShadowProjectTile(row, col);

                    bool paint = true;

                    foreach (Shadow s in end)
                    {
                        //miramos
                        if (a.Contains(s))
                        {
                            Messages.Print("yadasydadsasda");
                            paint = false;
                            break;
                        }
                    }

                    if (paint == false)
                    {
                        continue;
                    }

                    if (tempTile.Value.IsSightBloked)
                    {
                        end.Add(a);
                        lastWall = true;
                    }
                    else
                    {
                        lastWall = false;
                    }

                    toPaint.Add(new MyPoint(x, y));

                }
            }


            return toPaint;
        }

        private Vector2 TransformOctane(in int row, in int col, in int octant)
        {
            switch (octant)
            {
                case 0: return new Vector2(col, -row);
                case 1: return new Vector2(col, row);
                case 2: return new Vector2(-col, row);
                case 3: return new Vector2(-col, -row);
                case 4: return new Vector2(row, -col);
                case 5: return new Vector2(row, col);
                case 6: return new Vector2(-row, col);
                case 7: return new Vector2(-row, -col);
                default: return new Vector2(-50, -50);
            }
        }

        private Shadow ShadowProjectTile(in int row, in int col)
        {
            /*
            float topLeft = col / (float)(row + 2);
            float bottomRight = (float)(col + 1) / (float)(row + 1);*/
            float topLeft = (col + 0.5f) / (row - 0.5f);
            float bottomRight = (col - 0.5f) / (row + 0.5f);

            return new Shadow(topLeft, bottomRight);
        }


        public struct Shadow
        {
            public float Start;
            public float End;



            public Shadow(in float start, in float end)
            {
                this.Start = start;
                this.End = end;
                Messages.Print("Start and end: ", Start.ToString() + "__" + End.ToString());
            }

            public bool Contains(in Shadow otherShadow) => (this.Start <= otherShadow.Start && this.End >= otherShadow.End);

        }

        private void SetMap()
        {
            Vector2 pos;

            this.MyWorld = new WorldMap(this._generator.GetTiles(out pos));
            //this.PaintMap();

            //TODO: change this in it's file
            _player.GlobalPosition = new Vector2(pos.x * 24 + 12, pos.y * 24 + 12);
        }

        private void PaintMap(int xStart, int yStart, in int widht, in int height)
        {
            Tile? currentTile;
            int id;


            //Create a dictionary with TileType, TileName
            for (int x = xStart; x < widht; x++)
            {
                for (int y = yStart; y < height; y++)
                {

                    if (this.GetTileAt(out currentTile, x, y, false))
                    {
                        this.PaintCell(x, y, currentTile.Value.MyType);
                    }

                }
            }
        }

        private void PaintCell(in int posX, in int posY, in Tile.TileType type)
        {
            int id;
            switch (MyWorld.Tiles[posX, posY].Value.MyType)
            {
                case Tile.TileType.WALL:
                    id = 1;
                    break;

                case Tile.TileType.FLOOR:
                    id = 2;
                    break;

                case Tile.TileType.DOOR:
                    id = 4;
                    break;

                default:
                    id = 3;
                    break;
            }

            _tilemap.SetCell(posX, posY, id);
        }

        private void CreateCollisionTiles(in List<Vector2> walls)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                MyWorld.Tiles[(int)walls[i].x, (int)walls[i].y] = new Tile(true);
            }
        }

        private void CreateFloorTiles(in List<Vector2> floor)
        {
            for (int i = 0; i < floor.Count; i++)
            {
                MyWorld.Tiles[(int)floor[i].x, (int)floor[i].y] = new Tile(false);
            }
        }

        private void PaintTiles(in List<Vector2> tiles, in string tileName)
        {
            int id = _tilemap.TileSet.FindTileByName(tileName);

            for (int i = 0; i < tiles.Count; i++)
            {
                _tilemap.SetCell((int)tiles[i].x, (int)tiles[i].y, id);
            }
        }

        [Obsolete]
        private void PaintWalls(in List<Vector2> walls)
        {
            int id = _tilemap.TileSet.FindTileByName(WALL_NAME);
            for (int i = 0; i < walls.Count; i++)
            {
                _tilemap.SetCell((int)walls[i].x, (int)walls[i].y, id);
            }

        }

        [Obsolete]
        private void PaintFloor(in List<Vector2> floor)
        {
            int id = _tilemap.TileSet.FindTileByName(FLOOR_NAME);
            for (int i = 0; i < floor.Count; i++)
            {
                _tilemap.SetCell((int)floor[i].x, (int)floor[i].y, id);
            }
        }

        /// <summary>
        /// Is the <see cref="Tile"/> blocked at position?
        /// </summary>
        /// <param name="posX">Position X</param>
        /// <param name="posY">Position Y</param>
        /// <returns></returns>
        public bool IsTileBlocked(in int posX, in int posY, in bool globalPosition)
        {
            Tile? temp;

            if (this.GetTileAt(out temp, posX, posY, globalPosition))
            {
                return temp.Value.IsBlocked;
            }

            //if null, the player dosn't move
            return true;
        }

        public bool IsTileNoSee(in int posX, in int posY, in bool globalPosition)
        {
            Tile? temp;

            if (this.GetTileAt(out temp, posX, posY, globalPosition))
            {
                return temp.Value.IsSightBloked;
            }

            //if null, the player dosn't see
            return false;
        }

        /// <summary>
        /// Pass a world position to a tile position
        /// </summary>
        /// <param name="pos">World position</param>
        /// <returns>Tile position</returns>
        public Vector2 WorldToTilePos(Vector2 pos)
        {
            pos /= _tilemap.CellSize;
            pos.x = (int)pos.x;
            pos.y = (int)pos.y;

            return pos;
        }

        public bool GetTileAt(out Tile? tile, in int posX, in int posY, in bool isGlobalPos)
        {

            Vector2 tilePos;

            if (isGlobalPos)
            {
                tilePos = WorldToTilePos(new Vector2(posX, posY));
            }
            else
            {
                tilePos = new Vector2(posX, posY);
            }

            //out of bonds
            if (tilePos.x < 0 || tilePos.x >= MyWorld.WIDTH || tilePos.y < 0 || tilePos.y >= MyWorld.HEIGHT)
            {
                //Messages.Print(base.Name, "The tile at pos (" + posX + ", " + posY + ") is out of bonds");
                tile = null;
                return false;
            }

            tile = MyWorld.Tiles[(int)tilePos.x, (int)tilePos.y];

            if (tile != null)
            {
                return true;
            }

            return false;

        }

        public void OpenDoor(in int posX, in int posY, in bool isGlobalPos, in Tile.TileType newType)
        {
            Vector2 pos;

            if (isGlobalPos)
            {
                pos = this.WorldToTilePos(new Vector2(posX, posY));
            }
            else
            {
                pos = new Vector2(posX, posY);
            }

            //de momento lo cambiamos a suelo
            MyWorld.Tiles[(int)pos.x, (int)pos.y] = new Tile(newType);
            //joder hay que cambiar la pintura            
            this.PaintCell((int)pos.x, (int)pos.y, newType);
        }

        /// <summary>
        /// Paints the tile map        
        /// </summary>
        /// <remarks>
        /// TODO:
        /// Change name and autotile.
        ///</remarks>
        private void RunTileMap()
        {
            //primero pillamos tilemap


            //si no existe, nos vamos   
            if (_tilemap == null)
            {
                Messages.Print(base.Name, "Tilemap not found on children.");
                return;
            }

            TileSet set = _tilemap.TileSet;

            int a = set.FindTileByName(WALL_NAME);

            for (int x = 0; x < MyWorld.Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < MyWorld.Tiles.GetLength(1); y++)
                {
                    if (MyWorld.Tiles[x, y] != null && MyWorld.Tiles[x, y].Value.IsBlocked)
                    {
                        _tilemap.SetCell(x, y, a);
                    }
                }
            }
        }
    }
}
