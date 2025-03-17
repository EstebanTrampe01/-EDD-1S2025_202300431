using Gtk;
using System;
using Facturas;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazCF : Window
    {
        public InterfazCF(Pila pilaFacturas) : base("Última Factura Generada")
        {
            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);

            // Crear un contenedor para los elementos con márgenes
            VBox vbox = new VBox();
            vbox.BorderWidth = 20; // Margen alrededor del contenedor

            // Crear etiquetas y campos de entrada para cada entidad
            HBox hboxID = new HBox();
            Label labelID = new Label("ID:");
            Label labelID_data = new Label();
            hboxID.PackStart(labelID, false, false, 10);
            hboxID.PackStart(labelID_data, true, true, 10);
            vbox.PackStart(hboxID, false, false, 10);

            HBox hboxIdOrden = new HBox();
            Label labelIdOrden = new Label("Id_Orden:");
            Label labelIdOrden_data = new Label();
            hboxIdOrden.PackStart(labelIdOrden, false, false, 10);
            hboxIdOrden.PackStart(labelIdOrden_data, true, true, 10);
            vbox.PackStart(hboxIdOrden, false, false, 10);

            HBox hboxTotal = new HBox();
            Label labelTotal = new Label("Total:");
            Label labelTotal_data = new Label();
            hboxTotal.PackStart(labelTotal, false, false, 10);
            hboxTotal.PackStart(labelTotal_data, true, true, 10);
            vbox.PackStart(hboxTotal, false, false, 10);

            // Obtener la última factura generada
            Factura ultimaFactura = pilaFacturas.Pop();
            if (ultimaFactura != null)
            {
                labelID_data.Text = ultimaFactura.ID.ToString();
                labelIdOrden_data.Text = ultimaFactura.ID_Orden.ToString();
                labelTotal_data.Text = ultimaFactura.Total.ToString();
            }
            else
            {
                labelID_data.Text = "Aún no se registra ninguna factura.";
                labelIdOrden_data.Text = "-";
                labelTotal_data.Text = "-";
            }

            // Crear el botón de guardar
            Button buttonGuardar = new Button("Guardar");
            buttonGuardar.Clicked += delegate {
                Console.WriteLine("Datos guardados:");
            };
            vbox.PackStart(buttonGuardar, false, false, 10);

            Add(vbox);
            ShowAll();
        }
    }
}