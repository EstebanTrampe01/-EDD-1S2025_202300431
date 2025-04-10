using Gtk;
using System;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using Usuarios;
using AutoGest.Interfaces;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGE : Window
    {
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;

        private ListaSimple<Usuario> listaUsuarios;

        public InterfazGE(ListaSimple<Usuario>  listaUsuarios, ArbolAVL arbolRepuestos, ListaDoblementeEnlazada listaVehiculos) : base("Gestion de Entidades")
        {

        SetDefaultSize(300, 250);
        SetPosition(WindowPosition.Center);

        // Inicializar las listas
        this.listaUsuarios = listaUsuarios;
        this.arbolRepuestos = arbolRepuestos;
        this.listaVehiculos = listaVehiculos;

        // Crear un contenedor para los elementos
        VBox vbox = new VBox();

        // Crear los botones con margen
        Button button1 = new Button("Getinar Usuarios");
        button1.Clicked += delegate { 
            Console.WriteLine("Opción 1 seleccionada"); 
            InterfazGU interfazGU = new InterfazGU(listaUsuarios, listaVehiculos);
            interfazGU.ShowAll();
        };
        vbox.PackStart(button1, true, true, 10); // Margen de 10

        Button button2 = new Button("Gestionar Vehiculos");
        button2.Clicked += delegate { 
            Console.WriteLine("Opción 2 seleccionada"); 
            InterfazGV interfazGV = new InterfazGV(listaVehiculos);
            interfazGV.ShowAll();
        };
        vbox.PackStart(button2, true, true, 10); // Margen de 10
        

        Button button3 = new Button("Gestión Repuestos");
        button3.Clicked += delegate { 
            Console.WriteLine("Opción 3 seleccionada"); 
            InterfazGR InterfazGR = new InterfazGR(arbolRepuestos);
            InterfazGR.ShowAll();
        };
        vbox.PackStart(button3, true, true, 10); // Margen de 10
        
        Add(vbox);
    }

    }
}