using System;
using System.IO;
using System.Diagnostics;

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
            GenerarGrafico("Servicios.dot");
        }

        // Método desencolar: elimina y devuelve el servicio del nodo al frente de la cola.
        // Si la cola está vacía, retorna null.
        public Servicio Desencolar()
        {
            if (frente == null) return null; // Si la cola está vacía, retorna null.
            Servicio ret = frente.Data; // Guarda el servicio del nodo frente.
            frente = frente.Sig; // Mueve el frente al siguiente nodo.
            if (frente == null) final = null; // Si la cola queda vacía, el final también se establece como null.
            GenerarGrafico("Servicios.dot");
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
            GenerarGrafico("Servicios.dot");

            Console.WriteLine("NULL"); // Indica el final de la cola.
        }

        // Método para generar el gráfico de la cola.
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

                Nodo temp = frente;
                int index = 0;
                while (temp != null)
                {
                    Servicio servicio = temp.Data;
                    writer.WriteLine($"node{index} [label=\"ID: {servicio.ID} \\n Id_Repuesto: {servicio.Id_Repuesto} \\n Id_Vehiculo: {servicio.Id_Vehiculo} \\n Detalles: {servicio.Detalles} \\n Costo: {servicio.Costo}\"]");

                    if (temp.Sig != null)
                    {
                        // Flecha de izquierda a derecha
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
            Console.WriteLine("La gráfica de la cola se ha guardado en: " + Path.GetFullPath(Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))));
        }
    }
}