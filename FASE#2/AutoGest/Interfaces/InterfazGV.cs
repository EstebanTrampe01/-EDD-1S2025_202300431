using Gtk;
using System;
using Vehiculos;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGV : Window
    {
        private ListaDoblementeEnlazada listaVehiculos;

        public InterfazGV(ListaDoblementeEnlazada listaVehiculos) : base("Gestión de Vehículos")
        {
            this.listaVehiculos = listaVehiculos;

            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);

            // Crear un contenedor para las pestañas
            Notebook notebook = new Notebook();

            // Crear las pestañas
            VBox vboxVerVehiculos = new VBox();
            VBox vboxEditarVehiculo = new VBox();
            VBox vboxEliminarVehiculo = new VBox();

            // Contenido de la pestaña "Ver Vehículos"
            TextView textViewVehiculos = new TextView();
            textViewVehiculos.Editable = false;
            Button buttonVerVehiculos = new Button("Ver Vehículos");
            buttonVerVehiculos.Clicked += delegate {
                textViewVehiculos.Buffer.Text = listaVehiculos.ObtenerLista();
            };
            vboxVerVehiculos.PackStart(textViewVehiculos, true, true, 10);
            vboxVerVehiculos.PackStart(buttonVerVehiculos, false, false, 10);

            // Contenido de la pestaña "Editar Vehículo"
            Entry entryIdEditar = new Entry();
            Button buttonBuscarVehiculo = new Button("Buscar");
            Label labelIdUsuarioActual = new Label("ID Usuario: ");
            Entry entryIdUsuarioEditar = new Entry { Sensitive = false };
            Label labelMarcaActual = new Label("Marca: ");
            Entry entryMarcaEditar = new Entry { Sensitive = false };
            Label labelModeloActual = new Label("Modelo: ");
            Entry entryModeloEditar = new Entry { Sensitive = false };
            Label labelPlacaActual = new Label("Placa: ");
            Entry entryPlacaEditar = new Entry { Sensitive = false };
            Button buttonEditarVehiculo = new Button("Actualizar") { Sensitive = false };

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
                        Console.WriteLine("Vehículo no encontrado.");
                    }
                }
                else
                {
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

                        entryIdUsuarioEditar.Sensitive = false;
                        entryMarcaEditar.Sensitive = false;
                        entryModeloEditar.Sensitive = false;
                        entryPlacaEditar.Sensitive = false;
                        buttonEditarVehiculo.Sensitive = false;
                    }
                    else
                    {
                        Console.WriteLine("ID Usuario o Modelo inválido.");
                    }
                }
            };

            HBox hboxIdEditar = new HBox();
            hboxIdEditar.PackStart(new Label("ID:"), false, false, 10);
            hboxIdEditar.PackStart(entryIdEditar, true, true, 10);
            hboxIdEditar.PackStart(buttonBuscarVehiculo, false, false, 10);

            vboxEditarVehiculo.PackStart(hboxIdEditar, false, false, 10);
            vboxEditarVehiculo.PackStart(CreateLabeledEntryWithLabel("ID Usuario:", labelIdUsuarioActual, entryIdUsuarioEditar), false, false, 10);
            vboxEditarVehiculo.PackStart(CreateLabeledEntryWithLabel("Marca:", labelMarcaActual, entryMarcaEditar), false, false, 10);
            vboxEditarVehiculo.PackStart(CreateLabeledEntryWithLabel("Modelo:", labelModeloActual, entryModeloEditar), false, false, 10);
            vboxEditarVehiculo.PackStart(CreateLabeledEntryWithLabel("Placa:", labelPlacaActual, entryPlacaEditar), false, false, 10);
            vboxEditarVehiculo.PackStart(buttonEditarVehiculo, false, false, 10);

            // Contenido de la pestaña "Eliminar Vehículo"
            Entry entryIdEliminar = new Entry();
            Button buttonEliminarVehiculo = new Button("Eliminar Vehículo");
            buttonEliminarVehiculo.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEliminar.Text, out id))
                {
                    listaVehiculos.Eliminar(id);
                }
            };
            vboxEliminarVehiculo.PackStart(CreateLabeledEntry("ID:", entryIdEliminar), false, false, 10);
            vboxEliminarVehiculo.PackStart(buttonEliminarVehiculo, false, false, 10);

            // Agregar las pestañas al notebook
            notebook.AppendPage(vboxVerVehiculos, new Label("Ver Vehículos"));
            notebook.AppendPage(vboxEditarVehiculo, new Label("Editar Vehículo"));
            notebook.AppendPage(vboxEliminarVehiculo, new Label("Eliminar Vehículo"));

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
            Label label = new Label(labelText);
            hbox.PackStart(label, false, false, 10);
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