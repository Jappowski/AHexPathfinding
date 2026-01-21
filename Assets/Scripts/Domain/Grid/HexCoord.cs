using System;

namespace Domain.Grid {
    public readonly struct HexCoord : IEquatable<HexCoord> {
        public readonly int Q; //column
        public readonly int R; //row (axial)

        public HexCoord(int q, int r) {
            Q = q;
            R = r;
        }

        public bool Equals(HexCoord other) => Q == other.Q && R == other.R;

        public override bool Equals(object obj) => obj is HexCoord other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Q, R);
        
        public override string ToString() => $"({Q}, {R})";
    }
}