using Gtk;
using System;
using Repuestos;
using AutoGest;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGR : Box
    {
        private InterfazMain mainWindow;
        private ArbolAVL arbolRepuestos;

        public InterfazGR(InterfazMain mainWindow, ArbolAVL arbolRepuestos)
        {
            this.mainWindow = mainWindow;
            this.arbolRepuestos = arbolRepuestos;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>GESTIÓN DE REPUESTOS</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 10);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 5);

            // Crear un contenedor para las pestañas
            Notebook notebook = new Notebook();
            notebook.BorderWidth = 5;

            // Crear las pestañas
            VBox vboxVerRepuestos = new VBox(false, 10) { BorderWidth = 10 };
            VBox vboxEditarRepuesto = new VBox(false, 10) { BorderWidth = 10 };
            VBox vboxEliminarRepuesto = new VBox(false, 10) { BorderWidth = 10 };
            VBox vboxReportesRepuesto = new VBox(false, 10) { BorderWidth = 10 };

            // Contenido de la pestaña "Ver Repuestos"
            ScrolledWindow scrollWindow = new ScrolledWindow();
            TextView textViewRepuestos = new TextView();
            textViewRepuestos.Editable = false;
            textViewRepuestos.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));
            scrollWindow.Add(textViewRepuestos);
            scrollWindow.SetSizeRequest(-1, 200);
            
            // Crear un grupo de radio buttons para los tipos de recorrido
            Frame frameRecorridos = new Frame("Tipo de recorrido");
            HBox hboxRecorrido = new HBox(true, 5) { BorderWidth = 10 };
            
            RadioButton radioInOrder = new RadioButton("In-Orden");
            RadioButton radioPreOrder = new RadioButton(radioInOrder, "Pre-Orden");
            RadioButton radioPostOrder = new RadioButton(radioInOrder, "Post-Orden");
            radioInOrder.Active = true; // Por defecto usar recorrido In-Orden
            
            hboxRecorrido.PackStart(radioInOrder, false, false, 5);
            hboxRecorrido.PackStart(radioPreOrder, false, false, 5);
            hboxRecorrido.PackStart(radioPostOrder, false, false, 5);
            frameRecorridos.Add(hboxRecorrido);
            
            Button buttonVerRepuestos = new Button();
            Label verRepuestosLabel = new Label();
            verRepuestosLabel.Markup = "<span font='10' weight='bold'>Ver Repuestos</span>";
            buttonVerRepuestos.Add(verRepuestosLabel);
            buttonVerRepuestos.SetSizeRequest(150, 35);
            
            // Modify the buttonVerRepuestos.Clicked event handler (around line 76-87)
            
            buttonVerRepuestos.Clicked += delegate {
                try {
                    if (arbolRepuestos == null) {
                        MostrarMensaje(mainWindow, "No hay datos de repuestos disponibles", MessageType.Warning);
                        return;
                    }
                    
                    if (radioInOrder.Active)
                    {
                        textViewRepuestos.Buffer.Text = arbolRepuestos.ObtenerLista();
                    }
                    else if (radioPreOrder.Active)
                    {
                        textViewRepuestos.Buffer.Text = arbolRepuestos.ObtenerListaPreOrden();
                    }
                    else if (radioPostOrder.Active)
                    {
                        textViewRepuestos.Buffer.Text = arbolRepuestos.ObtenerListaPostOrden();
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error al mostrar repuestos: {ex.Message}");
                    MostrarMensaje(mainWindow, "Error al mostrar los repuestos", MessageType.Error);
                }
            };
            
            // Centrar el botón
            HBox buttonVerBox = new HBox();
            buttonVerBox.PackStart(new Label(""), true, true, 0);
            buttonVerBox.PackStart(buttonVerRepuestos, false, false, 0);
            buttonVerBox.PackStart(new Label(""), true, true, 0);
            
            vboxVerRepuestos.PackStart(scrollWindow, true, true, 5);
            vboxVerRepuestos.PackStart(frameRecorridos, false, false, 5);
            vboxVerRepuestos.PackStart(buttonVerBox, false, false, 5);
            
            // Contenido de la pestaña "Editar Repuesto"
            Frame frameBuscarEditar = new Frame("Buscar repuesto");
            HBox hboxIdEditar = new HBox(false, 5) { BorderWidth = 10 };
            
            Entry entryIdEditar = new Entry() { WidthRequest = 80 };
            Button buttonBuscarRepuesto = new Button("Buscar");
            buttonBuscarRepuesto.SetSizeRequest(100, 30);
            
            hboxIdEditar.PackStart(new Label("ID:"), false, false, 5);
            hboxIdEditar.PackStart(entryIdEditar, true, true, 5);
            hboxIdEditar.PackStart(buttonBuscarRepuesto, false, false, 5);
            frameBuscarEditar.Add(hboxIdEditar);
            
            // Frame para información actual
            Frame frameInfoActual = new Frame("Información Actual");
            VBox vboxInfoActual = new VBox(false, 5) { BorderWidth = 10 };
            Label labelNombreActual = new Label("Repuesto: ");
            Label labelDetallesActual = new Label("Detalles: ");
            Label labelCostoActual = new Label("Costo: ");
            vboxInfoActual.PackStart(labelNombreActual, false, false, 2);
            vboxInfoActual.PackStart(labelDetallesActual, false, false, 2);
            vboxInfoActual.PackStart(labelCostoActual, false, false, 2);
            frameInfoActual.Add(vboxInfoActual);
            
            // Frame para nueva información
            Frame frameNuevaInfo = new Frame("Nueva Información");
            Table tableEditar = new Table(3, 2, false);
            tableEditar.RowSpacing = 5;
            tableEditar.ColumnSpacing = 10;
            tableEditar.BorderWidth = 10;
            
            Entry entryNombreEditar = new Entry { Sensitive = false };
            Entry entryDetallesEditar = new Entry { Sensitive = false };
            Entry entryCostoEditar = new Entry { Sensitive = false };
            
            tableEditar.Attach(new Label("Repuesto:"), 0, 1, 0, 1);
            tableEditar.Attach(entryNombreEditar, 1, 2, 0, 1);
            tableEditar.Attach(new Label("Detalles:"), 0, 1, 1, 2);
            tableEditar.Attach(entryDetallesEditar, 1, 2, 1, 2);
            tableEditar.Attach(new Label("Costo:"), 0, 1, 2, 3);
            tableEditar.Attach(entryCostoEditar, 1, 2, 2, 3);
            
            frameNuevaInfo.Add(tableEditar);
            
            Button buttonEditarRepuesto = new Button();
            Label editarRepuestoLabel = new Label();
            editarRepuestoLabel.Markup = "<span font='10' weight='bold'>Actualizar Repuesto</span>";
            buttonEditarRepuesto.Add(editarRepuestoLabel);
            buttonEditarRepuesto.SetSizeRequest(180, 35);
            buttonEditarRepuesto.Sensitive = false;
            
            // Centrar el botón
            HBox buttonEditarBox = new HBox();
            buttonEditarBox.PackStart(new Label(""), true, true, 0);
            buttonEditarBox.PackStart(buttonEditarRepuesto, false, false, 0);
            buttonEditarBox.PackStart(new Label(""), true, true, 0);

            buttonBuscarRepuesto.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    LRepuesto* repuesto = arbolRepuestos.Buscar(id);
                    if (repuesto != null)
                    {
                        labelNombreActual.Text = $"Repuesto: {GetFixedString(repuesto->Repuesto)}";
                        labelDetallesActual.Text = $"Detalles: {GetFixedString(repuesto->Detalles)}";
                        labelCostoActual.Text = $"Costo: Q{repuesto->Costo:F2}";

                        entryNombreEditar.Text = GetFixedString(repuesto->Repuesto);
                        entryDetallesEditar.Text = GetFixedString(repuesto->Detalles);
                        entryCostoEditar.Text = repuesto->Costo.ToString();

                        entryNombreEditar.Sensitive = true;
                        entryDetallesEditar.Sensitive = true;
                        entryCostoEditar.Sensitive = true;
                        buttonEditarRepuesto.Sensitive = true;
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "Repuesto no encontrado", MessageType.Warning);
                        Console.WriteLine("Repuesto no encontrado.");
                    }
                }
                else
                {
                    MostrarMensaje(mainWindow, "ID inválido. Ingrese un número.", MessageType.Error);
                    Console.WriteLine("ID inválido.");
                }
            };

            buttonEditarRepuesto.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    double costo;
                    if (double.TryParse(entryCostoEditar.Text, out costo))
                    {
                        arbolRepuestos.ModificarRepuesto(id, entryNombreEditar.Text, entryDetallesEditar.Text, costo);

                        // Limpiar los inputs después de actualizar
                        entryIdEditar.Text = "";
                        entryNombreEditar.Text = "";
                        entryDetallesEditar.Text = "";
                        entryCostoEditar.Text = "";

                        labelNombreActual.Text = "Repuesto: ";
                        labelDetallesActual.Text = "Detalles: ";
                        labelCostoActual.Text = "Costo: ";

                        entryNombreEditar.Sensitive = false;
                        entryDetallesEditar.Sensitive = false;
                        entryCostoEditar.Sensitive = false;
                        buttonEditarRepuesto.Sensitive = false;
                        
                        MostrarMensaje(mainWindow, "Repuesto actualizado exitosamente", MessageType.Info);
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "Costo inválido. Ingrese un número.", MessageType.Error);
                        Console.WriteLine("Costo inválido.");
                    }
                }
            };

            vboxEditarRepuesto.PackStart(frameBuscarEditar, false, false, 5);
            vboxEditarRepuesto.PackStart(frameInfoActual, false, false, 5);
            vboxEditarRepuesto.PackStart(frameNuevaInfo, false, false, 5);
            vboxEditarRepuesto.PackStart(buttonEditarBox, false, false, 5);

            // Contenido de la pestaña "Eliminar Repuesto"
            Frame frameEliminar = new Frame("Eliminar Repuesto");
            VBox vboxEliminar = new VBox(false, 10) { BorderWidth = 10 };
            
            HBox hboxEliminar = new HBox(false, 10);
            Label labelEliminar = new Label("ID del Repuesto:");
            Entry entryIdEliminar = new Entry();
            
            hboxEliminar.PackStart(labelEliminar, false, false, 5);
            hboxEliminar.PackStart(entryIdEliminar, true, true, 5);
            
            Button buttonEliminarRepuesto = new Button();
            Label eliminarRepuestoLabel = new Label();
            eliminarRepuestoLabel.Markup = "<span font='10' weight='bold'>Eliminar Repuesto</span>";
            buttonEliminarRepuesto.Add(eliminarRepuestoLabel);
            buttonEliminarRepuesto.SetSizeRequest(180, 35);
            buttonEliminarRepuesto.ModifyBg(StateType.Normal, new Gdk.Color(255, 200, 200));
            
            // Centrar el botón
            HBox buttonEliminarBox = new HBox();
            buttonEliminarBox.PackStart(new Label(""), true, true, 0);
            buttonEliminarBox.PackStart(buttonEliminarRepuesto, false, false, 0);
            buttonEliminarBox.PackStart(new Label(""), true, true, 0);
            
            buttonEliminarRepuesto.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEliminar.Text, out id))
                {
                    LRepuesto* repuesto = arbolRepuestos.Buscar(id);
                    if (repuesto != null)
                    {
                        using (var dialog = new MessageDialog(mainWindow,
                            DialogFlags.Modal,
                            MessageType.Question,
                            ButtonsType.YesNo,
                            $"¿Está seguro que desea eliminar el repuesto {GetFixedString(repuesto->Repuesto)} (ID: {id})?"))
                        {
                            if (dialog.Run() == (int)ResponseType.Yes)
                            {
                                arbolRepuestos.Eliminar(id);
                                entryIdEliminar.Text = "";
                                MostrarMensaje(mainWindow, "Repuesto eliminado exitosamente", MessageType.Info);
                            }
                            dialog.Destroy();
                        }
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "Repuesto no encontrado", MessageType.Warning);
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
            vboxEliminarRepuesto.PackStart(frameEliminar, false, false, 5);

            // Contenido de la pestaña "Reportes"
            Frame frameReportes = new Frame("Generación de Reportes");
            VBox vboxReportes = new VBox(false, 10) { BorderWidth = 10 };
            
            // Botones de reportes con mejor estilo
            Button buttonReporteGeneral = new Button();
            Label reporteGeneralLabel = new Label();
            reporteGeneralLabel.Markup = "<span font='10' weight='bold'>Generar Reporte General</span>";
            buttonReporteGeneral.Add(reporteGeneralLabel);
            buttonReporteGeneral.SetSizeRequest(200, 35);
            
            Button buttonReportePorCosto = new Button();
            Label reporteCostoLabel = new Label();
            reporteCostoLabel.Markup = "<span font='10' weight='bold'>Generar Reporte por Costo</span>";
            buttonReportePorCosto.Add(reporteCostoLabel);
            buttonReportePorCosto.SetSizeRequest(200, 35);
            
            // Frame para reporte por rango
            Frame frameRangoCosto = new Frame("Reporte por Rango de Costo");
            VBox vboxRangoCosto = new VBox(false, 10) { BorderWidth = 10 };
            
            HBox hboxRangoCosto = new HBox(false, 10);
            Entry entryMinCosto = new Entry() { PlaceholderText = "Mínimo" };
            Entry entryMaxCosto = new Entry() { PlaceholderText = "Máximo" };
            
            hboxRangoCosto.PackStart(new Label("Desde:"), false, false, 5);
            hboxRangoCosto.PackStart(entryMinCosto, true, true, 5);
            hboxRangoCosto.PackStart(new Label("Hasta:"), false, false, 5);
            hboxRangoCosto.PackStart(entryMaxCosto, true, true, 5);
            
            Button buttonReporteRangoCosto = new Button();
            Label reporteRangoLabel = new Label();
            reporteRangoLabel.Markup = "<span font='10' weight='bold'>Generar</span>";
            buttonReporteRangoCosto.Add(reporteRangoLabel);
            buttonReporteRangoCosto.SetSizeRequest(100, 30);
            
            // Centralizar los botones
            HBox buttonGeneralBox = new HBox();
            buttonGeneralBox.PackStart(new Label(""), true, true, 0);
            buttonGeneralBox.PackStart(buttonReporteGeneral, false, false, 0);
            buttonGeneralBox.PackStart(new Label(""), true, true, 0);
            
            HBox buttonCostoBox = new HBox();
            buttonCostoBox.PackStart(new Label(""), true, true, 0);
            buttonCostoBox.PackStart(buttonReportePorCosto, false, false, 0);
            buttonCostoBox.PackStart(new Label(""), true, true, 0);
            
            HBox buttonRangoBox = new HBox();
            buttonRangoBox.PackStart(new Label(""), true, true, 0);
            buttonRangoBox.PackStart(buttonReporteRangoCosto, false, false, 0);
            buttonRangoBox.PackStart(new Label(""), true, true, 0);

            buttonReporteGeneral.Clicked += delegate {
                arbolRepuestos.GenerarGrafico("Repuestos.dot");
                MostrarMensaje(mainWindow, "Reporte general de repuestos generado exitosamente.", MessageType.Info);
                Console.WriteLine("Reporte general de repuestos generado exitosamente.");
            };

            buttonReportePorCosto.Clicked += delegate {
                arbolRepuestos.GenerarReportePorCosto();
                MostrarMensaje(mainWindow, "Reporte por costo generado exitosamente.", MessageType.Info);
                Console.WriteLine("Reporte por costo generado exitosamente.");
            };

            buttonReporteRangoCosto.Clicked += delegate {
                double minCosto, maxCosto;
                if (double.TryParse(entryMinCosto.Text, out minCosto) && double.TryParse(entryMaxCosto.Text, out maxCosto))
                {
                    if (minCosto <= maxCosto)
                    {
                        arbolRepuestos.GenerarReportePorRangoCosto(minCosto, maxCosto);
                        MostrarMensaje(mainWindow, $"Reporte por rango de costo ({minCosto}-{maxCosto}) generado exitosamente.", MessageType.Info);
                        Console.WriteLine($"Reporte por rango de costo ({minCosto}-{maxCosto}) generado exitosamente.");
                    }
                    else
                    {
                        MostrarMensaje(mainWindow, "El costo mínimo debe ser menor o igual al costo máximo.", MessageType.Warning);
                        Console.WriteLine("El costo mínimo debe ser menor o igual al costo máximo.");
                    }
                }
                else
                {
                    MostrarMensaje(mainWindow, "Los valores de costo deben ser números válidos.", MessageType.Error);
                    Console.WriteLine("Los valores de costo deben ser números válidos.");
                }
            };

            vboxRangoCosto.PackStart(hboxRangoCosto, false, false, 5);
            vboxRangoCosto.PackStart(buttonRangoBox, false, false, 5);
            frameRangoCosto.Add(vboxRangoCosto);
            
            vboxReportes.PackStart(buttonGeneralBox, false, false, 5);
            vboxReportes.PackStart(buttonCostoBox, false, false, 5);
            vboxReportes.PackStart(frameRangoCosto, false, false, 10);
            frameReportes.Add(vboxReportes);
            
            vboxReportesRepuesto.PackStart(frameReportes, false, false, 5);

            // Agregar las pestañas al notebook con iconos
            notebook.AppendPage(vboxVerRepuestos, new Label("Ver Repuestos"));
            notebook.AppendPage(vboxEditarRepuesto, new Label("Editar Repuesto"));
            notebook.AppendPage(vboxEliminarRepuesto, new Label("Eliminar Repuesto"));
            notebook.AppendPage(vboxReportesRepuesto, new Label("Reportes"));

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
                InterfazGE interfazGE = new InterfazGE(mainWindow, null, arbolRepuestos, null);
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