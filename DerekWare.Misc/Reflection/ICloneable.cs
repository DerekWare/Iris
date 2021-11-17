using System;

namespace DerekWare.Reflection
{
    public interface ICloneable<out T> : ICloneable
    {
        new T Clone();
    }
}
