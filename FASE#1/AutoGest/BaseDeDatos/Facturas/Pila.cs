using System;
using System.Diagnostics;
using System.IO;

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
            GenerarGrafico("Facturacion.dot");
        }

        // Método Pop: elimina y devuelve la factura del nodo superior.
        // Si la pila está vacía, devuelve null.
        public Factura Pop()
        {
            if (tope == null) return null; // Si la pila está vacía, retorna null.
            Factura ret = tope.Data; // Guarda la factura del nodo superior.
            tope = tope.Sig; // El tope pasa al siguiente nodo.
            GenerarGrafico("Facturacion.dot");
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
            GenerarGrafico("Facturacion.dot");
        }

        // Método para generar el gráfico de la pila.
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
                writer.WriteLine("rankdir=TB;"); // Orientación vertical
                writer.WriteLine("node [shape=record];");
                writer.WriteLine("splines=true;"); // Flechas rectas

                Nodo temp = tope;
                int index = 0;
                while (temp != null)
                {
                    Factura factura = temp.Data;
                    writer.WriteLine($"node{index} [label=\"ID: {factura.ID} \\n ID_Orden: {factura.ID_Orden} \\n Total: {factura.Total}\"]");

                    if (temp.Sig != null)
                    {
                        // Flecha de arriba a abajo
                        writer.WriteLine($"node{index} -> node{index + 1};");
                    }
                    temp = temp.Sig;
                    index++;
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
            Console.WriteLine("La gráfica de la pila se ha guardado en: " + Path.GetFullPath(Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))));
        }
    }
}