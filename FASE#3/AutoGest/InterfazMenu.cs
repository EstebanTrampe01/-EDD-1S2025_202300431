using Gtk;
using System;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using AutoGest.Interfaces;
using AutoGest;

namespace AutoGest.Interfaces
{
    public class InterfazMenu : Box
    {

        private GrafoServicios grafoServicios = new GrafoServicios();
        private InterfazMain mainWindow;
        private UserBlockchain listaUsuarios;
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolM arbolFacturas;
        private ArbolBinario arbolServicios;

        public InterfazMenu(
            InterfazMain mainWindow,
            UserBlockchain listaUsuarios,
            ArbolAVL arbolRepuestos,
            ListaDoblementeEnlazada listaVehiculos,
            ArbolM arbolFacturas,
            ArbolBinario arbolServicios)
        {
            this.mainWindow = mainWindow;
            this.listaUsuarios = listaUsuarios;
            this.arbolRepuestos = arbolRepuestos;
            this.listaVehiculos = listaVehiculos;
            this.arbolFacturas = arbolFacturas;
            this.arbolServicios = arbolServicios;

            // Configurar el espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='18' weight='bold'>MENÚ ADMINISTRADOR</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Crear grid para organizar los botones en 2 columnas
            Table buttonsGrid = new Table(4, 2, true);
            buttonsGrid.RowSpacing = 15;
            buttonsGrid.ColumnSpacing = 15;
            
            // Crear botones con mejor estilo y tamaño uniforme
            // ...existing imports and class declaration...
            
            // Inside constructor, update button click handlers:
            
            Button button1 = CreateStyledButton("Cargas Masivas");
            button1.Clicked += delegate { 
                Console.WriteLine("Opción 1 seleccionada"); 
                InterfazCM interfazCM = new InterfazCM(
                    mainWindow,
                    listaUsuarios, 
                    listaVehiculos, 
                    arbolRepuestos,
                    arbolServicios,
                    arbolFacturas);
                mainWindow.CambiarPanel(interfazCM);
            };
            buttonsGrid.Attach(button1, 0, 1, 0, 1);
            
            // Repeat for other buttons...
            
            Button button2 = CreateStyledButton("Ingreso Individual");
            button2.Clicked += delegate { 
                Console.WriteLine("Opción 2 seleccionada"); 
                InterfazII interfazII = new InterfazII(
                    mainWindow,
                    listaUsuarios, 
                    listaVehiculos, 
                    arbolRepuestos,
                    arbolServicios,
                    arbolFacturas);
            mainWindow.CambiarPanel(interfazII);
            };
            buttonsGrid.Attach(button2, 1, 2, 0, 1);
            
            Button buttonControlLogueo = CreateStyledButton("Control de Logueo");
            buttonControlLogueo.Clicked += delegate { 
                Console.WriteLine("Opción: Control de Logueo seleccionada - Generando JSON directamente");
                ExportarRegistrosLogin();
            };
            buttonsGrid.Attach(buttonControlLogueo, 0, 1, 1, 2);
            
            Button button3 = CreateStyledButton("Gestión de Entidades");
            button3.Clicked += delegate { 
                Console.WriteLine("Opción 3 seleccionada"); 
                InterfazGE interfazGE = new InterfazGE(
                    mainWindow,
                    listaUsuarios, 
                    arbolRepuestos,
                    listaVehiculos);                
            mainWindow.CambiarPanel(interfazGE);
            };
            buttonsGrid.Attach(button3, 1, 2, 1, 2);
            
            Button button4 = CreateStyledButton("Generar Servicios");
            button4.Clicked += delegate { 
                Console.WriteLine("Opción 4 seleccionada"); 
                InterfazGS interfazGS = new InterfazGS(
                    mainWindow,
                    arbolRepuestos,
                    listaVehiculos,
                    arbolFacturas, 
                    arbolServicios
                    );               
            mainWindow.CambiarPanel(interfazGS);
            };
            buttonsGrid.Attach(button4, 0, 1, 2, 3);
            
            Button button5 = CreateStyledButton("Cancelar Factura");
            button5.Clicked += delegate { 
                Console.WriteLine("Opción 5 seleccionada"); 
                InterfazCF interfazCF = new InterfazCF(
                    mainWindow,
                    arbolFacturas);
                mainWindow.CambiarPanel(interfazCF);
            };
            buttonsGrid.Attach(button5, 1, 2, 2, 3);
            
            Button button6 = CreateStyledButton("Generar Reportes");
            button6.Clicked += delegate { 
                Console.WriteLine("Generando Reportes"); 

                // Actualizar y generar grafo de servicios
                var listaServicios = arbolServicios.ObtenerServicios(); // Debes implementar este método en ArbolBinario
                grafoServicios.ActualizarDesdeServicios(listaServicios);
                grafoServicios.GenerarReporteDOT(
                    "grafo_servicios", 
                    "JUAN ESTEBAN CHACON TRAMPE", 
                    "202300431"
                );

                arbolServicios.GenerarGrafico("Servicios.dot");
                arbolFacturas.GenerarGrafico("Facturacion.dot");
                arbolRepuestos.GenerarGrafico("Repuestos.dot");
                listaVehiculos.GenerarGrafico("Vehiculos.dot");
                listaUsuarios.GenerarGrafico("Usuarios.dot");
                
                Console.WriteLine("Reportes generados");
            };
            buttonsGrid.Attach(button6, 0, 1, 3, 4);

            // Botón: Generar Backups
            Button buttonGenBackup = CreateStyledButton("Generar Backups");
            buttonGenBackup.Clicked += delegate {
                Console.WriteLine("Opción: Generar Backups seleccionada");
                InterfazGenB interfazGenB = new InterfazGenB(mainWindow, listaUsuarios, arbolRepuestos, listaVehiculos, arbolFacturas, arbolServicios);
                mainWindow.CambiarPanel(interfazGenB);
            };
            buttonsGrid.Attach(buttonGenBackup, 1, 2, 3, 4);

            // Botón: Cargar Backups
            Button buttonCarBackup = CreateStyledButton("Cargar Backups");
            buttonCarBackup.Clicked += delegate {
                Console.WriteLine("Opción: Cargar Backups seleccionada");
                InterfazCarB interfazCarB = new InterfazCarB(mainWindow, listaUsuarios, arbolRepuestos, listaVehiculos, arbolFacturas, arbolServicios);
                mainWindow.CambiarPanel(interfazCarB);
            };
            buttonsGrid.Attach(buttonCarBackup, 0, 1, 4, 5);

            
            // Añadir el grid de botones al contenedor principal
            contentBox.PackStart(buttonsGrid, true, true, 0);
            
            // Separador antes del botón de cerrar sesión
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 10);
            
