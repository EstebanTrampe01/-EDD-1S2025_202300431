using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Usuarios;
using Vehiculos;
using Repuestos;
using Facturas;
using Servicios;

namespace Backup
{
    // Nodo para el árbol de Huffman (optimizado)
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public char Character { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public int CompareTo(HuffmanNode other) => Frequency.CompareTo(other.Frequency);
        public bool IsLeaf() => Left == null && Right == null;
    }

    // Manager principal de backups
    public class BackupManager
    {
        private const string BackupFolder = "BackupData";

        // Genera backup completo
        public void GenerateBackup(UserBlockchain usuarios, ListaDoblementeEnlazada vehiculos, ArbolAVL repuestos)
        {
            Directory.CreateDirectory(BackupFolder);

            // 1. Backup de usuarios (JSON sin comprimir)
            var usersJson = JsonSerializer.Serialize(usuarios.GetAllBlocks());
            File.WriteAllText(Path.Combine(BackupFolder, "users_backup.json"), usersJson);

            // 2. Backup de vehículos (comprimido)
            var vehiculosList = vehiculos.ObtenerLista();
            var vehiclesJson = JsonSerializer.Serialize(vehiculosList);
            CompressWithHuffman(vehiclesJson, "vehicles");

            // 3. Backup de repuestos (comprimido)
            var repuestosList = repuestos.ObtenerLista();
            var partsJson = JsonSerializer.Serialize(repuestosList);
            CompressWithHuffman(partsJson, "parts");

            Console.WriteLine("Backup generado exitosamente");
        }

        // Carga backup completo y actualiza las estructuras actuales
        public bool LoadBackup(
            UserBlockchain usuarios, 
            ListaDoblementeEnlazada vehiculos, 
            ArbolAVL repuestos)
        {
            bool backupCargado = false;

            try
            {
                // 1. Cargar usuarios (Blockchain)
                var usersPath = Path.Combine(BackupFolder, "users_backup.json");
                
                if (File.Exists(usersPath))
                {
                    var usersJson = File.ReadAllText(usersPath);
                    var blocks = JsonSerializer.Deserialize<List<Block>>(usersJson);
                    usuarios.CargarDesdeBackup(blocks);
                    backupCargado = true;
                }

                // 2. Cargar vehículos (descomprimir)
                var vehiculosList = DecompressWithHuffmanInternal<List<Vehiculo>>("vehicles");
                if (vehiculosList != null && vehiculosList.Count > 0)
                {
                    vehiculos.CargarDesdeBackup(vehiculosList);
                    backupCargado = true;
                }

                // 3. Cargar repuestos (descomprimir)
                var repuestosList = DecompressWithHuffman<List<LRepuesto>>("parts");
                if (repuestosList != null && repuestosList.Count > 0)
                {
                    repuestos.CargarDesdeBackup(repuestosList);
                    backupCargado = true;
                }

                Console.WriteLine("Backup cargado exitosamente");
                return backupCargado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar backup: {ex.Message}");
                return false;
            }
        }

        // Compresión Huffman (para vehículos y repuestos)
        private void CompressWithHuffman(string data, string entityName)
        {
            var frequencies = CalculateFrequencies(data);
            var root = BuildHuffmanTree(frequencies);
            var codes = GenerateHuffmanCodes(root);

            // Convertir a bits
            var bitString = new StringBuilder();
            foreach (char c in data)
            {
                bitString.Append(codes[c]);
            }

            // Convertir a bytes y guardar
            var bytes = ConvertBitStringToBytes(bitString.ToString());
            File.WriteAllBytes(Path.Combine(BackupFolder, $"{entityName}.edd"), bytes);

            // Guardar cabecera con frecuencias (para reconstruir árbol)
            var header = string.Join(",", frequencies.Select(kvp => $"{(int)kvp.Key}:{kvp.Value}"));
            File.WriteAllText(Path.Combine(BackupFolder, $"{entityName}_freq.hdr"), header);
        }

        // Hacer público el método de descompresión para que sea accesible desde InterfazCarB
        public T DecompressWithHuffman<T>(string entityName)
        {
            // Intentar primero con la ruta relativa estándar
            string compressedPath = Path.Combine(BackupFolder, $"{entityName}.edd");
            string headerPath = Path.Combine(BackupFolder, $"{entityName}_freq.hdr");

            // Si no existe, probar buscando el archivo directamente por su nombre
            if (!File.Exists(compressedPath))
            {
                // Buscar en el directorio actual y sus subdirectorios
                string[] eddFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{entityName}.edd", SearchOption.AllDirectories);
                string[] hdrFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{entityName}_freq.hdr", SearchOption.AllDirectories);
                
                if (eddFiles.Length > 0 && hdrFiles.Length > 0)
                {
                    compressedPath = eddFiles[0];
                    headerPath = hdrFiles[0];
                }
            }

            if (!File.Exists(compressedPath) || !File.Exists(headerPath))
            {
                Console.WriteLine($"No se encontraron archivos para {entityName}");
                return default;
            }

            try
            {
                // Leer frecuencias desde cabecera
                var header = File.ReadAllText(headerPath);
                var frequencies = header.Split(',')
                    .ToDictionary(
                        s => (char)int.Parse(s.Split(':')[0]),
                        s => int.Parse(s.Split(':')[1]));

                // Reconstruir árbol
                var root = BuildHuffmanTree(frequencies);

                // Leer bytes comprimidos
                var bytes = File.ReadAllBytes(compressedPath);
                var bitString = ConvertBytesToBitString(bytes);

                // Descomprimir
                var decompressed = DecompressText(bitString, root);
                return JsonSerializer.Deserialize<T>(decompressed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al descomprimir {entityName}: {ex.Message}");
                return default;
            }
        }

        // Método para cargar backup desde una ruta específica
        public bool LoadBackupFromPath(
            string filePath,
            UserBlockchain usuarios = null, 
            ListaDoblementeEnlazada vehiculos = null, 
            ArbolAVL repuestos = null)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"El archivo {filePath} no existe");
                    return false;
                }

