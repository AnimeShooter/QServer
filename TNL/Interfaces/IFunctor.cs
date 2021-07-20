namespace TNL.Interfaces
{
    using Utils;

    public interface IFunctor
    {
        void Set(object[] parameters);
        void Read(BitStream stream);
        void Write(BitStream stream);
        void Dispatch(object obj);
    }
}
