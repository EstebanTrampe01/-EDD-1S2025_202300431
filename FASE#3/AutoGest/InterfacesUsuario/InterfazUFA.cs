using Gtk;
using System;
using System.Collections.Generic;
using Facturas;
using Vehiculos;
using Servicios;
using System.Text;
using System.Linq;

namespace AutoGest.InterfacesUsuario
{
    public unsafe class InterfazUFA : Window
    {
        private int idUsuario;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolB arbolFacturas;
        private ArbolBinario arbolServicios;
        private TextView textViewFacturas;
        private ComboBox comboServicios;
        private ListStore listStoreServicios;
        private CheckButton checkAllServices;
        private RadioButton radioInOrder;
        private RadioButton radioPreOrder;
        private RadioButton radioPostOrder;

        public InterfazUFA(int idUsuario, ListaDoblementeEnlazada listaVehiculos, ArbolB arbolFacturas, ArbolBinario arbolServicios) 
            : base("Facturas Activas")
        {
            this.idUsuario = idUsuario;
            this.listaVehiculos = listaVehiculos;
            this.arbolFacturas = arbolFacturas;
            this.arbolServicios = arbolServicios;

            SetDefaultSize(700, 500);
            SetPosition(WindowPosition.Center);

            // Contenedor principal
            VBox mainVBox = new VBox(false, 10);
            mainVBox.BorderWidth = 15;

            // Título
            Label labelTitulo = new Label("Facturas Pendientes de Pago");
            labelTitulo.ModifyFont(Pango.FontDescription.FromString("Sans Bold 14"));
            mainVBox.PackStart(labelTitulo, false, false, 10);

            // Sección de selección de servicio
            Frame frameSeleccion = new Frame("Selección de Servicio");
            VBox vboxSeleccion = new VBox(false, 5);
            vboxSeleccion.BorderWidth = 10;

            // ComboBox para seleccionar servicio
            HBox hboxServicio = new HBox(false, 5);
            hboxServicio.PackStart(new Label("Servicio:"), false, false, 5);

            // Modelo para el ComboBox: ID, ID_Vehiculo, Detalles, Costo
            listStoreServicios = new ListStore(typeof(int), typeof(int), typeof(string), typeof(double));
            comboServicios = new ComboBox(listStoreServicios);

            // Configurar las columnas del ComboBox
            CellRendererText rendererText = new CellRendererText();
            comboServicios.PackStart(rendererText, true);
            comboServicios.AddAttribute(rendererText, "text", 0); // Mostrar ID del servicio

            // Opciones de visualización (RadioButtons para tipos de recorrido)
            HBox hboxOpciones = new HBox(true, 10);
            radioInOrder = new RadioButton("In-Orden");
            radioPreOrder = new RadioButton(radioInOrder, "Pre-Orden");
            radioPostOrder = new RadioButton(radioInOrder, "Post-Orden");
            radioInOrder.Active = true; // Selección por defecto

            hboxOpciones.PackStart(radioInOrder, false, false, 5);
            hboxOpciones.PackStart(radioPreOrder, false, false, 5);
            hboxOpciones.PackStart(radioPostOrder, false, false, 5);

            vboxSeleccion.PackStart(hboxOpciones, false, false, 5);

            // Llenar el ComboBox con los servicios del usuario
            CargarServiciosUsuario();

            hboxServicio.PackStart(comboServicios, true, true, 5);
            vboxSeleccion.PackStart(hboxServicio, false, false, 5);

            // CheckButton para mostrar todos los servicios
            checkAllServices = new CheckButton("Mostrar facturas de todos mis servicios");
            checkAllServices.Toggled += OnCheckAllServicesToggled;
            vboxSeleccion.PackStart(checkAllServices, false, false, 5);

            // Botón para mostrar las facturas
            Button buttonMostrar = new Button("Mostrar Facturas");
            buttonMostrar.Clicked += OnMostrarFacturasClicked;
            vboxSeleccion.PackStart(buttonMostrar, false, false, 10);

            frameSeleccion.Add(vboxSeleccion);
            mainVBox.PackStart(frameSeleccion, false, false, 0);

            // Área de visualización de facturas
            Frame frameFacturas = new Frame("Lista de Facturas");
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            textViewFacturas = new TextView();
            textViewFacturas.Editable = false;
            textViewFacturas.WrapMode = WrapMode.Word;
            textViewFacturas.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));

            scrolledWindow.Add(textViewFacturas);
            frameFacturas.Add(scrolledWindow);
            mainVBox.PackStart(frameFacturas, true, true, 0);