                string extension = Path.GetExtension(filePath).ToLower();
                
                if (extension == ".json")
                {
                    // Leer el contenido del archivo
                    string contenido = File.ReadAllText(filePath);
                    
                    try
                    {
                        // Intentar cargar como usuarios (blockchain)
                        if (usuarios != null && contenido.Contains("\"Hash\"") && contenido.Contains("\"PreviousHash\""))
                        {
                            var blocks = JsonSerializer.Deserialize<List<Block>>(contenido);
                            if (blocks != null && blocks.Count > 0)
                            {
                                usuarios.CargarDesdeBackup(blocks);
                                Console.WriteLine($"Backup de usuarios cargado: {blocks.Count} bloques");
                                return true;
                            }
                        }
                        
                        // Intentar cargar como vehículos
                        else if (vehiculos != null && contenido.Contains("\"Marca\"") && contenido.Contains("\"Placa\""))
                        {
                            var vehiculosList = JsonSerializer.Deserialize<List<Vehiculo>>(contenido);
                            if (vehiculosList != null && vehiculosList.Count > 0)
                            {
                                vehiculos.CargarDesdeBackup(vehiculosList);
                                Console.WriteLine($"Backup de vehículos cargado: {vehiculosList.Count} elementos");
                                return true;
                            }
                        }
                        
                        // Intentar cargar como repuestos
                        else if (repuestos != null && contenido.Contains("\"Costo\"") && contenido.Contains("\"Id\""))
                        {
                            var repuestosList = JsonSerializer.Deserialize<List<LRepuesto>>(contenido);
                            if (repuestosList != null && repuestosList.Count > 0)
                            {
                                repuestos.CargarDesdeBackup(repuestosList);
                                Console.WriteLine($"Backup de repuestos cargado: {repuestosList.Count} elementos");
                                return true;
                            }
                        }
                        
                        Console.WriteLine("El contenido del archivo no corresponde a ninguna estructura conocida");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al deserializar JSON: {ex.Message}");
                        return false;
                    }
                }
                else if (extension == ".edd")
                {
                    string nombreBase = Path.GetFileNameWithoutExtension(filePath);
                    string directorio = Path.GetDirectoryName(filePath);
                    string headerPath = Path.Combine(directorio, $"{nombreBase}_freq.hdr");
                    
                    if (!File.Exists(headerPath))
                    {
                        Console.WriteLine($"No se encontró el archivo de cabecera {headerPath}");
                        return false;
                    }
                    
                    try
                    {
                        // Detectar tipo por nombre o contenido
                        if (nombreBase.Contains("vehicle") && vehiculos != null)
                        {
                            // Copiar a carpeta de backup para poder usar DecompressWithHuffman
                            Directory.CreateDirectory(BackupFolder);
                            File.Copy(filePath, Path.Combine(BackupFolder, $"{nombreBase}.edd"), true);
                            File.Copy(headerPath, Path.Combine(BackupFolder, $"{nombreBase}_freq.hdr"), true);
                            
                            var vehiculosList = DecompressWithHuffman<List<Vehiculo>>(nombreBase);
                            if (vehiculosList != null && vehiculosList.Count > 0)
                            {
                                vehiculos.CargarDesdeBackup(vehiculosList);
                                Console.WriteLine($"Backup de vehículos cargado: {vehiculosList.Count} elementos");
                                return true;
                            }
                        }
                        else if (nombreBase.Contains("part") && repuestos != null)
                        {
                            // Copiar a carpeta de backup para poder usar DecompressWithHuffman
                            Directory.CreateDirectory(BackupFolder);
                            File.Copy(filePath, Path.Combine(BackupFolder, $"{nombreBase}.edd"), true);
                            File.Copy(headerPath, Path.Combine(BackupFolder, $"{nombreBase}_freq.hdr"), true);
                            
                            var repuestosList = DecompressWithHuffman<List<LRepuesto>>(nombreBase);
                            if (repuestosList != null && repuestosList.Count > 0)
                            {
                                repuestos.CargarDesdeBackup(repuestosList);
                                Console.WriteLine($"Backup de repuestos cargado: {repuestosList.Count} elementos");
                                return true;
                            }
                        }
                        
                        Console.WriteLine("No se pudo determinar el tipo de datos del archivo comprimido");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al descomprimir archivo: {ex.Message}");
                        return false;
                    }
                }
                
