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
    public static ListaSimple<Usuario> listaUsuarios;
    public static ArbolAVL arbolRepuestos;
    public static ListaDoblementeEnlazada listaVehiculos;
    public static ArbolB arbolFacturas;
    public static ArbolBinario arbolServicios;

    public static void Main(string[] args)
    {
        Application.Init();

        // Inicializar las estructuras de datos
        listaUsuarios = new ListaSimple<Usuario>();
        arbolRepuestos = new ArbolAVL();
        listaVehiculos = new ListaDoblementeEnlazada();
        arbolFacturas = new ArbolB();
        arbolServicios = new ArbolBinario();

        InterfazMain interfazMain = new InterfazMain();
        interfazMain.ShowAll();

        Application.Run();
    }
}