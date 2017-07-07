using com.perroelectrico.demondrian.core;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// Describes the characteristics of a material in a way that can be used by the Material Pool
    /// </summary>
    public struct MaterialDef {

        public readonly Texture tex;
        public readonly PieceType type;
        public readonly int size;
        public readonly bool ghost;

        public bool Textured { get { return tex != null; } }

        public MaterialDef(Piece piece, bool ghost = false, Texture tex = null) : this(piece.type, piece.size, ghost, tex) { }

        public MaterialDef(PieceType type, int size, bool ghost = false, Texture tex = null) {
            this.type = type;
            this.size = size;
            this.ghost = ghost;
            this.tex = tex;
        }

        public override bool Equals(object obj) {
            return (obj is MaterialDef) &&
            ((MaterialDef)obj).type.Matches(type) &&
            ((MaterialDef)obj).size == size &&
            ((MaterialDef)obj).ghost == ghost &&
            ((MaterialDef)obj).tex == tex;
        }

        public override int GetHashCode() {
            return (((ghost ? 13 : 11) * size) * 7 + type.GetHashCode()) * (Textured ? 17 : 23);
        }
    }}