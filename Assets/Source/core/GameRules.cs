using System;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    [Serializable]
    public enum NextTypePolicy {
        Deterministic,
        Random,
        RemovedType
    };

    [Serializable]
    public struct GameRules {
        public float MatchAllWeight;
        public float MatchNoneWeight;
        public float OtherPiecesWeight;
        public bool MatchAllGrows;
        public NextTypePolicy NextTypePolicy;
        public bool AllowUndo;
        public bool ShowGhostPiece;

        public bool HasIncomingQueue {
            get { return NextTypePolicy != NextTypePolicy.RemovedType; }
        }

        public static GameRules KeepSameRules = new GameRules {
            MatchAllWeight = 0,
            MatchNoneWeight = 0,
            OtherPiecesWeight = 100,
            MatchAllGrows = false,
            NextTypePolicy = NextTypePolicy.RemovedType,
            AllowUndo = true,
            ShowGhostPiece = false,
        };

        public static GameRules ClassicRules = new GameRules {
            MatchAllWeight = 0,
            MatchNoneWeight = 0,
            OtherPiecesWeight = 100,
            MatchAllGrows = false,
            NextTypePolicy = NextTypePolicy.Deterministic,
            AllowUndo = true,
            ShowGhostPiece = false,
        };

        public static GameRules ArcadeRules = new GameRules {
            MatchAllWeight = 10,
            MatchNoneWeight = 10,
            OtherPiecesWeight = 80,
            MatchAllGrows = true,
            NextTypePolicy = NextTypePolicy.Random,
            AllowUndo = false,
            ShowGhostPiece = true,
        };

        public JSONNode ToJSON() {
            var json = new JSONClass();
            json["MatchAllWeight"] = new JSONData(MatchAllWeight);
            json["MatchNoneWeight"] = new JSONData(MatchNoneWeight);
            json["OtherPiecesWeight"] = new JSONData(OtherPiecesWeight);
            json["MatchAllGrows"] = new JSONData(MatchAllGrows);
            json["NextTypePolicy"] = new JSONData((int)NextTypePolicy);
            json["AllowUndo"] = new JSONData(AllowUndo);
            json["ShowGhostPiece"] = new JSONData(ShowGhostPiece);
            return json;
        }

        public static GameRules FromJSON(JSONNode json) {
            if (json == null)
                return ClassicRules;

            return new GameRules {
                MatchAllWeight = json["MatchAllWeight"].AsFloat,
                MatchNoneWeight = json["MatchNoneWeight"].AsFloat,
                OtherPiecesWeight = json["OtherPiecesWeight"].AsFloat,
                MatchAllGrows = json["MatchAllGrows"].AsBool,
                NextTypePolicy = (NextTypePolicy)json["NextTypePolicy"].AsInt,
                AllowUndo = json["AllowUndo"].AsBool,
                ShowGhostPiece = json["ShowGhostPiece"].AsBool,
            };
        }
    }
}