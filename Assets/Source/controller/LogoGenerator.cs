using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.generator;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {
    public class LogoGenerator : MonoBehaviour {

        private AudioSource[] flipSources;

        public Texture logo;
        private BoardController bc;

        public float animTime = 0.75f;
        public float preDelay = 0.5f;
        public float postDelay = 3f;

        void Awake() {
            flipSources = GetComponents<AudioSource>();
            bc = FindObjectOfType<BoardController>();
            bc.PieceSet += OnPieceAdded;
            bc.PieceRemoved += OnPieceRemoved;
        }

        void Destroy() {
            bc.PieceSet -= OnPieceAdded;
            bc.PieceRemoved -= OnPieceRemoved;
        }

	    void Start () {
            Invoke("Enter", preDelay);
            Invoke("Exit", preDelay + postDelay);
	    }

        private void OnPieceRemoved(Piece piece) {
            PlayFlipSound();
        }

        private void OnPieceAdded(Piece piece) {
            PlayFlipSound();
        }

        void Enter() {
            var board = new BoardGenerator().GenerateRandom(16, 3);
            var game = new Game(board, GameRules.KeepSameRules, PieceType.GetRange(0, 3));

            bc.SetGame(game, logo);
        }

        void Exit() {
            bc.AnimateRemovePieces(animTime);
        }

        private void PlayFlipSound() {
            if (flipSources != null && flipSources.Length > 0) {
                var source = flipSources[UnityEngine.Random.Range(0, flipSources.Length)];
                if (!source.isPlaying) {
                    source.Play();
                }
            }
        }

    }
}
