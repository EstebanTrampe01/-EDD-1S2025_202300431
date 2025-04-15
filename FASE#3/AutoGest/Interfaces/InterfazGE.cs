using Gtk;
using System;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using Usuarios;
using AutoGest;
using AutoGest.Interfaces;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazGE : Box
    {
        private InterfazMain mainWindow;
        private UserBlockchain listaUsuarios;
        private ArbolAVL arbolRepuestos;
        private ListaDoblementeEnlazada listaVehiculos;

        public InterfazGE(
            InterfazMain mainWindow,
            UserBlockchain listaUsuarios, 
            ArbolAVL arbolRepuestos, 
            ListaDoblementeEnlazada listaVehiculos)
        {
            this.mainWindow = mainWindow;
            this.listaUsuarios = listaUsuarios;
            this.arbolRepuestos = arbolRepuestos;
            this.listaVehiculos = listaVehiculos;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>GESTIÓN DE ENTIDADES</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Crear grid para organizar los botones en 1 columna
            Table buttonsGrid = new Table(3, 1, true);
            buttonsGrid.RowSpacing = 15;
            buttonsGrid.ColumnSpacing = 15;
            
            // Botón para Gestionar Usuarios
            Button buttonGestionUsuarios = CreateStyledButton("Gestionar Usuarios");
            buttonGestionUsuarios.Clicked += delegate { 
                Console.WriteLine("Opción: Gestionar Usuarios seleccionada"); 
                InterfazGU interfazGU = new InterfazGU(mainWindow, listaUsuarios, listaVehiculos);
                mainWindow.CambiarPanel(interfazGU);
            };
            buttonsGrid.Attach(buttonGestionUsuarios, 0, 1, 0, 1);
            
            // Botón para Gestionar Vehículos
            Button buttonGestionVehiculos = CreateStyledButton("Gestionar Vehículos");
            buttonGestionVehiculos.Clicked += delegate { 
                Console.WriteLine("Opción: Gestionar Vehículos seleccionada"); 
                InterfazGV interfazGV = new InterfazGV(mainWindow, listaVehiculos);
                mainWindow.CambiarPanel(interfazGV);
            };
            buttonsGrid.Attach(buttonGestionVehiculos, 0, 1, 1, 2);
            
            // Botón para Gestionar Repuestos
            Button buttonGestionRepuestos = CreateStyledButton("Gestionar Repuestos");
            buttonGestionRepuestos.Clicked += delegate { 
                Console.WriteLine("Opción: Gestionar Repuestos seleccionada"); 
                InterfazGR interfazGR = new InterfazGR(mainWindow, arbolRepuestos);
                mainWindow.CambiarPanel(interfazGR);
            };
            buttonsGrid.Attach(buttonGestionRepuestos, 0, 1, 2, 3);
            
            // Añadir el grid de botones al contenedor principal
            contentBox.PackStart(buttonsGrid, true, true, 0);
            
            // Separador antes del botón de volver
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 10);
            
            // Botón para volver al menú principal
            Button buttonVolver = new Button();
            Label volverLabel = new Label();
            volverLabel.Markup = "<span font='10'>Volver al Menú</span>";
            buttonVolver.Add(volverLabel);
            buttonVolver.SetSizeRequest(150, 35);
            buttonVolver.Clicked += delegate {
                Console.WriteLine("Volviendo al menú principal...");
                mainWindow.ShowMenuPanel();
            };
            
            // Centrar el botón de volver
            HBox volverBox = new HBox();
            volverBox.PackStart(new Label(""), true, true, 0);
            volverBox.PackStart(buttonVolver, false, false, 0);
            volverBox.PackStart(new Label(""), true, true, 0);
            contentBox.PackStart(volverBox, false, false, 10);
            
            // Centrar el contenido en el panel
            HBox centeringBox = new HBox();
            centeringBox.PackStart(new Label(""), true, true, 0);
            centeringBox.PackStart(contentBox, false, false, 0);
            centeringBox.PackStart(new Label(""), true, true, 0);
            
            PackStart(centeringBox, true, true, 0);
        }
        
        // Método para crear botones con estilo unificado
        private Button CreateStyledButton(string label)
        {
            Button button = new Button();
            button.SetSizeRequest(200, 60);
            
            // Crear una etiqueta formateada para el texto del botón
            Label buttonLabel = new Label();
            buttonLabel.Markup = $"<span font='11' weight='bold'>{label}</span>";
            
            // Añadir la etiqueta al botón
            button.Add(buttonLabel);
            
            return button;
        }
    }
}