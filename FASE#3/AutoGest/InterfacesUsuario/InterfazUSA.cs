using Gtk;
using System;
using System.Collections.Generic;
using Servicios;
using Vehiculos;
using System.Text;
using System.Linq;

namespace AutoGest.InterfacesUsuario
{
    public unsafe class InterfazUSA : Window
    {
        private int idUsuario;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolBinario arbolServicios;
        private TextView textViewServicios;
        private RadioButton radioInOrder;
        private RadioButton radioPreOrder;
        private RadioButton radioPostOrder;
        private ComboBox comboVehiculos;
        private ListStore listStoreVehiculos;
        private CheckButton checkAllVehicles;

        public InterfazUSA(int idUsuario, ListaDoblementeEnlazada listaVehiculos, ArbolBinario arbolServicios) 
            : base("Servicios de Vehículos")
        {
            this.idUsuario = idUsuario;
            this.listaVehiculos = listaVehiculos;
            this.arbolServicios = arbolServicios;

            SetDefaultSize(700, 500);
            SetPosition(WindowPosition.Center);

            // Contenedor principal
            VBox mainVBox = new VBox(false, 10);
            mainVBox.BorderWidth = 15;

            // Título
            Label labelTitulo = new Label("Servicios Realizados a Mis Vehículos");
            labelTitulo.ModifyFont(Pango.FontDescription.FromString("Sans Bold 14"));
            mainVBox.PackStart(labelTitulo, false, false, 10);

            // Sección de selección de vehículo
            Frame frameSeleccion = new Frame("Selección de Vehículo");
            VBox vboxSeleccion = new VBox(false, 5);
            vboxSeleccion.BorderWidth = 10;

            // ComboBox para seleccionar vehículo
            HBox hboxVehiculo = new HBox(false, 5);
            hboxVehiculo.PackStart(new Label("Vehículo:"), false, false, 5);

            // Modelo para el ComboBox: ID, Marca, Modelo, Placa
            listStoreVehiculos = new ListStore(typeof(int), typeof(string), typeof(int), typeof(string));
            comboVehiculos = new ComboBox(listStoreVehiculos);

            // Configurar las columnas del ComboBox
            CellRendererText rendererText = new CellRendererText();
            comboVehiculos.PackStart(rendererText, true);
            comboVehiculos.AddAttribute(rendererText, "text", 0); // Mostrar el ID

            // Llenar el ComboBox con los vehículos del usuario
            CargarVehiculosUsuario();

            hboxVehiculo.PackStart(comboVehiculos, true, true, 5);
            vboxSeleccion.PackStart(hboxVehiculo, false, false, 5);

            // CheckButton para mostrar todos los vehículos
            checkAllVehicles = new CheckButton("Mostrar servicios de todos mis vehículos");
            checkAllVehicles.Toggled += OnCheckAllVehiclesToggled;
            vboxSeleccion.PackStart(checkAllVehicles, false, false, 5);

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

            // Botón para mostrar los servicios
            Button buttonMostrar = new Button("Mostrar Servicios");
            buttonMostrar.Clicked += OnMostrarServiciosClicked;
            vboxSeleccion.PackStart(buttonMostrar, false, false, 10);

            frameSeleccion.Add(vboxSeleccion);
            mainVBox.PackStart(frameSeleccion, false, false, 0);

            // Área de visualización de servicios
            Frame frameServicios = new Frame("Lista de Servicios");
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            textViewServicios = new TextView();
            textViewServicios.Editable = false;
            textViewServicios.WrapMode = WrapMode.Word;
            textViewServicios.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));

            scrolledWindow.Add(textViewServicios);
            frameServicios.Add(scrolledWindow);
            mainVBox.PackStart(frameServicios, true, true, 0);

