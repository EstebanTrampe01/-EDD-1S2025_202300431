using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Usuarios;

namespace Usuarios
{
    public class UsuarioBlockchain
    {
        public List<Block> Chain { get; private set; }
        
        public UsuarioBlockchain()
        {
            Chain = new List<Block>();
            // Crear un bloque génesis con un usuario vacío
            Usuario genesisUser = new Usuario(0, "", "", "", 0,"");
            var genesisBlock = new Block(0, genesisUser, "0000");
            Chain.Add(genesisBlock);
        }
        
        public void AddBlock(Usuario data)
        {
            int index = Chain[Chain.Count-1].Index + 1;
            string previousHash = Chain[Chain.Count-1].Hash;
            var newBlock = new Block(index, data, previousHash);
            Chain.Add(newBlock);
        }

        public void GenerarGrafico(string fileName)
        {
            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph Blockchain {");
            dot.AppendLine("    node [shape=record, style=filled, fontname=\"Arial\"];");

            for (int i = 0; i < Chain.Count; i++)
            {
                Block block = Chain[i];
                Usuario user = block.Data;

                // Formato de fecha DD-MM-YY::HH:MM:SS
                DateTime dt;
                string timestamp = block.Timestamp;
                if (DateTime.TryParse(block.Timestamp, out dt))
                {
                    timestamp = dt.ToString("dd-MM-yy::HH:mm:ss");
                }

                string nombre = user.Name ?? "";
                string apellido = user.LastName ?? "";
                string correo = user.Correo ?? "";
                string contrasena = user.Contrasena ?? "";

                dot.AppendLine($"    Block{i} [label=\"{{ INDEX: {block.Index} | TIMESTAMP: {timestamp} | DATA: [ID: {user.Id}, Nombres: {nombre}, Apellidos: {apellido}, Correo: {correo}, Edad {user.Edad},Contraseña: {contrasena}] | NONCE: {block.Nonce} | PREVIOUS HASH: {block.PreviousHash} | HASH: {block.Hash} }}\"];");

                if (i < Chain.Count - 1)
                {
                    dot.AppendLine($"    Block{i} -> Block{i + 1};");
                }
            }

            dot.AppendLine("}");

            // Guardar en carpeta reports
            string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reports");
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            if (!fileName.EndsWith(".dot"))
                fileName += ".dot";

            string filePath = Path.Combine(carpeta, fileName);
            File.WriteAllText(filePath, dot.ToString());
            Console.WriteLine($"Archivo DOT generado: {filePath}");

            // Generar PNG usando Graphviz
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {filePath} -o {Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            Console.WriteLine($"Archivo PNG generado: {Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))}");
        }
    }
}