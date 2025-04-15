using Gtk;
using System;
using Vehiculos;
using AutoGest;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGV : Box
    {
        private InterfazMain mainWindow;
        private ListaDoblementeEnlazada listaVehiculos;

        public InterfazGV(InterfazMain mainWindow, ListaDoblementeEnlazada listaVehiculos)
        {
            this.mainWindow = mainWindow;
            this.listaVehiculos = listaVehiculos;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>GESTIÓN DE VEHÍCULOS</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Crear un contenedor para las pestañas
            Notebook notebook = new Notebook();
            notebook.BorderWidth = 5;

            // Crear las pestañas
            VBox vboxVerVehiculos = new VBox(false, 10) { BorderWidth = 10 };
            VBox vboxEditarVehiculo = new VBox(false, 10) { BorderWidth = 10 };
            VBox vboxEliminarVehiculo = new VBox(false, 10) { BorderWidth = 10 };

            // Contenido de la pestaña "Ver Vehículos"
            Frame frameVerVehiculos = new Frame("Lista de Vehículos");
            VBox boxVerVehiculos = new VBox(false, 10) { BorderWidth = 10 };
            
            ScrolledWindow scrollWindow = new ScrolledWindow();
            TextView textViewVehiculos = new TextView();
            textViewVehiculos.Editable = false;
            textViewVehiculos.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));
            scrollWindow.Add(textViewVehiculos);
            scrollWindow.SetSizeRequest(-1, 200);
            
            Button buttonVerVehiculos = new Button();
            Label buttonLabel = new Label();
            buttonLabel.Markup = "<span font='11' weight='bold'>Ver Vehículos</span>";
            buttonVerVehiculos.Add(buttonLabel);
            buttonVerVehiculos.SetSizeRequest(160, 40);
            
            buttonVerVehiculos.Clicked += delegate {
                textViewVehiculos.Buffer.Text = listaVehiculos.ObtenerLista();
            };
            
            // Centrar el botón
            HBox buttonVerBox = new HBox();
            buttonVerBox.PackStart(new Label(""), true, true, 0);
            buttonVerBox.PackStart(buttonVerVehiculos, false, false, 0);
            buttonVerBox.PackStart(new Label(""), true, true, 0);
            
            boxVerVehiculos.PackStart(scrollWindow, true, true, 5);
            boxVerVehiculos.PackStart(buttonVerBox, false, false, 5);
            frameVerVehiculos.Add(boxVerVehiculos);
            vboxVerVehiculos.PackStart(frameVerVehiculos, true, true, 5);

            // Contenido de la pestaña "Editar Vehículo"
            Frame frameBuscarEditar = new Frame("Buscar vehículo");
            HBox hboxIdEditar = new HBox(false, 5) { BorderWidth = 10 };
            
            Entry entryIdEditar = new Entry() { WidthRequest = 80 };
            Button buttonBuscarVehiculo = new Button("Buscar");
            buttonBuscarVehiculo.SetSizeRequest(100, 30);
            
            hboxIdEditar.PackStart(new Label("ID:"), false, false, 5);
            hboxIdEditar.PackStart(entryIdEditar, true, true, 5);
            hboxIdEditar.PackStart(buttonBuscarVehiculo, false, false, 5);
            frameBuscarEditar.Add(hboxIdEditar);
            
            // Frame para información actual
            Frame frameInfoActual = new Frame("Información Actual");
            VBox vboxInfoActual = new VBox(false, 5) { BorderWidth = 10 };
            Label labelIdUsuarioActual = new Label("ID Usuario: ");
            Label labelMarcaActual = new Label("Marca: ");
            Label labelModeloActual = new Label("Modelo: ");
            Label labelPlacaActual = new Label("Placa: ");
            vboxInfoActual.PackStart(labelIdUsuarioActual, false, false, 2);
            vboxInfoActual.PackStart(labelMarcaActual, false, false, 2);
            vboxInfoActual.PackStart(labelModeloActual, false, false, 2);
            vboxInfoActual.PackStart(labelPlacaActual, false, false, 2);
            frameInfoActual.Add(vboxInfoActual);
            
            // Frame para nueva información
            Frame frameNuevaInfo = new Frame("Nueva Información");
            Table tableEditar = new Table(4, 2, false);
            tableEditar.RowSpacing = 5;
            tableEditar.ColumnSpacing = 10;
            tableEditar.BorderWidth = 10;
            
            Entry entryIdUsuarioEditar = new Entry { Sensitive = false };
            Entry entryMarcaEditar = new Entry { Sensitive = false };
            Entry entryModeloEditar = new Entry { Sensitive = false };
            Entry entryPlacaEditar = new Entry { Sensitive = false };
            
            tableEditar.Attach(new Label("ID Usuario:"), 0, 1, 0, 1);
            tableEditar.Attach(entryIdUsuarioEditar, 1, 2, 0, 1);
            tableEditar.Attach(new Label("Marca:"), 0, 1, 1, 2);
            tableEditar.Attach(entryMarcaEditar, 1, 2, 1, 2);
            tableEditar.Attach(new Label("Modelo:"), 0, 1, 2, 3);
            tableEditar.Attach(entryModeloEditar, 1, 2, 2, 3);
            tableEditar.Attach(new Label("Placa:"), 0, 1, 3, 4);
            tableEditar.Attach(entryPlacaEditar, 1, 2, 3, 4);
            
            frameNuevaInfo.Add(tableEditar);
            
            Button buttonEditarVehiculo = new Button();
            Label editarVehiculoLabel = new Label();
            editarVehiculoLabel.Markup = "<span font='10' weight='bold'>Actualizar Vehículo</span>";
            buttonEditarVehiculo.Add(editarVehiculoLabel);
            buttonEditarVehiculo.SetSizeRequest(180, 35);
            buttonEditarVehiculo.Sensitive = false;
            
            // Centrar el botón
            HBox buttonEditarBox = new HBox();
            buttonEditarBox.PackStart(new Label(""), true, true, 0);
            buttonEditarBox.PackStart(buttonEditarVehiculo, false, false, 0);
            buttonEditarBox.PackStart(new Label(""), true, true, 0);

            buttonBuscarVehiculo.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    Vehiculo* vehiculo = listaVehiculos.Buscar(id);
                    if (vehiculo != null)
                    {
                        labelIdUsuarioActual.Text = $"ID Usuario: {vehiculo->ID_Usuario}";
                        labelMarcaActual.Text = $"Marca: {GetFixedString(vehiculo->Marca)}";
                        labelModeloActual.Text = $"Modelo: {vehiculo->Modelo}";
                        labelPlacaActual.Text = $"Placa: {GetFixedString(vehiculo->Placa)}";

                        entryIdUsuarioEditar.Text = vehiculo->ID_Usuario.ToString();
                        entryMarcaEditar.Text = GetFixedString(vehiculo->Marca);
                        entryModeloEditar.Text = vehiculo->Modelo.ToString();
                        entryPlacaEditar.Text = GetFixedString(vehiculo->Placa);

                        entryIdUsuarioEditar.Sensitive = true;
                        entryMarcaEditar.Sensitive = true;
                        entryModeloEditar.Sensitive = true;
                        entryPlacaEditar.Sensitive = true;
                        buttonEditarVehiculo.Sensitive = true;
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "Vehículo no encontrado", MessageType.Warning);
                        Console.WriteLine("Vehículo no encontrado.");
                    }
                }
                else
                {
                    MostrarMensaje(mainWindow, "ID inválido. Ingrese un número.", MessageType.Error);
                    Console.WriteLine("ID inválido.");
                }
            };

            buttonEditarVehiculo.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    int idUsuario, modelo;
                    if (int.TryParse(entryIdUsuarioEditar.Text, out idUsuario) && int.TryParse(entryModeloEditar.Text, out modelo))
                    {
                        listaVehiculos.ModificarVehiculo(id, idUsuario, entryMarcaEditar.Text, modelo, entryPlacaEditar.Text);

                        // Limpiar los inputs después de actualizar
                        entryIdEditar.Text = "";
                        entryIdUsuarioEditar.Text = "";
                        entryMarcaEditar.Text = "";
                        entryModeloEditar.Text = "";
                        entryPlacaEditar.Text = "";

                        labelIdUsuarioActual.Text = "ID Usuario: ";
                        labelMarcaActual.Text = "Marca: ";
                        labelModeloActual.Text = "Modelo: ";
                        labelPlacaActual.Text = "Placa: ";

                        entryIdUsuarioEditar.Sensitive = false;
                        entryMarcaEditar.Sensitive = false;
                        entryModeloEditar.Sensitive = false;
                        entryPlacaEditar.Sensitive = false;
                        buttonEditarVehiculo.Sensitive = false;
                        
                        MostrarMensaje(mainWindow, "Vehículo actualizado exitosamente", MessageType.Info);
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "ID Usuario o Modelo inválido. Ingrese un número.", MessageType.Error);
                        Console.WriteLine("ID Usuario o Modelo inválido.");
                    }
                }
            };

            vboxEditarVehiculo.PackStart(frameBuscarEditar, false, false, 5);
            vboxEditarVehiculo.PackStart(frameInfoActual, false, false, 5);
            vboxEditarVehiculo.PackStart(frameNuevaInfo, false, false, 5);
            vboxEditarVehiculo.PackStart(buttonEditarBox, false, false, 5);

            // Contenido de la pestaña "Eliminar Vehículo"
            Frame frameEliminar = new Frame("Eliminar Vehículo");
            VBox vboxEliminar = new VBox(false, 10) { BorderWidth = 10 };
            
            HBox hboxEliminar = new HBox(false, 10);
            Label labelEliminar = new Label("ID del Vehículo:");
            Entry entryIdEliminar = new Entry();
            
            hboxEliminar.PackStart(labelEliminar, false, false, 5);
            hboxEliminar.PackStart(entryIdEliminar, true, true, 5);
            
            Button buttonEliminarVehiculo = new Button();
            Label eliminarVehiculoLabel = new Label();
            eliminarVehiculoLabel.Markup = "<span font='10' weight='bold'>Eliminar Vehículo</span>";
            buttonEliminarVehiculo.Add(eliminarVehiculoLabel);
            buttonEliminarVehiculo.SetSizeRequest(180, 35);
            buttonEliminarVehiculo.ModifyBg(StateType.Normal, new Gdk.Color(255, 200, 200));
            
            // Centrar el botón
            HBox buttonEliminarBox = new HBox();
            buttonEliminarBox.PackStart(new Label(""), true, true, 0);
            buttonEliminarBox.PackStart(buttonEliminarVehiculo, false, false, 0);
            buttonEliminarBox.PackStart(new Label(""), true, true, 0);
            
            buttonEliminarVehiculo.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEliminar.Text, out id))
                {
                    Vehiculo* vehiculo = listaVehiculos.Buscar(id);
                    if (vehiculo != null)
                    {
                        using (var dialog = new MessageDialog(mainWindow,
                            DialogFlags.Modal,
                            MessageType.Question,
                            ButtonsType.YesNo,
                            $"¿Está seguro que desea eliminar el vehículo {GetFixedString(vehiculo->Marca)} con placa {GetFixedString(vehiculo->Placa)} (ID: {id})?"))
                        {
                            if (dialog.Run() == (int)ResponseType.Yes)
                            {
                                listaVehiculos.Eliminar(id);
                                entryIdEliminar.Text = "";
                                MostrarMensaje(mainWindow, "Vehículo eliminado exitosamente", MessageType.Info);
                            }
                            dialog.Destroy();
                        }
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "Vehículo no encontrado", MessageType.Warning);
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
            vboxEliminarVehiculo.PackStart(frameEliminar, false, false, 5);

            // Agregar las pestañas al notebook con iconos
            notebook.AppendPage(vboxVerVehiculos, new Label("Ver Vehículos"));
            notebook.AppendPage(vboxEditarVehiculo, new Label("Editar Vehículo"));
            notebook.AppendPage(vboxEliminarVehiculo, new Label("Eliminar Vehículo"));

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
                InterfazGE interfazGE = new InterfazGE(mainWindow, null, null, listaVehiculos);
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