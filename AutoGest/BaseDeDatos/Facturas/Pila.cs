using System;

namespace Facturas
{
    public class Pila
    {
        private Nodo tope; // Apunta al nodo superior de la pila.

        public Pila()
        {
            tope = null;
        }

        // Método Push: agrega un nuevo nodo al tope de la pila.
        public void Push(Factura factura)
        {
            Nodo nuevoNodo = new Nodo(factura); // Crea un nuevo nodo con la factura.
            nuevoNodo.Sig = tope; // Apunta al nodo anterior.
            tope = nuevoNodo; // El nuevo nodo es ahora el tope de la pila.
        }

        // Método Pop: elimina y devuelve la factura del nodo superior.
        // Si la pila está vacía, devuelve null.
        public Factura Pop()
        {
            if (tope == null) return null; // Si la pila está vacía, retorna null.
            Factura ret = tope.Data; // Guarda la factura del nodo superior.
            tope = tope.Sig; // El tope pasa al siguiente nodo.
            return ret; // Retorna la factura eliminada.
        }

        // Método Print: imprime las facturas de la pila.
        public void Print()
        {
            Nodo temp = tope; // Comienza en el tope de la pila.
            while (temp != null) // Mientras haya nodos en la pila.
            {
                Console.Write(temp.Data + " -> "); // Imprime la factura del nodo.
                temp = temp.Sig; // Se mueve al siguiente nodo.
            }
            Console.WriteLine("NULL"); // Indica el final de la pila.
        }
    }
}