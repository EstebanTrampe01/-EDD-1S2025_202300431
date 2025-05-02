using Gtk;
using System;
using System.Collections.Generic;
using Facturas;
using Vehiculos;
using Servicios;
using System.Text;
using System.Linq;
using AutoGest.Utils;

namespace AutoGest.InterfacesUsuario
{
    public unsafe class InterfazUCF : Window
    {
        private int idUsuario;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolM arbolFacturas;
        private ArbolBinario arbolServicios;
        
        private Entry entryIdFactura;
        private TextView textViewDetalleFactura;
        private Button buttonBuscar;
        private Button buttonPagar;
        private List<Factura> facturasUsuario;
        private Label statusLabel;

        public InterfazUCF(int idUsuario, ListaDoblementeEnlazada listaVehiculos, ArbolM arbolFacturas, ArbolBinario arbolServicios) 
            : base("Cancelación de Facturas")
        {
            this.idUsuario = idUsuario;
            this.listaVehiculos = listaVehiculos;
            this.arbolFacturas = arbolFacturas;
            this.arbolServicios = arbolServicios;
            this.facturasUsuario = ObtenerFacturasDelUsuario();

            SetDefaultSize(600, 450);
            SetPosition(WindowPosition.Center);

            // Contenedor principal
            VBox mainVBox = new VBox(false, 10);
            mainVBox.BorderWidth = 15;

            // Título
            Label labelTitulo = new Label("Cancelación de Facturas");
            labelTitulo.ModifyFont(Pango.FontDescription.FromString("Sans Bold 14"));
            mainVBox.PackStart(labelTitulo, false, false, 10);

            // Frame para la selección de factura
            Frame frameSeleccion = new Frame("Seleccionar Factura");
            VBox vboxSeleccion = new VBox(false, 8);
            vboxSeleccion.BorderWidth = 10;

            // Sección para ingresar ID de factura
            HBox hboxIdFactura = new HBox(false, 5);
            Label labelIdFactura = new Label("ID Factura:");
            entryIdFactura = new Entry();
            hboxIdFactura.PackStart(labelIdFactura, false, false, 5);
            hboxIdFactura.PackStart(entryIdFactura, true, true, 5);
            
            // Botón de búsqueda
            buttonBuscar = new Button("Buscar");
            buttonBuscar.Clicked += OnBuscarFacturaClicked;
            hboxIdFactura.PackStart(buttonBuscar, false, false, 5);
            
            vboxSeleccion.PackStart(hboxIdFactura, false, false, 0);

            // Lista de facturas disponibles del usuario
            Frame frameFacturasDisponibles = new Frame("Facturas Disponibles");
            VBox vboxFacturas = new VBox(false, 5);
            vboxFacturas.BorderWidth = 10;

            string infoFacturas = ObtenerInfoFacturasDisponibles();
            Label labelFacturas = new Label(infoFacturas);
            vboxFacturas.PackStart(labelFacturas, false, false, 0);

            frameFacturasDisponibles.Add(vboxFacturas);
            vboxSeleccion.PackStart(frameFacturasDisponibles, false, false, 10);

            frameSeleccion.Add(vboxSeleccion);
            mainVBox.PackStart(frameSeleccion, false, false, 0);

            // Frame para mostrar detalles de la factura
            Frame frameDetalles = new Frame("Detalles de la Factura");
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            textViewDetalleFactura = new TextView();
            textViewDetalleFactura.Editable = false;
            textViewDetalleFactura.WrapMode = WrapMode.Word;
            textViewDetalleFactura.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));
            textViewDetalleFactura.Buffer.Text = "Seleccione una factura para ver los detalles.";

            scrolledWindow.Add(textViewDetalleFactura);
            frameDetalles.Add(scrolledWindow);
            mainVBox.PackStart(frameDetalles, true, true, 0);

            // Botón para pagar la factura
            buttonPagar = new Button("Pagar Factura");
            buttonPagar.Sensitive = false; // Inicialmente desactivado
            buttonPagar.Clicked += OnPagarFacturaClicked;
            mainVBox.PackStart(buttonPagar, false, false, 10);

