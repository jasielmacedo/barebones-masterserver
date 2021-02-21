using Mirror;

public struct TokenMessage : NetworkMessage {
        public string token;

        public TokenMessage(string _token){ this.token = _token ; }
    }