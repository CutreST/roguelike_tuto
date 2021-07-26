using Godot;
using System;
using MySystems;
using Base;
using System.Collections.Generic;

namespace World
{
    /// <summary>
    /// System responsible of rendering the gameworld and the objects.
    /// It relies on CPU power, so at some point I thing I'm gona go with a shader 
    /// or something similar, but i'm not sure.
    /// Maybe create a shadow map to.
    /// </summary>
    /// <remarks>
    /// Ok, I have some problem with doors, i use the VisibleMap instead of an
    /// independent object and, if colision the <see cref="WorldMapCont"/> handles it, but, i think
    /// it's best to create an object door and write the collision in it's collision complement.
    ///</remarks> 
    ///<remarks>
    /// There's a lot of room for optimization, but this is something for the future me. 
    /// I have, though, some ideas as using a quadtree for the renderable objects, a two tilempas, one for actual FOV
    /// and other for no FOV, a Renderable Component (first on list), and so on.
    ///</remaks>

    public class RenderSystem : System_Base
    {
        /// <summary>
        /// The visibleMap
        /// </summary>
        public TileMap VisibleMap { get; set; }

        /// <summary>
        /// The shadowMap
        /// </summary>
        public TileMap ShadowMap { get; set; }

        private bool[,] paintedCells;

        public WorldMapCont Cont { get; set; }

        #region FV settings
        /// <summary>
        /// The number of octanes = 8 (change to test)
        /// </summary>
        const int OCTANE = 8;

        /// <summary>
        /// The octane wich we start (0 by default, change to test)
        /// </summary>
        const int OCTANE_START = 0;

        /// <summary>
        /// A default FOV used if no fov provided on <see cref="CastLight"/>
        /// </summary>
        const int FOV_DIST_DEF = 6;

        #endregion

        /// <summary>
        /// A list of points to paint.
        /// </summary>
        private List<MyPoint> toPaint;



        List<Sprite> _renderableObjects;
        List<Sprite> _renderedObjects;

        #region System Methods

        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            this.toPaint = new List<MyPoint>();
            this._renderedObjects = new List<Sprite>();
            this._renderableObjects = new List<Sprite>();
        }

        public override void OnExitSystem(params object[] obj)
        {
            Messages.ExitSystem(this);
            this.toPaint.Clear();
            this.toPaint = null;
            this.VisibleMap.Clear();
            this.VisibleMap = null;
            this.ShadowMap.Clear();
            this.ShadowMap = null;
            this.paintedCells = new bool[0, 0];
        }

        #endregion

        #region Render Methods

        public void StartMap(in int width, in int height, in List<Sprite> renderable)
        {
            toPaint.Clear();
            this.ClearMap(width, height);
            this._renderableObjects = renderable;
            this._renderedObjects.Clear();

            foreach (Sprite s in _renderableObjects)
            {
                s.Visible = false;
            }
        }


        /// <summary>
        /// Paints the fov. Only discovery
        /// </summary>
        /// <param name="posInTile">the start position on tile pos</param>
        /// <param name="fov">the distance fov,</param>
        public void PaintFOV(in Vector2 posInTile, in int fov = FOV_DIST_DEF)
        {
            //clear the previos fov and the last toPaint
            VisibleMap.Clear();
            toPaint.Clear();

            foreach (Sprite s in _renderedObjects)
            {
                s.Visible = false;
            }

            this._renderableObjects.AddRange(_renderedObjects);
            this._renderedObjects.Clear();

            //first, we check each octant for
            for (int oct = OCTANE_START; oct < OCTANE; oct++)
            {
                this.CastLight(posInTile, fov, 1, 1.0f, 0.0f, oct);
            }

            for (int i = 0; i < toPaint.Count; i++)
            {
                this.PaintCell(toPaint[i].X, toPaint[i].Y);
                //miramos si hay obj que renderizar
                //OJU! esto será un complemento al sprite para saber si está visible y demás
                this.PaintObjectInPos(toPaint[i]);
            }
        }

        public void PaintObjectInPos(in MyPoint pos)
        {
            MyPoint tilePos;
            for (int i = 0; i < _renderableObjects.Count; i++)
            {
                tilePos = (MyPoint)Cont.WorldToTilePos(_renderableObjects[i].GlobalPosition);

                if (tilePos.X == pos.X && tilePos.Y == pos.Y)
                {
                    _renderableObjects[i].Visible = true;
                    _renderedObjects.Add(_renderableObjects[i]);
                    _renderableObjects.RemoveAt(i);
                    return;
                }

            }
        }