            // Botón para cerrar sesión al final
            Button buttonCerrarSesion = CreateStyledButton("Cerrar Sesión");
            buttonCerrarSesion.ModifyBg(StateType.Normal, new Gdk.Color(255, 200, 200));
            buttonCerrarSesion.Clicked += delegate {
                Console.WriteLine("Cerrando sesión de administrador...");
                
                // Registrar salida del administrador
                RegistrosLoginManager.RegistrarSalida("admin@usac.com");
                Console.WriteLine("Salida de administrador registrada");
                
                // Volver al panel de login
                mainWindow.ShowLoginPanel();
            };
            
            // Centrar el botón de cerrar sesión
            HBox logoutBox = new HBox();
            logoutBox.PackStart(new Label(""), true, true, 0);
            logoutBox.PackStart(buttonCerrarSesion, false, false, 0);
            logoutBox.PackStart(new Label(""), true, true, 0);
            contentBox.PackStart(logoutBox, false, false, 10);
            
            // Centrar todo el contenido en el panel principal
            HBox centeringBox = new HBox();
            centeringBox.PackStart(new Label(""), true, true, 0);
            centeringBox.PackStart(contentBox, false, false, 0);
            centeringBox.PackStart(new Label(""), true, true, 0);
            
            PackStart(centeringBox, true, true, 0);
        }
        
        // Método para crear botones con estilo unificado
        private Button CreateStyledButton(string label)
        {
            Button button = new Button(label);
            button.SetSizeRequest(180, 60);
            
            // Crear una etiqueta formateada para el texto del botón
            Label buttonLabel = new Label();
            buttonLabel.Markup = $"<span font='11' weight='bold'>{label}</span>";
            
            // Reemplazar la etiqueta predeterminada del botón
            button.Child.Destroy();
            button.Add(buttonLabel);
            
            return button;
        }
        
        // Método para exportar registros de login
        private void ExportarRegistrosLogin()
        {
            FileChooserDialog filechooser = new FileChooserDialog(
                "Guardar Registros de Login", 
                mainWindow, 
                FileChooserAction.Save,
                "Cancelar", ResponseType.Cancel,
                "Guardar", ResponseType.Accept);

            filechooser.CurrentName = "registros_login.json";

            if (filechooser.Run() == (int)ResponseType.Accept)
            {
                RegistrosLoginManager.ExportarRegistros(filechooser.Filename);
                
                MessageDialog md = new MessageDialog(
                    mainWindow,
                    DialogFlags.DestroyWithParent,
                    MessageType.Info,
                    ButtonsType.Ok,
                    $"Registros exportados exitosamente a:\n{filechooser.Filename}");
                md.Run();
                md.Destroy();
            }

            filechooser.Destroy();
        }
    }
}