            Add(mainVBox);
            ShowAll();
        }

        private void OnCheckAllVehiclesToggled(object sender, EventArgs e)
        {
            comboVehiculos.Sensitive = !checkAllVehicles.Active;
        }

        private void CargarVehiculosUsuario()
        {
            listStoreVehiculos.Clear();

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
                listStoreVehiculos.AppendValues(-1, "No tiene vehículos registrados", 0, "");
                comboVehiculos.Active = 0;
                return;
            }

            foreach (var vehiculo in vehiculosUsuario)
            {
                string marca = GetFixedString(vehiculo.Marca);
                string placa = GetFixedString(vehiculo.Placa);
                string displayText = $"{vehiculo.Id} - {marca} - {placa}";
                
                listStoreVehiculos.AppendValues(
                    vehiculo.Id, 
                    marca, 
                    vehiculo.Modelo, 
                    placa
                );
            }

            comboVehiculos.Active = 0;
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

        private void OnMostrarServiciosClicked(object sender, EventArgs e)
        {
            if (checkAllVehicles.Active)
            {
                MostrarServiciosTodosVehiculos();
                return;
            }

            if (comboVehiculos.Active < 0)
            {
                textViewServicios.Buffer.Text = "Por favor, seleccione un vehículo.";
                return;
            }

            TreeIter iter;
            if (comboVehiculos.GetActiveIter(out iter))
            {
                int idVehiculo = (int)listStoreVehiculos.GetValue(iter, 0);
                if (idVehiculo == -1)
                {
                    textViewServicios.Buffer.Text = "No tiene vehículos registrados para mostrar servicios.";
                    return;
                }

                List<Servicio> servicios = new List<Servicio>();

                if (radioInOrder.Active)
                {
                    servicios = arbolServicios.ObtenerServiciosPorVehiculoInOrden(idVehiculo);
                    MostrarServiciosTabla(servicios, "In-Orden", idVehiculo);
                }
                else if (radioPreOrder.Active)
                {
                    servicios = arbolServicios.ObtenerServiciosPorVehiculoPreOrden(idVehiculo);
                    MostrarServiciosTabla(servicios, "Pre-Orden", idVehiculo);
                }
                else if (radioPostOrder.Active)
                {
                    servicios = arbolServicios.ObtenerServiciosPorVehiculoPostOrden(idVehiculo);
                    MostrarServiciosTabla(servicios, "Post-Orden", idVehiculo);
                }
            }
        }

        // Método para mostrar servicios de todos los vehículos, unificados y ordenados según el recorrido
        private void MostrarServiciosTodosVehiculos()
        {
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
                textViewServicios.Buffer.Text = "No tiene vehículos registrados.";
                return;
            }

            // Lista combinada de todos los servicios
            List<ServicioConInfoVehiculo> todosLosServicios = new List<ServicioConInfoVehiculo>();

            // Obtener todos los servicios de todos los vehículos
            foreach (var vehiculo in vehiculosUsuario)
            {
                List<Servicio> serviciosVehiculo = new List<Servicio>();
                
                if (radioInOrder.Active)
                {
                    serviciosVehiculo = arbolServicios.ObtenerServiciosPorVehiculoInOrden(vehiculo.Id);
                }
                else if (radioPreOrder.Active)
                {
                    serviciosVehiculo = arbolServicios.ObtenerServiciosPorVehiculoPreOrden(vehiculo.Id);
                }
                else if (radioPostOrder.Active)
                {
                    serviciosVehiculo = arbolServicios.ObtenerServiciosPorVehiculoPostOrden(vehiculo.Id);
                }

                // Agregar información del vehículo a cada servicio
                foreach (var servicio in serviciosVehiculo)
                {
                    todosLosServicios.Add(new ServicioConInfoVehiculo
                    {
                        Servicio = servicio,
                        Vehiculo = vehiculo,
                        MarcaVehiculo = GetFixedString(vehiculo.Marca),
                        ModeloVehiculo = vehiculo.Modelo,
                        PlacaVehiculo = GetFixedString(vehiculo.Placa)
                    });
                }
            }

            // Ahora mostramos todos los servicios en formato tabla
            MostrarServiciosTodosFormatoTabla(todosLosServicios);
        }

        private void MostrarServiciosTabla(List<Servicio> servicios, string tipoRecorrido, int idVehiculo)
        {
            StringBuilder sb = new StringBuilder();
            
            // Obtener la información del vehículo
            TreeIter iter;
            string marca = "", placa = "";
            int modelo = 0;
            
            if (comboVehiculos.GetActiveIter(out iter))
            {
                marca = (string)listStoreVehiculos.GetValue(iter, 1);
                modelo = (int)listStoreVehiculos.GetValue(iter, 2);
                placa = (string)listStoreVehiculos.GetValue(iter, 3);
            }
            
            // Encabezado
            sb.AppendLine($"Vehículo: {marca} - {modelo} - {placa} (ID: {idVehiculo})");
            sb.AppendLine($"Tipo de recorrido: {tipoRecorrido}");
            sb.AppendLine("==============================================================");
            
            if (servicios.Count == 0)
            {
                sb.AppendLine("No hay servicios registrados para este vehículo.");
            }
            else
            {
                // Encabezado de la tabla
                sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-35} | {3,-10}", 
                              "ID", "Repuesto", "Detalles", "Costo (Q)"));
                sb.AppendLine("------+----------+-------------------------------------+------------");
                
                double costoTotal = 0;
                
                // Filas de la tabla
                foreach (var servicio in servicios)
                {
                    sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-35} | {3,10:F2}", 
                                  servicio.ID, 
                                  servicio.Id_Repuesto, 
                                  servicio.Detalles.Length > 35 ? servicio.Detalles.Substring(0, 32) + "..." : servicio.Detalles, 
                                  servicio.Costo));
                    
                    costoTotal += servicio.Costo;
                }
                
                // Pie de la tabla
                sb.AppendLine("------+----------+-------------------------------------+------------");
                sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-35} | {3,10:F2}", 
                              "", "", "TOTAL:", costoTotal));
            }
            
            textViewServicios.Buffer.Text = sb.ToString();
        }

        private void MostrarServiciosTodosFormatoTabla(List<ServicioConInfoVehiculo> servicios)
        {
            StringBuilder sb = new StringBuilder();
            
            string tipoRecorrido = radioInOrder.Active ? "In-Orden" : (radioPreOrder.Active ? "Pre-Orden" : "Post-Orden");
            
            // Encabezado
            sb.AppendLine($"LISTA COMPLETA DE SERVICIOS");
            sb.AppendLine($"Tipo de recorrido: {tipoRecorrido}");
            sb.AppendLine("==================================================================================");
            
            if (servicios.Count == 0)
            {
                sb.AppendLine("No hay servicios registrados para sus vehículos.");
            }
            else
            {
                // Encabezado de la tabla
                sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-12} | {3,-35} | {4,-10}", 
                              "ID", "Repuesto", "Vehículo", "Detalles", "Costo (Q)"));
                sb.AppendLine("------+----------+--------------+-------------------------------------+------------");
                
                double costoTotal = 0;
                
                // Filas de la tabla
                foreach (var item in servicios)
                {
                    var servicio = item.Servicio;
                    string infoVehiculo = $"{item.MarcaVehiculo}-{item.PlacaVehiculo}";
                    if (infoVehiculo.Length > 12)
                        infoVehiculo = infoVehiculo.Substring(0, 9) + "...";
                    
                    sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-12} | {3,-35} | {4,10:F2}", 
                                  servicio.ID, 
                                  servicio.Id_Repuesto,
                                  infoVehiculo, 
                                  servicio.Detalles.Length > 35 ? servicio.Detalles.Substring(0, 32) + "..." : servicio.Detalles, 
                                  servicio.Costo));
                    
                    costoTotal += servicio.Costo;
                }
                
                // Pie de la tabla
                sb.AppendLine("------+----------+--------------+-------------------------------------+------------");
                sb.AppendLine(string.Format("{0,-5} | {1,-8} | {2,-12} | {3,-35} | {4,10:F2}", 
                              "", "", "", "TOTAL:", costoTotal));
                
                // Resumen
                sb.AppendLine("\nRESUMEN:");
                sb.AppendLine($"Total de servicios: {servicios.Count}");
                sb.AppendLine($"Costo total: Q{costoTotal:F2}");
            }
            
            textViewServicios.Buffer.Text = sb.ToString();
        }
    }

    // Clase auxiliar para mantener la información del vehículo junto con el servicio
    public class ServicioConInfoVehiculo
    {
        public Servicio Servicio { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public string MarcaVehiculo { get; set; }
        public int ModeloVehiculo { get; set; }
        public string PlacaVehiculo { get; set; }
    }
}