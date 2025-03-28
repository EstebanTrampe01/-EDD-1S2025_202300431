using System;
using System.Runtime.InteropServices;

namespace Repuestos
{
    // se define la estructura LRepuesto pero tipo unsafe
    public unsafe struct LRepuesto
    {
        public int Id;
        public fixed char Repuesto[50];
        public fixed char Detalles[100];
        public double Costo;
        
        // Eliminar este campo ya que no se necesita para el árbol AVL
        // public NodoAVL<LRepuesto>* Next;

        // Constructor que inicializa un nuevo LRepuesto con sus datos.
        public LRepuesto(int id, string repuesto, string detalles, double costo)
        {
            this.Id = id;
            this.Costo = costo;
            // this.Next = null;  // Eliminar esta línea

            fixed (char* r = this.Repuesto, d = this.Detalles)
            {
                setFixedString(repuesto, r, 50);
                setFixedString(detalles, d, 100);
            }
        }

        private static void setFixedString(string str, char* fixedStr, int maxLength)
        {
            int i;
            for (i = 0; i < str.Length && i < maxLength - 1; i++)
            {
                fixedStr[i] = str[i];
            }
            fixedStr[i] = '\0';
        }

        private static string getFixedString(char* fixedStr)
        {
            string str = "";
            for (int i = 0; fixedStr[i] != '\0'; i++)
            {
                str += fixedStr[i];
            }
            return str;
        }

        public override string ToString()
        {
            fixed (char* r = Repuesto, d = Detalles)
            {
                return $"ID: {Id}, Repuesto: {getFixedString(r)}, Detalles: {getFixedString(d)}, Costo: {Costo}";
            }
        }
    }
}