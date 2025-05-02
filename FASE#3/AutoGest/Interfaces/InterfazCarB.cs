using Gtk;
using System;
using System.IO;
using Backup;
using Usuarios;
using Vehiculos;
using Repuestos;
using Facturas;
using Servicios;
using AutoGest;

namespace AutoGest.Interfaces
{
    public class InterfazCarB : Box
    {
        private InterfazMain mainWindow;
        private UserBlockchain listaUsuarios;
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolM arbolFacturas;
        private ArbolBinario arbolServicios;

        public InterfazCarB(
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

            BorderWidth = 20;
            Spacing = 15;

            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>Cargar Backup</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            PackStart(labelTitulo, false, false, 20);

            Label labelInstrucciones = new Label("Seleccione un archivo de backup individual para cargar (usuarios, vehículos o repuestos).");
            labelInstrucciones.LineWrap = true;
            labelInstrucciones.MaxWidthChars = 40;
            PackStart(labelInstrucciones, false, false, 10);

            // Botón para seleccionar archivo
            Button buttonSeleccionarArchivo = new Button("Seleccionar Archivo...");
            buttonSeleccionarArchivo.Clicked += OnSeleccionarArchivoClicked;
            PackStart(buttonSeleccionarArchivo, false, false, 10);

            // Botón para volver al menú
            Button buttonVolver = new Button("Volver al Menú");
            buttonVolver.Clicked += delegate {
                mainWindow.ShowMenuPanel();
            };
            PackStart(buttonVolver, false, false, 10);
        }

        private void OnSeleccionarArchivoClicked(object sender, EventArgs e)
        {
            FileChooserDialog filechooser = new FileChooserDialog(
                "Seleccionar archivo de backup",
                mainWindow,
                FileChooserAction.Open,
                "Cancelar", ResponseType.Cancel,
                "Abrir", ResponseType.Accept);

            FileFilter filter = new FileFilter();
            filter.Name = "Archivos de Backup";
            filter.AddPattern("*.json");
            filter.AddPattern("*.edd");
            filechooser.AddFilter(filter);

            if (filechooser.Run() == (int)ResponseType.Accept)
            {
                string filePath = filechooser.Filename;
                filechooser.Destroy();

                try
                {
                    BackupManager backupManager = new BackupManager();
                    bool resultado = backupManager.LoadBackupFromPath(
                        filePath, 
                        listaUsuarios, 
                        listaVehiculos, 
                        arbolRepuestos);

                    MessageDialog md = new MessageDialog(
                        mainWindow,
                        DialogFlags.DestroyWithParent,
                        resultado ? MessageType.Info : MessageType.Warning,
                        ButtonsType.Ok,
                        resultado ? "Backup cargado exitosamente." : "No se pudo cargar el backup seleccionado.");
                    md.Run();
                    md.Destroy();
                }
                catch (Exception ex)
                {
                    MessageDialog md = new MessageDialog(
                        mainWindow,
                        DialogFlags.DestroyWithParent,
                        MessageType.Error,
                        ButtonsType.Ok,
                        $"Error al cargar backup: {ex.Message}");
                    md.Run();
                    md.Destroy();
                }
            }
            else
            {
                filechooser.Destroy();
            }
        }
    }
}