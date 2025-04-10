using Gtk;
using System;
using AutoGest.Interfaces;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using System.Collections.Generic;

public class InterfazUser : Window
{
    private int idUsuario;
    private ListaSimple<Usuario> listaUsuarios;
    private ListaDoblementeEnlazada listaVehiculos;
    private ArbolB arbolFacturas;
    private ArbolBinario arbolServicios;
    
    // Lista para mantener referencias a ventanas activas
    private List<Window> activeWindows = new List<Window>();

    public InterfazUser(
        int idUsuario,
        ListaSimple<Usuario> listaUsuarios,
        ListaDoblementeEnlazada listaVehiculos,
        ArbolB arbolFacturas,
        ArbolBinario arbolServicios) : base("Portal de Usuario")
    {
        // Asignar las estructuras de datos recibidas
        this.idUsuario = idUsuario;
        this.listaUsuarios = listaUsuarios;
        this.listaVehiculos = listaVehiculos;
        this.arbolFacturas = arbolFacturas;
        this.arbolServicios = arbolServicios;

        SetDefaultSize(350, 300);
        SetPosition(WindowPosition.Center);

        // Obtener información del usuario actual
        string nombreUsuario = "Usuario";
        unsafe
        {
            Usuario* usuario = listaUsuarios.Buscar(idUsuario);
            if (usuario != null)
            {
                nombreUsuario = GetFixedString(usuario->name);
            }
        }

        // Crear un contenedor para los elementos
        VBox vbox = new VBox();

        // Añadir un mensaje de bienvenida
        Label labelBienvenida = new Label($"Bienvenido, {nombreUsuario}");
        labelBienvenida.ModifyFont(Pango.FontDescription.FromString("Sans Bold 14"));
        vbox.PackStart(labelBienvenida, false, false, 10);

        // Crear los botones con margen
        Button buttonRegistrarVehiculo = new Button("Registrar Vehículo");
        buttonRegistrarVehiculo.Clicked += delegate { 
            Console.WriteLine("Opción: Registrar Vehículo seleccionada"); 
            // Aquí iría la implementación para registrar vehículos
        };
        vbox.PackStart(buttonRegistrarVehiculo, true, true, 10);

        Button buttonMisVehiculos = new Button("Mis Vehículos");
        buttonMisVehiculos.Clicked += delegate { 
            Console.WriteLine("Opción: Mis Vehículos seleccionada"); 
            // Aquí iría la implementación para ver vehículos del usuario
        };
        vbox.PackStart(buttonMisVehiculos, true, true, 10);

        Button buttonVerServicios = new Button("Servicios Activos");
        buttonVerServicios.Clicked += delegate { 
            Console.WriteLine("Opción: Servicios Activos seleccionada"); 
            // Aquí iría la implementación para ver servicios
        };
        vbox.PackStart(buttonVerServicios, true, true, 10);

        Button buttonMisFacturas = new Button("Mis Facturas");
        buttonMisFacturas.Clicked += delegate { 
            Console.WriteLine("Opción: Mis Facturas seleccionada"); 
            // Aquí iría la implementación para ver facturas
        };
        vbox.PackStart(buttonMisFacturas, true, true, 10);

        Button buttonCancelarFactura = new Button("Cancelar Factura");
        buttonCancelarFactura.Clicked += delegate { 
            Console.WriteLine("Opción: Cancelar Factura seleccionada"); 
            // Aquí iría la implementación para cancelar facturas
        };
        vbox.PackStart(buttonCancelarFactura, true, true, 10);

        // Botón para cerrar sesión
        Button buttonCerrarSesion = new Button("Cerrar Sesión");
        buttonCerrarSesion.Clicked += delegate {
            Console.WriteLine("Cerrando sesión de usuario...");
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
        vbox.PackStart(buttonCerrarSesion, true, true, 10);

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
        };
        window.ShowAll();
    }

    // Método para convertir char* a string
    private unsafe string GetFixedString(char* fixedStr)
    {
        string str = "";
        for (int i = 0; fixedStr[i] != '\0'; i++)
        {
            str += fixedStr[i];
        }
        return str;
    }
}