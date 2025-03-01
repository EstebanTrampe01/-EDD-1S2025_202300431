using Gtk;
using System;
using AutoGest.Interfaces;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;

public class InterfazMenu : Window
{
    private ListaSimple<Usuario> listaUsuarios;
    private ListaCircular listaRepuestos;
    private ListaDoblementeEnlazada listaVehiculos;

    private Pila pilaFacturas;


    public InterfazMenu() : base("Menu")
    {
        SetDefaultSize(300, 250);
        SetPosition(WindowPosition.Center);

        // Inicializar las listas
        listaUsuarios = new ListaSimple<Usuario>();
        listaRepuestos = new ListaCircular();
        listaVehiculos = new ListaDoblementeEnlazada();
        pilaFacturas = new Pila();

        // Crear un contenedor para los elementos
        VBox vbox = new VBox();

        // Crear los botones con margen
        Button button1 = new Button("Cargas Masivas");
        button1.Clicked += delegate { 
            Console.WriteLine("Opción 1 seleccionada"); 
            InterfazCM interfazCM = new InterfazCM(listaUsuarios, listaVehiculos, listaRepuestos);
            interfazCM.ShowAll();
        };
        vbox.PackStart(button1, true, true, 10); // Margen de 10

        Button button2 = new Button("Ingreso Individual");
        button2.Clicked += delegate { 
            Console.WriteLine("Opción 2 seleccionada"); 
            InterfazII interfazII = new InterfazII(listaUsuarios, listaVehiculos, listaRepuestos);
            interfazII.ShowAll();
        };
        vbox.PackStart(button2, true, true, 10); // Margen de 10

        Button button3 = new Button("Gestión de Usuarios");
        button3.Clicked += delegate { 
            Console.WriteLine("Opción 3 seleccionada"); 
            InterfazGU interfazGU = new InterfazGU(listaUsuarios);
            interfazGU.ShowAll();
        };
        vbox.PackStart(button3, true, true, 10); // Margen de 10

        Button button4 = new Button("Generar Servicios");
        button4.Clicked += delegate { 
            Console.WriteLine("Opción 4 seleccionada"); 
            InterfazGS interfazGS = new InterfazGS(listaRepuestos, listaVehiculos, pilaFacturas);
            interfazGS.ShowAll();
        };
        vbox.PackStart(button4, true, true, 10); // Margen de 10

        Button button5 = new Button("Cancelar Factura");
        button5.Clicked += delegate { 
            Console.WriteLine("Opción 5 seleccionada"); 
            InterfazCF interfazCF = new InterfazCF(pilaFacturas);
            interfazCF.ShowAll();
        };
        vbox.PackStart(button5, true, true, 10); // Margen de 10

        Add(vbox);
    }
}