using System;
using System.Runtime.InteropServices;

namespace Repuestos
{
    public unsafe class ListaCircular
    {
        private Nodo<LRepuesto>* head;

        public ListaCircular()
        {
            head = null;
        }

        public void Insertar(int id, string repuesto, string detalles, double costo)
        {
            Nodo<LRepuesto>* nuevoNodo = (Nodo<LRepuesto>*)Marshal.AllocHGlobal(sizeof(Nodo<LRepuesto>));
            LRepuesto nuevoRepuesto = new LRepuesto(id, repuesto, detalles, costo);
            nuevoNodo->Data = nuevoRepuesto;
            nuevoNodo->Next = null;

            if (head == null)
            {
                head = nuevoNodo;
                head->Next = head; // Apunta a sí mismo
            }
            else
            {
                Nodo<LRepuesto>* temp = head;
                // Recorremos la lista hasta llegar al último nodo
                while (temp->Next != head)
                {
                    temp = temp->Next;
                }
                // Insertamos el nuevo nodo al final de la lista
                temp->Next = nuevoNodo;
                nuevoNodo->Next = head;
            }
        }

        public void Eliminar(int id)
        {
            // si la lista está vacía, no hay nada que eliminar
            if (head == null) return;
            // si el nodo a eliminar es la cabeza de la lista
            if (head->Data.Id == id && head->Next == head)
            {
                Marshal.FreeHGlobal((IntPtr)head);
                head = null;
                return;
            }

            Nodo<LRepuesto>* temp = head;
            Nodo<LRepuesto>* prev = null;
            do
            {
                // si el nodo a eliminar es la cabeza de la lista
                if (temp->Data.Id == id)
                {
                    if (prev != null)
                    {
                        prev->Next = temp->Next;
                    }
                    else
                    {
                        Nodo<LRepuesto>* last = head;
                        while (last->Next != head)
                        {
                            last = last->Next;
                        }
                        head = head->Next;
                        last->Next = head;
                    }
                    // liberamos la memoria del nodo eliminado
                    Marshal.FreeHGlobal((IntPtr)temp);
                    return;
                }
                // avanzamos al siguiente nodo
                prev = temp;
                temp = temp->Next;
            } while (temp != head);
        }

        public LRepuesto* Buscar(int id)
        {
            if (head == null) return null;

            Nodo<LRepuesto>* temp = head;
            do
            {
                if (temp->Data.Id == id)
                {
                    return &temp->Data;
                }
                temp = temp->Next;
            } while (temp != head);

            return null;
        }

        public string ObtenerLista()
        {
            if (head == null) return "Lista vacía.";

            string lista = "";
            Nodo<LRepuesto>* temp = head;
            do
            {
                lista += temp->Data.ToString() + "\n";
                temp = temp->Next;
            } while (temp != head);

            return lista;
        }

        public void Mostrar()
        {
            if (head == null)
            {
                Console.WriteLine("Lista vacía.");
                return;
            }

            Nodo<LRepuesto>* temp = head;
            do
            {
                Console.WriteLine(temp->Data.ToString());
                temp = temp->Next;
            } while (temp != head);
        }

        // Destructor de la clase
        ~ListaCircular()
        {
            if (head == null) return;

            Nodo<LRepuesto>* temp = head;
            do
            {
                Nodo<LRepuesto>* next = temp->Next;
                Marshal.FreeHGlobal((IntPtr)temp);
                temp = next;
            } while (temp != head);
        }
    }
}