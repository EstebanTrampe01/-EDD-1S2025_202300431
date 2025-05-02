using Gtk;
using System;
using System.Collections.Generic;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using AutoGest.Interfaces;

namespace AutoGest
{
    public class InterfazMain : Window
    {
        // Contenedor principal donde se mostrarán los distintos paneles
        private VBox mainContainer;
        
        // ScrolledWindow para permitir desplazamiento vertical
        private ScrolledWindow scrolledWindow;
        
        // Referencia a los paneles activos
        private Widget currentPanel;
        
        // Instancias de las estructuras de datos
        private UserBlockchain listaUsuarios;
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolM arbolFacturas;
        private ArbolBinario arbolServicios;

        public InterfazMain() : base("AutoGest")
        {
            // Obtener referencias a las estructuras de datos
            this.listaUsuarios = Program.listaUsuarios;
            this.arbolRepuestos = Program.arbolRepuestos;
            this.listaVehiculos = Program.listaVehiculos;
            this.arbolFacturas = Program.arbolFacturas;
            this.arbolServicios = Program.arbolServicios;
            
            // Configurar la ventana principal
            SetDefaultSize(800, 600);
            SetPosition(WindowPosition.Center);
            DeleteEvent += OnDeleteEvent;

            // Crear el ScrolledWindow para soportar desplazamiento
            scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);  // Sólo permitir scroll vertical
            
            // Crear el contenedor principal
            mainContainer = new VBox();
            
            // Añadir el contenedor al ScrolledWindow
            scrolledWindow.Add(mainContainer);
            
            // Añadir el ScrolledWindow a la ventana principal
            Add(scrolledWindow);
            
            // Mostrar el panel de login inicialmente
            ShowLoginPanel();
            
            // Mostrar la ventana
            ShowAll();
        }

        private void OnDeleteEvent(object sender, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }

        // Método para cambiar el panel activo - ahora es público para que otras clases puedan llamarlo
        public void CambiarPanel(Widget newPanel)
        {
            // Eliminar el panel actual si existe
            if (currentPanel != null)
            {
                mainContainer.Remove(currentPanel);
                currentPanel = null;
            }

            // Añadir el nuevo panel
            if (newPanel != null)
            {
                mainContainer.PackStart(newPanel, true, true, 0);
                currentPanel = newPanel;
                newPanel.ShowAll(); // Asegurarse de que el nuevo panel sea visible
                
                // Desplazar al inicio cuando se cambia de panel
                scrolledWindow.Vadjustment.Value = 0;
            }

            // Actualizar la interfaz
            QueueDraw();
            ShowAll();
        }

        // Método para mostrar el panel de login
        public void ShowLoginPanel()
        {
            // Usar la clase Login existente como un panel
            var loginPanel = new Login(this);
            CambiarPanel(loginPanel);
        }
        
        // Método para mostrar el panel de menú de administrador
        public void ShowMenuPanel()
        {
            var menuPanel = new InterfazMenu(
                this,
                Program.listaUsuarios, 
                Program.arbolRepuestos, 
                Program.listaVehiculos, 
                Program.arbolFacturas, 
                Program.arbolServicios);
            CambiarPanel(menuPanel);
        }
        
        // Método para mostrar el panel de usuario
        public void ShowUserPanel(int idUsuario)
        {
            var userPanel = new InterfazUser(
                this,
                idUsuario,
                Program.listaUsuarios, 
                Program.listaVehiculos, 
                Program.arbolFacturas, 
                Program.arbolServicios);
            CambiarPanel(userPanel);
        }
        
        // Método para cerrar sesión y volver al login
        public void Logout()
        {
            ShowLoginPanel();
        }
        
        // Método para mostrar un mensaje de error/información/advertencia
        public void MostrarMensaje(string mensaje, MessageType tipo)
        {
            using (var md = new MessageDialog(this, 
                DialogFlags.DestroyWithParent, 
                tipo, 
                ButtonsType.Close, 
                mensaje))
            {
                md.Run();
                md.Destroy();
            }
        }
    }
}