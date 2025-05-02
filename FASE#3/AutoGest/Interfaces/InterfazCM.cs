using Gtk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Usuarios;
using Vehiculos;
using Repuestos;
using AutoGest;
using Facturas;
using Servicios;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazCM : Box
    {
        private InterfazMain mainWindow;
        private UserBlockchain listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolAVL arbolRepuestos;
        private ArbolBinario arbolServicios;
        private ArbolM arbolFacturas;
        private ComboBoxText comboBox;
        private Label statusLabel;

        public InterfazCM(
            InterfazMain mainWindow,
            UserBlockchain listaUsuarios, 
            ListaDoblementeEnlazada listaVehiculos, 
            ArbolAVL arbolRepuestos,
            ArbolBinario arbolServicios,
            ArbolM arbolFacturas)
        {
            this.mainWindow = mainWindow;
            this.listaUsuarios = listaUsuarios;
            this.listaVehiculos = listaVehiculos;
            this.arbolRepuestos = arbolRepuestos;
            this.arbolServicios = arbolServicios;
            this.arbolFacturas = arbolFacturas;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>CARGA MASIVA DE DATOS</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Instrucciones
            Label labelInstrucciones = new Label("Seleccione el tipo de datos a cargar:");
            labelInstrucciones.SetAlignment(0, 0.5f);
            contentBox.PackStart(labelInstrucciones, false, false, 10);
            
            // Combo box para seleccionar el tipo de datos
            comboBox = new ComboBoxText();
            comboBox.AppendText("Usuarios");
            comboBox.AppendText("Vehículos");
            comboBox.AppendText("Repuestos");
            comboBox.AppendText("Servicios");
            comboBox.Active = 0; // Seleccionar la primera opción por defecto
            
            // Contenedor para el combo box con mejor organización
            Frame frameCombo = new Frame("Tipo de datos");
            frameCombo.Add(comboBox);
            contentBox.PackStart(frameCombo, false, false, 10);
            
            // Botón de carga con mejor estilo
            Button buttonCargar = new Button();
            Label buttonLabel = new Label();
            buttonLabel.Markup = "<span font='11' weight='bold'>Seleccionar Archivo...</span>";
            buttonCargar.Add(buttonLabel);
            buttonCargar.SetSizeRequest(200, 40);
            buttonCargar.Clicked += OnCargarButtonClicked;
            
            // Centrar el botón
            HBox buttonBox = new HBox();
            buttonBox.PackStart(new Label(""), true, true, 0);
            buttonBox.PackStart(buttonCargar, false, false, 0);
            buttonBox.PackStart(new Label(""), true, true, 0);
            contentBox.PackStart(buttonBox, false, false, 20);
            
            // Etiqueta para mostrar el estado de la operación
            statusLabel = new Label("");
            statusLabel.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(statusLabel, false, false, 10);
            
            // Separador antes del botón de volver
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 10);
            
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
            
            // Centrar el contenido en el panel
            HBox centeringBox = new HBox();
            centeringBox.PackStart(new Label(""), true, true, 0);
            centeringBox.PackStart(contentBox, false, false, 0);
            centeringBox.PackStart(new Label(""), true, true, 0);
            
            PackStart(centeringBox, true, true, 0);
        }
        
        private void OnCargarButtonClicked(object sender, EventArgs e)
        {
            string selectedOption = comboBox.ActiveText;
            Console.WriteLine("Opción seleccionada: " + selectedOption);

            FileChooserDialog filechooser = new FileChooserDialog(
                "Seleccione un archivo JSON",
                mainWindow,
                FileChooserAction.Open,
                "Cancelar", ResponseType.Cancel,
                "Abrir", ResponseType.Accept);

            // Filtro para archivos JSON
            FileFilter filter = new FileFilter();
            filter.Name = "Archivos JSON";
            filter.AddPattern("*.json");
            filechooser.AddFilter(filter);

            if (filechooser.Run() == (int)ResponseType.Accept)
            {
                try
                {
                    Console.WriteLine($"Archivo seleccionado: {filechooser.Filename}");
                    
                    string contenido = File.ReadAllText(filechooser.Filename);
                    JsonDocument doc = JsonDocument.Parse(contenido);
                    JsonElement root = doc.RootElement;
                    
                    int contadorElementos = 0;

                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        if (selectedOption == "Usuarios")
                        {
                            foreach (JsonElement element in root.EnumerateArray())
                            {
                                int id = element.GetProperty("ID").GetInt32();
                                string name = element.GetProperty("Nombres").GetString();
                                string lastName = element.GetProperty("Apellidos").GetString();
                                string correo = element.GetProperty("Correo").GetString();
                                int edad = element.GetProperty("Edad").GetInt32();
                                string contrasena = element.GetProperty("Contrasenia").GetString();

                                Usuario usuario = new Usuario(id, name, lastName, correo, edad,contrasena);
                                Console.WriteLine($"ID: {usuario.Id}, Nombres: {name}, Apellidos: {lastName}, Correo: {correo}, Edad: {edad},Contraseña: {contrasena}");
                                listaUsuarios.Insertar(usuario);
                                contadorElementos++;
                            }
                            
                            // Mostrar la lista en consola para verificación
                            listaUsuarios.Imprimir();
                        }
                        else if (selectedOption == "Vehículos")
                        {
                            foreach (JsonElement element in root.EnumerateArray())
                            {
                                int id = element.GetProperty("ID").GetInt32();
                                int idUsuario = element.GetProperty("ID_Usuario").GetInt32();
                                string marca = element.GetProperty("Marca").GetString();
                                int modelo = element.GetProperty("Modelo").GetInt32();
                                string placa = element.GetProperty("Placa").GetString();

                                Console.WriteLine($"ID: {id}, ID Usuario: {idUsuario}, Marca: {marca}, Modelo: {modelo}, Placa: {placa}");
                                listaVehiculos.Insertar(id, idUsuario, marca, modelo, placa);
                                contadorElementos++;
                            }
                            
                            // Mostrar la lista en consola para verificación
                            listaVehiculos.Mostrar();
                        }
                        else if (selectedOption == "Repuestos")
                        {
                            foreach (JsonElement element in root.EnumerateArray())
                            {
                                int id = element.GetProperty("ID").GetInt32();
                                string repuesto = element.GetProperty("Repuesto").GetString();
                                string detalles = element.GetProperty("Detalles").GetString();
                                double costo = element.GetProperty("Costo").GetDouble();

                                Console.WriteLine($"ID: {id}, Repuesto: {repuesto}, Detalles: {detalles}, Costo: {costo}");
                                arbolRepuestos.Insertar(id, repuesto, detalles, costo);
                                contadorElementos++;
                            }
                            
                            // Mostrar el árbol en consola para verificación
                            arbolRepuestos.Mostrar();
                        }
                        else if (selectedOption == "Servicios")
                        {
                            foreach (JsonElement element in root.EnumerateArray())
                            {
                                int id = element.GetProperty("Id").GetInt32();
                                int idRepuesto = element.GetProperty("Id_repuesto").GetInt32();
                                int idVehiculo = element.GetProperty("Id_vehiculo").GetInt32();
                                string detalles = element.GetProperty("Detalles").GetString();
                                double costo = element.GetProperty("Costo").GetDouble();
                                string metodoPago = "Efectivo"; // Valor por defecto
                                
                                // Intentar obtener método de pago si existe
                                if (element.TryGetProperty("MetodoPago", out JsonElement metodoPagoElement))
                                {
                                    metodoPago = metodoPagoElement.GetString();
                                }

                                // Verificar si existen el vehículo y el repuesto
                                Vehiculo* vehiculo = listaVehiculos.Buscar(idVehiculo);
                                LRepuesto* repuesto = arbolRepuestos.Buscar(idRepuesto);
                                
                                if (vehiculo != null && repuesto != null)
                                {
                                    // Crear y guardar el servicio
                                    Servicios.Servicio servicio = new Servicios.Servicio(id, idVehiculo, idRepuesto, detalles, costo, metodoPago);
                                    arbolServicios.Insertar(servicio);
                                    
                                    // Crear y guardar la factura correspondiente
                                    double total = costo + repuesto->Costo;
                                    Facturas.Factura factura = new Facturas.Factura(id + 100, id, total);
                                    factura.MetodoPago = metodoPago;
                                    arbolFacturas.Insertar(factura);
                                    
                                    Console.WriteLine($"Servicio cargado: ID={id}, ID Vehículo={idVehiculo}, ID Repuesto={idRepuesto}, Costo={costo}, Método Pago={metodoPago}");
                                    Console.WriteLine($"Factura generada: ID={factura.ID}, Total={total:F2}, Método Pago={factura.MetodoPago}");
                                    
                                    contadorElementos++;
                                }
                                else
                                {
                                    string error = "";
                                    if (vehiculo == null) error += $"No existe un vehículo con ID {idVehiculo}. ";
                                    if (repuesto == null) error += $"No existe un repuesto con ID {idRepuesto}. ";
                                    
                                    Console.WriteLine($"Error en servicio ID={id}: {error}");
                                }
                            }
                            
                            // Mostrar la lista en consola para verificación
                            //arbolServicios.Print();
                            //arbolFacturas.Print();
                        }
                        
                        // Actualizar etiqueta de estado con información de éxito
                        if (contadorElementos > 0) {
                            statusLabel.Markup = $"<span foreground='green'>Carga exitosa: {contadorElementos} {selectedOption.ToLower()} cargados.</span>";
                        } else {
                            statusLabel.Markup = $"<span foreground='orange'>Advertencia: No se pudieron cargar {selectedOption.ToLower()}. Verifique los datos y dependencias.</span>";
                        }
                    }
                    else
                    {
                        Console.WriteLine("El archivo no contiene datos válidos.");
                        statusLabel.Markup = "<span foreground='red'>Error: El archivo no contiene un arreglo JSON válido.</span>";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar el archivo: {ex.Message}");
                    statusLabel.Markup = $"<span foreground='red'>Error: {ex.Message}</span>";
                }
            }

            filechooser.Destroy();
        }
    }
}