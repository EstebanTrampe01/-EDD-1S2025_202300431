using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Facturas
{
    public class ArbolM
    {
        private List<MerkleNode> Leaves; // Lista de nodos hoja 
        private MerkleNode raiz;         // Raíz del árbol

        // Constructor del árbol
        public ArbolM()
        {
            Leaves = new List<MerkleNode>();
            raiz = null;
        }

        // Método para insertar una nueva factura (compatible con el método original Insertar)
        public void Insertar(Factura factura)
        {
            // Verificar unicidad del ID
            foreach (var leaf in Leaves)
            {
                if (leaf.Factura.ID == factura.ID)
                {
                    Console.WriteLine($"Error: Ya existe una factura con ID {factura.ID}.");
                    return;
                }
            }

            // Crear el nodo hoja
            MerkleNode newLeaf = new MerkleNode(factura);
            Leaves.Add(newLeaf);

            // Reconstruir el árbol con las hojas actuales
            BuildTree();
            
            // Generar gráfico automáticamente después de insertar
        }

        // Método para añadir una factura con parámetros detallados
        public void Insertar(int id, int idOrden, int idServicio, double total, string fecha, string metodoPago)
        {
            Factura factura = new Factura(id, idOrden, idServicio, total, fecha, metodoPago);
            Insertar(factura);
        }

        // Método privado para construir el árbol a partir de las hojas
        private void BuildTree()
        {
            if (Leaves.Count == 0)
            {
                raiz = null;
                return;
            }

            List<MerkleNode> currentLevel = new List<MerkleNode>(Leaves);

            while (currentLevel.Count > 1)
            {
                List<MerkleNode> nextLevel = new List<MerkleNode>();

                for (int i = 0; i < currentLevel.Count; i += 2)
                {
                    MerkleNode left = currentLevel[i];
                    MerkleNode right = (i + 1 < currentLevel.Count) ? currentLevel[i + 1] : null;
                    MerkleNode parent = new MerkleNode(left, right);
                    nextLevel.Add(parent);
                }

                currentLevel = nextLevel;
            }

            raiz = currentLevel[0]; // La raíz es el único nodo que queda
        }

        // Busca una factura por su ID
        public Factura Buscar(int id)
        {
            foreach (var leaf in Leaves)
            {
                if (leaf.Factura.ID == id)
                {
                    return leaf.Factura;
                }
            }
            return null;
        }

        // Método para eliminar una factura por su ID
        public bool Eliminar(int id)
        {
            // Buscar la factura
            int index = -1;
            for (int i = 0; i < Leaves.Count; i++)
            {
                if (Leaves[i].Factura.ID == id)
                {
                    index = i;
                    break;
                }
            }

            // Si no se encontró, retornar false
            if (index == -1)
            {
                return false;
            }

            // Eliminar la factura
            Leaves.RemoveAt(index);
            
            // Reconstruir el árbol
            BuildTree();
            
            // Generar gráfico automáticamente después de eliminar
            
            return true;
        }

        // Imprime las facturas en orden de ID
        public void Print()
        {
            List<Factura> facturas = RecorridoInOrden();
            foreach (var factura in facturas)
            {
                Console.Write(factura + " -> ");
            }
            Console.WriteLine("NULL");
        }

        // Recorrido InOrden del árbol para obtener las facturas ordenadas por ID
        public List<Factura> RecorridoInOrden()
        {
            List<Factura> resultado = new List<Factura>();
            
            // Obtener todas las facturas de las hojas
            foreach (var leaf in Leaves)
            {
                resultado.Add(leaf.Factura);
            }
            
            // Ordenar por ID para mantener compatibilidad con el orden esperado
            resultado.Sort((a, b) => a.ID.CompareTo(b.ID));
            
            return resultado;
        }

        // Método para generar el gráfico del árbol Merkle
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
                writer.WriteLine("digraph MerkleTree {");
                writer.WriteLine("  node [shape=record, style=filled, fillcolor=lightblue];");
                writer.WriteLine("  graph [rankdir=TB];");
                writer.WriteLine("  subgraph cluster_0 {");
                writer.WriteLine("    label=\"Facturas - Árbol de Merkle\";");

                if (raiz == null)
                {
                    writer.WriteLine("    empty [label=\"Árbol vacío\"];");
                }
                else
                {
                    Dictionary<string, int> nodeIds = new Dictionary<string, int>();
                    int idCounter = 0;
                    GenerarNodosGrafico(raiz, writer, nodeIds, ref idCounter);
                }

                writer.WriteLine("  }");
                writer.WriteLine("}");
            }

            var process = new Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {filePath} -o {Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            Console.WriteLine("La gráfica del árbol Merkle se ha guardado en: " + Path.GetFullPath(Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))));
        }

        private void GenerarNodosGrafico(MerkleNode node, StreamWriter writer, Dictionary<string, int> nodeIds, ref int idCounter)
        {
            if (node == null) return;

            // Asignar un identificador único al nodo si no tiene uno
            if (!nodeIds.ContainsKey(node.Hash))
            {
                nodeIds[node.Hash] = idCounter++;
                node.Tag = "node" + (idCounter - 1);
            }
            
            string label;
            if (node.Factura != null)
            {
                // Es un nodo hoja (factura)
                label = $"\"Factura ID: {node.Factura.ID}\\nOrden: {node.Factura.ID_Orden}\\nTotal: ${node.Factura.Total:F2}\\nMétodo: {node.Factura.MetodoPago}\\nHash: {node.Hash.Substring(0, 8)}...\"";
            }
            else
            {
                // Es un nodo interno
                label = $"\"Hash: {node.Hash.Substring(0, 8)}...\"";
            }
            writer.WriteLine($"  {node.Tag} [label={label}];");

            if (node.Left != null)
            {
                GenerarNodosGrafico(node.Left, writer, nodeIds, ref idCounter);
                writer.WriteLine($"  {node.Tag} -> {node.Left.Tag};");
            }

            if (node.Right != null)
            {
                GenerarNodosGrafico(node.Right, writer, nodeIds, ref idCounter);
                writer.WriteLine($"  {node.Tag} -> {node.Right.Tag};");
            }
        }

        /// <summary>
        /// Busca una factura por ID_Orden.
        /// </summary>
        public Factura BuscarPorOrden(int idOrden)
        {
            foreach (var leaf in Leaves)
            {
                if (leaf.Factura.ID_Orden == idOrden)
                {
                    return leaf.Factura;
                }
            }
            return null;
        }

        /// <summary>
        /// Implementa un método Push para mantener compatibilidad con el código anterior.
        /// Este método simplemente llama a Insertar.
        /// </summary>
        public void Push(Factura factura)
        {
            // Llamamos al método insertar para mantener compatibilidad
            Insertar(factura);
        }
    }
}