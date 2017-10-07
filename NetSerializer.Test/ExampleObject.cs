using System;

namespace NetSerializer.Test
{
    [Serializable]
    public class ExampleObject
    {
        public byte Abyte { get; set; }
        public sbyte Asbyte { get; set; }
        public int Aint { get; set; }
        public uint Auint { get; set; }
        public short Ashort { get; set; }
        public ushort Aushort { get; set; }
        public long Along { get; set; }
        public ulong Aulong { get; set; }
        public float Afloat { get; set; }
        public double Adouble { get; set; }
        public char Achar { get; set; }
        public bool Abool { get; set; }
        public object Aobject { get; set; }
        public string Astring { get; set; }
        public decimal Adecimal { get; set; }
        public DateTime AdateTime { get; set; }
        public TimeSpan AtimeSpan { get; set; }
    }
}