        /// <summary>
        /// Ok, it populates the <see cref="toPaint"/> list of paintable points.
        /// <remarks>
        /// https://fadden.com/tech/ShadowCast.cs.txt -> application link
        /// </remarks>
        /// </summary>
        /// <param name="posInTile"></param>
        /// <param name="fovValue"></param>
        /// <param name="startCol"></param>
        /// <param name="leftSlope"></param>
        /// <param name="rightSlope"></param>
        /// <param name="oct"></param>
        private void CastLight(in Vector2 posInTile, in int fovValue, in int startCol, float leftSlope, float rightSlope, in int oct)
        {
            //saves the previous tile blockenes
            bool previousBlock = false;

            //saved slope, check if we're looking out dimension
            float savedSlope = -1;
            //temptile
            Tile? tempTile;

            //radios of fov 
            //TODO: maybe change this
            int rad = fovValue * fovValue;
            for (int currentCol = startCol; currentCol <= rad; currentCol++)
            {
                int xCol = currentCol;

                for (int yCol = currentCol; yCol >= 0; yCol--)
                {
                    //get the octane position value and transform the pos onto octane pos
                    Vector2 octPos = TransformOctane(xCol, yCol, oct);

                    int x = (int)posInTile.x + (int)octPos.x;
                    int y = (int)posInTile.y + (int)octPos.y;

                    Cont.GetTileAt(out tempTile, x, y, false);

                    //if tile is empty, look for another
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

                    //get distance square to check with radius, if inside, then paint
                    float distanceSquared = xCol * xCol + yCol * yCol;

                    if (distanceSquared <= rad)
                    {
                        toPaint.Add(new MyPoint(x, y));
                    }

                    bool curBlocked = tempTile.Value.IsSightBloked;

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
                                CastLight(posInTile, fovValue, currentCol + 1, leftSlope, leftBlockSlope, oct);
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

        /// <summary>
        /// Provided a simpre row-colum coords, it passes an octane coords
        /// The numbers are the octanes, so, when passing octane 3, for example, we get the right coordenates
        /// of each position
        /// 111|222/3
        ///  11|22/33
        ///   1|2/333
        ///     @
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="octant"></param>
        /// <returns></returns>
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

        #endregion

        #region Paint methods

        /// <summary>
        /// Paints a segment withd-height of the map, starting at xStart and ystart
        /// </summary>
        /// <param name="xStart">The start of the x axis</param>
        /// <param name="yStart">The start of the y axis</param>
        /// <param name="widht">The widht of the segment we want to paint</param>
        /// <param name="height">The height of the segment we want to paint</param>
        public void PaintMap(int xStart, int yStart, in int widht, in int height)
        {
            Tile? currentTile;

            //Gets a tile at x,y and paints
            for (int x = xStart; x < widht; x++)
            {
                for (int y = yStart; y < height; y++)
                {

                    if (Cont.GetTileAt(out currentTile, x, y, false))
                    {
                        this.PaintCell(x, y, currentTile.Value.MyType);
                    }

                }
            }
        }

        /// <summary>
        /// Paints a simple cell on the VisibleMap depending on its <see cref="Tile.MyType"/>
        /// </summary>
        /// <param name="posX">The x position of the tile</param>
        /// <param name="posY">The y position of the tile</param>
        /// <param name="type">The type of the tile</param>
        public void PaintCell(in int posX, in int posY, in Tile.TileType type, in bool checkPaint = true)
        {
            //Ok, so this is going to be a dictionary at some point,
            //OJU! Right now is hardcoded!!!!!

            int id;
            switch (Cont.MyWorld.Tiles[posX, posY].Value.MyType)
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
                    return;
            }
            VisibleMap.SetCell(posX, posY, id);

            if (paintedCells[posX, posY] == false)
            {
                paintedCells[posX, posY] = true;
                ShadowMap.SetCell(posX, posY, id);
            }
        }

        private void ClearMap(in int width, in int height)
        {
            paintedCells = new bool[width, height];            
            VisibleMap.Clear();
            ShadowMap.Clear();
            foreach(Sprite s in _renderedObjects){
                s.Visible = false;
            }
            
            _renderableObjects.Clear();
            _renderedObjects.Clear();
        }
        public void PaintCell(in int posX, in int posY)
        {
            this.PaintCell(posX, posY, Cont.GetTileType(posX, posY, false));
        }

        public void RemoveEntityFromRender(in Entities.Entity ent){
            Sprite s = ent.TryGetFromChild_Rec<Sprite>();

            if(s != null){
                if(this._renderableObjects.Contains(s)){
                    this._renderableObjects.Remove(s);
                }
            }
        }

        #endregion

    }
}
