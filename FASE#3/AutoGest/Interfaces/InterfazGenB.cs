using Gtk;
using System;
using Backup;
using Usuarios;
using Vehiculos;
using Repuestos;
using Facturas;
using Servicios;

namespace AutoGest.Interfaces
{
    public class InterfazGenB : Box
    {
        private InterfazMain mainWindow;
        private UserBlockchain listaUsuarios;
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolM arbolFacturas;
        private ArbolBinario arbolServicios;

        public InterfazGenB(
            InterfazMain mainWindow,
            UserBlockchain listaUsuarios,
            ArbolAVL arbolRepuestos,
            ListaDoblementeEnlazada listaVehiculos,
            ArbolM arbolFacturas,
            ArbolBinario arbolServicios) : base(Orientation.Vertical, 0)
        {
            this.mainWindow = mainWindow;
            this.listaUsuarios = listaUsuarios;
            this.arbolRepuestos = arbolRepuestos;
            this.listaVehiculos = listaVehiculos;
            this.arbolFacturas = arbolFacturas;
            this.arbolServicios = arbolServicios;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;

            // Título del panel
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>Generar Backup</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            PackStart(labelTitulo, false, false, 20);

            // Botón para generar backup
            Button buttonGenerarBackup = new Button("Generar Backup");
            buttonGenerarBackup.Clicked += delegate {
                BackupManager backupManager = new BackupManager();
                backupManager.GenerateBackup(listaUsuarios, listaVehiculos, arbolRepuestos);
                
                MessageDialog md = new MessageDialog(
                    mainWindow,
                    DialogFlags.DestroyWithParent,
                    MessageType.Info,
                    ButtonsType.Ok,
                    "Backup generado exitosamente");
                md.Run();
                md.Destroy();
                
                Console.WriteLine("Backup generado exitosamente.");
            };
            PackStart(buttonGenerarBackup, false, false, 10);

            // Botón para volver al menú
            Button buttonVolver = new Button("Volver al Menú");
            buttonVolver.Clicked += delegate {
                mainWindow.ShowMenuPanel();
            };
            PackStart(buttonVolver, false, false, 10);
        }
    }
}