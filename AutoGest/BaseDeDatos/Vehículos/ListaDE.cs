using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

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

            // Llamar a la función para generar el gráfico
            GenerarGrafico("Vehiculos.dot");
                        GenerarTopVehiculosAntiguos(5); // Puedes cambiar el número 5 por el número de vehículos que quieras en el top

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

        public void GenerarGrafico(string fileName)
        {
            string contenido = "digraph G {\n";
            contenido += "rankdir=LR;\n"; // Orientación horizontal
            contenido += "node [shape=record];\n";
            contenido += "splines=false;\n"; // Flechas rectas

            Nodo<Vehiculo>* temp = head;
            int index = 0;
            while (temp != null)
            {
                Vehiculo* vehiculo = &temp->Data;
                contenido += $"node{index} [label=\"ID: {vehiculo->Id} \\n ID Usuario: {vehiculo->ID_Usuario} \\n Marca: {GetFixedString(vehiculo->Marca)} \\n Modelo: {vehiculo->Modelo} \\n Placa: {GetFixedString(vehiculo->Placa)}\"];\n";

                if (temp->Next != null)
                {
                    contenido += $"node{index} -> node{index + 1} [dir=forward];\n";
                    contenido += $"node{index} -> node{index + 1} [dir=back];\n";
                }
                temp = temp->Next;
                index++;
            }

            contenido += "}\n";

            // Generar el archivo .dot en la carpeta "reports"
            GenerarArchivoDot(fileName, contenido);

            // Generar el archivo PNG usando Graphviz
            var process = new Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {Path.Combine(Directory.GetCurrentDirectory(), "reports", fileName)} -o {Path.Combine(Directory.GetCurrentDirectory(), "reports", Path.ChangeExtension(fileName, ".png"))}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        public void GenerarTopVehiculosAntiguos(int topN)
        {
            List<Vehiculo> vehiculos = new List<Vehiculo>();
            Nodo<Vehiculo>* temp = head;
            while (temp != null)
            {
                vehiculos.Add(temp->Data);
                temp = temp->Next;
            }

            var vehiculosAntiguos = vehiculos.OrderBy(v => v.Modelo).Take(topN).ToList();

            string contenido = "digraph G {\n";
            contenido += "rankdir=LR;\n"; // Orientación horizontal
            contenido += "node [shape=record];\n";
            contenido += "splines=false;\n"; // Flechas rectas

            for (int i = 0; i < vehiculosAntiguos.Count; i++)
            {
                var vehiculo = vehiculosAntiguos[i];
                contenido += $"node{i} [label=\"ID: {vehiculo.Id} \\n ID Usuario: {vehiculo.ID_Usuario} \\n Marca: {GetFixedString(vehiculo.Marca)} \\n Modelo: {vehiculo.Modelo} \\n Placa: {GetFixedString(vehiculo.Placa)}\"];\n";

                if (i < vehiculosAntiguos.Count - 1)
                {
                    contenido += $"node{i} -> node{i + 1} [dir=forward];\n";
                    contenido += $"node{i} -> node{i + 1} [dir=back];\n";
                }
            }

            contenido += "}\n";

            // Generar el archivo .dot en la carpeta "reports"
            GenerarArchivoDot("TopVehiculosAntiguos.dot", contenido);

            // Generar el archivo PNG usando Graphviz
            var process = new Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {Path.Combine(Directory.GetCurrentDirectory(), "reports", "TopVehiculosAntiguos.dot")} -o {Path.Combine(Directory.GetCurrentDirectory(), "reports", "TopVehiculosAntiguos.png")}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        public static void GenerarArchivoDot(string nombre, string contenido)
        {
            try
            {
                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reports");
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                if (string.IsNullOrEmpty(nombre)) // Verificar que el nombre no sea nulo o vacío
                {
                    Console.WriteLine("El nombre del archivo no puede ser nulo o vacío.");
                    return;
                }

                if (!nombre.EndsWith(".dot"))
                {
                    nombre += ".dot";
                }

                string rutaArchivo = Path.Combine(carpeta, nombre);
                File.WriteAllText(rutaArchivo, contenido);

                Console.WriteLine($"Archivo generado con éxito en: {rutaArchivo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el archivo: {ex.Message}");
            }
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