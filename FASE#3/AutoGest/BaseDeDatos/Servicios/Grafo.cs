using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Servicios
{
    public class GrafoServicios
    {
        // Diccionario para almacenar las relaciones actuales
        private Dictionary<string, HashSet<string>> relaciones = new Dictionary<string, HashSet<string>>();

        // Actualiza el grafo a partir de la lista de servicios
        public void ActualizarDesdeServicios(List<Servicio> servicios)
        {
            relaciones.Clear();
            foreach (var servicio in servicios)
            {
                string vehiculo = $"V{servicio.Id_Vehiculo}";
                string repuesto = $"R{servicio.Id_Repuesto}";

                if (!relaciones.ContainsKey(vehiculo))
                    relaciones[vehiculo] = new HashSet<string>();
                relaciones[vehiculo].Add(repuesto);

                if (!relaciones.ContainsKey(repuesto))
                    relaciones[repuesto] = new HashSet<string>();
                relaciones[repuesto].Add(vehiculo);
            }
        }

        // Genera el reporte DOT igual que en Tarea.cs, pero usando los datos actuales
        public void GenerarReporteDOT(string nombreArchivo, string nombreEstudiante, string carnet)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("graph ServiciosVehiculoRepuesto {");
            sb.AppendLine($"    label=\"Grafo Servicios Vehículo-Repuesto {nombreEstudiante} {carnet}\";");
            sb.AppendLine("    labelloc=top;");
            sb.AppendLine("    fontsize=20;");
            sb.AppendLine("    rankdir=LR;");

            // Identificar vehículos y repuestos
            HashSet<string> vehiculos = new HashSet<string>();
            HashSet<string> repuestos = new HashSet<string>();
            foreach (var nodo in relaciones)
            {
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