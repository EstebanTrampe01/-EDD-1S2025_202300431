using System;
using System.Runtime.InteropServices;
using Vehiculos;

namespace Vehiculos
{
    public unsafe struct Nodo<T> where T : unmanaged
    {
        public T Data;
        public Nodo<T>* Next;
        public Nodo<T>* Prev;
    }

}