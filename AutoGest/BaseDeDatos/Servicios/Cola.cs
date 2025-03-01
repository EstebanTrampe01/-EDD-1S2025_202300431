using System;

namespace Servicios
{
    public class Cola
    {
        private Nodo frente; 
        private Nodo final; 

        public Cola()
        {
            frente = null;
            final = null;
        }

        // Método encolar: agrega un nuevo nodo al final de la cola.
        public void Encolar(Servicio servicio)
        {
            Nodo nuevoNodo = new Nodo(servicio); // Crea un nuevo nodo con el servicio.
            if (final == null) // Si la cola está vacía, el nuevo nodo es tanto el frente como el final.
            {
                frente = nuevoNodo;
                final = nuevoNodo;
            }
            else // Si la cola no está vacía, agrega el nodo al final y actualiza el final.
            {
                final.Sig = nuevoNodo;
                final = nuevoNodo;
            }
        }

        // Método desencolar: elimina y devuelve el servicio del nodo al frente de la cola.
        // Si la cola está vacía, retorna null.
        public Servicio Desencolar()
        {
            if (frente == null) return null; // Si la cola está vacía, retorna null.
            Servicio ret = frente.Data; // Guarda el servicio del nodo frente.
            frente = frente.Sig; // Mueve el frente al siguiente nodo.
            if (frente == null) final = null; // Si la cola queda vacía, el final también se establece como null.
            return ret; // Retorna el servicio eliminado.
        }

        // Método Print: imprime los servicios de la cola.
        public void Print()
        {
            Nodo temp = frente; // Comienza desde el frente de la cola.
            while (temp != null) // Mientras haya nodos en la cola.
            {
                Console.WriteLine(temp.Data.ToString()); // Imprime el servicio del nodo.
                temp = temp.Sig; // Se mueve al siguiente nodo.
            }
            Console.WriteLine("NULL"); // Indica el final de la cola.
        }
    }
}