using Gtk;
using System;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using AutoGest;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGS : Box
    {
        private InterfazMain mainWindow;
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolB arbolFacturas;
        private ArbolBinario arbolServicios;

        public InterfazGS(
            InterfazMain mainWindow,
            ArbolAVL arbolRepuestos, 
            ListaDoblementeEnlazada listaVehiculos, 
            ArbolB arbolFacturas, 
            ArbolBinario arbolServicios)
        {
            this.mainWindow = mainWindow;
            this.arbolRepuestos = arbolRepuestos;
            this.listaVehiculos = listaVehiculos;
            this.arbolFacturas = arbolFacturas;
            this.arbolServicios = arbolServicios;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>GENERACIÓN DE SERVICIOS</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Frame para el formulario de servicio
            Frame frameServicio = new Frame("Datos del Servicio");
            Table formTable = new Table(5, 2, false);
            formTable.RowSpacing = 10;
            formTable.ColumnSpacing = 15;
            formTable.BorderWidth = 15;
            
            // Crear etiquetas y campos de entrada para cada entidad con mejor formato
            AddFormRow(formTable, 0, "ID del Servicio:", out Entry entryID);
            AddFormRow(formTable, 1, "ID del Repuesto:", out Entry entryIdRepuesto);
            AddFormRow(formTable, 2, "ID del Vehículo:", out Entry entryIdVehiculo);
            AddFormRow(formTable, 3, "Detalles:", out Entry entryDetalles);
            AddFormRow(formTable, 4, "Costo del Servicio:", out Entry entryCosto);
            
            frameServicio.Add(formTable);
            contentBox.PackStart(frameServicio, false, false, 10);
            
            // Frame para información de verificación
            Frame frameVerificacion = new Frame("Información de Verificación");
            VBox infoBox = new VBox(false, 10);
            infoBox.BorderWidth = 15;
            
            Label labelInfoRepuesto = new Label("Repuesto: [Seleccione un ID]");
            Label labelInfoVehiculo = new Label("Vehículo: [Seleccione un ID]");
            
            Button buttonVerificar = new Button();
            Label verificarLabel = new Label();
            verificarLabel.Markup = "<span font='10' weight='bold'>Verificar IDs</span>";
            buttonVerificar.Add(verificarLabel);
            buttonVerificar.SetSizeRequest(150, 30);
            
            buttonVerificar.Clicked += delegate {
                if (!int.TryParse(entryIdRepuesto.Text, out int idRepuesto))
                {
                    MostrarError(mainWindow, "ID de repuesto inválido. Debe ser un número entero.");
                    return;
                }
                
                if (!int.TryParse(entryIdVehiculo.Text, out int idVehiculo))
                {
                    MostrarError(mainWindow, "ID de vehículo inválido. Debe ser un número entero.");
                    return;
                }
                
                LRepuesto* repuesto = arbolRepuestos.Buscar(idRepuesto);
                Vehiculo* vehiculo = listaVehiculos.Buscar(idVehiculo);
                
                if (repuesto != null)
                {
                    string nombreRepuesto = GetFixedString(repuesto->Repuesto);
                    double costoRepuesto = repuesto->Costo;
                    labelInfoRepuesto.Markup = $"<span foreground='green'>Repuesto: {nombreRepuesto} - Costo: Q{costoRepuesto:F2}</span>";
                }
                else
                {
                    labelInfoRepuesto.Markup = $"<span foreground='red'>Repuesto con ID {idRepuesto} no encontrado</span>";
                }
                
                if (vehiculo != null)
                {
                    string marca = GetFixedString(vehiculo->Marca);
                    string placa = GetFixedString(vehiculo->Placa);
                    int idUsuario = vehiculo->ID_Usuario;
                    labelInfoVehiculo.Markup = $"<span foreground='green'>Vehículo: {marca} - Placa: {placa} - ID Usuario: {idUsuario}</span>";
                }
                else
                {
                    labelInfoVehiculo.Markup = $"<span foreground='red'>Vehículo con ID {idVehiculo} no encontrado</span>";
                }
            };
            
            // Centrar el botón
            HBox verificarBox = new HBox();
            verificarBox.PackStart(new Label(""), true, true, 0);
            verificarBox.PackStart(buttonVerificar, false, false, 0);
            verificarBox.PackStart(new Label(""), true, true, 0);
            
            infoBox.PackStart(labelInfoRepuesto, false, false, 5);
            infoBox.PackStart(labelInfoVehiculo, false, false, 5);
            infoBox.PackStart(verificarBox, false, false, 10);
            
            frameVerificacion.Add(infoBox);
            contentBox.PackStart(frameVerificacion, false, false, 10);
            
            // Separador antes del botón de guardar
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 10);
            
            // Botón para guardar el servicio
            Button buttonGuardar = new Button();
            Label guardarLabel = new Label();
            guardarLabel.Markup = "<span font='11' weight='bold'>Guardar Servicio</span>";
            buttonGuardar.Add(guardarLabel);
            buttonGuardar.SetSizeRequest(200, 40);
            buttonGuardar.ModifyBg(StateType.Normal, new Gdk.Color(150, 220, 150));
            
            // Centrar el botón de guardar
            HBox guardarBox = new HBox();
            guardarBox.PackStart(new Label(""), true, true, 0);
            guardarBox.PackStart(buttonGuardar, false, false, 0);
            guardarBox.PackStart(new Label(""), true, true, 0);
            contentBox.PackStart(guardarBox, false, false, 10);
            
            // Evento para guardar el servicio
            buttonGuardar.Clicked += delegate {
                if (!int.TryParse(entryID.Text, out int idServicio))
                {
                    MostrarError(mainWindow, "ID de servicio inválido. Debe ser un número entero.");
                    return;
                }
                
                if (!int.TryParse(entryIdRepuesto.Text, out int idRepuesto))
                {
                    MostrarError(mainWindow, "ID de repuesto inválido. Debe ser un número entero.");
                    return;
                }
                
                if (!int.TryParse(entryIdVehiculo.Text, out int idVehiculo))
                {
                    MostrarError(mainWindow, "ID de vehículo inválido. Debe ser un número entero.");
                    return;
                }
                
                if (!double.TryParse(entryCosto.Text, out double costoServicio))
                {
                    MostrarError(mainWindow, "Costo inválido. Debe ser un valor numérico.");
                    return;
                }

                LRepuesto* repuesto = arbolRepuestos.Buscar(idRepuesto);
                Vehiculo* vehiculo = listaVehiculos.Buscar(idVehiculo);

                if (repuesto != null && vehiculo != null)
                {
                    string detalles = entryDetalles.Text;
                    if (string.IsNullOrWhiteSpace(detalles))
                    {
                        detalles = "Sin detalles";
                    }

                    try
                    {
                        // Crear y guardar el servicio
                        Servicio servicio = new Servicio(idServicio, idVehiculo, idRepuesto, detalles, costoServicio);
                        arbolServicios.Insertar(servicio);
                        
                        // Crear y guardar la factura
                        double total = costoServicio + repuesto->Costo;
                        Factura factura = new Factura(idServicio + 100, idServicio, total);
                        arbolFacturas.Insertar(factura);
                        
                        Console.WriteLine("Servicio guardado:");
                        Console.WriteLine($"ID: {idServicio}, ID Repuesto: {idRepuesto}, ID Vehículo: {idVehiculo}");
                        Console.WriteLine($"Detalles: {detalles}, Costo: {costoServicio}");
                        Console.WriteLine("Factura generada: " + factura.ToString());
                        
                        // Mostrar mensaje de éxito
                        MessageDialog md = new MessageDialog(
                            mainWindow, 
                            DialogFlags.Modal, 
                            MessageType.Info, 
                            ButtonsType.Ok, 
                            $"Servicio y factura generados exitosamente.\n\n" +
                            $"ID Servicio: {idServicio}\n" +
                            $"ID Factura: {factura.ID}\n" +
                            $"Total: Q{total:F2}"
                        );
                        md.Run();
                        md.Destroy();
                        
                        // Limpiar campos para próximo ingreso
                        entryID.Text = string.Empty;
                        entryIdRepuesto.Text = string.Empty;
                        entryIdVehiculo.Text = string.Empty;
                        entryDetalles.Text = string.Empty;
                        entryCosto.Text = string.Empty;
                        labelInfoRepuesto.Text = "Repuesto: [Seleccione un ID]";
                        labelInfoVehiculo.Text = "Vehículo: [Seleccione un ID]";
                        
                        // Generar visualización actualizada
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al guardar: {ex.Message}");
                        MostrarError(mainWindow, $"Error al guardar el servicio: {ex.Message}");
                    }
                }
                else
                {
                    if (repuesto == null)
                    {
                        MostrarError(mainWindow, $"No se encontró un repuesto con ID {idRepuesto}.");
                    }
                    if (vehiculo == null)
                    {
                        MostrarError(mainWindow, $"No se encontró un vehículo con ID {idVehiculo}.");
                    }
                }
            };
            
            // Separador antes del botón de volver
            HSeparator separator3 = new HSeparator();
            contentBox.PackStart(separator3, false, false, 10);
            
            // Botón para volver al menú principal
            Button buttonVolver = new Button();
            Label volverLabel = new Label();
            volverLabel.Markup = "<span font='10'>Volver al Menú</span>";
            buttonVolver.Add(volverLabel);
            buttonVolver.SetSizeRequest(150, 35);
            buttonVolver.Clicked += delegate {
                Console.WriteLine("Volviendo al menú principal...");
                mainWindow.ShowMenuPanel();
            };
            
            // Centrar el botón de volver
            HBox volverBox = new HBox();
            volverBox.PackStart(new Label(""), true, true, 0);
            volverBox.PackStart(buttonVolver, false, false, 0);
            volverBox.PackStart(new Label(""), true, true, 0);
            contentBox.PackStart(volverBox, false, false, 10);
            
            // Centrar todo el contenido en el panel
            HBox centeringBox = new HBox();
            centeringBox.PackStart(new Label(""), true, true, 0);
            centeringBox.PackStart(contentBox, false, false, 0);
            centeringBox.PackStart(new Label(""), true, true, 0);
            
            PackStart(centeringBox, true, true, 0);
        }
        
        // Método para agregar una fila al formulario
        private void AddFormRow(Table table, uint row, string labelText, out Entry entry)
        {
            Label label = new Label(labelText);
            label.SetAlignment(0, 0.5f);
            
            entry = new Entry();
            entry.WidthRequest = 200;
            
            table.Attach(label, 0, 1, row, row + 1, 
                AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            table.Attach(entry, 1, 2, row, row + 1, 
                AttachOptions.Fill | AttachOptions.Expand, AttachOptions.Fill, 0, 0);
        }
        
        // Método para mostrar un error
        private void MostrarError(Window parent, string mensaje)
        {
            MessageDialog md = new MessageDialog(
                parent,
                DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Ok,
                mensaje
            );
            md.Run();
            md.Destroy();
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
}