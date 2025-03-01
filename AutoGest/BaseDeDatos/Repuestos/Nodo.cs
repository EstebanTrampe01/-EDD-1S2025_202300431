using System;
using System.Runtime.InteropServices;

namespace Repuestos
{
    public unsafe struct Nodo<T> where T : unmanaged
    {
        public T Data;
        public Nodo<T>* Next;
    }
}