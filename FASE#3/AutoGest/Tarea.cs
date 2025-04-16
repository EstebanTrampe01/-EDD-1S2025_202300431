using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoGest
{
    public class GrafoVehiculoRepuesto
    {
        // Diccionario para almacenar las relaciones
        private Dictionary<string, HashSet<string>> relaciones = new Dictionary<string, HashSet<string>>();

        // Agregar relación entre vehículo y repuesto
        public void AgregarRelacion(string idVehiculo, string idRepuesto)
        {
            if (!relaciones.ContainsKey(idVehiculo))
                relaciones[idVehiculo] = new HashSet<string>();
            relaciones[idVehiculo].Add(idRepuesto);

            // Como es no dirigido, también agregamos la relación inversa
            if (!relaciones.ContainsKey(idRepuesto))
                relaciones[idRepuesto] = new HashSet<string>();
            relaciones[idRepuesto].Add(idVehiculo);
        }
                public void GenerarReporteDOT(string nombreArchivo)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("graph VehiculosRepuestos {");
                    sb.AppendLine($"    label=\"Grafo Vehículos-Repuestos JUAN ESTEBAN CHACON TRAMPE 202300431\";");
                    sb.AppendLine("    labelloc=top;");
                    sb.AppendLine("    fontsize=20;");
                    sb.AppendLine("    rankdir=LR;"); // De izquierda a derecha
        
                    // Identificar vehículos y repuestos
                    HashSet<string> vehiculos = new HashSet<string>();
                    HashSet<string> repuestos = new HashSet<string>();
                    foreach (var nodo in relaciones)
                    {
                        // Si el nodo tiene vecinos que empiezan con 'R', es vehículo
                        if (nodo.Key.StartsWith("V"))
                            vehiculos.Add(nodo.Key);
                        else if (nodo.Key.StartsWith("R"))
                            repuestos.Add(nodo.Key);
                    }
        
                    // Subgrafo para vehículos (izquierda)
                    sb.AppendLine("    subgraph cluster_vehiculos {");
                    sb.AppendLine("        label=\"Vehículos\";");
                    sb.AppendLine("        color=white;");
                    sb.AppendLine("        rank=same;");
                    foreach (var v in vehiculos)
                        sb.AppendLine($"        \"{v}\";");
                    sb.AppendLine("    }");
        
                    // Subgrafo para repuestos (derecha)
                    sb.AppendLine("    subgraph cluster_repuestos {");
                    sb.AppendLine("        label=\"Repuestos\";");
                    sb.AppendLine("        color=white;");
                    sb.AppendLine("        rank=same;");
                    foreach (var r in repuestos)
                        sb.AppendLine($"        \"{r}\";");
                    sb.AppendLine("    }");
        
                    // Agregar las aristas
                    HashSet<string> agregadas = new HashSet<string>();
                    foreach (var nodo in relaciones)
                    {
                        foreach (var vecino in nodo.Value)
                        {
                            // Solo conectar de vehículo a repuesto
                            if (vehiculos.Contains(nodo.Key) && repuestos.Contains(vecino))
                            {
                                string arista = $"{nodo.Key}--{vecino}";
                                if (!agregadas.Contains(arista))
                                {
                                    sb.AppendLine($"    \"{nodo.Key}\" -- \"{vecino}\";");
                                    agregadas.Add(arista);
                                }
                            }
                        }
                    }
        
                    sb.AppendLine("}");
        
                    string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reports");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);
        
                    string ruta = Path.Combine(carpeta, nombreArchivo + ".dot");
                    File.WriteAllText(ruta, sb.ToString());
                    Console.WriteLine($"Reporte DOT generado en: {ruta}");
        
                    // Generar PNG usando Graphviz
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "dot";
                    process.StartInfo.Arguments = $"-Tpng {ruta} -o {Path.Combine(carpeta, nombreArchivo + ".png")}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                    Console.WriteLine($"Imagen PNG generada en: {Path.Combine(carpeta, nombreArchivo + ".png")}");
                }
    }
}