using Mirror;

namespace Barebones.Networking
{
    public interface MessageBase : NetworkMessage {}

    public struct IntegerMessage : MessageBase
    { 
        public int value; 
        public IntegerMessage(int _value){ 
            this.value = _value; 
        } 
    }

    public struct StringMessage : MessageBase
    { 
        public string value; 
        public StringMessage(string _value){ 
            this.value = _value; 
        } 
    }

    public struct DoubleMessage : MessageBase
    { 
        public double value; 
        public DoubleMessage(double _value){ 
            this.value = _value; 
        } 
    }

    public struct FloatMessage : MessageBase
    { 
        public float value; 
        public FloatMessage(float _value){ 
            this.value = _value; 
        } 
    }
}