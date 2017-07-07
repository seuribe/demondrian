using System.Linq;
using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.util;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    public class MaterialGenerator : MonoBehaviour {

        public Material ghostMaterial;
        public Material matchAllMaterial;
        public Material matchNoneMaterial;
        public Material simpleTexMaterial;
        public Material stdMaterial;

        public Color[] colors;
        [Range(0, 1)]
        public float ghostPieceAlpha = 0.5f;

        MaterialPool<MaterialDef> matPool;

        readonly Vector2[] TextureCoords = new Vector2[] {
            Vector2.zero, Vector2.one,
            Vector2.right, Vector2.up };

        void Awake() {
            matPool = new MaterialPool<MaterialDef>(GenerateMaterial);
        }

        public void ApplyMaterial(PieceController pc, MaterialDef materialDef) {
            var mr = pc.GetComponentInChildren<MeshRenderer>();
            mr.material = matPool.GetMaterial(materialDef);
        }

        public void ApplyTexture(PieceController pc, Coord coord, Texture image, int boardSize) {
            var uvUnit = 1f / boardSize;
            var uv = new Vector2(coord.col, coord.row) * uvUnit;
            var len = pc.piece.size * uvUnit;
            MapTextureCoordinates(pc, uv, len);
        }

        void MapTextureCoordinates(PieceController pc, Vector2 uv, float len) {
            var mesh = pc.GetComponent<MeshFilter>().mesh;
            mesh.uv = TextureCoords.Select ( tc => (tc * len) + uv).ToArray();
        }

        Color GetColor(MaterialDef def) {
            var color = colors[def.type.index];

            if (def.ghost)
                color.a = ghostPieceAlpha;

            return color;
        }

        Material GenerateMaterial(MaterialDef def) {
            if (def.type == PieceType.MatchAll)
                return matchAllMaterial;

            if (def.type == PieceType.MatchNone)
                return matchNoneMaterial;

            if (def.Textured)
                return BuildTexturedMaterial(def.tex);

            return BuildStandardMaterial(def);
        }

        Material BuildStandardMaterial(MaterialDef def) {
            Material mat = GameObject.Instantiate<Material>(def.ghost ? ghostMaterial : stdMaterial);
            var color = GetColor(def);
            mat.SetColor("_Color", color);
            mat.SetFloat("_Scale", def.size);
            return mat;
        }

        Material BuildTexturedMaterial(Texture image) {
            Material texMat = GameObject.Instantiate<Material>(simpleTexMaterial);
            texMat.SetTexture("_MainTex", image);
            texMat.SetFloat("_Border", 0f);
            texMat.SetInt("_OnlyTexture", 1);
            return texMat;
        }
    }
}