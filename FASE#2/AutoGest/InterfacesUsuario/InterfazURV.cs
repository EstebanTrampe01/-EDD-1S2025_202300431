using Gtk;
using System;
using Vehiculos;

namespace AutoGest.InterfacesUsuario
{
    public unsafe class InterfazURV : Window
    {
        private int idUsuario;
        private ListaDoblementeEnlazada listaVehiculos;
        
        private Entry entryIdVehiculo, entryMarca, entryModelo, entryPlaca;

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
            labelIdUsuarioValor.ModifyFont(Pango.FontDescription.FromString("Sans Bold 10"));
            hboxIdUsuario.PackStart(labelIdUsuario, false, false, 10);
            hboxIdUsuario.PackStart(labelIdUsuarioValor, true, true, 10);
            vbox.PackStart(hboxIdUsuario, false, false, 10);

            // Crear los campos de formulario
            // ID del vehículo
            HBox hboxIdVehiculo = new HBox(false, 5);
            Label labelIdVehiculo = new Label("ID Vehículo:");
            entryIdVehiculo = new Entry();
            hboxIdVehiculo.PackStart(labelIdVehiculo, false, false, 10);
            hboxIdVehiculo.PackStart(entryIdVehiculo, true, true, 10);
            vbox.PackStart(hboxIdVehiculo, false, false, 10);

            // Marca
            HBox hboxMarca = new HBox(false, 5);
            Label labelMarca = new Label("Marca:");
            entryMarca = new Entry();
            hboxMarca.PackStart(labelMarca, false, false, 10);
            hboxMarca.PackStart(entryMarca, true, true, 10);
            vbox.PackStart(hboxMarca, false, false, 10);

            // Modelo
            HBox hboxModelo = new HBox(false, 5);
            Label labelModelo = new Label("Modelo:");
            entryModelo = new Entry();
            hboxModelo.PackStart(labelModelo, false, false, 10);
            hboxModelo.PackStart(entryModelo, true, true, 10);
            vbox.PackStart(hboxModelo, false, false, 10);

            // Placa
            HBox hboxPlaca = new HBox(false, 5);
            Label labelPlaca = new Label("Placa:");
            entryPlaca = new Entry();
            hboxPlaca.PackStart(labelPlaca, false, false, 10);
            hboxPlaca.PackStart(entryPlaca, true, true, 10);
            vbox.PackStart(hboxPlaca, false, false, 10);

            // Botón de registro
            Button buttonRegistrar = new Button("Registrar Vehículo");
            buttonRegistrar.Clicked += RegistrarVehiculo;
            vbox.PackStart(buttonRegistrar, false, false, 10);

            Add(vbox);
            ShowAll();
        }

        private void RegistrarVehiculo(object sender, EventArgs e)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(entryIdVehiculo.Text) ||
                string.IsNullOrWhiteSpace(entryMarca.Text) ||
                string.IsNullOrWhiteSpace(entryModelo.Text) ||
                string.IsNullOrWhiteSpace(entryPlaca.Text))
            {
                MostrarError("Todos los campos son obligatorios.");
                return;
            }

            // Convertir y validar ID del vehículo
            if (!int.TryParse(entryIdVehiculo.Text, out int idVehiculo))
            {
                MostrarError("El ID del vehículo debe ser un número entero.");
                return;
            }

            // Convertir y validar modelo
            if (!int.TryParse(entryModelo.Text, out int modelo))
            {
                MostrarError("El modelo debe ser un número entero que represente el año.");
                return;
            }

            // Verificar si ya existe un vehículo con ese ID
            if (listaVehiculos.Buscar(idVehiculo) != null)
            {
                MostrarError("Ya existe un vehículo con ese ID. Por favor, utilice otro ID.");
                return;
            }

            // Registrar el vehículo en la lista
            listaVehiculos.Insertar(idVehiculo, idUsuario, entryMarca.Text, modelo, entryPlaca.Text);
            
            // Mostrar mensaje de éxito
            MessageDialog md = new MessageDialog(this, 
                DialogFlags.DestroyWithParent, 
                MessageType.Info, 
                ButtonsType.Ok, 
                "Vehículo registrado correctamente");
            md.Run();
            md.Destroy();

            // Limpiar los campos para permitir registrar otro vehículo
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            entryIdVehiculo.Text = "";
            entryMarca.Text = "";
            entryModelo.Text = "";
            entryPlaca.Text = "";
        }

        private void MostrarError(string mensaje)
        {
            MessageDialog md = new MessageDialog(this, 
                DialogFlags.DestroyWithParent, 
                MessageType.Error, 
                ButtonsType.Close, 
                mensaje);
            md.Run();
            md.Destroy();
        }
    }
}