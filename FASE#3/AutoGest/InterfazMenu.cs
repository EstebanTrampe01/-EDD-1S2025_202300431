using Gtk;
using System;
using AutoGest.Interfaces;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using System.Collections.Generic;

public class InterfazMenu : Window
{
    private ListaSimple<Usuario> listaUsuarios;
    private ArbolAVL arbolRepuestos;
    private ListaDoblementeEnlazada listaVehiculos;
    private ArbolB arbolFacturas;
    private ArbolBinario arbolServicios;
    
    // Lista para mantener referencias a ventanas activas
    private List<Window> activeWindows = new List<Window>();

    public InterfazMenu(
        ListaSimple<Usuario> listaUsuarios,
        ArbolAVL arbolRepuestos,
        ListaDoblementeEnlazada listaVehiculos,
        ArbolB arbolFacturas,
        ArbolBinario arbolServicios) : base("Menu")
    {
        // Asignar las estructuras de datos recibidas
        this.listaUsuarios = listaUsuarios;
        this.arbolRepuestos = arbolRepuestos;
        this.listaVehiculos = listaVehiculos;
        this.arbolFacturas = arbolFacturas;
        this.arbolServicios = arbolServicios;

        SetDefaultSize(300, 300); // Aumentado para acomodar el botón de cerrar sesión
        SetPosition(WindowPosition.Center);

        // Crear un contenedor para los elementos
        VBox vbox = new VBox();

        // Crear los botones con margen
        Button button1 = new Button("Cargas Masivas");
        button1.Clicked += delegate { 
            Console.WriteLine("Opción 1 seleccionada"); 
            InterfazCM interfazCM = new InterfazCM(listaUsuarios, listaVehiculos, arbolRepuestos);
            ManageActiveWindow(interfazCM);
        };
        vbox.PackStart(button1, true, true, 10); // Margen de 10

        Button button2 = new Button("Ingreso Individual");
        button2.Clicked += delegate { 
            Console.WriteLine("Opción 2 seleccionada"); 
            InterfazII interfazII = new InterfazII(listaUsuarios, listaVehiculos, arbolRepuestos);
            ManageActiveWindow(interfazII);
        };
        vbox.PackStart(button2, true, true, 10); // Margen de 10

        Button button3 = new Button("Gestión de Entidades");
        button3.Clicked += delegate { 
            Console.WriteLine("Opción 3 seleccionada"); 
            InterfazGE interfazGE = new InterfazGE(listaUsuarios, arbolRepuestos, listaVehiculos);
            ManageActiveWindow(interfazGE);
        };
        vbox.PackStart(button3, true, true, 10); // Margen de 10

        Button button4 = new Button("Generar Servicios");
        button4.Clicked += delegate { 
            Console.WriteLine("Opción 4 seleccionada"); 
            InterfazGS interfazGS = new InterfazGS(arbolRepuestos, listaVehiculos, arbolFacturas, arbolServicios);
            ManageActiveWindow(interfazGS);
        };
        vbox.PackStart(button4, true, true, 10); // Margen de 10

        Button button5 = new Button("Cancelar Factura");
        button5.Clicked += delegate { 
            Console.WriteLine("Opción 5 seleccionada"); 
            InterfazCF interfazCF = new InterfazCF(arbolFacturas);
            ManageActiveWindow(interfazCF);
        };
        vbox.PackStart(button5, true, true, 10); // Margen de 10

        // Botón para cerrar sesión
        Button buttonCerrarSesion = new Button("Cerrar Sesión");
        buttonCerrarSesion.Clicked += delegate {
            Console.WriteLine("Cerrando sesión...");
            // Cerrar todas las ventanas activas
            foreach (var window in activeWindows)
            {
                window.Destroy();
            }
            activeWindows.Clear();
            
            // Volver a mostrar la ventana de login manteniendo todas las estructuras de datos
            Login login = new Login();
            login.ShowAll();
            this.Destroy();
        };
        vbox.PackStart(buttonCerrarSesion, true, true, 10); // Margen de 10

        Add(vbox);
        
        // Asegurarse de limpiar recursos al cerrar
        DeleteEvent += (o, args) => {
            // Cerrar todas las ventanas activas
            foreach (var window in activeWindows)
            {
                window.Destroy();
            }
            // Permitir que el evento continue y cierre la ventana
        };
    }
    
    // Método para gestionar ventanas activas y evitar problemas de referencia
    private void ManageActiveWindow(Window window)
    {
        activeWindows.Add(window);
        window.DeleteEvent += (o, args) => {
            activeWindows.Remove(window);
            // Permitir que el evento continue y cierre la ventana
        };
        window.ShowAll();
    }
}