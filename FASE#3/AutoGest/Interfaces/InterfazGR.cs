using Gtk;
using System;
using Repuestos;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGR : Window
    {
        private ArbolAVL arbolRepuestos;

        public InterfazGR(ArbolAVL arbolRepuestos) : base("Gestión de Repuestos")
        {
            this.arbolRepuestos = arbolRepuestos;

            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);

            // Crear un contenedor para las pestañas
            Notebook notebook = new Notebook();

            // Crear las pestañas
            VBox vboxVerRepuestos = new VBox();
            VBox vboxEditarRepuesto = new VBox();
            VBox vboxEliminarRepuesto = new VBox();
            VBox vboxReportesRepuesto = new VBox();

            // Contenido de la pestaña "Ver Repuestos"
            TextView textViewRepuestos = new TextView();
            textViewRepuestos.Editable = false;
            Button buttonVerRepuestos = new Button("Ver Repuestos");
            buttonVerRepuestos.Clicked += delegate {
                textViewRepuestos.Buffer.Text = arbolRepuestos.ObtenerLista();
            };
            vboxVerRepuestos.PackStart(textViewRepuestos, true, true, 10);
            vboxVerRepuestos.PackStart(buttonVerRepuestos, false, false, 10);

            // Contenido de la pestaña "Editar Repuesto"
            Entry entryIdEditar = new Entry();
            Button buttonBuscarRepuesto = new Button("Buscar");
            Label labelNombreActual = new Label("Repuesto: ");
            Entry entryNombreEditar = new Entry { Sensitive = false };
            Label labelDetallesActual = new Label("Detalles: ");
            Entry entryDetallesEditar = new Entry { Sensitive = false };
            Label labelCostoActual = new Label("Costo: ");
            Entry entryCostoEditar = new Entry { Sensitive = false };
            Button buttonEditarRepuesto = new Button("Actualizar") { Sensitive = false };

            buttonBuscarRepuesto.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    LRepuesto* repuesto = arbolRepuestos.Buscar(id);
                    if (repuesto != null)
                    {
                        labelNombreActual.Text = $"Repuesto: {GetFixedString(repuesto->Repuesto)}";
                        labelDetallesActual.Text = $"Detalles: {GetFixedString(repuesto->Detalles)}";
                        labelCostoActual.Text = $"Costo: {repuesto->Costo}";

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
                        Console.WriteLine("Repuesto no encontrado.");
                    }
                }
                else
                {
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

                        entryNombreEditar.Sensitive = false;
                        entryDetallesEditar.Sensitive = false;
                        entryCostoEditar.Sensitive = false;
                        buttonEditarRepuesto.Sensitive = false;
                    }
                    else
                    {
                        Console.WriteLine("Costo inválido.");
                    }
                }
            };

            HBox hboxIdEditar = new HBox();
            hboxIdEditar.PackStart(new Label("ID:"), false, false, 10);
            hboxIdEditar.PackStart(entryIdEditar, true, true, 10);
            hboxIdEditar.PackStart(buttonBuscarRepuesto, false, false, 10);

            vboxEditarRepuesto.PackStart(hboxIdEditar, false, false, 10);
            vboxEditarRepuesto.PackStart(CreateLabeledEntryWithLabel("Repuesto:", labelNombreActual, entryNombreEditar), false, false, 10);
            vboxEditarRepuesto.PackStart(CreateLabeledEntryWithLabel("Detalles:", labelDetallesActual, entryDetallesEditar), false, false, 10);
            vboxEditarRepuesto.PackStart(CreateLabeledEntryWithLabel("Costo:", labelCostoActual, entryCostoEditar), false, false, 10);
            vboxEditarRepuesto.PackStart(buttonEditarRepuesto, false, false, 10);

            // Contenido de la pestaña "Eliminar Repuesto"
            Entry entryIdEliminar = new Entry();
            Button buttonEliminarRepuesto = new Button("Eliminar Repuesto");
            buttonEliminarRepuesto.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEliminar.Text, out id))
                {
                    arbolRepuestos.Eliminar(id);
                    entryIdEliminar.Text = "";
                }
                else
                {
                    Console.WriteLine("ID inválido.");
                }
            };
            vboxEliminarRepuesto.PackStart(CreateLabeledEntry("ID:", entryIdEliminar), false, false, 10);
            vboxEliminarRepuesto.PackStart(buttonEliminarRepuesto, false, false, 10);

            // Contenido de la pestaña "Reportes"
            Button buttonReporteGeneral = new Button("Generar Reporte General");
            Button buttonReportePorCosto = new Button("Generar Reporte por Costo");
            
            // Entrada para reporte por rango de costo
            Label labelRangoCosto = new Label("Reporte por rango de costo:");
            Entry entryMinCosto = new Entry() { PlaceholderText = "Costo mínimo" };
            Entry entryMaxCosto = new Entry() { PlaceholderText = "Costo máximo" };
            Button buttonReporteRangoCosto = new Button("Generar");

            buttonReporteGeneral.Clicked += delegate {
                arbolRepuestos.GenerarGrafico("Repuestos.dot");
                Console.WriteLine("Reporte general de repuestos generado exitosamente.");
            };

            buttonReportePorCosto.Clicked += delegate {
                arbolRepuestos.GenerarReportePorCosto();
                Console.WriteLine("Reporte por costo generado exitosamente.");
            };

            buttonReporteRangoCosto.Clicked += delegate {
                double minCosto, maxCosto;
                if (double.TryParse(entryMinCosto.Text, out minCosto) && double.TryParse(entryMaxCosto.Text, out maxCosto))
                {
                    if (minCosto <= maxCosto)
                    {
                        arbolRepuestos.GenerarReportePorRangoCosto(minCosto, maxCosto);
                        Console.WriteLine($"Reporte por rango de costo ({minCosto}-{maxCosto}) generado exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("El costo mínimo debe ser menor o igual al costo máximo.");
                    }
                }
                else
                {
                    Console.WriteLine("Los valores de costo deben ser números válidos.");
                }
            };

            vboxReportesRepuesto.PackStart(buttonReporteGeneral, false, false, 10);
            vboxReportesRepuesto.PackStart(buttonReportePorCosto, false, false, 10);
            vboxReportesRepuesto.PackStart(labelRangoCosto, false, false, 10);
            
            HBox hboxRangoCosto = new HBox();
            hboxRangoCosto.PackStart(entryMinCosto, true, true, 10);
            hboxRangoCosto.PackStart(entryMaxCosto, true, true, 10);
            hboxRangoCosto.PackStart(buttonReporteRangoCosto, false, false, 10);
            vboxReportesRepuesto.PackStart(hboxRangoCosto, false, false, 10);

            // Agregar las pestañas al notebook
            notebook.AppendPage(vboxVerRepuestos, new Label("Ver Repuestos"));
            notebook.AppendPage(vboxEditarRepuesto, new Label("Editar Repuesto"));
            notebook.AppendPage(vboxEliminarRepuesto, new Label("Eliminar Repuesto"));
            notebook.AppendPage(vboxReportesRepuesto, new Label("Reportes"));

            // Agregar el notebook a la ventana
            Add(notebook);
            ShowAll();
        }

        private HBox CreateLabeledEntry(string labelText, Entry entry)
        {
            HBox hbox = new HBox();
            Label label = new Label(labelText);
            hbox.PackStart(label, false, false, 10);
            hbox.PackStart(entry, true, true, 10);
            return hbox;
        }

        private HBox CreateLabeledEntryWithLabel(string labelText, Label labelActual, Entry entry)
        {
            HBox hbox = new HBox();
            hbox.PackStart(labelActual, false, false, 10);
            hbox.PackStart(entry, true, true, 10);
            return hbox;
        }

        private string GetFixedString(char* fixedStr)
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