                Console.WriteLine($"Formato de archivo no soportado: {extension}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al cargar backup: {ex.Message}");
                return false;
            }
        }

        // Descompresión Huffman (renamed to avoid ambiguity)
        private T DecompressWithHuffmanInternal<T>(string entityName)
        {
            var compressedPath = Path.Combine(BackupFolder, $"{entityName}.edd");
            var headerPath = Path.Combine(BackupFolder, $"{entityName}_freq.hdr");

            if (!File.Exists(compressedPath) || !File.Exists(headerPath))
                return default;

            // Leer frecuencias desde cabecera
            var header = File.ReadAllText(headerPath);
            var frequencies = header.Split(',')
                .ToDictionary(
                    s => (char)int.Parse(s.Split(':')[0]),
                    s => int.Parse(s.Split(':')[1]));

            // Reconstruir árbol
            var root = BuildHuffmanTree(frequencies);

            // Leer bytes comprimidos
            var bytes = File.ReadAllBytes(compressedPath);
            var bitString = ConvertBytesToBitString(bytes);

            // Descomprimir
            var decompressed = DecompressText(bitString, root);
            return JsonSerializer.Deserialize<T>(decompressed);
        }

        #region Algoritmo Huffman (implementación optimizada)
        private Dictionary<char, int> CalculateFrequencies(string text)
        {
            var frequencies = new Dictionary<char, int>();
            foreach (char c in text)
            {
                frequencies[c] = frequencies.ContainsKey(c) ? frequencies[c] + 1 : 1;
            }
            return frequencies;
        }

        private HuffmanNode BuildHuffmanTree(Dictionary<char, int> frequencies)
        {
            var queue = new PriorityQueue<HuffmanNode>();

            foreach (var kvp in frequencies)
            {
                queue.Enqueue(new HuffmanNode { Character = kvp.Key, Frequency = kvp.Value });
            }

            while (queue.Count > 1)
            {
                var left = queue.Dequeue();
                var right = queue.Dequeue();

                queue.Enqueue(new HuffmanNode {
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                });
            }

            return queue.Dequeue();
        }

        private Dictionary<char, string> GenerateHuffmanCodes(HuffmanNode root)
        {
            var codes = new Dictionary<char, string>();
            TraverseTree(root, "", codes);
            return codes;
        }

        private void TraverseTree(HuffmanNode node, string code, Dictionary<char, string> codes)
        {
            if (node == null) return;

            if (node.IsLeaf())
            {
                codes[node.Character] = code;
                return;
            }

            TraverseTree(node.Left, code + "0", codes);
            TraverseTree(node.Right, code + "1", codes);
        }

        private string DecompressText(string bitString, HuffmanNode root)
        {
            var result = new StringBuilder();
            var current = root;

            foreach (char bit in bitString)
            {
                current = (bit == '0') ? current.Left : current.Right;

                if (current.IsLeaf())
                {
                    result.Append(current.Character);
                    current = root;
                }
            }

            return result.ToString();
        }
        #endregion

        #region Conversión bits/bytes
        private byte[] ConvertBitStringToBytes(string bits)
        {
            int numBytes = (bits.Length + 7) / 8;
            byte[] bytes = new byte[numBytes];

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == '1')
                {
                    bytes[i / 8] |= (byte)(1 << (7 - (i % 8)));
                }
            }

            return bytes;
        }

        private string ConvertBytesToBitString(byte[] bytes)
        {
            var bits = new StringBuilder();

            foreach (byte b in bytes)
            {
                for (int i = 7; i >= 0; i--)
                {
                    bits.Append((b & (1 << i)) != 0 ? "1" : "0");
                }
            }

            return bits.ToString();
        }
        #endregion
    }

    // Cola de prioridad optimizada
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private readonly List<T> _elements = new List<T>();

        public int Count => _elements.Count;

        public void Enqueue(T item)
        {
            _elements.Add(item);
            int ci = _elements.Count - 1;

            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (_elements[ci].CompareTo(_elements[pi]) >= 0)
                    break;
                Swap(ci, pi);
                ci = pi;
            }
        }

        public T Dequeue()
        {
            T frontItem = _elements[0];
            _elements[0] = _elements[_elements.Count - 1];
            _elements.RemoveAt(_elements.Count - 1);

            int pi = 0;
            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci >= _elements.Count) break;
                int rc = ci + 1;
                if (rc < _elements.Count && _elements[rc].CompareTo(_elements[ci]) < 0)
                    ci = rc;
                if (_elements[pi].CompareTo(_elements[ci]) <= 0) break;
                Swap(pi, ci);
                pi = ci;
            }

            return frontItem;
        }

        private void Swap(int i, int j)
        {
            (_elements[i], _elements[j]) = (_elements[j], _elements[i]);
        }
    }
}