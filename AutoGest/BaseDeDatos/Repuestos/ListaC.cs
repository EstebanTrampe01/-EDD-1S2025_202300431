using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

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

            // Llamar a la función para generar el gráfico
            GenerarGrafico("Repuestos.dot");
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

        public void GenerarGrafico(string fileName)
        {
            string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reports");
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            string filePath = Path.Combine(carpeta, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("digraph G {");
                writer.WriteLine("rankdir=LR;"); // Orientación horizontal
                writer.WriteLine("node [shape=record];");
                writer.WriteLine("splines=true;"); // Flechas rectas

                Nodo<LRepuesto>* temp = head;
                int index = 0;
                do
                {
                    LRepuesto* repuesto = &temp->Data;
                    writer.WriteLine($"node{index} [label=\"ID: {repuesto->Id} \\n Repuesto: {GetFixedString(repuesto->Repuesto)} \\n Detalles: {GetFixedString(repuesto->Detalles)} \\n Costo: {repuesto->Costo}\"]");

                    if (temp->Next != head)
                    {
                        // Flecha de izquierda a derecha
                        writer.WriteLine($"node{index} -> node{index + 1};");
                        writer.WriteLine($"node{index} -> node{index + 1} [dir=back];");

                    }
                    else
                    {
                        // Flecha de conexión circular (último nodo al primer nodo)
                        writer.WriteLine($"node{index} -> node0 [constraint=false];");
                        writer.WriteLine($"node{index} -> node0 [constraint=false, dir=back];");
                    }
                    temp = temp->Next;
                    index++;
                } while (temp != head);

                writer.WriteLine("}");
            }

            // Generar el archivo PNG usando Graphviz
            var process = new Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {filePath} -o {Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            // Mostrar la URL en la consola
            Console.WriteLine("La gráfica de la lista circular se ha guardado en: " + Path.GetFullPath(Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))));
        }

        private static string GetFixedString(char* fixedStr)
        {
            string str = "";
            for (int i = 0; fixedStr[i] != '\0'; i++)
            {
                str += fixedStr[i];
            }
            return str;
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