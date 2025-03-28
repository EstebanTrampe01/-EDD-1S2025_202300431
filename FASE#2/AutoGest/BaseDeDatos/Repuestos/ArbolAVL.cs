using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Repuestos
{
    public unsafe class ArbolAVL
    {
        private NodoAVL* root;

        public ArbolAVL()
        {
            root = null;
        }

        // Función para obtener la altura de un nodo
        private int GetHeight(NodoAVL* node)
        {
            if (node == null)
                return -1;
            return node->Height;
        }

        // Función para obtener el máximo de dos enteros
        private int GetMax(int a, int b)
        {
            return (a > b) ? a : b;
        }

        // Rotación simple a la derecha
        private NodoAVL* RotateRight(NodoAVL* y)
        {
            NodoAVL* x = y->Left;
            NodoAVL* T2 = x->Right;

            // Realizar rotación
            x->Right = y;
            y->Left = T2;

            // Actualizar alturas
            y->Height = GetMax(GetHeight(y->Left), GetHeight(y->Right)) + 1;
            x->Height = GetMax(GetHeight(x->Left), GetHeight(x->Right)) + 1;

            // Devolver nueva raíz
            return x;
        }

        // Rotación simple a la izquierda
        private NodoAVL* RotateLeft(NodoAVL* x)
        {
            NodoAVL* y = x->Right;
            NodoAVL* T2 = y->Left;

            // Realizar rotación
            y->Left = x;
            x->Right = T2;

            // Actualizar alturas
            x->Height = GetMax(GetHeight(x->Left), GetHeight(x->Right)) + 1;
            y->Height = GetMax(GetHeight(y->Left), GetHeight(y->Right)) + 1;

            // Devolver nueva raíz
            return y;
        }

        // Obtener el factor de balance de un nodo
        private int GetBalance(NodoAVL* node)
        {
            if (node == null)
                return 0;
            return GetHeight(node->Left) - GetHeight(node->Right);
        }

        // Función para insertar un nuevo repuesto
        public void Insertar(int id, string repuesto, string detalles, double costo)
        {
            // Crear nuevo nodo AVL
            NodoAVL* nuevoNodo = (NodoAVL*)Marshal.AllocHGlobal(sizeof(NodoAVL));
            nuevoNodo->Repuesto = new LRepuesto(id, repuesto, detalles, costo);
            nuevoNodo->Left = null;
            nuevoNodo->Right = null;
            nuevoNodo->Height = 0;

            // Insertar el nodo en el árbol
            root = InsertarRecursivo(root, nuevoNodo);

            // Generar gráfico
            GenerarGrafico("Repuestos.dot");
        }

        // Función recursiva para insertar un nodo en el árbol
        private NodoAVL* InsertarRecursivo(NodoAVL* node, NodoAVL* nuevoNodo)
        {
            // Paso 1: Realizar inserción estándar BST
            if (node == null)
                return nuevoNodo;

            if (nuevoNodo->Repuesto.Id < node->Repuesto.Id)
                node->Left = InsertarRecursivo(node->Left, nuevoNodo);
            else if (nuevoNodo->Repuesto.Id > node->Repuesto.Id)
                node->Right = InsertarRecursivo(node->Right, nuevoNodo);
            else
            {
                // Los ID deben ser únicos
                Console.WriteLine($"El repuesto con ID {nuevoNodo->Repuesto.Id} ya existe.");
                Marshal.FreeHGlobal((IntPtr)nuevoNodo);
                return node;
            }

            // Paso 2: Actualizar altura del nodo actual
            node->Height = GetMax(GetHeight(node->Left), GetHeight(node->Right)) + 1;

            // Paso 3: Obtener el factor de balance
            int balance = GetBalance(node);

            // Caso Izquierda-Izquierda
            if (balance > 1 && nuevoNodo->Repuesto.Id < node->Left->Repuesto.Id)
                return RotateRight(node);

            // Caso Derecha-Derecha
            if (balance < -1 && nuevoNodo->Repuesto.Id > node->Right->Repuesto.Id)
                return RotateLeft(node);

            // Caso Izquierda-Derecha
            if (balance > 1 && nuevoNodo->Repuesto.Id > node->Left->Repuesto.Id)
            {
                node->Left = RotateLeft(node->Left);
                return RotateRight(node);
            }

            // Caso Derecha-Izquierda
            if (balance < -1 && nuevoNodo->Repuesto.Id < node->Right->Repuesto.Id)
            {
                node->Right = RotateRight(node->Right);
                return RotateLeft(node);
            }

            // Devolver el puntero sin cambios
            return node;
        }

        // Encontrar el nodo con valor mínimo
        private NodoAVL* MinValueNode(NodoAVL* node)
        {
            NodoAVL* current = node;

            // Encontrar la hoja más a la izquierda
            while (current->Left != null)
                current = current->Left;

            return current;
        }

        // Función para eliminar un repuesto por ID
        public void Eliminar(int id)
        {
            root = EliminarRecursivo(root, id);
            GenerarGrafico("Repuestos.dot");
        }

        // Función recursiva para eliminar un nodo del árbol
        private NodoAVL* EliminarRecursivo(NodoAVL* node, int id)
        {
            // Paso 1: Realizar eliminación estándar BST
            if (node == null)
                return null;

            // Si el ID a eliminar es menor que el ID del nodo, está en el subárbol izquierdo
            if (id < node->Repuesto.Id)
                node->Left = EliminarRecursivo(node->Left, id);
            
            // Si el ID a eliminar es mayor que el ID del nodo, está en el subárbol derecho
            else if (id > node->Repuesto.Id)
                node->Right = EliminarRecursivo(node->Right, id);
            
            // Si el ID es igual, este es el nodo a eliminar
            else
            {
                // Nodo con uno o ningún hijo
                if (node->Left == null || node->Right == null)
                {
                    NodoAVL* temp = node->Left != null ? node->Left : node->Right;

                    // Ningún hijo
                    if (temp == null)
                    {
                        temp = node;
                        node = null;
                    }
                    else // Un hijo
                    {
                        // Copiar contenido del hijo no nulo
                        *node = *temp;
                    }

                    // Liberar memoria
                    Marshal.FreeHGlobal((IntPtr)temp);
                }
                else
                {
                    // Nodo con dos hijos: obtener el sucesor in-order (mínimo en el subárbol derecho)
                    NodoAVL* temp = MinValueNode(node->Right);

                    // Copiar la información del sucesor a este nodo
                    node->Repuesto = temp->Repuesto;

                    // Eliminar el sucesor
                    node->Right = EliminarRecursivo(node->Right, temp->Repuesto.Id);
                }
            }

            // Si el árbol tenía solo un nodo, retornar
            if (node == null)
                return null;

            // Paso 2: Actualizar altura del nodo actual
            node->Height = GetMax(GetHeight(node->Left), GetHeight(node->Right)) + 1;

            // Paso 3: Obtener el factor de balance
            int balance = GetBalance(node);

            // Caso Izquierda-Izquierda
            if (balance > 1 && GetBalance(node->Left) >= 0)
                return RotateRight(node);

            // Caso Izquierda-Derecha
            if (balance > 1 && GetBalance(node->Left) < 0)
            {
                node->Left = RotateLeft(node->Left);
                return RotateRight(node);
            }

            // Caso Derecha-Derecha
            if (balance < -1 && GetBalance(node->Right) <= 0)
                return RotateLeft(node);

            // Caso Derecha-Izquierda
            if (balance < -1 && GetBalance(node->Right) > 0)
            {
                node->Right = RotateRight(node->Right);
                return RotateLeft(node);
            }

            return node;
        }

        // Buscar un repuesto por ID
        public LRepuesto* Buscar(int id)
        {
            NodoAVL* resultado = BuscarRecursivo(root, id);
            if (resultado != null)
                return &resultado->Repuesto;
            return null;
        }

        // Función recursiva para buscar un nodo por ID
        private NodoAVL* BuscarRecursivo(NodoAVL* node, int id)
        {
            if (node == null || node->Repuesto.Id == id)
                return node;

            if (id < node->Repuesto.Id)
                return BuscarRecursivo(node->Left, id);
            else
                return BuscarRecursivo(node->Right, id);
        }

        // Modificar un repuesto existente
        public void ModificarRepuesto(int id, string nuevoRepuesto, string nuevosDetalles, double nuevoCosto)
        {
            NodoAVL* nodo = BuscarRecursivo(root, id);
            if (nodo != null)
            {
                LRepuesto* pRepuesto = &nodo->Repuesto;
                char* r = pRepuesto->Repuesto;
                char* d = pRepuesto->Detalles;
                int i;

                // Actualizar el nombre del repuesto
                for (i = 0; i < nuevoRepuesto.Length && i < 50 - 1; i++)
                {
                    r[i] = nuevoRepuesto[i];
                }
                r[i] = '\0';

                // Actualizar los detalles
                for (i = 0; i < nuevosDetalles.Length && i < 100 - 1; i++)
                {
                    d[i] = nuevosDetalles[i];
                }
                d[i] = '\0';

                // Actualizar el costo
                pRepuesto->Costo = nuevoCosto;

                Console.WriteLine("Repuesto modificado exitosamente.");
                GenerarGrafico("Repuestos.dot");
            }
            else
            {
                Console.WriteLine("Repuesto no encontrado.");
            }
        }

        // Obtener una lista de todos los repuestos (recorrido in-order)
        public string ObtenerLista()
        {
            if (root == null) 
                return "Árbol vacío.";

            List<string> arbolRepuestos = new List<string>();
            ObtenerListaRecursivo(root, arbolRepuestos);
            return string.Join("\n", arbolRepuestos);
        }

        // Recorrer el árbol en in-order para obtener la lista de repuestos
        private void ObtenerListaRecursivo(NodoAVL* node, List<string> lista)
        {
            if (node != null)
            {
                ObtenerListaRecursivo(node->Left, lista);
                lista.Add(node->Repuesto.ToString());
                ObtenerListaRecursivo(node->Right, lista);
            }
        }

        // Mostrar repuestos (recorrido in-order)
        public void Mostrar()
        {
            if (root == null)
            {
                Console.WriteLine("Árbol vacío.");
                return;
            }

            MostrarRecursivo(root);
        }

        // Recorrer el árbol en in-order para mostrar los repuestos
        private void MostrarRecursivo(NodoAVL* node)
        {
            if (node != null)
            {
                MostrarRecursivo(node->Left);
                Console.WriteLine(node->Repuesto.ToString());
                MostrarRecursivo(node->Right);
            }
        }

        // Generar reporte por costo (ordenados de mayor a menor)
        public void GenerarReportePorCosto()
        {
            if (root == null)
            {
                Console.WriteLine("No hay repuestos para generar el reporte.");
                return;
            }

            // Crear una lista temporal para ordenar los repuestos
            List<LRepuesto> repuestos = new List<LRepuesto>();
            ColectarRepuestosRecursivo(root, repuestos);
            
            // Ordenar los repuestos por costo (de mayor a menor)
            var repuestosOrdenados = repuestos.OrderByDescending(r => r.Costo).ToList();

            string contenido = "digraph G {\n";
            contenido += "rankdir=TB;\n";
            contenido += "node [shape=record];\n";
            contenido += "label=\"Repuestos ordenados por costo (mayor a menor)\";\n";

            for (int i = 0; i < repuestosOrdenados.Count; i++)
            {
                var repuesto = repuestosOrdenados[i];
                contenido += $"node{i} [label=\"ID: {repuesto.Id} \\n Repuesto: {GetFixedString(repuesto.Repuesto)} \\n Detalles: {GetFixedString(repuesto.Detalles)} \\n Costo: {repuesto.Costo}\"];\n";
                
                if (i > 0)
                {
                    contenido += $"node{i-1} -> node{i};\n";
                }
            }

            contenido += "}\n";
            GenerarArchivoDot("RepuestosPorCosto.dot", contenido);
        }

        // Colectar todos los repuestos del árbol
        private void ColectarRepuestosRecursivo(NodoAVL* node, List<LRepuesto> repuestos)
        {
            if (node != null)
            {
                ColectarRepuestosRecursivo(node->Left, repuestos);
                repuestos.Add(node->Repuesto);
                ColectarRepuestosRecursivo(node->Right, repuestos);
            }
        }

        // Genera un reporte filtrando por rango de costo
        public void GenerarReportePorRangoCosto(double minCosto, double maxCosto)
        {
            if (root == null)
            {
                Console.WriteLine("No hay repuestos para generar el reporte.");
                return;
            }

            // Crear una lista temporal para los repuestos en el rango
            List<LRepuesto> repuestosEnRango = new List<LRepuesto>();
            ColectarRepuestosPorRangoRecursivo(root, repuestosEnRango, minCosto, maxCosto);

            if (repuestosEnRango.Count == 0)
            {
                Console.WriteLine($"No hay repuestos en el rango de costo {minCosto}-{maxCosto}.");
                return;
            }

            string contenido = "digraph G {\n";
            contenido += "rankdir=TB;\n";
            contenido += "node [shape=record];\n";
            contenido += $"label=\"Repuestos con costo entre {minCosto} y {maxCosto}\";\n";

            for (int i = 0; i < repuestosEnRango.Count; i++)
            {
                var repuesto = repuestosEnRango[i];
                contenido += $"node{i} [label=\"ID: {repuesto.Id} \\n Repuesto: {GetFixedString(repuesto.Repuesto)} \\n Detalles: {GetFixedString(repuesto.Detalles)} \\n Costo: {repuesto.Costo}\", color=blue];\n";
                
                if (i > 0)
                {
                    contenido += $"node{i-1} -> node{i};\n";
                }
            }

            contenido += "}\n";
            GenerarArchivoDot($"RepuestosCosto{minCosto}-{maxCosto}.dot", contenido);
        }

        // Colectar repuestos por rango de costo
        private void ColectarRepuestosPorRangoRecursivo(NodoAVL* node, List<LRepuesto> repuestos, double minCosto, double maxCosto)
        {
            if (node != null)
            {
                ColectarRepuestosPorRangoRecursivo(node->Left, repuestos, minCosto, maxCosto);
                
                if (node->Repuesto.Costo >= minCosto && node->Repuesto.Costo <= maxCosto)
                {
                    repuestos.Add(node->Repuesto);
                }
                
                ColectarRepuestosPorRangoRecursivo(node->Right, repuestos, minCosto, maxCosto);
            }
        }

        // Método para generar archivos DOT de manera reutilizable
        public static void GenerarArchivoDot(string nombre, string contenido)
        {
            try
            {
                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reports");
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                if (string.IsNullOrEmpty(nombre))
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
                
                // Generar el archivo PNG usando Graphviz
                var process = new Process();
                process.StartInfo.FileName = "dot";
                process.StartInfo.Arguments = $"-Tpng {rutaArchivo} -o {Path.Combine(carpeta, Path.ChangeExtension(nombre, ".png"))}";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el archivo: {ex.Message}");
            }
        }

        // Generar gráfico del árbol AVL
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
                writer.WriteLine("digraph AVL {");
                writer.WriteLine("node [shape=record];");
                writer.WriteLine("rankdir=TB;");
                
                if (root != null)
                {
                    // Generar nodos
                    GenerarNodosRecursivo(writer, root);
                    
                    // Generar conexiones
                    GenerarConexionesRecursivo(writer, root);
                }

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
            Console.WriteLine("La gráfica del árbol AVL se ha guardado en: " + Path.GetFullPath(Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))));
        }

        // Generar nodos recursivamente
        private void GenerarNodosRecursivo(StreamWriter writer, NodoAVL* node)
        {
            if (node != null)
            {
                writer.WriteLine($"node{node->Repuesto.Id} [label=\"<f0> | <f1> ID: {node->Repuesto.Id}\\nRepuesto: {GetFixedString(node->Repuesto.Repuesto)}\\nDetalles: {GetFixedString(node->Repuesto.Detalles)}\\nCosto: {node->Repuesto.Costo}\\nFB: {GetBalance(node)} | <f2>\"];");
                
                GenerarNodosRecursivo(writer, node->Left);
                GenerarNodosRecursivo(writer, node->Right);
            }
        }

        // Generar conexiones recursivamente        // Generar conexiones recursivamente (continuación)
        private void GenerarConexionesRecursivo(StreamWriter writer, NodoAVL* node)
        {
            if (node != null)
            {
                if (node->Left != null)
                {
                    writer.WriteLine($"\"node{node->Repuesto.Id}\":f0 -> \"node{node->Left->Repuesto.Id}\":f1;");
                    GenerarConexionesRecursivo(writer, node->Left);
                }
                
                if (node->Right != null)
                {
                    writer.WriteLine($"\"node{node->Repuesto.Id}\":f2 -> \"node{node->Right->Repuesto.Id}\":f1;");
                    GenerarConexionesRecursivo(writer, node->Right);
                }
            }
        }
        
        // Método para convertir de char* a string
        private string GetFixedString(char* fixedStr)
        {
            string str = "";
            for (int i = 0; fixedStr[i] != '\0'; i++)
            {
                str += fixedStr[i];
            }
            return str;
        }
        
        // Destructor
        ~ArbolAVL()
        {
            LiberarMemoriaRecursivo(root);
        }
        
        // Liberar memoria recursivamente
        private void LiberarMemoriaRecursivo(NodoAVL* node)
        {
            if (node != null)
            {
                LiberarMemoriaRecursivo(node->Left);
                LiberarMemoriaRecursivo(node->Right);
                Marshal.FreeHGlobal((IntPtr)node);
            }
        }
    }
}