            Add(mainVBox);
            ShowAll();
        }

        private void OnCheckAllServicesToggled(object sender, EventArgs e)
        {
            comboServicios.Sensitive = !checkAllServices.Active;
        }

        private void CargarServiciosUsuario()
        {
            listStoreServicios.Clear();

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

            if (vehiculosUsuario.Count == 0)
            {
                listStoreServicios.AppendValues(-1, -1, "No tiene vehículos registrados", 0.0);
                comboServicios.Active = 0;
                return;
            }

            // Lista para almacenar todos los servicios del usuario
            List<Servicio> serviciosUsuario = new List<Servicio>();

            // Obtener todos los servicios de los vehículos del usuario
            foreach (var vehiculo in vehiculosUsuario)
            {
                List<Servicio> serviciosVehiculo;
                
                // Usar el tipo de recorrido seleccionado
                if (radioInOrder.Active)
                {
                    serviciosVehiculo = arbolServicios.ObtenerServiciosPorVehiculoInOrden(vehiculo.Id);
                }
                else if (radioPreOrder.Active)
                {
                    serviciosVehiculo = arbolServicios.ObtenerServiciosPorVehiculoPreOrden(vehiculo.Id);
                }
                else
                {
                    serviciosVehiculo = arbolServicios.ObtenerServiciosPorVehiculoPostOrden(vehiculo.Id);
                }
                
                serviciosUsuario.AddRange(serviciosVehiculo);
            }

            if (serviciosUsuario.Count == 0)
            {
                listStoreServicios.AppendValues(-1, -1, "No tiene servicios registrados", 0.0);
                comboServicios.Active = 0;
                return;
            }

            foreach (var servicio in serviciosUsuario)
            {
                string detalles = servicio.Detalles;
                if (detalles.Length > 30)
                    detalles = detalles.Substring(0, 27) + "...";
                
                listStoreServicios.AppendValues(
                    servicio.ID,
                    servicio.Id_Vehiculo, 
                    detalles,
                    servicio.Costo
                );
            }

            comboServicios.Active = 0;
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

        private void OnMostrarFacturasClicked(object sender, EventArgs e)
        {
            // Actualizar la lista de servicios basado en el tipo de recorrido seleccionado
            CargarServiciosUsuario();
            
            if (checkAllServices.Active)
            {
                MostrarFacturasTodosServicios();
                return;
            }

            if (comboServicios.Active < 0)
            {
                textViewFacturas.Buffer.Text = "Por favor, seleccione un servicio.";
                return;
            }

            TreeIter iter;
            if (comboServicios.GetActiveIter(out iter))
            {
                int idServicio = (int)listStoreServicios.GetValue(iter, 0);
                if (idServicio == -1)
                {
                    textViewFacturas.Buffer.Text = "No tiene servicios registrados para mostrar facturas.";
                    return;
                }

                int idVehiculo = (int)listStoreServicios.GetValue(iter, 1);
                string detallesServicio = (string)listStoreServicios.GetValue(iter, 2);
                double costoServicio = (double)listStoreServicios.GetValue(iter, 3);

                // Obtener la factura relacionada con este servicio
                Factura factura = ObtenerFacturaPorServicio(idServicio);

                if (factura != null)
                {
                    // Obtener información del vehículo
                    Vehiculo* vehiculo = listaVehiculos.Buscar(idVehiculo);
                    string marca = vehiculo != null ? GetFixedString(vehiculo->Marca) : "Desconocido";
                    int modelo = vehiculo != null ? vehiculo->Modelo : 0;
                    string placa = vehiculo != null ? GetFixedString(vehiculo->Placa) : "Desconocido";

                    // Mostrar la factura
                    MostrarFacturaTabla(factura, idServicio, idVehiculo, marca, modelo, placa, detallesServicio);
                }
                else
                {
                    textViewFacturas.Buffer.Text = $"No se encontró una factura para el servicio con ID {idServicio}.";
                }
            }
        }

        private void MostrarFacturasTodosServicios()
        {
            string tipoRecorrido = radioInOrder.Active ? "In-Orden" : (radioPreOrder.Active ? "Pre-Orden" : "Post-Orden");
            
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

            if (vehiculosUsuario.Count == 0)
            {
                textViewFacturas.Buffer.Text = "No tiene vehículos registrados.";
                return;
            }

            // Lista para almacenar todas las facturas con información adicional
            List<FacturaConInfoCompleta> todasLasFacturas = new List<FacturaConInfoCompleta>();

            // Para cada vehículo, obtener los servicios y sus facturas relacionadas
            foreach (var vehiculo in vehiculosUsuario)
            {
                // Obtener servicios del vehículo según el tipo de recorrido seleccionado
                List<Servicio> servicios;
                if (radioInOrder.Active)
                {
                    servicios = arbolServicios.ObtenerServiciosPorVehiculoInOrden(vehiculo.Id);
                }
                else if (radioPreOrder.Active)
                {
                    servicios = arbolServicios.ObtenerServiciosPorVehiculoPreOrden(vehiculo.Id);
                }
                else
                {
                    servicios = arbolServicios.ObtenerServiciosPorVehiculoPostOrden(vehiculo.Id);
                }
                
                // Obtener facturas relacionadas con esos servicios
                foreach (var servicio in servicios)
                {
                    Factura factura = ObtenerFacturaPorServicio(servicio.ID);
                    
                    if (factura != null)
                    {
                        todasLasFacturas.Add(new FacturaConInfoCompleta
                        {
                            Factura = factura,
                            ServicioId = servicio.ID,
                            IdVehiculo = vehiculo.Id,
                            MarcaVehiculo = GetFixedString(vehiculo.Marca),
                            ModeloVehiculo = vehiculo.Modelo,
                            PlacaVehiculo = GetFixedString(vehiculo.Placa),
                            DetallesServicio = servicio.Detalles
                        });
                    }
                }
            }

            // Mostrar todas las facturas en una tabla
            MostrarFacturasTodosFormatoTabla(todasLasFacturas, tipoRecorrido);
        }

        // Método para obtener la factura relacionada con un servicio dado
        private Factura ObtenerFacturaPorServicio(int idServicio)
        {
            // Obtener todas las facturas del árbol B
            List<Factura> todasLasFacturas = arbolFacturas.RecorridoInOrden();
            
            // Buscar la factura que corresponde al servicio
            return todasLasFacturas.FirstOrDefault(f => f.ID_Orden == idServicio);
        }

        private void MostrarFacturaTabla(Factura factura, int idServicio, int idVehiculo, string marca, int modelo, string placa, string detallesServicio)
        {
            StringBuilder sb = new StringBuilder();
            
            // Encabezado
            sb.AppendLine($"Factura del Servicio ID: {idServicio}");
            sb.AppendLine($"Vehículo: {marca} - {modelo} - {placa} (ID: {idVehiculo})");
            sb.AppendLine("==============================================================");
            
            // Detalles de la factura
            sb.AppendLine($"ID Factura: {factura.ID}");
            sb.AppendLine($"Detalles del servicio: {detallesServicio}");
            sb.AppendLine($"Total a pagar: Q{factura.Total:F2}");
            
            textViewFacturas.Buffer.Text = sb.ToString();
        }

        private void MostrarFacturasTodosFormatoTabla(List<FacturaConInfoCompleta> facturas, string tipoRecorrido)
        {
            StringBuilder sb = new StringBuilder();
            
            // Encabezado
            sb.AppendLine($"LISTA COMPLETA DE FACTURAS PENDIENTES");
            sb.AppendLine($"Tipo de recorrido: {tipoRecorrido}");
            sb.AppendLine("==================================================================================");
            
            if (facturas.Count == 0)
            {
                sb.AppendLine("No hay facturas pendientes para sus servicios.");
            }
            else
            {
                // Encabezado de la tabla
                sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-12} | {3,-25} | {4,-10}", 
                              "ID", "Servicio", "Vehículo", "Detalles", "Total (Q)"));
                sb.AppendLine("------+----------+--------------+---------------------------+------------");
                
                double montoTotal = 0;
                
                // Filas de la tabla
                foreach (var item in facturas)
                {
                    var factura = item.Factura;
                    string infoVehiculo = $"{item.MarcaVehiculo}-{item.PlacaVehiculo}";
                    if (infoVehiculo.Length > 12)
                        infoVehiculo = infoVehiculo.Substring(0, 9) + "...";
                    
                    string detalles = item.DetallesServicio;
                    if (detalles.Length > 25)
                        detalles = detalles.Substring(0, 22) + "...";
                    
                    sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-12} | {3,-25} | {4,10:F2}", 
                                  factura.ID, 
                                  item.ServicioId,
                                  infoVehiculo, 
                                  detalles,
                                  factura.Total));
                    
                    montoTotal += factura.Total;
                }
                
                // Pie de la tabla
                sb.AppendLine("------+----------+--------------+---------------------------+------------");
                sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-12} | {3,-25} | {4,10:F2}", 
                              "", "", "", "TOTAL:", montoTotal));
                
                // Resumen
                sb.AppendLine("\nRESUMEN:");
                sb.AppendLine($"Total de facturas: {facturas.Count}");
                sb.AppendLine($"Monto total a pagar: Q{montoTotal:F2}");
            }
            
            textViewFacturas.Buffer.Text = sb.ToString();
        }
    }

    // Clase auxiliar para mantener la información completa de la factura, servicio y vehículo
    public class FacturaConInfoCompleta
    {
        public Factura Factura { get; set; }
        public int ServicioId { get; set; }
        public int IdVehiculo { get; set; }
        public string MarcaVehiculo { get; set; }
        public int ModeloVehiculo { get; set; }
        public string PlacaVehiculo { get; set; }
        public string DetallesServicio { get; set; }
    }
}