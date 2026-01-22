using UnityEngine;

namespace Domain.Grid {
    public static class GridUtils {
       const float SQRT3 = 1.7320508075688772f;
       
       public static readonly HexCoord[] AxialDirs = new[] {
           new HexCoord(+1,  0),
           new HexCoord(+1, -1),
           new HexCoord( 0, -1),
           new HexCoord(-1,  0),
           new HexCoord(-1, +1),
           new HexCoord( 0, +1),
       };
       
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

       public static HexCoord Add(in HexCoord a, in HexCoord b) => new HexCoord(a.q + b.q, a.r+ b.r);

        /// <summary>
        /// Converts axial (q,r) to world position (x,z) for pointy-top layout.
        /// y is 0. Origin offset is applied.
        /// </summary>
        public static Vector3 HexToWorld(in HexLayout layout, in HexCoord coord) {
            var size = layout.HexRadius;

            // Pointy-top axial to pixel
            // x = size * sqrt(3) * (q + r/2)
            // z = size * 3/2 * r
            var x = size * SQRT3 * (coord.q + coord.r * 0.5f);
            var z = size * 1.5f * coord.r;

            return layout.Origin + new Vector3(x, 0f, z);
        }

        /// <summary>
        /// Converts world position (x,z) to nearest axial hex (q,r) for pointy-top layout.
        /// </summary>
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

        /// <summary>
        /// Rounds fractional axial coords to nearest integer axial coords using cube rounding.
        /// </summary>
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