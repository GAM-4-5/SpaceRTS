using System;
using System.Collections.Generic;
using Map;
using Microsoft.Xna.Framework;

namespace SpaceRts.Map
{
    public class Cell
    {

        public const float HEIGHT_SCALE = 10f;
        public const float HEIGHT_CHANGE_INSET_SCALE = 0.7f;
        public const int HEIGHT_CHANGE_NUMBER_OF_TERRACES = 2;
        public const float HEIGHT_CHANGE_SLOPE_INSET = 0.6f;
        private const float HEIGHT_CHANGE_TERRACE_SCALE = 1f / (HEIGHT_CHANGE_NUMBER_OF_TERRACES + 1);
        public const float outerRadius = 10f;

        public const float innerRadius = outerRadius * 0.86602540378f;

        public static Vector3[] corners = {
            new Vector3(0f, outerRadius, 0f),
            new Vector3(innerRadius, 0.5f * outerRadius, 0f),
            new Vector3(innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(0f, -outerRadius, 0f),
            new Vector3(-innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(-innerRadius, 0.5f * outerRadius, 0f),
            new Vector3(0f, outerRadius, 0f),
        };

        public const float uvOuterRadius = 0.5f;
        public const float uvInnerRadius = outerRadius * 0.86602540378f;

        public static Vector3[] uvCorners =
        {
            new Vector3(0f, uvOuterRadius, 0f),
            new Vector3(uvInnerRadius, 0.5f * uvOuterRadius, 0f),
            new Vector3(uvInnerRadius, -0.5f * uvOuterRadius, 0f),
            new Vector3(0f, -uvOuterRadius, 0f),
            new Vector3(-uvInnerRadius, -0.5f * uvOuterRadius, 0f),
            new Vector3(-uvInnerRadius, 0.5f * uvOuterRadius, 0f),
            new Vector3(0f, uvOuterRadius, 0f),
        };

        public static Tuple<int, int>[] adjacentsEven = {
            new Tuple<int, int>(0, 1),
            new Tuple<int, int>(1,0),
            new Tuple<int, int>(0,-1),
            new Tuple<int, int>(-1,-1),
            new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(-1,1),
        };

        public static Tuple<int, int>[] adjacentsOdd = {
            new Tuple<int, int>(1, 1),
            new Tuple<int, int>(1,0),
            new Tuple<int, int>(1,-1),
            new Tuple<int, int>(0,-1),
            new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(0, 1),
        };

        public Cell()
        {
        }

        NoiseGenerator noiseGenerator;
        /// <summary>
        /// Generates the mesh for cell.
        /// </summary>
        /// <returns>The mesh.</returns>
        /// <param name="chunkX">Chunk x.</param>
        /// <param name="chunkY">Chunk y.</param>
        /// <param name="chunkWidth">Chunk width.</param>
        /// <param name="chunkHeight">Chunk height.</param>
        /// <param name="mapWidth">Map width.</param>
        /// <param name="mapHeight">Map height.</param>
        /// <param name="cellX">Cell x.</param>
        /// <param name="cellY">Cell y.</param>
        /// <param name="noiseGenerator">Noise generator.</param>
        public (List<Vector3>, List<Vector2>, List<Color>, List<int>) GenerateMesh(int chunkX, int chunkY, int chunkWidth, int chunkHeight, int mapWidth, int mapHeight, int cellX, int cellY, NoiseGenerator noiseGenerator)
        {
            List<Vector3> positions = new List<Vector3>();
            List<Vector2> texturePositions = new List<Vector2>();
            List<Color> colors = new List<Color>();
            List<int> indicies = new List<int>();

            this.noiseGenerator = noiseGenerator;

            float _chunkDrawX = chunkX * chunkWidth * innerRadius * 2;
            float _chunkDrawY = chunkY * chunkHeight * outerRadius * 3 / 2;


            float _cellDrawCenterX = _chunkDrawX + (cellX + cellY * 0.5f - cellY / 2) * (innerRadius * 2f);
            float _cellDrawCenterY = _chunkDrawY + cellY * (outerRadius * 1.5f);

            int _cellGlobalIndex = (cellY + chunkY * chunkHeight) * chunkWidth * mapWidth + cellX + chunkX * chunkWidth;
            float _cellHeightValue = noiseGenerator.GenerateAtPosition(_cellGlobalIndex);
            float _cellHeightScaled = _cellHeightValue* HEIGHT_SCALE;

            Color _cellColor = colorAtHeight(_cellHeightScaled);

            Vector3 _cellDrawCenter = new Vector3(_cellDrawCenterX, _cellDrawCenterY, _cellHeightScaled);

            Vector3[] _cellCorners = new Vector3[7];
            bool[] _adjacentCellsExist = new bool[6];

            Vector3[] _adjacentCellsDrawCenters = new Vector3[6];
            Vector3[] _adjacentCellsDrawEdges = new Vector3[12];
            Vector3[] _cellEdgesToAdjacentCellsEdgesPositionDeltas = new Vector3[12];

            for (int _adjacentDirection = 0; _adjacentDirection < 6; _adjacentDirection++)
            {
                _cellCorners[_adjacentDirection] = _cellDrawCenter + corners[_adjacentDirection] * HEIGHT_CHANGE_INSET_SCALE;
                _cellCorners[_adjacentDirection + 1] = _cellDrawCenter + corners[_adjacentDirection + 1] * HEIGHT_CHANGE_INSET_SCALE;

                int _adjacentCellXOffset = cellY % 2 == 0 ? adjacentsEven[_adjacentDirection].Item1 : adjacentsOdd[_adjacentDirection].Item1;
                int _adjacentCellYOffset = cellY % 2 == 0 ? adjacentsEven[_adjacentDirection].Item2 : adjacentsOdd[_adjacentDirection].Item2;

                int _adjacentCellX = cellX + _adjacentCellXOffset;
                int _adjacentCellY = cellY + _adjacentCellYOffset;

                float _adjacentCellDrawCenterX = _chunkDrawX + (_adjacentCellX + _adjacentCellY * 0.5f - _adjacentCellY / 2) * (innerRadius * 2f);
                float _adjacentCellDrawCenterY = _chunkDrawY + _adjacentCellY * (outerRadius * 1.5f);

                int _adjacentCellGlobalIndex = (_adjacentCellY + chunkY * chunkHeight) * chunkWidth * mapWidth + _adjacentCellX + chunkX * chunkWidth;
                float _adjacentCellHeightValue = noiseGenerator.TryGenerateAtIndex(_adjacentCellGlobalIndex, out bool valid);

                _adjacentCellsExist[_adjacentDirection] = valid;
                if (valid)
                {
                    float _adjacentCellHeightScaled = _adjacentCellHeightValue * HEIGHT_SCALE;

                    Vector3 _adjacentCellDrawCenter = new Vector3(_adjacentCellDrawCenterX, _adjacentCellDrawCenterY, _adjacentCellHeightScaled);

                    _adjacentCellsDrawCenters[_adjacentDirection] = _adjacentCellDrawCenter;

                    _adjacentCellsDrawEdges[_adjacentDirection * 2] = _adjacentCellDrawCenter + corners[(_adjacentDirection + 4) % 6] * HEIGHT_CHANGE_INSET_SCALE;
                    _adjacentCellsDrawEdges[_adjacentDirection * 2 + 1] = _adjacentCellDrawCenter + corners[(_adjacentDirection + 3) % 6] * HEIGHT_CHANGE_INSET_SCALE;


                    _cellEdgesToAdjacentCellsEdgesPositionDeltas[_adjacentDirection * 2] = _cellCorners[_adjacentDirection] - _adjacentCellsDrawEdges[_adjacentDirection * 2];
                    _cellEdgesToAdjacentCellsEdgesPositionDeltas[_adjacentDirection * 2 + 1] = _cellCorners[_adjacentDirection + 1] - _adjacentCellsDrawEdges[_adjacentDirection * 2 + 1];
                }
                else
                {

                }


                // Position is invalid, skip drawing side of cell
            }
            for (int _adjacentDirection = 0; _adjacentDirection < 6; _adjacentDirection++)
            {
                Vector3 _cellCorner1 = _cellCorners[_adjacentDirection];
                Vector3 _cellCorner2 = _cellCorners[_adjacentDirection + 1];

                Vector3 _adjacentCellCorner1 = _adjacentCellsDrawEdges[_adjacentDirection * 2];
                Vector3 _adjacentCellCorner2 = _adjacentCellsDrawEdges[_adjacentDirection * 2 + 1];

                Vector3 _cellToAdjacentCellCorner1PositionDelta = _cellEdgesToAdjacentCellsEdgesPositionDeltas[_adjacentDirection * 2];
                Vector3 _cellToAdjacentCellCorner2PositionDelta = _cellEdgesToAdjacentCellsEdgesPositionDeltas[_adjacentDirection * 2 + 1];


                Color _adjacentCellCorner1Color = colorAtHeight(_adjacentCellCorner1.Z);
                Color _adjacentCellCorner2Color = colorAtHeight(_adjacentCellCorner2.Z);

                if (!_adjacentCellsExist[_adjacentDirection])
                {
                    positions.Add(_cellCorner1);
                    positions.Add(_cellDrawCenter);
                    positions.Add(_cellCorner2);

                    texturePositions.Add(new Vector2(0, 1));
                    texturePositions.Add(new Vector2(1, 1));
                    texturePositions.Add(new Vector2(1, 0));

                    colors.Add(_cellColor);
                    colors.Add(_cellColor);
                    colors.Add(_cellColor);

                    continue;
                }

                if (_adjacentDirection == 0 || _adjacentDirection == 4 || _adjacentDirection == 5)
                {// SCALE; BUILD

                    if (Math.Abs(_cellToAdjacentCellCorner1PositionDelta.Z) > HEIGHT_SCALE - 1)
                    {
                        positions.Add(_cellCorner1);
                        positions.Add(_cellCorner2);
                        positions.Add(_adjacentCellCorner1);
                        texturePositions.Add(new Vector2(0, 1));
                        texturePositions.Add(new Vector2(1, 1));
                        texturePositions.Add(new Vector2(1, 0));
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);

                        positions.Add(_adjacentCellCorner2);
                        positions.Add(_adjacentCellCorner1);
                        positions.Add(_cellCorner2);
                        texturePositions.Add(new Vector2(0, 1));
                        texturePositions.Add(new Vector2(1, 1));
                        texturePositions.Add(new Vector2(1, 0));
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);

                        if (_adjacentCellsExist[(_adjacentDirection + 1) % 6])
                        {

                            positions.Add(_cellCorner2);
                            positions.Add(_adjacentCellCorner2);
                            positions.Add(_adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2]);
                            texturePositions.Add(new Vector2(0, 1));
                            texturePositions.Add(new Vector2(1, 1));
                            texturePositions.Add(new Vector2(1, 0));
                            colors.Add(Color.Black);
                            colors.Add(Color.Black);
                            colors.Add(Color.Black);
                        }
                    }
                    else if((Math.Abs(_cellToAdjacentCellCorner1PositionDelta.Z) > HEIGHT_SCALE * 0.1f) || (_adjacentCellsExist[(_adjacentDirection + 1) % 6] && Math.Abs(_cellHeightScaled - _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2].Z) > HEIGHT_SCALE * 0.1f))
                    {
                        //int color2 = (int)(MathHelper.Clamp(_adjacentCellHeightValue, 0.1f, 0.9f) * 255);

                        float cHeight = _cellHeightScaled;


                        //float _verticalChange1 = MathHelper.Lerp(_cellCorner1.Z, _adjacentCellCorner1.Z, );

                        for (int _terrace = 0; _terrace < HEIGHT_CHANGE_NUMBER_OF_TERRACES + 1; _terrace++)
                        {
                            Vector3 _bottomCorner1, _bottomCorner2, _topCorner1, _topCorner2;
                            if(_cellCorner1.Z > _adjacentCellCorner1.Z)
                            {
                                _bottomCorner1 = _cellCorner1;
                                _bottomCorner2 = _cellCorner2;
                                _topCorner1 = _adjacentCellCorner1;
                                _topCorner2 = _adjacentCellCorner2; 
                            }
                            else
                            {
                                _bottomCorner1 = _adjacentCellCorner1;
                                _bottomCorner2 = _adjacentCellCorner2;
                                _topCorner1 = _cellCorner1;
                                _topCorner2 = _cellCorner2;
                            }

                            Vector3 _bottomEdge1 = Vector3.Lerp(_bottomCorner1, _topCorner1, HEIGHT_CHANGE_TERRACE_SCALE * _terrace);
                            Vector3 _bottomEdge2 = Vector3.Lerp(_bottomCorner2, _topCorner2, HEIGHT_CHANGE_TERRACE_SCALE * _terrace);

                            Vector3 _topEdge1 = Vector3.Lerp(_bottomCorner1, _topCorner1, HEIGHT_CHANGE_TERRACE_SCALE * (_terrace + 1));
                            Vector3 _topEdge2 = Vector3.Lerp(_bottomCorner2, _topCorner2, HEIGHT_CHANGE_TERRACE_SCALE * (_terrace + 1));

                            Vector3 _middleEdge1 = Vector3.Lerp(new Vector3(_topEdge1.X, _topEdge1.Y, _bottomEdge1.Z), _bottomEdge1, HEIGHT_CHANGE_SLOPE_INSET);
                            Vector3 _middleEdge2 = Vector3.Lerp(new Vector3(_topEdge2.X, _topEdge2.Y, _bottomEdge2.Z), _bottomEdge2, HEIGHT_CHANGE_SLOPE_INSET);

                            Color _bottomEdge1Color = colorAtHeight(_bottomEdge1.Z);
                            Color _bottomEdge2Color = colorAtHeight(_bottomEdge2.Z);

                            Color _middleEdge1Color = colorAtHeight(_middleEdge1.Z);
                            Color _middleEdge2Color = colorAtHeight(_middleEdge2.Z);

                            Color _topEdge1Color = colorAtHeight(_topEdge1.Z);
                            Color _topEdge2Color = colorAtHeight(_topEdge2.Z);

                            positions.Add(_bottomEdge1);
                            positions.Add(_bottomEdge2);
                            positions.Add(_middleEdge1);
                            texturePositions.Add(new Vector2(0, 1));
                            texturePositions.Add(new Vector2(1, 1));
                            texturePositions.Add(new Vector2(1, 0));
                            colors.Add(_bottomEdge1Color);
                            colors.Add(_bottomEdge2Color);
                            colors.Add(_middleEdge1Color);

                            positions.Add(_middleEdge2);
                            positions.Add(_middleEdge1);
                            positions.Add(_bottomEdge2);
                            texturePositions.Add(new Vector2(0, 1));
                            texturePositions.Add(new Vector2(1, 1));
                            texturePositions.Add(new Vector2(1, 0));
                            colors.Add(_middleEdge2Color);
                            colors.Add(_middleEdge1Color);
                            colors.Add(_bottomEdge2Color);

                            positions.Add(_middleEdge1);
                            positions.Add(_middleEdge2);
                            positions.Add(_topEdge1);
                            texturePositions.Add(new Vector2(0, 1));
                            texturePositions.Add(new Vector2(1, 1));
                            texturePositions.Add(new Vector2(1, 0));
                            colors.Add(_middleEdge1Color);
                            colors.Add(_middleEdge2Color);
                            colors.Add(_topEdge1Color);

                            positions.Add(_topEdge2);
                            positions.Add(_topEdge1);
                            positions.Add(_middleEdge2);
                            texturePositions.Add(new Vector2(0, 1));
                            texturePositions.Add(new Vector2(1, 1));
                            texturePositions.Add(new Vector2(1, 0));
                            colors.Add(_topEdge2Color);
                            colors.Add(_topEdge1Color);
                            colors.Add(_middleEdge2Color);


                            if (_adjacentCellsExist[(_adjacentDirection + 1) % 6])
                            {
                                Vector3 _terraceTopCorner, _terraceBottomCorner1, _terraceBottomCorner2;
                                if (_adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2].Z > _cellCorner2.Z && _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2].Z > _adjacentCellCorner2.Z)
                                {
                                    _terraceTopCorner = _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2];
                                    _terraceBottomCorner1 = _adjacentCellCorner2;
                                    _terraceBottomCorner2 = _cellCorner2;
                                }
                                else if (_cellCorner2.Z > _adjacentCellCorner2.Z && _cellCorner2.Z > _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2].Z)
                                {
                                    _terraceTopCorner = _cellCorner2; 
                                    _terraceBottomCorner1 = _adjacentCellCorner2;
                                    _terraceBottomCorner2 = _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2];
                                }
                                else if (_adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2].Z < _cellCorner2.Z)
                                {
                                    _terraceTopCorner = _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2];
                                    _terraceBottomCorner1 = _adjacentCellCorner2;
                                    _terraceBottomCorner2 = _cellCorner2;
                                }
                                else if (_cellCorner2.Z < _adjacentCellCorner2.Z && _cellCorner2.Z < _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2].Z)
                                {
                                    _terraceTopCorner = _cellCorner2;
                                    _terraceBottomCorner1 = _adjacentCellCorner2;
                                    _terraceBottomCorner2 = _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2];
                                }
                                else
                                {
                                    _terraceTopCorner = _adjacentCellCorner2;
                                    _terraceBottomCorner1 = _adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2];
                                    _terraceBottomCorner2 = _cellCorner2;
                                }

                                if (_terraceTopCorner.Z > _terraceBottomCorner1.Z)
                                {
                                    Vector3 _terraceTopEdge1 = Vector3.Lerp(_terraceTopCorner, _terraceBottomCorner1, _terrace * HEIGHT_CHANGE_TERRACE_SCALE);
                                    Vector3 _terraceTopEdge2 = Vector3.Lerp(_terraceTopCorner, _terraceBottomCorner2, _terrace * HEIGHT_CHANGE_TERRACE_SCALE);

                                    Vector3 _terraceBottomEdge1= Vector3.Lerp(_terraceTopCorner, _terraceBottomCorner1 ,  HEIGHT_CHANGE_TERRACE_SCALE * (_terrace + 1));
                                    Vector3 _terraceBottomEdge2 = Vector3.Lerp(_terraceTopCorner,  _terraceBottomCorner2, HEIGHT_CHANGE_TERRACE_SCALE * (_terrace + 1));

                                    Vector3 _terraceMiddleEdge1 = Vector3.Lerp(new Vector3(_terraceBottomEdge1.X, _terraceBottomEdge1.Y, _terraceTopEdge1.Z), _terraceTopEdge1, HEIGHT_CHANGE_SLOPE_INSET);
                                    Vector3 _terraceMiddleEdge2 = Vector3.Lerp(new Vector3(_terraceBottomEdge2.X, _terraceBottomEdge2.Y, _terraceTopEdge2.Z), _terraceTopEdge2, HEIGHT_CHANGE_SLOPE_INSET);

                                    //Vector3 _terraceMiddleEdge1 = Vector3.Lerp();

                                    Color _cellColorx = Color.Gray;
                                    if (_terrace == 1 )
                                        _cellColorx = Color.Pink;

                                    if (_terrace == 0)
                                    {
                                        positions.Add(_terraceTopCorner);
                                        positions.Add(_terraceMiddleEdge1);
                                        positions.Add(_terraceMiddleEdge2);
                                        texturePositions.Add(new Vector2(0, 1));
                                        texturePositions.Add(new Vector2(1, 1));
                                        texturePositions.Add(new Vector2(1, 0));
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                    }
                                    else
                                    {
                                        positions.Add(_terraceTopEdge1);
                                        positions.Add(_terraceMiddleEdge1);
                                        positions.Add(_terraceMiddleEdge2);
                                        texturePositions.Add(new Vector2(0, 1));
                                        texturePositions.Add(new Vector2(1, 1));
                                        texturePositions.Add(new Vector2(1, 0));
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);

                                        positions.Add(_terraceTopEdge1);
                                        positions.Add(_terraceTopEdge2);
                                        positions.Add(_terraceMiddleEdge2);
                                        texturePositions.Add(new Vector2(0, 1));
                                        texturePositions.Add(new Vector2(1, 1));
                                        texturePositions.Add(new Vector2(1, 0));
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                    }

                                    positions.Add(_terraceMiddleEdge1);
                                    positions.Add(_terraceMiddleEdge2);
                                    positions.Add(_terraceBottomEdge1);
                                    texturePositions.Add(new Vector2(0, 1));
                                    texturePositions.Add(new Vector2(1, 1));
                                    texturePositions.Add(new Vector2(1, 0));
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);

                                    positions.Add(_terraceBottomEdge1);
                                    positions.Add(_terraceBottomEdge2);
                                    positions.Add(_terraceMiddleEdge2);
                                    texturePositions.Add(new Vector2(0, 1));
                                    texturePositions.Add(new Vector2(1, 1));
                                    texturePositions.Add(new Vector2(1, 0));
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);


                                }
                                else
                                {
                                    Vector3 _terraceTopEdge1 = Vector3.Lerp(_terraceTopCorner, _terraceBottomCorner1, _terrace * HEIGHT_CHANGE_TERRACE_SCALE);
                                    Vector3 _terraceTopEdge2 = Vector3.Lerp(_terraceTopCorner, _terraceBottomCorner2, _terrace * HEIGHT_CHANGE_TERRACE_SCALE);

                                    Vector3 _terraceBottomEdge1 = Vector3.Lerp(_terraceTopCorner, _terraceBottomCorner1, HEIGHT_CHANGE_TERRACE_SCALE * (_terrace + 1));
                                    Vector3 _terraceBottomEdge2 = Vector3.Lerp(_terraceTopCorner, _terraceBottomCorner2, HEIGHT_CHANGE_TERRACE_SCALE * (_terrace + 1));

                                    Vector3 _terraceMiddleEdge1 = Vector3.Lerp(new Vector3(_terraceTopEdge1.X, _terraceTopEdge1.Y, _terraceBottomEdge1.Z), _terraceBottomEdge1, HEIGHT_CHANGE_SLOPE_INSET);
                                    Vector3 _terraceMiddleEdge2 = Vector3.Lerp(new Vector3(_terraceTopEdge2.X, _terraceTopEdge2.Y, _terraceBottomEdge2.Z), _terraceBottomEdge2, HEIGHT_CHANGE_SLOPE_INSET);

                                    //Vector3 _terraceMiddleEdge1 = Vector3.Lerp();

                                    Color _cellColorx = Color.Blue;
                                    if (_terrace == 1)
                                        _cellColorx = Color.Green;

                                    if (_terrace == 0)
                                    {
                                        positions.Add(_terraceTopCorner);
                                        positions.Add(_terraceMiddleEdge1);
                                        positions.Add(_terraceMiddleEdge2);
                                        texturePositions.Add(new Vector2(0, 1));
                                        texturePositions.Add(new Vector2(1, 1));
                                        texturePositions.Add(new Vector2(1, 0));
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                    }
                                    else
                                    {
                                        positions.Add(_terraceTopEdge1);
                                        positions.Add(_terraceMiddleEdge1);
                                        positions.Add(_terraceMiddleEdge2);
                                        texturePositions.Add(new Vector2(0, 1));
                                        texturePositions.Add(new Vector2(1, 1));
                                        texturePositions.Add(new Vector2(1, 0));
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);

                                        positions.Add(_terraceTopEdge1);
                                        positions.Add(_terraceTopEdge2);
                                        positions.Add(_terraceMiddleEdge2);
                                        texturePositions.Add(new Vector2(0, 1));
                                        texturePositions.Add(new Vector2(1, 1));
                                        texturePositions.Add(new Vector2(1, 0));
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                        colors.Add(_cellColorx);
                                    }

                                    positions.Add(_terraceMiddleEdge1);
                                    positions.Add(_terraceMiddleEdge2);
                                    positions.Add(_terraceBottomEdge1);
                                    texturePositions.Add(new Vector2(0, 1));
                                    texturePositions.Add(new Vector2(1, 1));
                                    texturePositions.Add(new Vector2(1, 0));
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);

                                    positions.Add(_terraceBottomEdge1);
                                    positions.Add(_terraceBottomEdge2);
                                    positions.Add(_terraceMiddleEdge2);
                                    texturePositions.Add(new Vector2(0, 1));
                                    texturePositions.Add(new Vector2(1, 1));
                                    texturePositions.Add(new Vector2(1, 0));
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);
                                    colors.Add(_cellColorx);


                                    //positions.Add(_terraceMiddleEdge1);
                                    //positions.Add(_terraceMiddleEdge2);
                                    //positions.Add(_terraceBottomCorner1);
                                    //colors.Add(_cellColorx);
                                    //colors.Add(_cellColorx);
                                    //colors.Add(_cellColorx);

                                    //positions.Add(_terraceBottomCorner1);
                                    //positions.Add(_terraceBottomCorner2);
                                    //positions.Add(_terraceMiddleEdge2);
                                    //colors.Add(_cellColorx);
                                    //colors.Add(_cellColorx);
                                    //colors.Add(_cellColorx);
                                }


                                //positions.Add(_terraceTopCorner);
                                //positions.Add(_terraceBottomCorner1);
                                //positions.Add(_terraceBottomCorner2);
                                //colors.Add(Color.Yellow);
                                //colors.Add(Color.Red);
                                //colors.Add(Color.Blue);

                            }
                        }

                        //positions.Add(_cellCorner1);
                        //positions.Add(_cellCorner2);
                        //positions.Add(_adjacentCellCorner1);
                        //colors.Add(new Color(_cellColor, 125, 125));
                        //colors.Add(new Color(_cellColor, 125, 125));
                        //colors.Add(new Color(_cellColor, 125, 125));

                        //positions.Add(_adjacentCellCorner2);
                        //positions.Add(_adjacentCellCorner1);
                        //positions.Add(_cellCorner2);
                        //colors.Add(new Color(_cellColor, 125, 125));
                        //colors.Add(new Color(_cellColor, 125, 125));
                        //colors.Add(new Color(_cellColor, 125, 125));

                    }
                    else
                    {
                        positions.Add(_cellCorner1);
                        positions.Add(_cellCorner2);
                        positions.Add(_adjacentCellCorner1);
                        texturePositions.Add(new Vector2(0, 1));
                        texturePositions.Add(new Vector2(1, 1));
                        texturePositions.Add(new Vector2(1, 0));
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);

                        positions.Add(_adjacentCellCorner2);
                        positions.Add(_adjacentCellCorner1);
                        positions.Add(_cellCorner2);
                        texturePositions.Add(new Vector2(0, 1));
                        texturePositions.Add(new Vector2(1, 1));
                        texturePositions.Add(new Vector2(1, 0));
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);
                        colors.Add(Color.Black);

                        if (_adjacentCellsExist[(_adjacentDirection + 1) % 6])
                        {

                            positions.Add(_cellCorner2);
                            positions.Add(_adjacentCellCorner2);
                            positions.Add(_adjacentCellsDrawEdges[(_adjacentDirection + 1) % 6 * 2]);
                            texturePositions.Add(new Vector2(0, 1));
                            texturePositions.Add(new Vector2(1, 1));
                            texturePositions.Add(new Vector2(1, 0));
                            colors.Add(Color.Black);
                            colors.Add(Color.Black);
                            colors.Add(Color.Black);
                        }

                    }
                }
                else
                {// SCALE

                }

                positions.Add(_cellCorner1);
                positions.Add(_cellDrawCenter);
                positions.Add(_cellCorner2);

                texturePositions.Add(new Vector2(0, 1));
                texturePositions.Add(new Vector2(1, 1));
                texturePositions.Add(new Vector2(1, 0));

                colors.Add(_cellColor);
                colors.Add(_cellColor);
                colors.Add(_cellColor);
            }


            for (int i = 0; i < positions.Count; i++)
            {
                indicies.Add(i);
            }

            return (positions, texturePositions, colors, indicies);
        }

        Color colorAtHeight(float height)
        {
            return Color.Lerp(Color.LightBlue, Color.Red, height / 30);
        }
    }
}
