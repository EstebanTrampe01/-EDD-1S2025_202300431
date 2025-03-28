using Gtk;
using System;
using Usuarios;
using Vehiculos;
using Repuestos;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazII : Window
    {
        private VBox vbox;
        private HBox hboxUsuarios;
        private HBox hboxVehiculos;
        private HBox hboxRepuestos;
        private HBox hboxServicios;

        private Entry entryIdUsuario, entryNombres, entryApellidos, entryCorreo, entryContrasenia;
        private Entry entryIdVehiculo, entryIdUsuarioVehiculo, entryMarca, entryModelo, entryPlaca;
        private Entry entryIdRepuesto, entryRepuesto, entryDetalles, entryCosto;
        private Entry entryIdServicio, entryServicio, entryDetallesServicio, entryCostoServicio;

        private ListaSimple<Usuario> listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolAVL arbolRepuestos;
        private ArbolAVL listaServicios;

        public InterfazII(ListaSimple<Usuario> listaUsuarios, ListaDoblementeEnlazada listaVehiculos, ArbolAVL arbolRepuestos) : base("Ingreso de Datos")
        {
            this.listaUsuarios = listaUsuarios;
            this.listaVehiculos = listaVehiculos;
            this.arbolRepuestos = arbolRepuestos;
            this.listaServicios = arbolRepuestos;

            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);

            // Crear una barra de menú
            MenuBar menuBar = new MenuBar();

            // Crear los menús
            MenuItem menuUsuarios = new MenuItem("Usuarios");
            MenuItem menuVehiculos = new MenuItem("Vehículos");
            MenuItem menuRepuestos = new MenuItem("Repuestos");
            MenuItem menuServicios = new MenuItem("Servicios");

            // Agregar los menús a la barra de menú
            menuBar.Append(menuUsuarios);
            menuBar.Append(menuVehiculos);
            menuBar.Append(menuRepuestos);
            menuBar.Append(menuServicios);

            // Crear un contenedor para los elementos con márgenes
            vbox = new VBox();
            vbox.BorderWidth = 20; // Margen alrededor del contenedor

            // Crear etiquetas y campos de entrada para cada entidad
            hboxUsuarios = new HBox();
            VBox vboxUsuarios = new VBox();
            vboxUsuarios.PackStart(CreateLabeledEntry("ID:", out entryIdUsuario), false, false, 10);
            vboxUsuarios.PackStart(CreateLabeledEntry("Nombres:", out entryNombres), false, false, 10);
            vboxUsuarios.PackStart(CreateLabeledEntry("Apellidos:", out entryApellidos), false, false, 10);
            vboxUsuarios.PackStart(CreateLabeledEntry("Correo:", out entryCorreo), false, false, 10);
            vboxUsuarios.PackStart(CreateLabeledEntry("Contrasenia:", out entryContrasenia), false, false, 10);
            hboxUsuarios.PackStart(vboxUsuarios, true, true, 10);

            hboxVehiculos = new HBox();
            VBox vboxVehiculos = new VBox();
            vboxVehiculos.PackStart(CreateLabeledEntry("ID:", out entryIdVehiculo), false, false, 10);
            vboxVehiculos.PackStart(CreateLabeledEntry("ID Usuario:", out entryIdUsuarioVehiculo), false, false, 10);
            vboxVehiculos.PackStart(CreateLabeledEntry("Marca:", out entryMarca), false, false, 10);
            vboxVehiculos.PackStart(CreateLabeledEntry("Modelo:", out entryModelo), false, false, 10);
            vboxVehiculos.PackStart(CreateLabeledEntry("Placa:", out entryPlaca), false, false, 10);
            hboxVehiculos.PackStart(vboxVehiculos, true, true, 10);

            hboxRepuestos = new HBox();
            VBox vboxRepuestos = new VBox();
            vboxRepuestos.PackStart(CreateLabeledEntry("ID:", out entryIdRepuesto), false, false, 10);
            vboxRepuestos.PackStart(CreateLabeledEntry("Repuesto:", out entryRepuesto), false, false, 10);
            vboxRepuestos.PackStart(CreateLabeledEntry("Detalles:", out entryDetalles), false, false, 10);
            vboxRepuestos.PackStart(CreateLabeledEntry("Costo:", out entryCosto), false, false, 10);
            hboxRepuestos.PackStart(vboxRepuestos, true, true, 10);

            hboxServicios = new HBox();
            VBox vboxServicios = new VBox();
            vboxServicios.PackStart(CreateLabeledEntry("ID:", out entryIdServicio), false, false, 10);
            vboxServicios.PackStart(CreateLabeledEntry("Servicio:", out entryServicio), false, false, 10);
            vboxServicios.PackStart(CreateLabeledEntry("Detalles:", out entryDetallesServicio), false, false, 10);
            vboxServicios.PackStart(CreateLabeledEntry("Costo:", out entryCostoServicio), false, false, 10);
            hboxServicios.PackStart(vboxServicios, true, true, 10);

            // Crear el botón de guardar
            Button buttonGuardar = new Button("Guardar");
            buttonGuardar.Clicked += delegate {
                Console.WriteLine("Datos guardados:");
                if (vbox.Children[0] == hboxUsuarios)
                {
                    GuardarUsuario();
                }
                else if (vbox.Children[0] == hboxVehiculos)
                {
                    GuardarVehiculo();
                }
                else if (vbox.Children[0] == hboxRepuestos)
                {
                    GuardarRepuesto();
                }
                else if (vbox.Children[0] == hboxServicios)
                {
                    GuardarServicio();
                }
                LimpiarCampos();
            };

            // Agregar la barra de menú y el botón de guardar al contenedor principal
            VBox mainVBox = new VBox();
            mainVBox.PackStart(menuBar, false, false, 0);
            mainVBox.PackStart(vbox, true, true, 0);
            mainVBox.PackStart(buttonGuardar, false, false, 10);

            // Agregar el contenedor principal a la ventana
            Add(mainVBox);

            // Mostrar los campos de entrada correspondientes al seleccionar una opción del menú
            menuUsuarios.Activated += (sender, e) => { MostrarCampos(hboxUsuarios); LimpiarCampos(); };
            menuVehiculos.Activated += (sender, e) => { MostrarCampos(hboxVehiculos); LimpiarCampos(); };
            menuRepuestos.Activated += (sender, e) => { MostrarCampos(hboxRepuestos); LimpiarCampos(); };
            menuServicios.Activated += (sender, e) => { MostrarCampos(hboxServicios); LimpiarCampos(); };

            // Mostrar los campos de entrada de "Usuarios" por defecto
            MostrarCampos(hboxUsuarios);

            ShowAll();
        }

        private void GuardarUsuario()
        {
            int idUsuario = int.Parse(entryIdUsuario.Text);
            if (listaUsuarios.Buscar(idUsuario) != null)
            {
                Console.WriteLine("Error: Ya existe un usuario con esa ID.");
            }
            else
            {
                Usuario usuario = new Usuario(
                    idUsuario,
                    entryNombres.Text,
                    entryApellidos.Text,
                    entryCorreo.Text,
                    entryContrasenia.Text
                );
                listaUsuarios.Insertar(usuario);
                Console.WriteLine($"Usuarios: ID={usuario.id}, Nombres={usuario.ToString()}, Apellidos={usuario.ToString()}, Correo={usuario.ToString()}, Contrasenia={usuario.ToString()}");
            }
        }

        private void GuardarVehiculo()
        {
            int idVehiculo = int.Parse(entryIdVehiculo.Text);
            if (listaVehiculos.Buscar(idVehiculo) != null)
            {
                Console.WriteLine("Error: Ya existe un vehículo con esa ID.");
            }
            else
            {
                Vehiculo vehiculo = new Vehiculo(
                    idVehiculo,
                    int.Parse(entryIdUsuarioVehiculo.Text),
                    entryMarca.Text,
                    int.Parse(entryModelo.Text),
                    entryPlaca.Text
                );
                listaVehiculos.Insertar(vehiculo.Id, vehiculo.ID_Usuario, ConvertFixedCharArrayToString(vehiculo.Marca, 50), vehiculo.Modelo, ConvertFixedCharArrayToString(vehiculo.Placa, 100));
                Console.WriteLine($"Vehículos: ID={vehiculo.Id}, ID Usuario={vehiculo.ID_Usuario}, Marca={ConvertFixedCharArrayToString(vehiculo.Marca, 50)}, Modelo={vehiculo.Modelo}, Placa={ConvertFixedCharArrayToString(vehiculo.Placa, 100)}");
            }
        }
        
        private void GuardarRepuesto()
        {
            int idRepuesto = int.Parse(entryIdRepuesto.Text);
            if (arbolRepuestos.Buscar(idRepuesto) != null)
            {
                Console.WriteLine("Error: Ya existe un repuesto con esa ID.");
            }
            else
            {
                LRepuesto repuesto = new LRepuesto(
                    idRepuesto,
                    entryRepuesto.Text,
                    entryDetalles.Text,
                    double.Parse(entryCosto.Text)
                );
                arbolRepuestos.Insertar(repuesto.Id, ConvertFixedCharArrayToString(repuesto.Repuesto, 50), ConvertFixedCharArrayToString(repuesto.Detalles, 100), repuesto.Costo);
                Console.WriteLine($"Repuestos: ID={repuesto.Id}, Repuesto={ConvertFixedCharArrayToString(repuesto.Repuesto, 50)}, Detalles={ConvertFixedCharArrayToString(repuesto.Detalles, 100)}, Costo={repuesto.Costo}");
            }
        }
        
        private string ConvertFixedCharArrayToString(char* fixedCharArray, int length)
        {
            string result = "";
            for (int i = 0; i < length && fixedCharArray[i] != '\0'; i++)
            {
                result += fixedCharArray[i];
            }
            return result;
        }

        private void GuardarServicio()
        {
            int idServicio = int.Parse(entryIdServicio.Text);
            if (listaServicios.Buscar(idServicio) != null)
            {
                Console.WriteLine("Error: Ya existe un servicio con esa ID.");
            }
            else
            {
                Console.WriteLine("Servicios: ID=" + entryIdServicio.Text + ", Servicio=" + entryServicio.Text + ", Detalles=" + entryDetallesServicio.Text + ", Costo=" + entryCostoServicio.Text);
            }
        }

        private HBox CreateLabeledEntry(string labelText, out Entry entry)
        {
            HBox hbox = new HBox();
            Label label = new Label(labelText);
            entry = new Entry();
            hbox.PackStart(label, false, false, 10);
            hbox.PackStart(entry, true, true, 10);
            return hbox;
        }

        private void MostrarCampos(HBox hbox)
        {
            // Limpiar el contenedor principal
            vbox.Foreach(widget => vbox.Remove(widget));

            // Agregar el contenedor correspondiente
            vbox.PackStart(hbox, false, false, 10);
            vbox.ShowAll();
        }

        private void LimpiarCampos()
        {
            entryIdUsuario.Text = "";
            entryNombres.Text = "";
            entryApellidos.Text = "";
            entryCorreo.Text = "";
            entryContrasenia.Text = "";

            entryIdVehiculo.Text = "";
            entryIdUsuarioVehiculo.Text = "";
            entryMarca.Text = "";
            entryModelo.Text = "";
            entryPlaca.Text = "";

            entryIdRepuesto.Text = "";
            entryRepuesto.Text = "";
            entryDetalles.Text = "";
            entryCosto.Text = "";

            entryIdServicio.Text = "";
            entryServicio.Text = "";
            entryDetallesServicio.Text = "";
            entryCostoServicio.Text = "";
        }
    }
}