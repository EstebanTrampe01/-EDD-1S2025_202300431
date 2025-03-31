using Gtk;
using System;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using AutoGest.Interfaces;
using System.Collections.Generic;
using AutoGest;

public class Login : Box
{
    private Entry entryCorreo;
    private Entry entryContrasena;
    private InterfazMain mainWindow;
    private Label labelTitulo;
    
    // Constructor para cuando se usa como panel dentro de InterfazMain
    public Login(InterfazMain mainWindow)
    {
        this.mainWindow = mainWindow;
        
        // Configurar espaciado y bordes
        BorderWidth = 20;
        Spacing = 15;
        
        // Usar un contenedor vertical centralizado
        VBox contentBox = new VBox(false, 10);
        contentBox.BorderWidth = 20;
        
        // Crear el título con mejor formato
        labelTitulo = new Label();
        labelTitulo.Markup = "<span font='16' weight='bold'>INICIO DE SESIÓN</span>";
        labelTitulo.SetAlignment(0.5f, 0.5f); // Centrar el título
        contentBox.PackStart(labelTitulo, false, false, 20);
        
        // Crear el campo de correo con mejor organización
        Frame frameCorreo = new Frame("Correo electrónico");
        entryCorreo = new Entry();
        entryCorreo.WidthChars = 30;
        entryCorreo.SetSizeRequest(300, 30);
        entryCorreo.Changed += OnEntryChanged;
        frameCorreo.Add(entryCorreo);
        contentBox.PackStart(frameCorreo, false, false, 10);
        
        // Crear el campo de contraseña con mejor organización
        Frame frameContrasena = new Frame("Contraseña");
        entryContrasena = new Entry { Visibility = false };
        entryContrasena.ActivatesDefault = true;
        entryContrasena.WidthChars = 30;
        entryContrasena.SetSizeRequest(300, 30);
        entryContrasena.Changed += OnEntryChanged;
        frameContrasena.Add(entryContrasena);
        contentBox.PackStart(frameContrasena, false, false, 10);
        
        // Crear el botón de validar con mejor formato
        Button buttonValidar = new Button("Iniciar Sesión");
        buttonValidar.SetSizeRequest(200, 40);
        buttonValidar.Clicked += OnValidarClicked;
        buttonValidar.CanDefault = true;
        
        // Centrar el botón
        HBox buttonBox = new HBox();
        buttonBox.PackStart(new Label(""), true, true, 0);
        buttonBox.PackStart(buttonValidar, false, false, 0);
        buttonBox.PackStart(new Label(""), true, true, 0);
        contentBox.PackStart(buttonBox, false, false, 20);
        
        // Centrar el contenido en el panel
        HBox centeringBox = new HBox();
        centeringBox.PackStart(new Label(""), true, true, 0);
        centeringBox.PackStart(contentBox, false, false, 0);
        centeringBox.PackStart(new Label(""), true, true, 0);
        
        PackStart(centeringBox, true, true, 0);
    }

    // Método para deshabilitar el botón si los campos están vacíos
    private void OnEntryChanged(object sender, EventArgs e)
    {
        // Puedes agregar lógica adicional para validación aquí
    }
    
    // Método para manejar el click en el botón de validar
    private void OnValidarClicked(object sender, EventArgs e)
    {
        try
        {
            string correo = entryCorreo.Text;
            string contrasena = entryContrasena.Text;

            bool esAdmin = (correo == "admin@usac.com" && contrasena == "admin123");
            bool esUsuarioRegistrado = false;
            int idUsuario = -1;

            // Verificar si es un usuario registrado
            if (!esAdmin && Program.listaUsuarios != null)
            {
                (esUsuarioRegistrado, idUsuario) = Program.listaUsuarios.VerificarCredencialesConId(correo, contrasena);
                
                if (esUsuarioRegistrado)
                {
                    Console.WriteLine($"Usuario autenticado: {correo} con ID: {idUsuario}");
                }
            }

            if (esAdmin)
            {
                // Registrar entrada del administrador
                RegistrosLoginManager.RegistrarEntrada("admin@usac.com");
                Console.WriteLine("Entrada de administrador registrada");
                
                // Cambiar al panel de menú en la ventana principal
                if (mainWindow != null)
                {
                    LimpiarPanelActual();
                    InterfazMenu interfazMenu = new InterfazMenu(
                        mainWindow,
                        Program.listaUsuarios, 
                        Program.arbolRepuestos, 
                        Program.listaVehiculos, 
                        Program.arbolFacturas, 
                        Program.arbolServicios);
                    mainWindow.CambiarPanel(interfazMenu);
                }
                else
                {
                    // Código de compatibilidad para la transición
                    InterfazMenu interfazMenu = new InterfazMenu(
                        mainWindow,
                        Program.listaUsuarios, 
                        Program.arbolRepuestos, 
                        Program.listaVehiculos, 
                        Program.arbolFacturas, 
                        Program.arbolServicios);
                    interfazMenu.ShowAll();
                    if (this.Parent is Window window)
                    {
                        window.Destroy();
                    }
                }
            }
            else if (esUsuarioRegistrado && idUsuario != -1)
            {
                // Registrar entrada del usuario
                RegistrosLoginManager.RegistrarEntrada(correo);
                Console.WriteLine($"Entrada de usuario {correo} registrada");
                
                // Cambiar al panel de usuario en la ventana principal
                if (mainWindow != null)
                {
                    LimpiarPanelActual();
                    InterfazUser interfazUser = new InterfazUser(
                        mainWindow,
                        idUsuario,
                        Program.listaUsuarios, 
                        Program.listaVehiculos, 
                        Program.arbolFacturas, 
                        Program.arbolServicios);
                    mainWindow.CambiarPanel(interfazUser);
                }
                else
                {
                    // Código de compatibilidad para la transición
                    InterfazUser interfazUser = new InterfazUser(
                        mainWindow,
                        idUsuario,
                        Program.listaUsuarios, 
                        Program.listaVehiculos, 
                        Program.arbolFacturas, 
                        Program.arbolServicios);
                    interfazUser.ShowAll();
                    if (this.Parent is Window window)
                    {
                        window.Destroy();
                    }
                }
            }
            else
            {
                // Credenciales incorrectas
                MostrarError("Correo o contraseña incorrectos");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en el proceso de validación: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            MostrarError("Error al procesar la solicitud");
        }
    }

    // Método para limpiar el panel actual antes de cambiar a otro panel
    private void LimpiarPanelActual()
    {
        // Limpiar los campos de texto
        entryCorreo.Text = string.Empty;
        entryContrasena.Text = string.Empty;
        
        // Ocultar todos los widgets en este panel
        foreach (Widget widget in this.Children)
        {
            this.Remove(widget);
        }
    }

    // Método para mostrar mensajes de error
    private void MostrarError(string mensaje)
    {
        Window parentWindow = mainWindow != null ? mainWindow as Window : (this.Parent as Window);
        
        using (var md = new MessageDialog(parentWindow, 
            DialogFlags.DestroyWithParent, 
            MessageType.Error, 
            ButtonsType.Close, 
            mensaje))
        {
            md.Run();
            md.Destroy();
        }
    }
}