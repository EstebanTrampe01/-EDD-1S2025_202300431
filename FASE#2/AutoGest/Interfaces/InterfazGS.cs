using Gtk;
using System;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGS : Window
    {
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolB arbolFacturas;
        private ArbolBinario arbolServicios; // Añadir una instancia de ArbolBinario

        public InterfazGS(ArbolAVL arbolRepuestos, ListaDoblementeEnlazada listaVehiculos, ArbolB arbolFacturas, ArbolBinario arbolServicios) : base("Ingreso de Servicios")
        {
            this.arbolRepuestos = arbolRepuestos;
            this.listaVehiculos = listaVehiculos;
            this.arbolFacturas = arbolFacturas;
            this.arbolServicios = arbolServicios; // Usar la instancia de ArbolBinario pasada como parámetro

            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);

            // Crear un contenedor para los elementos con márgenes
            VBox vbox = new VBox();
            vbox.BorderWidth = 20; // Margen alrededor del contenedor

            // Crear etiquetas y campos de entrada para cada entidad
            HBox hboxID = new HBox();
            Label labelID = new Label("ID:");
            Entry entryID = new Entry();
            hboxID.PackStart(labelID, false, false, 10);
            hboxID.PackStart(entryID, true, true, 10);
            vbox.PackStart(hboxID, false, false, 10);

            HBox hboxIdRepuesto = new HBox();
            Label labelIdRepuesto = new Label("Id_Repuesto:");
            Entry entryIdRepuesto = new Entry();
            hboxIdRepuesto.PackStart(labelIdRepuesto, false, false, 10);
            hboxIdRepuesto.PackStart(entryIdRepuesto, true, true, 10);
            vbox.PackStart(hboxIdRepuesto, false, false, 10);

            HBox hboxIdVehiculo = new HBox();
            Label labelIdVehiculo = new Label("Id_Vehiculo:");
            Entry entryIdVehiculo = new Entry();
            hboxIdVehiculo.PackStart(labelIdVehiculo, false, false, 10);
            hboxIdVehiculo.PackStart(entryIdVehiculo, true, true, 10);
            vbox.PackStart(hboxIdVehiculo, false, false, 10);

            HBox hboxDetalles = new HBox();
            Label labelDetalles = new Label("Detalles:");
            Entry entryDetalles = new Entry();
            hboxDetalles.PackStart(labelDetalles, false, false, 10);
            hboxDetalles.PackStart(entryDetalles, true, true, 10);
            vbox.PackStart(hboxDetalles, false, false, 10);

            HBox hboxCosto = new HBox();
            Label labelCosto = new Label("Costo:");
            Entry entryCosto = new Entry();
            hboxCosto.PackStart(labelCosto, false, false, 10);
            hboxCosto.PackStart(entryCosto, true, true, 10);
            vbox.PackStart(hboxCosto, false, false, 10);

            // Crear el botón de guardar
            Button buttonGuardar = new Button("Guardar");
            buttonGuardar.Clicked += delegate {
                int idRepuesto, idVehiculo;
                double costoServicio;
                if (int.TryParse(entryIdRepuesto.Text, out idRepuesto) && int.TryParse(entryIdVehiculo.Text, out idVehiculo) && double.TryParse(entryCosto.Text, out costoServicio))
                {
                    LRepuesto* repuesto = arbolRepuestos.Buscar(idRepuesto);
                    Vehiculo* vehiculo = listaVehiculos.Buscar(idVehiculo);

                    if (repuesto != null && vehiculo != null)
                    {
                        Console.WriteLine("Datos guardados:");
                        Console.WriteLine("ID: " + entryID.Text);
                        Console.WriteLine("Id_Repuesto: " + entryIdRepuesto.Text);
                        Console.WriteLine("Id_Vehiculo: " + entryIdVehiculo.Text);
                        Console.WriteLine("Detalles: " + entryDetalles.Text);
                        Console.WriteLine("Costo: " + entryCosto.Text);

                        // Crear y guardar el servicio
                        Servicio servicio = new Servicio(int.Parse(entryID.Text), idRepuesto, idVehiculo, entryDetalles.Text, costoServicio);
                        // Aquí puedes agregar la lógica para guardar el servicio

                        // Encolar el servicio
                        arbolServicios.Encolar(servicio);

                        // Crear y guardar la factura
                        double total = costoServicio + repuesto->Costo; // Asumiendo que el costo del vehículo no se suma
                        Factura factura = new Factura(servicio.ID, servicio.ID, total);
                        arbolFacturas.Push(factura);

                        Console.WriteLine("Factura generada:");
                        Console.WriteLine(factura.ToString());
                    }
                    else
                    {
                        if (repuesto == null)
                        {
                            Console.WriteLine("Repuesto no encontrado.");
                            MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Repuesto no encontrado.");
                            md.Run();
                            md.Destroy();
                        }
                        if (vehiculo == null)
                        {
                            Console.WriteLine("Vehículo no encontrado.");
                            MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Vehículo no encontrado.");
                            md.Run();
                            md.Destroy();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ID de repuesto, vehículo o costo inválido.");
                    MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "ID de repuesto, vehículo o costo inválido.");
                    md.Run();
                    md.Destroy();
                }
            };
            vbox.PackStart(buttonGuardar, false, false, 10);

            Add(vbox);
            ShowAll();
        }
    }
}