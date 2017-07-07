using System;

namespace com.perroelectrico.demondrian.core {

    public class InvalidGameOperationException : Exception {
        public InvalidGameOperationException() {}
        public InvalidGameOperationException(string msg) : base(msg) {}
    }

    public class InvalidBoardOperationException : Exception {
        public InvalidBoardOperationException() {}
        public InvalidBoardOperationException(string msg) : base(msg) {}
    }

    public class InvalidBoardStateException : Exception {
        public InvalidBoardStateException() {}
        public InvalidBoardStateException(string msg) : base(msg) {}
    }

    public class InvalidGameStateException : Exception {
        public InvalidGameStateException() {}
        public InvalidGameStateException(string msg) : base(msg) {}
    }

}