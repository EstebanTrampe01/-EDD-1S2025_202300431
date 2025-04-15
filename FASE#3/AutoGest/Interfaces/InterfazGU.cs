using Gtk;
using System;
using Usuarios;
using Vehiculos;
using AutoGest;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGU : Box
    {
        private InterfazMain mainWindow;
        private UserBlockchain listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;

        public InterfazGU(
            InterfazMain mainWindow,
            UserBlockchain listaUsuarios, 
            ListaDoblementeEnlazada listaVehiculos)
        {
            this.mainWindow = mainWindow;
            this.listaUsuarios = listaUsuarios;
            this.listaVehiculos = listaVehiculos;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>GESTIÓN DE USUARIOS</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Crear un contenedor para las pestañas
            Notebook notebook = new Notebook();
            notebook.BorderWidth = 5;

            // Crear las pestañas
            VBox vboxVerUsuarios = new VBox(false, 10) { BorderWidth = 10 };
            VBox vboxEditarUsuario = new VBox(false, 10) { BorderWidth = 10 };
            VBox vboxEliminarUsuario = new VBox(false, 10) { BorderWidth = 10 };

            // Contenido de la pestaña "Ver Usuarios"
            Frame frameVerUsuarios = new Frame("Lista de Usuarios");
            VBox boxVerUsuarios = new VBox(false, 10) { BorderWidth = 10 };
            
            ScrolledWindow scrollWindow = new ScrolledWindow();
            TextView textViewUsuarios = new TextView();
            textViewUsuarios.Editable = false;
            textViewUsuarios.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));
            scrollWindow.Add(textViewUsuarios);
            scrollWindow.SetSizeRequest(-1, 200);
            
            Button buttonVerUsuarios = new Button();
            Label buttonLabel = new Label();
            buttonLabel.Markup = "<span font='11' weight='bold'>Ver Usuarios</span>";
            buttonVerUsuarios.Add(buttonLabel);
            buttonVerUsuarios.SetSizeRequest(160, 40);
            
            buttonVerUsuarios.Clicked += delegate {
                textViewUsuarios.Buffer.Text = listaUsuarios.ObtenerLista();
            };
            
            // Centrar el botón
            HBox buttonVerBox = new HBox();
            buttonVerBox.PackStart(new Label(""), true, true, 0);
            buttonVerBox.PackStart(buttonVerUsuarios, false, false, 0);
            buttonVerBox.PackStart(new Label(""), true, true, 0);
            
            boxVerUsuarios.PackStart(scrollWindow, true, true, 5);
            boxVerUsuarios.PackStart(buttonVerBox, false, false, 5);
            frameVerUsuarios.Add(boxVerUsuarios);
            vboxVerUsuarios.PackStart(frameVerUsuarios, true, true, 5);

            // Contenido de la pestaña "Editar Usuario"
            Frame frameBuscarEditar = new Frame("Buscar usuario");
            HBox hboxIdEditar = new HBox(false, 5) { BorderWidth = 10 };
            
            Entry entryIdEditar = new Entry() { WidthRequest = 80 };
            Button buttonBuscarUsuario = new Button("Buscar");
            buttonBuscarUsuario.SetSizeRequest(100, 30);
            
            hboxIdEditar.PackStart(new Label("ID:"), false, false, 5);
            hboxIdEditar.PackStart(entryIdEditar, true, true, 5);
            hboxIdEditar.PackStart(buttonBuscarUsuario, false, false, 5);
            frameBuscarEditar.Add(hboxIdEditar);
            
            // Frame para información actual
            Frame frameInfoActual = new Frame("Información Actual");
            VBox vboxInfoActual = new VBox(false, 5) { BorderWidth = 10 };
            Label labelNombresActual = new Label("Nombres: ");
            Label labelApellidosActual = new Label("Apellidos: ");
            Label labelCorreoActual = new Label("Correo: ");
            Label labelContraseniaActual = new Label("Contraseña: ");
            vboxInfoActual.PackStart(labelNombresActual, false, false, 2);
            vboxInfoActual.PackStart(labelApellidosActual, false, false, 2);
            vboxInfoActual.PackStart(labelCorreoActual, false, false, 2);
            vboxInfoActual.PackStart(labelContraseniaActual, false, false, 2);
            frameInfoActual.Add(vboxInfoActual);
            
            // Frame para nueva información
            Frame frameNuevaInfo = new Frame("Nueva Información");
            Table tableEditar = new Table(4, 2, false);
            tableEditar.RowSpacing = 5;
            tableEditar.ColumnSpacing = 10;
            tableEditar.BorderWidth = 10;
            
            Entry entryNombresEditar = new Entry { Sensitive = false };
            Entry entryApellidosEditar = new Entry { Sensitive = false };
            Entry entryCorreoEditar = new Entry { Sensitive = false };
            Entry entryContraseniaEditar = new Entry { Sensitive = false };
            
            tableEditar.Attach(new Label("Nombres:"), 0, 1, 0, 1);
            tableEditar.Attach(entryNombresEditar, 1, 2, 0, 1);
            tableEditar.Attach(new Label("Apellidos:"), 0, 1, 1, 2);
            tableEditar.Attach(entryApellidosEditar, 1, 2, 1, 2);
            tableEditar.Attach(new Label("Correo:"), 0, 1, 2, 3);
            tableEditar.Attach(entryCorreoEditar, 1, 2, 2, 3);
            tableEditar.Attach(new Label("Contraseña:"), 0, 1, 3, 4);
            tableEditar.Attach(entryContraseniaEditar, 1, 2, 3, 4);
            
            frameNuevaInfo.Add(tableEditar);
            
            Button buttonEditarUsuario = new Button();
            Label editarUsuarioLabel = new Label();
            editarUsuarioLabel.Markup = "<span font='10' weight='bold'>Actualizar Usuario</span>";
            buttonEditarUsuario.Add(editarUsuarioLabel);
            buttonEditarUsuario.SetSizeRequest(180, 35);
            buttonEditarUsuario.Sensitive = false;
            
            // Centrar el botón
            HBox buttonEditarBox = new HBox();
            buttonEditarBox.PackStart(new Label(""), true, true, 0);
            buttonEditarBox.PackStart(buttonEditarUsuario, false, false, 0);
            buttonEditarBox.PackStart(new Label(""), true, true, 0);

            buttonBuscarUsuario.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                                        Usuario usuario = listaUsuarios.Buscar(id);
                    if (usuario != null)
                    {
                        labelNombresActual.Text = $"Nombres: {usuario.Name}";
                        labelApellidosActual.Text = $"Apellidos: {usuario.LastName}";
                        labelCorreoActual.Text = $"Correo: {usuario.Correo}";
                        labelContraseniaActual.Text = $"Contrasenia: {usuario.Contrasena}";
                    
                        entryNombresEditar.Text = usuario.Name;
                        entryApellidosEditar.Text = usuario.LastName;
                        entryCorreoEditar.Text = usuario.Correo;
                        entryContraseniaEditar.Text = usuario.Contrasena;
                    
                        entryNombresEditar.Sensitive = true;
                        entryApellidosEditar.Sensitive = true;
                        entryCorreoEditar.Sensitive = true;
                        entryContraseniaEditar.Sensitive = true;
                        buttonEditarUsuario.Sensitive = true;
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "Usuario no encontrado", MessageType.Warning);
                        Console.WriteLine("Usuario no encontrado.");
                    }
                }
                else
                {
                    MostrarMensaje(mainWindow, "ID inválido. Ingrese un número.", MessageType.Error);
                    Console.WriteLine("ID inválido.");
                }
            };

            buttonEditarUsuario.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    listaUsuarios.ModificarUsuario(id, entryNombresEditar.Text, entryApellidosEditar.Text, entryCorreoEditar.Text, entryContraseniaEditar.Text);

                    // Limpiar los inputs después de actualizar
                    entryIdEditar.Text = "";
                    entryNombresEditar.Text = "";
                    entryApellidosEditar.Text = "";
                    entryCorreoEditar.Text = "";
                    entryContraseniaEditar.Text = "";

                    labelNombresActual.Text = "Nombres: ";
                    labelApellidosActual.Text = "Apellidos: ";
                    labelCorreoActual.Text = "Correo: ";
                    labelContraseniaActual.Text = "Contraseña: ";

                    entryNombresEditar.Sensitive = false;
                    entryApellidosEditar.Sensitive = false;
                    entryCorreoEditar.Sensitive = false;
                    entryContraseniaEditar.Sensitive = false;
                    buttonEditarUsuario.Sensitive = false;
                    
                    MostrarMensaje(mainWindow, "Usuario actualizado exitosamente", MessageType.Info);
                }
            };

            vboxEditarUsuario.PackStart(frameBuscarEditar, false, false, 5);
            vboxEditarUsuario.PackStart(frameInfoActual, false, false, 5);
            vboxEditarUsuario.PackStart(frameNuevaInfo, false, false, 5);
            vboxEditarUsuario.PackStart(buttonEditarBox, false, false, 5);

            // Contenido de la pestaña "Eliminar Usuario"
            Frame frameEliminar = new Frame("Eliminar Usuario");
            VBox vboxEliminar = new VBox(false, 10) { BorderWidth = 10 };
            
            HBox hboxEliminar = new HBox(false, 10);
            Label labelEliminar = new Label("ID del Usuario:");
            Entry entryIdEliminar = new Entry();
            
            hboxEliminar.PackStart(labelEliminar, false, false, 5);
            hboxEliminar.PackStart(entryIdEliminar, true, true, 5);
            
            Button buttonEliminarUsuario = new Button();
            Label eliminarUsuarioLabel = new Label();
            eliminarUsuarioLabel.Markup = "<span font='10' weight='bold'>Eliminar Usuario</span>";
            buttonEliminarUsuario.Add(eliminarUsuarioLabel);
            buttonEliminarUsuario.SetSizeRequest(180, 35);
            buttonEliminarUsuario.ModifyBg(StateType.Normal, new Gdk.Color(255, 200, 200));
            
            // Centrar el botón
            HBox buttonEliminarBox = new HBox();
            buttonEliminarBox.PackStart(new Label(""), true, true, 0);
            buttonEliminarBox.PackStart(buttonEliminarUsuario, false, false, 0);
            buttonEliminarBox.PackStart(new Label(""), true, true, 0);
            
            buttonEliminarUsuario.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEliminar.Text, out id))
                {
                    Usuario usuario = listaUsuarios.Buscar(id);
                    if (usuario != null)
                    {
                        using (var dialog = new MessageDialog(mainWindow,
                            DialogFlags.Modal,
                            MessageType.Question,
                            ButtonsType.YesNo,
                            $"¿Está seguro que desea eliminar al usuario {usuario.Name} {usuario.LastName} (ID: {id})?"))
                        {
                            if (dialog.Run() == (int)ResponseType.Yes)
                            {
                               
                                    listaUsuarios.Eliminar(id);
                                    entryIdEliminar.Text = "";
                                    MostrarMensaje(mainWindow, "Usuario eliminado exitosamente", MessageType.Info);
                                
                            }
                            dialog.Destroy();
                        }
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "Usuario no encontrado", MessageType.Warning);
                    }
                }
                else
                {
                    MostrarMensaje(mainWindow, "ID inválido. Ingrese un número.", MessageType.Error);
                    Console.WriteLine("ID inválido.");
                }
            };
            
            vboxEliminar.PackStart(hboxEliminar, false, false, 5);
            vboxEliminar.PackStart(buttonEliminarBox, false, false, 5);
            frameEliminar.Add(vboxEliminar);
            vboxEliminarUsuario.PackStart(frameEliminar, false, false, 5);

            // Agregar las pestañas al notebook con iconos
            notebook.AppendPage(vboxVerUsuarios, new Label("Ver Usuarios"));
            notebook.AppendPage(vboxEditarUsuario, new Label("Editar Usuario"));
            notebook.AppendPage(vboxEliminarUsuario, new Label("Eliminar Usuario"));

            // Agregar el notebook al contenedor
            contentBox.PackStart(notebook, true, true, 0);
            
            // Separador antes del botón de volver
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 5);
            
            // Botón para volver al menú de gestión
            Button buttonVolver = new Button();
            Label volverLabel = new Label();
            volverLabel.Markup = "<span font='10'>Volver a Gestión</span>";
            buttonVolver.Add(volverLabel);
            buttonVolver.SetSizeRequest(150, 35);
            buttonVolver.Clicked += delegate {
                Console.WriteLine("Volviendo a Gestión de Entidades...");
                InterfazGE interfazGE = new InterfazGE(mainWindow, listaUsuarios, null, listaVehiculos);
                mainWindow.CambiarPanel(interfazGE);
            };
            
            // Centrar el botón de volver
            HBox volverBox = new HBox();
            volverBox.PackStart(new Label(""), true, true, 0);
            volverBox.PackStart(buttonVolver, false, false, 0);
            volverBox.PackStart(new Label(""), true, true, 0);
            contentBox.PackStart(volverBox, false, false, 5);
            
            // Centrar todo el contenido en el panel
            HBox centeringBox = new HBox();
            centeringBox.PackStart(new Label(""), true, true, 0);
            centeringBox.PackStart(contentBox, true, true, 0);
            centeringBox.PackStart(new Label(""), true, true, 0);
            
            PackStart(centeringBox, true, true, 0);
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