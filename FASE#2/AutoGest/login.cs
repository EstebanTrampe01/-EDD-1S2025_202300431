using Gtk;
using System;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;

public class Login : Window
{
    public Login() : base("LOGIN")
    {
        SetDefaultSize(300, 200);
        SetPosition(WindowPosition.Center);

        VBox vbox = new VBox(false, 10);

        // Crear el label superior
        Label labelTitulo = new Label("INICIO DE SESION");
        vbox.PackStart(labelTitulo, false, false, 0);

        // Crear el label y el entry para el correo
        HBox hboxCorreo = new HBox(false, 5);
        Label labelCorreo = new Label("Correo:");
        Entry entryCorreo = new Entry();
        hboxCorreo.PackStart(labelCorreo, false, false, 10);
        hboxCorreo.PackStart(entryCorreo, true, true, 10);

        // Crear el label y el entry para la contraseña
        HBox hboxContrasena = new HBox(false, 5);
        Label labelContrasena = new Label("Contraseña:");
        Entry entryContrasena = new Entry();
        entryContrasena.Visibility = false; // Ocultar la contraseña
        hboxContrasena.PackStart(labelContrasena, false, false, 10);
        hboxContrasena.PackStart(entryContrasena, true, true, 10);

        // Crear el botón de validar
        Button buttonValidar = new Button("Validar");
        buttonValidar.Clicked += (sender, e) => {
            // Lógica de validación
            string correo = entryCorreo.Text;
            string contrasena = entryContrasena.Text;

            bool esAdmin = (correo == "1" && contrasena == "1");
            bool esUsuarioRegistrado = false;
            int idUsuario = -1;

            // Verificar si es un usuario registrado (si ya se han cargado usuarios)
            if (!esAdmin && Program.listaUsuarios != null)
            {
                // Usar el método VerificarCredencialesConId de la lista de usuarios
                (esUsuarioRegistrado, idUsuario) = Program.listaUsuarios.VerificarCredencialesConId(correo, contrasena);
                
                if (esUsuarioRegistrado)
                {
                    Console.WriteLine($"Usuario autenticado: {correo} con ID: {idUsuario}");
                }
            }

            if (esAdmin)
            {
                // Mostrar la interfaz de menú para administradores
                InterfazMenu interfazMenu = new InterfazMenu(
                    Program.listaUsuarios, 
                    Program.arbolRepuestos, 
                    Program.listaVehiculos, 
                    Program.arbolFacturas, 
                    Program.arbolServicios);
                
                interfazMenu.ShowAll();
                this.Destroy(); // Cerrar la ventana de login
            }
            else if (esUsuarioRegistrado && idUsuario != -1)
            {
                // Mostrar la interfaz de usuario para usuarios normales
                InterfazUser interfazUser = new InterfazUser(
                    idUsuario,
                    Program.listaUsuarios, 
                    Program.listaVehiculos, 
                    Program.arbolFacturas, 
                    Program.arbolServicios);
                
                interfazUser.ShowAll();
                this.Destroy(); // Cerrar la ventana de login
            }
            else
            {
                // Mostrar un mensaje de error si las credenciales son incorrectas
                MessageDialog md = new MessageDialog(this, 
                    DialogFlags.DestroyWithParent, MessageType.Error, 
                    ButtonsType.Close, "Correo o contraseña incorrectos");
                md.Run();
                md.Destroy();
            }
        };

        vbox.PackStart(hboxCorreo, false, false, 10);
        vbox.PackStart(hboxContrasena, false, false, 10);
        vbox.PackStart(buttonValidar, false, false, 10);

        // Añadir el evento DeleteEvent para manejar el cierre de ventana
        DeleteEvent += (o, args) => {
            Application.Quit();
            args.RetVal = true;
        };

        Add(vbox);
        ShowAll();
    }
}