using System;
using System.Runtime.InteropServices;

namespace Repuestos
{
    public unsafe struct NodoAVL
    {
        public LRepuesto Repuesto;  // Datos del repuesto
        public NodoAVL* Left;       // Hijo izquierdo
        public NodoAVL* Right;      // Hijo derecho
        public int Height;          // Altura del nodo para balanceo
    }
}