            // Etiqueta para mensajes de estado
            statusLabel = MessageManager.CreateStatusLabel();
            mainVBox.PackStart(statusLabel, false, false, 10);

            Add(mainVBox);
            ShowAll();
        }

        private List<Factura> ObtenerFacturasDelUsuario()
        {
            List<Factura> todasLasFacturas = arbolFacturas.RecorridoInOrden();
            List<Factura> facturasDelUsuario = new List<Factura>();

            // Obtener todos los vehículos del usuario
            List<Vehiculo> vehiculosUsuario = new List<Vehiculo>();
            Nodo<Vehiculo>* temp = listaVehiculos.ObtenerPrimerNodo();
            
            while (temp != null)
            {
                if (temp->Data.ID_Usuario == idUsuario)
                {
                    vehiculosUsuario.Add(temp->Data);
                }
                temp = temp->Next;
            }

            // Obtener los servicios de esos vehículos
            List<int> idsServiciosUsuario = new List<int>();
            foreach (var vehiculo in vehiculosUsuario)
            {
                List<Servicio> serviciosVehiculo = arbolServicios.ObtenerServiciosPorVehiculoInOrden(vehiculo.Id);
                foreach (var servicio in serviciosVehiculo)
                {
                    idsServiciosUsuario.Add(servicio.ID);
                }
            }

            // Filtrar facturas por los IDs de servicios del usuario
            foreach (var factura in todasLasFacturas)
            {
                if (idsServiciosUsuario.Contains(factura.ID_Orden))
                {
                    facturasDelUsuario.Add(factura);
                }
            }

            return facturasDelUsuario;
        }

        private string ObtenerInfoFacturasDisponibles()
        {
            if (facturasUsuario.Count == 0)
            {
                return "No tiene facturas pendientes.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ID Factura | ID Servicio | Total | Método Pago");
            sb.AppendLine("---------------------------------------");
            
            foreach (var factura in facturasUsuario)
            {
                sb.AppendLine($"{factura.ID,-9} | {factura.ID_Orden,-11} | Q{factura.Total:F2} | {factura.MetodoPago}");
            }

            return sb.ToString();
        }

        private unsafe string GetFixedString(char* fixedStr)
        {
            string str = "";
            for (int i = 0; fixedStr[i] != '\0'; i++)
            {
                str += fixedStr[i];
            }
            return str;
        }

        private void OnBuscarFacturaClicked(object sender, EventArgs e)
        {
            string idFacturaStr = entryIdFactura.Text.Trim();
            
            if (string.IsNullOrEmpty(idFacturaStr))
            {
                MessageManager.ShowMessage(statusLabel, "Por favor, ingrese el ID de la factura.", MessageManager.MessageType.Warning);
                return;
            }

            if (!int.TryParse(idFacturaStr, out int idFactura))
            {
                MessageManager.ShowMessage(statusLabel, "El ID de la factura debe ser un número entero.", MessageManager.MessageType.Error);
                return;
            }

            // Buscar la factura
            Factura facturaSeleccionada = facturasUsuario.FirstOrDefault(f => f.ID == idFactura);

            if (facturaSeleccionada == null)
            {
                MessageManager.ShowMessage(statusLabel, $"No se encontró ninguna factura con ID {idFactura} asociada a sus servicios.", MessageManager.MessageType.Warning);
                textViewDetalleFactura.Buffer.Text = "Factura no encontrada.";
                buttonPagar.Sensitive = false;
                return;
            }

            // Obtener detalles del servicio asociado
            Servicio servicio = null;
            Vehiculo* vehiculo = null;
            string marcaVehiculo = "";
            string placaVehiculo = "";
            int modeloVehiculo = 0;

            List<Servicio> todosLosServicios = arbolServicios.RecorridoInOrden();
            servicio = todosLosServicios.FirstOrDefault(s => s.ID == facturaSeleccionada.ID_Orden);

            if (servicio != null)
            {
                vehiculo = listaVehiculos.Buscar(servicio.Id_Vehiculo);
                if (vehiculo != null)
                {
                    marcaVehiculo = GetFixedString(vehiculo->Marca);
                    placaVehiculo = GetFixedString(vehiculo->Placa);
                    modeloVehiculo = vehiculo->Modelo;
                }
            }

            // Mostrar detalles de la factura
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== DETALLES DE LA FACTURA ===");
            sb.AppendLine($"ID Factura: {facturaSeleccionada.ID}");
            sb.AppendLine($"ID Servicio: {facturaSeleccionada.ID_Orden}");
            sb.AppendLine($"Total a pagar: Q{facturaSeleccionada.Total:F2}");
            sb.AppendLine($"Método de pago: {facturaSeleccionada.MetodoPago}");
            
            if (servicio != null)
            {
                sb.AppendLine("\n=== DETALLES DEL SERVICIO ===");
                sb.AppendLine($"Descripción: {servicio.Detalles}");
                sb.AppendLine($"ID: {servicio.ID}");
                sb.AppendLine($"Costo: Q{servicio.Costo:F2}");
                sb.AppendLine($"Método de pago: {servicio.MetodoPago}");
            }

            if (vehiculo != null)
            {
                sb.AppendLine("\n=== DETALLES DEL VEHÍCULO ===");
                sb.AppendLine($"Marca: {marcaVehiculo}");
                sb.AppendLine($"Modelo: {modeloVehiculo}");
                sb.AppendLine($"Placa: {placaVehiculo}");
            }

            textViewDetalleFactura.Buffer.Text = sb.ToString();
            buttonPagar.Sensitive = true;
            buttonPagar.Label = $"Pagar Factura #{facturaSeleccionada.ID} (Q{facturaSeleccionada.Total:F2})";
            
            MessageManager.ShowMessage(statusLabel, $"Factura #{idFactura} encontrada correctamente", MessageManager.MessageType.Success, true);
        }

        private void OnPagarFacturaClicked(object sender, EventArgs e)
        {
            string idFacturaStr = entryIdFactura.Text.Trim();
            
            if (string.IsNullOrEmpty(idFacturaStr) || !int.TryParse(idFacturaStr, out int idFactura))
            {
                MessageManager.ShowMessage(statusLabel, "ID de factura inválido.", MessageManager.MessageType.Error);
                return;
            }

            // Confirmar pago
            MessageManager.AskConfirmation(
                (Box)this.Child, // Asumiendo que es una Box
                $"¿Está seguro que desea pagar la factura #{idFactura}?",
                () => {
                    try
                    {
                        // Eliminar la factura del árbol
                        arbolFacturas.Eliminar(idFactura);
                        
                        MessageManager.ShowMessage(statusLabel, $"¡Factura #{idFactura} pagada correctamente!", MessageManager.MessageType.Success);
                        
                        // Actualizar la lista de facturas disponibles
                        facturasUsuario = ObtenerFacturasDelUsuario();
                        string infoFacturas = ObtenerInfoFacturasDisponibles();
                        
                        // Limpiar campos
                        entryIdFactura.Text = "";
                        textViewDetalleFactura.Buffer.Text = "Factura pagada correctamente. Seleccione otra factura para ver detalles.";
                        buttonPagar.Sensitive = false;
                        buttonPagar.Label = "Pagar Factura";
                        
                        // Actualizar lista de facturas disponibles
                        Frame frameParent = (Frame)((VBox)((Frame)((VBox)buttonBuscar.Parent.Parent).Parent).Parent).Parent;
                        VBox vboxFacturas = (VBox)((Frame)frameParent.Child).Child;
                        
                        // Limpiar y actualizar el widget de facturas disponibles
                        foreach (Widget w in vboxFacturas.Children)
                        {
                            vboxFacturas.Remove(w);
                        }
                        
                        Label labelFacturas = new Label(infoFacturas);
                        vboxFacturas.PackStart(labelFacturas, false, false, 0);
                        vboxFacturas.ShowAll();
                    }
                    catch (Exception ex)
                    {
                        MessageManager.ShowMessage(statusLabel, $"Error: {ex.Message}", MessageManager.MessageType.Error);
                        Console.WriteLine($"Error al pagar factura: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            );
        }
    }
}