using Gtk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Usuarios;
using Vehiculos;
using Repuestos;
using AutoGest;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazCM : Box
    {
        private InterfazMain mainWindow;
        private ListaSimple<Usuario> listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolAVL arbolRepuestos;
        private ComboBoxText comboBox;
        private Label statusLabel;

        public InterfazCM(
            InterfazMain mainWindow,
            ListaSimple<Usuario> listaUsuarios, 
            ListaDoblementeEnlazada listaVehiculos, 
            ArbolAVL arbolRepuestos)
        {
            this.mainWindow = mainWindow;
            this.listaUsuarios = listaUsuarios;
            this.listaVehiculos = listaVehiculos;
            this.arbolRepuestos = arbolRepuestos;

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
                                string contrasena = element.GetProperty("Contrasenia").GetString();

                                Usuario usuario = new Usuario(id, name, lastName, correo, contrasena);
                                Console.WriteLine($"ID: {usuario.id}, Nombres: {name}, Apellidos: {lastName}, Correo: {correo}, Contraseña: {contrasena}");
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
                        
                        // Actualizar etiqueta de estado con información de éxito
                        statusLabel.Markup = $"<span foreground='green'>Carga exitosa: {contadorElementos} {selectedOption.ToLower()} cargados.</span>";
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