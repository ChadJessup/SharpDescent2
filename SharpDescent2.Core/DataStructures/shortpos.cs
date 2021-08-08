namespace SharpDescent2.Core.DataStructures
{
    //	A compressed form for sending crucial data about via slow devices, such as modems and buggies.
    public class shortpos
    {
        public byte[] bytemat = new byte[9];
        public short xo, yo, zo;
        public short segment;
        public short velx, vely, velz;
    }
}
