﻿using Gtk;
using Repuestos;
using Vehiculos;
using Servicios;
using Matriz;
using System.IO;
using System.Diagnostics;

class Program
{
    public static void Main(string[] args)
    {
        Application.Init();

        // Crear la ventana principal
        InterfazMenu interfazMenu = new InterfazMenu();
        interfazMenu.ShowAll();

        // Inicializar las listas de vehículos, repuestos y servicios
        ListaDoblementeEnlazada listaVehiculos = new ListaDoblementeEnlazada();
        ListaCircular listaRepuestos = new ListaCircular();
        Cola listaServicios = new Cola();

        // Agregar algunos vehículos y repuestos a las listas
        listaVehiculos.Insertar(3, 101, "Toyota", 2020, "ABC123");
        listaVehiculos.Insertar(2, 102, "Honda", 2019, "XYZ789");
        listaRepuestos.Insertar(3, "Filtro de Aceite", "Filtro de aceite para motor", 50.0);
        listaRepuestos.Insertar(2, "Bujía", "Bujía de encendido", 20.0);

        // Agregar algunos servicios a la lista de servicios
        Servicio servicio1 = new Servicio(3, 3, 3, "Cambio de filtro de aceite", 100.0);
        Servicio servicio2 = new Servicio(2, 2, 2, "Cambio de bujía", 40.0);
        listaServicios.Encolar(servicio1);
        listaServicios.Encolar(servicio2);

        // Inicializar la matriz dispersa
        MatrizDispersa<int> matriz = new MatrizDispersa<int>(1, listaVehiculos, listaRepuestos, listaServicios);

        // Insertar servicios en la matriz dispersa
        matriz.insert(1, 1, servicio1);
        matriz.insert(2, 2, servicio2);

        // Mostrar la matriz dispersa
        matriz.mostrar();

        // Generar el gráfico de la matriz dispersa
        string graphviz = matriz.GenerarGraphviz();
        File.WriteAllText("matrizDispersa.dot", graphviz);

        // Ejecutar el comando dot para generar el archivo PNG
        GenerarImagenGraphviz("matrizDispersa.dot", "matrizDispersa.png");

        Application.Run();
    }

    // Método para ejecutar el comando dot y generar el archivo PNG
    private static void GenerarImagenGraphviz(string inputDotFile, string outputPngFile)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "dot";
        startInfo.Arguments = $"-Tpng {inputDotFile} -o {outputPngFile}";
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error al generar la imagen Graphviz:");
                Console.WriteLine(error);
            }
            else
            {
                Console.WriteLine("Imagen Graphviz generada exitosamente.");
            }
        }
    }
}