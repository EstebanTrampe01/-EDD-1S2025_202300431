﻿using Gtk;
using System;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using AutoGest;

class Program
{
    // Declaramos las estructuras de datos como variables estáticas
    public static UserBlockchain listaUsuarios;
    public static ArbolAVL arbolRepuestos;
    public static ListaDoblementeEnlazada listaVehiculos;
    public static ArbolB arbolFacturas;
    public static ArbolBinario arbolServicios;

    public static void Main(string[] args)
    {
        Console.WriteLine("Seleccione una opción:");
        Console.WriteLine("1. Ejecutar tarea de grafo Vehículo-Repuesto");
        Console.WriteLine("2. Abrir interfaz gráfica");
        Console.Write("Opción: ");
        string opcion = Console.ReadLine();

        if (opcion == "1")
        {
            GrafoVehiculoRepuesto grafo = new GrafoVehiculoRepuesto();

            Console.WriteLine("Ingrese relaciones Vehículo-Repuesto (ejemplo: V1 R1). Escriba 'fin' para terminar.");
            while (true)
            {
                Console.Write("Relacion (Vehiculo Repuesto): ");
                string linea = Console.ReadLine();
                if (linea.Trim().ToLower() == "fin") break;
                var partes = linea.Split(' ');
                if (partes.Length == 2)
                {
                    grafo.AgregarRelacion(partes[0], partes[1]);
                    Console.WriteLine($"Relacion agregada: {partes[0]} <-> {partes[1]}");
                }
                else
                {
                    Console.WriteLine("Formato incorrecto. Ejemplo válido: V1 R1");
                }
            }

            grafo.GenerarReporteDOT("grafo_vehiculos_repuestos");

            Console.WriteLine("¡Reporte generado! Presione cualquier tecla para salir.");
            Console.ReadKey();
            return;
        }

        // Inicializar las estructuras de datos
        Application.Init();
        listaUsuarios = new UserBlockchain();
        arbolRepuestos = new ArbolAVL();
        listaVehiculos = new ListaDoblementeEnlazada();
        arbolFacturas = new ArbolB();
        arbolServicios = new ArbolBinario();

        InterfazMain interfazMain = new InterfazMain();
        interfazMain.ShowAll();

        Application.Run();
    }
}