using System.Collections.Generic;
using UnityEngine;

namespace Domain.Grid {
    /// A utility class for operations and calculations related to hexagonal grids.
    public static class GridUtils {
        /// Represents the square root of 3 (approximately 1.7320508075688772f).
        /// This constant is often used in calculations related to hexagonal grid geometry,
        /// such as converting between axial and world coordinates.
        const float SQRT3 = 1.7320508075688772f;

        /// Represents the six axial direction vectors for hexagonal grid navigation.
        /// The directions are ordered clockwise starting from the positive q-axis:
        /// (+1, 0), (+1, -1), (0, -1), (-1, 0), (-1, +1), and (0, +1).
        /// These directions are commonly used for adjacency calculations and pathfinding in an axial hex grid system.
        public static readonly HexCoord[] AxialDirs = new[] {
           new HexCoord(+1,  0),
           new HexCoord(+1, -1),
           new HexCoord( 0, -1),
           new HexCoord(-1,  0),
           new HexCoord(-1, +1),
           new HexCoord( 0, +1),
       };

        /// Calculates the distance between two hexagonal grid coordinates using the axial coordinate system.
        /// The distance is determined using the Manhattan distance in a cube coordinate space representation of the hex grid.
        /// <param name="a">The first hexagonal coordinate.</param>
        /// <param name="b">The second hexagonal coordinate.</param>
        /// <returns>The distance between the two hex coordinates.</returns>
        public static int HexDistance(in HexCoord a, in HexCoord b) {
           // axial -> cube and distance
           var ax = a.q;
           var az = a.r;
           var ay = -ax - az;
           
           var bx = b.q;
           var bz = b.r;
           var by = -bx - bz;

           return (Mathf.Abs(ax - bx) + Mathf.Abs(ay - by) + Mathf.Abs(az - bz)) / 2;
       }

        /// Adds two hexagonal coordinates together using their axial coordinates (q, r).
        /// This operation combines the q and r components of both coordinates to produce a resulting coordinate.
        /// <param name="a">The first hexagonal coordinate to add.</param>
        /// <param name="b">The second hexagonal coordinate to add.</param>
        /// <returns>A new hexagonal coordinate that is the sum of the input coordinates.</returns>
        public static HexCoord Add(in HexCoord a, in HexCoord b) => new HexCoord(a.q + b.q, a.r+ b.r);

        /// Converts an array of hexagonal grid coordinates to their corresponding world positions based on the specified hexagonal grid layout.
        /// This method applies the transformation using the provided layout configuration for each hexagonal coordinate.
        /// <param name="layout">The hexagonal grid layout, including the hex radius and origin to use for the transformation.</param>
        /// <param name="coords">An array of hexagonal coordinates to be converted to world positions.</param>
        /// <returns>An array of Vector3 containing the world positions corresponding to the input hexagonal coordinates.</returns>
        public static Vector3[] HexToWorld(in HexLayout layout, in IReadOnlyList<HexCoord> coords) {
            var worldCoords = new Vector3[coords.Count];
            for (var i = 0; i < coords.Count; i++)
                worldCoords[i] = HexToWorld(layout, coords[i]);

            return worldCoords;
       }
        
        /// Converts a hexagonal grid coordinate to world coordinates based on the specified hex layout.
        /// The conversion is performed for an axial pointy-top hexagonal grid, transforming grid coordinates
        /// into their corresponding positions in a 3D space, considering the hex layout's radius and origin.
        /// <param name="layout">The hexagonal grid layout containing origin and hex radius.</param>
        /// <param name="coord">The axial coordinate on the hexagonal grid to be converted.</param>
        /// <returns>The 3D world coordinates corresponding to the specified hexagonal grid coordinate.</returns>
        public static Vector3 HexToWorld(in HexLayout layout, in HexCoord coord) {
            var size = layout.HexRadius;

            // Pointy-top axial to pixel
            // x = size * sqrt(3) * (q + r/2)
            // z = size * 3/2 * r
            var x = size * SQRT3 * (coord.q + coord.r * 0.5f);
            var z = size * 1.5f * coord.r;

            return layout.Origin + new Vector3(x, 0f, z);
        }

        /// Converts a world position into a hexagonal grid coordinate.
        /// This method calculates the corresponding hexagonal coordinate for a given world position
        /// based on the specified hexagonal layout. It performs the inverse operation of HexToWorld,
        /// calculating the axial hex coordinates from the world position.
        /// <param name="layout">The layout configuration of the hexagonal grid, including hex radius and origin.</param>
        /// <param name="worldPos">The world position to be converted into a hexagonal coordinate.</param>
        /// <returns>The hexagonal grid coordinate corresponding to the provided world position.</returns>
        public static HexCoord WorldToHex(in HexLayout layout, in Vector3 worldPos) {
            var size = layout.HexRadius;
            var p = worldPos - layout.Origin;

            // Inverse of HexToWorld for pointy-top:
            // q = (sqrt(3)/3 * x - 1/3 * z) / size
            // r = (2/3 * z) / size
            var q = (SQRT3 / 3f * p.x - 1f / 3f * p.z) / size;
            var r = (2f / 3f * p.z) / size;

            return AxialRound(q, r);
        }

        /// Rounds the given axial coordinates to the nearest integer hexagonal grid coordinates.
        /// This method ensures that the sum of the coordinates (q, r, and the implicitly calculated y) remains zero,
        /// preserving the constraints of cube coordinate systems used in hexagonal grids.
        /// <param name="q">The axial q-coordinate to be rounded.</param>
        /// <param name="r">The axial r-coordinate to be rounded.</param>
        /// <returns>A HexCoord structure representing the nearest hexagonal grid coordinates.</returns>
        static HexCoord AxialRound(float q, float r) {
            var y = -q - r;

            var roundX = Mathf.RoundToInt(q);
            var roundY = Mathf.RoundToInt(y);
            var roundZ = Mathf.RoundToInt(r);

            var absoluteX = Mathf.Abs(roundX - q);
            var absoluteY = Mathf.Abs(roundY - y);
            var absoluteZ = Mathf.Abs(roundZ - r);
            
            if (absoluteX > absoluteY && absoluteX > absoluteZ)
                roundX = -roundY - roundZ;
            else if (absoluteY > absoluteZ)
                roundY = -roundX - roundZ;
            else
                roundZ = -roundX - roundY;
            
            return new HexCoord(roundX, roundZ);
        }
    }
}