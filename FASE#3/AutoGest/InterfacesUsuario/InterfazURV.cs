using Gtk;
using System;
using Vehiculos;
using AutoGest.Utils;

namespace AutoGest.InterfacesUsuario
{
    public unsafe class InterfazURV : Window
    {
        private int idUsuario;
        private ListaDoblementeEnlazada listaVehiculos;
        
        private Entry entryIdVehiculo, entryMarca, entryModelo, entryPlaca;
        private Label statusLabel;

        public InterfazURV(int idUsuario, ListaDoblementeEnlazada listaVehiculos) : base("Registrar Vehículo")
        {
            this.idUsuario = idUsuario;
            this.listaVehiculos = listaVehiculos;

            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);
            
            // Crear un contenedor principal con margen
            VBox vbox = new VBox(false, 10);
            vbox.BorderWidth = 20;

            // Título de la ventana
            Label labelTitulo = new Label("Registrar nuevo vehículo");
            labelTitulo.ModifyFont(Pango.FontDescription.FromString("Sans Bold 14"));
            vbox.PackStart(labelTitulo, false, false, 10);

            // Mostrar ID de usuario (no editable)
            HBox hboxIdUsuario = new HBox(false, 5);
            Label labelIdUsuario = new Label("ID Usuario:");
            Label labelIdUsuarioValor = new Label(idUsuario.ToString());
            labelIdUsuarioValor.ModifyFg(StateType.Normal, new Gdk.Color(0, 0, 200));
            hboxIdUsuario.PackStart(labelIdUsuario, false, false, 0);
            hboxIdUsuario.PackStart(labelIdUsuarioValor, false, false, 5);
            vbox.PackStart(hboxIdUsuario, false, false, 5);

            // Formulario para ingresar datos del vehículo
            Table formTable = new Table(4, 2, false);
            formTable.RowSpacing = 10;
            formTable.ColumnSpacing = 10;

            // ID Vehículo
            Label labelIdVehiculo = new Label("ID Vehículo:");
            labelIdVehiculo.SetAlignment(0, 0.5f);
            entryIdVehiculo = new Entry();
            formTable.Attach(labelIdVehiculo, 0, 1, 0, 1);
            formTable.Attach(entryIdVehiculo, 1, 2, 0, 1);

            // Marca
            Label labelMarca = new Label("Marca:");
            labelMarca.SetAlignment(0, 0.5f);
            entryMarca = new Entry();
            formTable.Attach(labelMarca, 0, 1, 1, 2);
            formTable.Attach(entryMarca, 1, 2, 1, 2);

            // Modelo
            Label labelModelo = new Label("Modelo (año):");
            labelModelo.SetAlignment(0, 0.5f);
            entryModelo = new Entry();
            formTable.Attach(labelModelo, 0, 1, 2, 3);
            formTable.Attach(entryModelo, 1, 2, 2, 3);

            // Placa
            Label labelPlaca = new Label("Placa:");
            labelPlaca.SetAlignment(0, 0.5f);
            entryPlaca = new Entry();
            formTable.Attach(labelPlaca, 0, 1, 3, 4);
            formTable.Attach(entryPlaca, 1, 2, 3, 4);

            vbox.PackStart(formTable, false, false, 10);

            // Etiqueta para mensajes de estado
            statusLabel = MessageManager.CreateStatusLabel();
            vbox.PackStart(statusLabel, false, false, 10);

            // Botones
            HBox hboxBotones = new HBox(true, 10);
            Button buttonRegistrar = new Button("Registrar Vehículo");
            Button buttonCancelar = new Button("Cancelar");

            buttonRegistrar.Clicked += OnRegistrarClicked;
            buttonCancelar.Clicked += (sender, e) => Destroy();

            hboxBotones.PackStart(buttonRegistrar, true, true, 0);
            hboxBotones.PackStart(buttonCancelar, true, true, 0);
            vbox.PackStart(hboxBotones, false, false, 10);

            Add(vbox);
            ShowAll();
        }

        private void OnRegistrarClicked(object sender, EventArgs e)
        {
            try
            {
                // Validar que todos los campos estén completos
                if (string.IsNullOrWhiteSpace(entryIdVehiculo.Text) || 
                    string.IsNullOrWhiteSpace(entryMarca.Text) ||
                    string.IsNullOrWhiteSpace(entryModelo.Text) ||
                    string.IsNullOrWhiteSpace(entryPlaca.Text))
                {
                    MessageManager.ShowMessage(statusLabel, "Todos los campos son obligatorios", MessageManager.MessageType.Warning);
                    return;
                }

                // Convertir ID y modelo a enteros
                if (!int.TryParse(entryIdVehiculo.Text, out int idVehiculo))
                {
                    MessageManager.ShowMessage(statusLabel, "El ID del vehículo debe ser un número entero", MessageManager.MessageType.Error);
                    return;
                }

                if (!int.TryParse(entryModelo.Text, out int modelo))
                {
                    MessageManager.ShowMessage(statusLabel, "El modelo debe ser un año válido (número entero)", MessageManager.MessageType.Error);
                    return;
                }

                // Verificar si ya existe un vehículo con este ID
                if (listaVehiculos.Buscar(idVehiculo) != null)
                {
                    MessageManager.ShowMessage(statusLabel, $"Ya existe un vehículo con ID {idVehiculo}", MessageManager.MessageType.Error);
                    return;
                }

                // Insertar el vehículo
                listaVehiculos.Insertar(idVehiculo, idUsuario, entryMarca.Text, modelo, entryPlaca.Text);

                // Mostrar mensaje de éxito
                MessageManager.ShowMessage(statusLabel, "Vehículo registrado exitosamente", MessageManager.MessageType.Success);

                // Limpiar los campos después de registrar
                entryIdVehiculo.Text = "";
                entryMarca.Text = "";
                entryModelo.Text = "";
                entryPlaca.Text = "";

                Console.WriteLine($"Vehículo registrado: ID={idVehiculo}, Marca={entryMarca.Text}, Modelo={modelo}, Placa={entryPlaca.Text}");
            }
            catch (Exception ex)
            {
                MessageManager.ShowMessage(statusLabel, $"Error al registrar vehículo: {ex.Message}", MessageManager.MessageType.Error);
                Console.WriteLine($"Error al registrar vehículo: {ex.Message}");
            }
        }
    }
}