using Gtk;
using System;
using Usuarios;
using Vehiculos;
using Repuestos;
using AutoGest;
using Servicios;
using Facturas;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazII : Box
    {
        private InterfazMain mainWindow;

        private VBox vbox;
        private Frame frameFormulario;
        private VBox formularioActual;
        private Frame frameUsuarios;
        private Frame frameVehiculos;
        private Frame frameRepuestos;
        private Frame frameServicios;

        private Entry entryIdUsuario, entryNombres, entryApellidos, entryCorreo, entryEdad, entryContrasenia;
        private Entry entryIdVehiculo, entryIdUsuarioVehiculo, entryMarca, entryModelo, entryPlaca;
        private Entry entryIdRepuesto, entryRepuesto, entryDetalles, entryCosto;
        private Entry entryIdServicio, entryIdVehiculoServicio, entryIdRepuestoServicio, entryDetallesServicio, entryCostoServicio;

        private UserBlockchain listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolAVL arbolRepuestos;
        private ArbolBinario arbolServicios;
        private ArbolB arbolFacturas;

        public InterfazII(
            InterfazMain mainWindow,
            UserBlockchain listaUsuarios, 
            ListaDoblementeEnlazada listaVehiculos, 
            ArbolAVL arbolRepuestos,
            ArbolBinario arbolServicios,
            ArbolB arbolFacturas)
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
            labelTitulo.Markup = "<span font='16' weight='bold'>INGRESO DE DATOS</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);

            // Frame para los botones de selección de tipo de formulario
            Frame frameTipos = new Frame("Seleccione el tipo de dato a ingresar");
            HBox tiposBox = new HBox(true, 10);
            tiposBox.BorderWidth = 15;
            
            // Crear botones estilizados para cada tipo de formulario
            Button btnUsuarios = CreateStyledButton("Usuarios");
            Button btnVehiculos = CreateStyledButton("Vehículos");
            Button btnRepuestos = CreateStyledButton("Repuestos");
            Button btnServicios = CreateStyledButton("Servicios");
            
            tiposBox.PackStart(btnUsuarios, true, true, 5);
            tiposBox.PackStart(btnVehiculos, true, true, 5);
            tiposBox.PackStart(btnRepuestos, true, true, 5);
            tiposBox.PackStart(btnServicios, true, true, 5);
            
            frameTipos.Add(tiposBox);
            contentBox.PackStart(frameTipos, false, false, 10);
            
            // Crear el frame para contener los formularios
            frameFormulario = new Frame("Formulario de ingreso");
            vbox = new VBox(false, 10);
            vbox.BorderWidth = 15;
            frameFormulario.Add(vbox);
            contentBox.PackStart(frameFormulario, true, true, 10);
            
            // Crear los formularios para cada entidad
            CrearFormularioUsuarios();
            CrearFormularioVehiculos();
            CrearFormularioRepuestos();
            CrearFormularioServicios();
            
            // Botones para guardar y volver
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 10);
            
            HBox botonesBox = new HBox(true, 20);
            
            // Botón para guardar
            Button btnGuardar = new Button();
            Label guardarLabel = new Label();
            guardarLabel.Markup = "<span font='11' weight='bold'>Guardar Datos</span>";
            btnGuardar.Add(guardarLabel);
            btnGuardar.SetSizeRequest(180, 40);
            btnGuardar.ModifyBg(StateType.Normal, new Gdk.Color(150, 220, 150));
            
            // Botón para volver al menú
            Button btnVolver = new Button();
            Label volverLabel = new Label();
            volverLabel.Markup = "<span font='11' weight='bold'>Volver al Menú</span>";
            btnVolver.Add(volverLabel);
            btnVolver.SetSizeRequest(180, 40);
            
            botonesBox.PackStart(btnGuardar, true, true, 10);
            botonesBox.PackStart(btnVolver, true, true, 10);
            contentBox.PackStart(botonesBox, false, false, 10);
            
            // Eventos para los botones de selección de formulario
            btnUsuarios.Clicked += delegate {
                MostrarFormulario(frameUsuarios);
                frameFormulario.Label = "Formulario de Usuarios";
            };
            
            btnVehiculos.Clicked += delegate {
                MostrarFormulario(frameVehiculos);
                frameFormulario.Label = "Formulario de Vehículos";
            };
            
            btnRepuestos.Clicked += delegate {
                MostrarFormulario(frameRepuestos);
                frameFormulario.Label = "Formulario de Repuestos";
            };
            
            btnServicios.Clicked += delegate {
                MostrarFormulario(frameServicios);
                frameFormulario.Label = "Formulario de Servicios";
            };
            
            // Evento para el botón de guardar
            btnGuardar.Clicked += delegate {
                GuardarDatos();
            };
            
            // Evento para el botón volver
            btnVolver.Clicked += delegate {
                Console.WriteLine("Volviendo al menú principal...");
                mainWindow.ShowMenuPanel();
            };
            
            // Mostrar el formulario de usuarios por defecto
            MostrarFormulario(frameUsuarios);
            frameFormulario.Label = "Formulario de Usuarios";
            
            // Centrar el contenido en el panel
            HBox centeringBox = new HBox();
            centeringBox.PackStart(new Label(""), true, true, 0);
            centeringBox.PackStart(contentBox, true, true, 0);
            centeringBox.PackStart(new Label(""), true, true, 0);
            
            PackStart(centeringBox, true, true, 0);
        }
        
        private Button CreateStyledButton(string text)
        {
            Button button = new Button();
            Label label = new Label();
            label.Markup = $"<span font='10' weight='bold'>{text}</span>";
            button.Add(label);
            button.SetSizeRequest(120, 35);
            return button;
        }
        
        private void CrearFormularioUsuarios()
        {
            VBox formBox = new VBox(false, 10);
            formBox.BorderWidth = 10;
            
            Table table = new Table(5, 2, false);
            table.RowSpacing = 10;
            table.ColumnSpacing = 15;
            
            // Crear las etiquetas y campos
            Label lblId = new Label("ID:") { Xalign = 0 };
            Label lblNombres = new Label("Nombres:") { Xalign = 0 };
            Label lblApellidos = new Label("Apellidos:") { Xalign = 0 };
            Label lblCorreo = new Label("Correo:") { Xalign = 0 };
            Label lblEdad = new Label("Edad:") { Xalign = 0 };
            Label lblContrasenia = new Label("Contraseña:") { Xalign = 0 };

            
            entryIdUsuario = new Entry();
            entryNombres = new Entry();
            entryApellidos = new Entry();
            entryCorreo = new Entry();
            entryEdad = new Entry();
            entryContrasenia = new Entry();
            entryContrasenia.Visibility = false;
            
            // Agregar a la tabla
            table.Attach(lblId, 0, 1, 0, 1);
            table.Attach(entryIdUsuario, 1, 2, 0, 1);
            table.Attach(lblNombres, 0, 1, 1, 2);
            table.Attach(entryNombres, 1, 2, 1, 2);
            table.Attach(lblApellidos, 0, 1, 2, 3);
            table.Attach(entryApellidos, 1, 2, 2, 3);
            table.Attach(lblCorreo, 0, 1, 3, 4);
            table.Attach(entryCorreo, 1, 2, 3, 4);
            table.Attach(lblEdad, 0, 1, 4, 5);
            table.Attach(entryEdad, 1, 2, 4, 5);
            table.Attach(lblContrasenia, 0, 1, 5, 6);
            table.Attach(entryContrasenia, 1, 2, 5, 6);
            
            formBox.PackStart(table, true, true, 10);
            
            frameUsuarios = new Frame();
            frameUsuarios.Add(formBox);
            frameUsuarios.ShowAll();
        }
        
        private void CrearFormularioVehiculos()
        {
            VBox formBox = new VBox(false, 10);
            formBox.BorderWidth = 10;
            
            Table table = new Table(5, 2, false);
            table.RowSpacing = 10;
            table.ColumnSpacing = 15;
            
            // Crear las etiquetas y campos
            Label lblId = new Label("ID:") { Xalign = 0 };
            Label lblIdUsuario = new Label("ID Usuario:") { Xalign = 0 };
            Label lblMarca = new Label("Marca:") { Xalign = 0 };
            Label lblModelo = new Label("Modelo (año):") { Xalign = 0 };
            Label lblPlaca = new Label("Placa:") { Xalign = 0 };
            
            entryIdVehiculo = new Entry();
            entryIdUsuarioVehiculo = new Entry();
            entryMarca = new Entry();
            entryModelo = new Entry();
            entryPlaca = new Entry();
            
            // Botón para verificar usuario
            Button btnVerificarUsuario = new Button("Verificar Usuario");
            btnVerificarUsuario.Clicked += delegate {
                VerificarUsuario(entryIdUsuarioVehiculo.Text);
            };
            
            // Agregar a la tabla
            table.Attach(lblId, 0, 1, 0, 1);
            table.Attach(entryIdVehiculo, 1, 2, 0, 1);
            table.Attach(lblIdUsuario, 0, 1, 1, 2);
            
            HBox hboxUsuario = new HBox(false, 5);
            hboxUsuario.PackStart(entryIdUsuarioVehiculo, true, true, 0);
            hboxUsuario.PackStart(btnVerificarUsuario, false, false, 0);
            table.Attach(hboxUsuario, 1, 2, 1, 2);
            
            table.Attach(lblMarca, 0, 1, 2, 3);
            table.Attach(entryMarca, 1, 2, 2, 3);
            table.Attach(lblModelo, 0, 1, 3, 4);
            table.Attach(entryModelo, 1, 2, 3, 4);
            table.Attach(lblPlaca, 0, 1, 4, 5);
            table.Attach(entryPlaca, 1, 2, 4, 5);
            
            formBox.PackStart(table, true, true, 10);
            
            frameVehiculos = new Frame();
            frameVehiculos.Add(formBox);
            frameVehiculos.ShowAll();
        }
        
        private void CrearFormularioRepuestos()
        {
            VBox formBox = new VBox(false, 10);
            formBox.BorderWidth = 10;
            
            Table table = new Table(4, 2, false);
            table.RowSpacing = 10;
            table.ColumnSpacing = 15;
            
            // Crear las etiquetas y campos
            Label lblId = new Label("ID:") { Xalign = 0 };
            Label lblRepuesto = new Label("Repuesto:") { Xalign = 0 };
            Label lblDetalles = new Label("Detalles:") { Xalign = 0 };
            Label lblCosto = new Label("Costo (Q):") { Xalign = 0 };
            
            entryIdRepuesto = new Entry();
            entryRepuesto = new Entry();
            entryDetalles = new Entry();
            entryCosto = new Entry();
            
            // Agregar a la tabla
            table.Attach(lblId, 0, 1, 0, 1);
            table.Attach(entryIdRepuesto, 1, 2, 0, 1);
            table.Attach(lblRepuesto, 0, 1, 1, 2);
            table.Attach(entryRepuesto, 1, 2, 1, 2);
            table.Attach(lblDetalles, 0, 1, 2, 3);
            table.Attach(entryDetalles, 1, 2, 2, 3);
            table.Attach(lblCosto, 0, 1, 3, 4);
            table.Attach(entryCosto, 1, 2, 3, 4);
            
            formBox.PackStart(table, true, true, 10);
            
            frameRepuestos = new Frame();
            frameRepuestos.Add(formBox);
            frameRepuestos.ShowAll();
        }
        
        private void CrearFormularioServicios()
        {
            VBox formBox = new VBox(false, 10);
            formBox.BorderWidth = 10;
            
            Table table = new Table(5, 2, false);
            table.RowSpacing = 10;
            table.ColumnSpacing = 15;
            
            // Crear las etiquetas y campos
            Label lblId = new Label("ID:") { Xalign = 0 };
            Label lblIdVehiculo = new Label("ID Vehículo:") { Xalign = 0 };
            Label lblIdRepuesto = new Label("ID Repuesto:") { Xalign = 0 };
            Label lblDetalles = new Label("Detalles:") { Xalign = 0 };
            Label lblCosto = new Label("Costo (Q):") { Xalign = 0 };
            
            entryIdServicio = new Entry();
            entryIdVehiculoServicio = new Entry();
            entryIdRepuestoServicio = new Entry();
            entryDetallesServicio = new Entry();
            entryCostoServicio = new Entry();
            
            // Botones para verificar
            Button btnVerificarVehiculo = new Button("Verificar");
            btnVerificarVehiculo.Clicked += delegate {
                VerificarVehiculo(entryIdVehiculoServicio.Text);
            };
            
            Button btnVerificarRepuesto = new Button("Verificar");
            btnVerificarRepuesto.Clicked += delegate {
                VerificarRepuesto(entryIdRepuestoServicio.Text);
            };
            
            // Agregar a la tabla
            table.Attach(lblId, 0, 1, 0, 1);
            table.Attach(entryIdServicio, 1, 2, 0, 1);
            table.Attach(lblIdVehiculo, 0, 1, 1, 2);
            
            HBox hboxVehiculo = new HBox(false, 5);
            hboxVehiculo.PackStart(entryIdVehiculoServicio, true, true, 0);
            hboxVehiculo.PackStart(btnVerificarVehiculo, false, false, 0);
            table.Attach(hboxVehiculo, 1, 2, 1, 2);
            
            table.Attach(lblIdRepuesto, 0, 1, 2, 3);
            
            HBox hboxRepuesto = new HBox(false, 5);
            hboxRepuesto.PackStart(entryIdRepuestoServicio, true, true, 0);
            hboxRepuesto.PackStart(btnVerificarRepuesto, false, false, 0);
            table.Attach(hboxRepuesto, 1, 2, 2, 3);
            
            table.Attach(lblDetalles, 0, 1, 3, 4);
            table.Attach(entryDetallesServicio, 1, 2, 3, 4);
            table.Attach(lblCosto, 0, 1, 4, 5);
            table.Attach(entryCostoServicio, 1, 2, 4, 5);
            
            formBox.PackStart(table, true, true, 10);
            
            frameServicios = new Frame();
            frameServicios.Add(formBox);
            frameServicios.ShowAll();
        }
        
        private void MostrarFormulario(Frame frame)
        {
            if (formularioActual != null)
            {
                vbox.Remove(formularioActual);
            }
            
            formularioActual = new VBox(false, 0);
            formularioActual.PackStart(frame, true, true, 0);
            vbox.PackStart(formularioActual, true, true, 0);
            LimpiarCamposFormulario();
            ShowAll();
        }
        
        private void GuardarDatos()
        {
            try
            {
                if (formularioActual.Children[0] == frameUsuarios)
                {
                    GuardarUsuario();
                }
                else if (formularioActual.Children[0] == frameVehiculos)
                {
                    GuardarVehiculo();
                }
                else if (formularioActual.Children[0] == frameRepuestos)
                {
                    GuardarRepuesto();
                }
                else if (formularioActual.Children[0] == frameServicios)
                {
                    GuardarServicio();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje(mainWindow, $"Error al guardar datos: {ex.Message}", MessageType.Error);
                Console.WriteLine($"Error al guardar datos: {ex.Message}");
            }
        }

        private void GuardarUsuario()
        {
            // Validar ID
            if (string.IsNullOrWhiteSpace(entryIdUsuario.Text))
            {
                MostrarMensaje(mainWindow, "Debe ingresar un ID de usuario", MessageType.Error);
                return;
            }
            
            if (!int.TryParse(entryIdUsuario.Text, out int idUsuario))
            {
                MostrarMensaje(mainWindow, "El ID debe ser un número entero", MessageType.Error);
                return;
            }
            
            // Validar que no exista
            if (listaUsuarios.Buscar(idUsuario) != null)
            {
                MostrarMensaje(mainWindow, $"Ya existe un usuario con ID {idUsuario}", MessageType.Warning);
                return;
            }

            if (listaUsuarios.BuscarCorreo(entryCorreo.Text) != null)
            {
                MostrarMensaje(mainWindow, $"Ya existe un Correo con ese nombre {entryCorreo.Text}", MessageType.Warning);
                return;
            }
            
            // Validar resto de campos
            if (string.IsNullOrWhiteSpace(entryNombres.Text) || 
                string.IsNullOrWhiteSpace(entryApellidos.Text) || 
                string.IsNullOrWhiteSpace(entryCorreo.Text) || 
                string.IsNullOrWhiteSpace(entryContrasenia.Text))
            {
                MostrarMensaje(mainWindow, "Todos los campos son obligatorios", MessageType.Error);
                return;
            }
            
            // Guardar usuario

                Usuario usuario = new Usuario(
                    idUsuario,
                    entryNombres.Text,
                    entryApellidos.Text,
                    entryCorreo.Text,
                    int.Parse(entryEdad.Text),
                    entryContrasenia.Text
                );            
                listaUsuarios.Insertar(usuario);
            
            //MostrarMensaje(mainWindow, $"Usuario {entryNombres.Text} {entryApellidos.Text} guardado exitosamente", MessageType.Info);
            Console.WriteLine($"Usuario guardado: ID={idUsuario}, Nombre={entryNombres.Text} {entryApellidos.Text}");
            
            GLib.Timeout.Add(100, () => {
                LimpiarCamposFormulario();
                return false; // false para que se ejecute solo una vez
            });       
    }

        private void GuardarVehiculo()
        {
            // Validar ID
            if (string.IsNullOrWhiteSpace(entryIdVehiculo.Text))
            {
                MostrarMensaje(mainWindow, "Debe ingresar un ID de vehículo", MessageType.Error);
                return;
            }
            
            if (!int.TryParse(entryIdVehiculo.Text, out int idVehiculo))
            {
                MostrarMensaje(mainWindow, "El ID debe ser un número entero", MessageType.Error);
                return;
            }
            
            // Validar que no exista
            if (listaVehiculos.Buscar(idVehiculo) != null)
            {
                MostrarMensaje(mainWindow, $"Ya existe un vehículo con ID {idVehiculo}", MessageType.Warning);
                return;
            }
            
            // Validar resto de campos
            if (string.IsNullOrWhiteSpace(entryIdUsuarioVehiculo.Text) || 
                string.IsNullOrWhiteSpace(entryMarca.Text) || 
                string.IsNullOrWhiteSpace(entryModelo.Text) || 
                string.IsNullOrWhiteSpace(entryPlaca.Text))
            {
                MostrarMensaje(mainWindow, "Todos los campos son obligatorios", MessageType.Error);
                return;
            }
            
            if (!int.TryParse(entryIdUsuarioVehiculo.Text, out int idUsuario))
            {
                                // Code to add to InterfazII.cs
                                MostrarMensaje(mainWindow, "El ID de usuario debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            // Verificar que exista el usuario
                            if (listaUsuarios.Buscar(idUsuario) == null)
                            {
                                MostrarMensaje(mainWindow, $"No existe un usuario con ID {idUsuario}", MessageType.Warning);
                                return;
                            }
                            
                            // Validar modelo
                            if (!int.TryParse(entryModelo.Text, out int modelo))
                            {
                                MostrarMensaje(mainWindow, "El modelo debe ser un número entero (año)", MessageType.Error);
                                return;
                            }
                            
                            // Guardar vehículo
                            listaVehiculos.Insertar(idVehiculo, idUsuario, entryMarca.Text, modelo, entryPlaca.Text);
                            
                            MostrarMensaje(mainWindow, $"Vehículo {entryMarca.Text} con placa {entryPlaca.Text} guardado exitosamente", MessageType.Info);
                            Console.WriteLine($"Vehículo guardado: ID={idVehiculo}, Marca={entryMarca.Text}, Placa={entryPlaca.Text}");
                            
                            LimpiarCamposFormulario();
                        }
                        
                        private void GuardarRepuesto()
                        {
                            // Validar ID
                            if (string.IsNullOrWhiteSpace(entryIdRepuesto.Text))
                            {
                                MostrarMensaje(mainWindow, "Debe ingresar un ID de repuesto", MessageType.Error);
                                return;
                            }
                            
                            if (!int.TryParse(entryIdRepuesto.Text, out int idRepuesto))
                            {
                                MostrarMensaje(mainWindow, "El ID debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            // Validar que no exista
                            if (arbolRepuestos.Buscar(idRepuesto) != null)
                            {
                                MostrarMensaje(mainWindow, $"Ya existe un repuesto con ID {idRepuesto}", MessageType.Warning);
                                return;
                            }
                            
                            // Validar resto de campos
                            if (string.IsNullOrWhiteSpace(entryRepuesto.Text) || 
                                string.IsNullOrWhiteSpace(entryDetalles.Text) || 
                                string.IsNullOrWhiteSpace(entryCosto.Text))
                            {
                                MostrarMensaje(mainWindow, "Todos los campos son obligatorios", MessageType.Error);
                                return;
                            }
                            
                            // Validar costo
                            if (!double.TryParse(entryCosto.Text, out double costo))
                            {
                                MostrarMensaje(mainWindow, "El costo debe ser un valor numérico", MessageType.Error);
                                return;
                            }
                            
                            // Guardar repuesto
                            arbolRepuestos.Insertar(idRepuesto, entryRepuesto.Text, entryDetalles.Text, costo);
                            
                            MostrarMensaje(mainWindow, $"Repuesto {entryRepuesto.Text} guardado exitosamente", MessageType.Info);
                            Console.WriteLine($"Repuesto guardado: ID={idRepuesto}, Nombre={entryRepuesto.Text}, Costo={costo}");
                            
                            LimpiarCamposFormulario();
                        }
                        
                        private void GuardarServicio()
                        {
                            // Validar ID
                            if (string.IsNullOrWhiteSpace(entryIdServicio.Text))
                            {
                                MostrarMensaje(mainWindow, "Debe ingresar un ID de servicio", MessageType.Error);
                                return;
                            }
                            
                            if (!int.TryParse(entryIdServicio.Text, out int idServicio))
                            {
                                MostrarMensaje(mainWindow, "El ID debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            // Validar que no exista
                            if (arbolServicios.Buscar(idServicio) != null)
                            {
                                MostrarMensaje(mainWindow, $"Ya existe un servicio con ID {idServicio}", MessageType.Warning);
                                return;
                            }
                            
                            // Validar resto de campos
                            if (string.IsNullOrWhiteSpace(entryIdVehiculoServicio.Text) || 
                                string.IsNullOrWhiteSpace(entryIdRepuestoServicio.Text) || 
                                string.IsNullOrWhiteSpace(entryCostoServicio.Text))
                            {
                                MostrarMensaje(mainWindow, "Todos los campos son obligatorios", MessageType.Error);
                                return;
                            }
                            
                            // Validar IDs y costo
                            if (!int.TryParse(entryIdVehiculoServicio.Text, out int idVehiculo))
                            {
                                MostrarMensaje(mainWindow, "El ID de vehículo debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            if (!int.TryParse(entryIdRepuestoServicio.Text, out int idRepuesto))
                            {
                                MostrarMensaje(mainWindow, "El ID de repuesto debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            if (!double.TryParse(entryCostoServicio.Text, out double costo))
                            {
                                MostrarMensaje(mainWindow, "El costo debe ser un valor numérico", MessageType.Error);
                                return;
                            }
                            
                            // Verificar que existan el vehículo y el repuesto
                            if (listaVehiculos.Buscar(idVehiculo) == null)
                            {
                                MostrarMensaje(mainWindow, $"No existe un vehículo con ID {idVehiculo}", MessageType.Warning);
                                return;
                            }
                            
                            if (arbolRepuestos.Buscar(idRepuesto) == null)
                            {
                                MostrarMensaje(mainWindow, $"No existe un repuesto con ID {idRepuesto}", MessageType.Warning);
                                return;
                            }
                            
                            // Preparar detalles
                            string detalles = entryDetallesServicio.Text;
                            if (string.IsNullOrWhiteSpace(detalles))
                            {
                                detalles = "Sin detalles";
                            }
                            
                            try
                            {
                                // Guardar servicio
                                Servicios.Servicio servicio = new Servicios.Servicio(idServicio, idVehiculo, idRepuesto, detalles, costo);
                                arbolServicios.Insertar(servicio);
                                
                                // Crear y guardar la factura
                                LRepuesto* repuesto = arbolRepuestos.Buscar(idRepuesto);
                                double total = costo + repuesto->Costo;
                                Facturas.Factura factura = new Facturas.Factura(idServicio + 100, idServicio, total);
                                arbolFacturas.Insertar(factura);
                                
                                MostrarMensaje(mainWindow, $"Servicio y factura generados exitosamente.\nID Servicio: {idServicio}\nID Factura: {factura.ID}\nTotal: Q{total:F2}", MessageType.Info);
                                Console.WriteLine($"Servicio guardado: ID={idServicio}, ID Vehículo={idVehiculo}, ID Repuesto={idRepuesto}");
                                Console.WriteLine($"Factura generada: ID={factura.ID}, Total={total:F2}");
                                
                                LimpiarCamposFormulario();
                            }
                            catch (Exception ex)
                            {
                                MostrarMensaje(mainWindow, $"Error al guardar servicio: {ex.Message}", MessageType.Error);
                                Console.WriteLine($"Error al guardar servicio: {ex.Message}");
                            }
                        }
                        
                        private void VerificarUsuario(string idStr)
                        {
                            if (!int.TryParse(idStr, out int id))
                            {
                                MostrarMensaje(mainWindow, "El ID de usuario debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            Usuario usuario = listaUsuarios.Buscar(id);
                            if (usuario != null)
                            {
                                string nombreCompleto = $"{usuario.Name} {usuario.LastName}";
                                MostrarMensaje(mainWindow, $"Usuario encontrado: {nombreCompleto}", MessageType.Info);
                            }
                            else
                            {
                                MostrarMensaje(mainWindow, $"No existe un usuario con ID {id}", MessageType.Warning);
                            }
                        }
                        
                        private void VerificarVehiculo(string idStr)
                        {
                            if (!int.TryParse(idStr, out int id))
                            {
                                MostrarMensaje(mainWindow, "El ID de vehículo debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            Vehiculo* vehiculo = listaVehiculos.Buscar(id);
                            if (vehiculo != null)
                            {
                                string info = $"Marca: {GetFixedString(vehiculo->Marca)}\nPlaca: {GetFixedString(vehiculo->Placa)}";
                                MostrarMensaje(mainWindow, $"Vehículo encontrado:\n{info}", MessageType.Info);
                            }
                            else
                            {
                                MostrarMensaje(mainWindow, $"No existe un vehículo con ID {id}", MessageType.Warning);
                            }
                        }
                        
                        private void VerificarRepuesto(string idStr)
                        {
                            if (!int.TryParse(idStr, out int id))
                            {
                                MostrarMensaje(mainWindow, "El ID de repuesto debe ser un número entero", MessageType.Error);
                                return;
                            }
                            
                            LRepuesto* repuesto = arbolRepuestos.Buscar(id);
                            if (repuesto != null)
                            {
                                string info = $"Repuesto: {GetFixedString(repuesto->Repuesto)}\nCosto: Q{repuesto->Costo:F2}";
                                MostrarMensaje(mainWindow, $"Repuesto encontrado:\n{info}", MessageType.Info);
                            }
                            else
                            {
                                MostrarMensaje(mainWindow, $"No existe un repuesto con ID {id}", MessageType.Warning);
                            }
                        }
                        
                        private void LimpiarCamposFormulario()
                        {
                            // Ejecutar la limpieza en el hilo de UI con un pequeño retraso
                            Application.Invoke(delegate {
                                // Limpiar campos de usuario
                                LimpiarCampo(entryIdUsuario);
                                LimpiarCampo(entryNombres);
                                LimpiarCampo(entryApellidos);
                                LimpiarCampo(entryCorreo);
                                LimpiarCampo(entryEdad);
                                LimpiarCampo(entryContrasenia);
                                
                                // Limpiar campos de vehículo
                                LimpiarCampo(entryIdVehiculo);
                                LimpiarCampo(entryIdUsuarioVehiculo);
                                LimpiarCampo(entryMarca);
                                LimpiarCampo(entryModelo);
                                LimpiarCampo(entryPlaca);
                                
                                // Limpiar campos de repuesto
                                LimpiarCampo(entryIdRepuesto);
                                LimpiarCampo(entryRepuesto);
                                LimpiarCampo(entryDetalles);
                                LimpiarCampo(entryCosto);
                                
                                // Limpiar campos de servicio
                                LimpiarCampo(entryIdServicio);
                                LimpiarCampo(entryIdVehiculoServicio);
                                LimpiarCampo(entryIdRepuestoServicio);
                                LimpiarCampo(entryDetallesServicio);
                                LimpiarCampo(entryCostoServicio);
                            });
                        }
                        
                        // Método mejorado para limpiar un Entry
                        private void LimpiarCampo(Entry entry)
                        {
                            if (entry != null && entry.Handle != IntPtr.Zero && entry.IsRealized)
                            {
                                try
                                {
                                    entry.Text = string.Empty;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error al limpiar entrada: {ex.Message}");
                                }
                            }
                        }
                        
                        private void MostrarMensaje(Window parent, string mensaje, MessageType tipo)
                        {
                            using (var md = new MessageDialog(parent, 
                                DialogFlags.DestroyWithParent, 
                                tipo, 
                                ButtonsType.Close, 
                                mensaje))
                            {
                                md.Run();
                                md.Destroy();
                            }
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
                    }
                }
            