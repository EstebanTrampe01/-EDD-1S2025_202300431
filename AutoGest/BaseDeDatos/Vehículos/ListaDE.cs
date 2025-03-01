using System;
using System.Runtime.InteropServices;
using Vehiculos;

namespace Vehiculos
{
    public unsafe class ListaDoblementeEnlazada
    {
        // apuntador head y tail
        private Nodo<Vehiculo>* head;
        // apuntador tail que es el último nodo de la lista
        private Nodo<Vehiculo>* tail;

        public ListaDoblementeEnlazada()
        {
            head = null;
            tail = null;
        }

        public void Insertar(int id, int idUsuario, string marca, int modelo, string placa)
        {
            Nodo<Vehiculo>* nuevoNodo = (Nodo<Vehiculo>*)NativeMemory.Alloc((nuint)sizeof(Nodo<Vehiculo>));
            Vehiculo nuevoVehiculo = new Vehiculo(id, idUsuario, marca, modelo, placa);
            nuevoNodo->Data = nuevoVehiculo;
            nuevoNodo->Next = null;
            nuevoNodo->Prev = null;

            // Si la lista está vacía, el nuevo nodo será el primero
            if (head == null)
            {
                head = tail = nuevoNodo;
            }
            else
            {
                // Si la lista no está vacía, recorremos la lista hasta llegar al último nodo
                tail->Next = nuevoNodo;
                nuevoNodo->Prev = tail;
                tail = nuevoNodo;
            }
        }

        public void Eliminar(int id)
        {
            // Si la lista está vacía, no hay nada que eliminar
            Nodo<Vehiculo>* actual = head;
            while (actual != null)
            {
                // Si el nodo a eliminar es la cabeza de la lista, movemos la cabeza al siguiente nodo
                if (actual->Data.Id == id)
                {
                    if (actual->Prev != null)
                        actual->Prev->Next = actual->Next;
                    else
                        head = actual->Next;

                    if (actual->Next != null)
                        actual->Next->Prev = actual->Prev;
                    else
                        tail = actual->Prev;
                    // Liberamos la memoria del nodo eliminado
                    NativeMemory.Free(actual);
                    return;
                }
                // Si el nodo a eliminar no es la cabeza de la lista, avanzamos al siguiente nodo
                actual = actual->Next;
            }
        }

        public Vehiculo* Buscar(int id)
        {
            Nodo<Vehiculo>* actual = head;
            while (actual != null)
            {
                if (actual->Data.Id == id)
                {
                    return &actual->Data;
                }
                actual = actual->Next;
            }
            return null;
        }

        public string ObtenerLista()
        {
            string lista = "";
            Nodo<Vehiculo>* actual = head;
            while (actual != null)
            {
                lista += actual->Data.ToString() + "\n";
                actual = actual->Next;
            }
            return lista;
        }

        // Método para mostrar los nodos de la lista inicializados del head al tail
        public void Mostrar()
        {
            // Si la lista está vacía, no hay nada que mostrar
            Nodo<Vehiculo>* actual = head;
            // Si la lista no está vacía, recorremos la lista hasta llegar al último nodo
            while (actual != null)
            {
                Console.WriteLine(actual->Data.ToString());
                actual = actual->Next;
            }
        }

        // Método para mostrar los nodos de la lista inicializados del tail al head
        public void MostrarReversa()
        {
            // Si la lista está vacía, no hay nada que mostrar
            Nodo<Vehiculo>* actual = tail;
            // Si la lista no está vacía, recorremos la lista hasta llegar al último nodo
            while (actual != null)
            {
                Console.WriteLine(actual->Data.ToString());
                actual = actual->Prev;
            }
        }

        // Destructor de la clase
        ~ListaDoblementeEnlazada()
        {
            // Liberamos la memoria de todos los nodos de la lista
            Nodo<Vehiculo>* actual = head;
            while (actual != null)
            {
                Nodo<Vehiculo>* temp = actual;
                actual = actual->Next;
                NativeMemory.Free(temp);
            }
        }
    }
}