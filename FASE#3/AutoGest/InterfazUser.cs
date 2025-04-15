using Gtk;
using System;
using Usuarios;
using Repuestos;
using Vehiculos;
using Facturas;
using Servicios;
using AutoGest.Interfaces;
using AutoGest;
using AutoGest.InterfacesUsuario;

namespace AutoGest.Interfaces
{
    public class InterfazUser : Box
    {
        private InterfazMain mainWindow;
        private int idUsuario;
        private UserBlockchain listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;
        private ArbolB arbolFacturas;
        private ArbolBinario arbolServicios;
        private string correoUsuarioActual;

        public InterfazUser(
            InterfazMain mainWindow,
            int idUsuario,
            UserBlockchain listaUsuarios,
            ListaDoblementeEnlazada listaVehiculos,
            ArbolB arbolFacturas,
            ArbolBinario arbolServicios)
        {
            this.mainWindow = mainWindow;
            this.idUsuario = idUsuario;
            this.listaUsuarios = listaUsuarios;
            this.listaVehiculos = listaVehiculos;
            this.arbolFacturas = arbolFacturas;
            this.arbolServicios = arbolServicios;

            // Configurar el espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;

            // Obtener información del usuario actual
            string nombreUsuario = "Usuario";
            unsafe
            {
                Usuario usuario = listaUsuarios.Buscar(idUsuario);
                if (usuario != null)
                {
                    nombreUsuario = usuario.Name;
                    correoUsuarioActual = usuario.Correo;
                }
            }

            // Título del panel con nombre de usuario y mejor formato
            Label labelBienvenida = new Label();
            labelBienvenida.Markup = $"<span font='16' weight='bold'>Bienvenido, {nombreUsuario}</span>";
            labelBienvenida.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelBienvenida, false, false, 20);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Crear grid para organizar los botones en 2 columnas
            Table buttonsGrid = new Table(2, 2, true);
            buttonsGrid.RowSpacing = 15;
            buttonsGrid.ColumnSpacing = 15;
            
            // Botones con mejor estilo y organización
            Button buttonRegistrarVehiculo = CreateStyledButton("Registrar Vehículo");
            buttonRegistrarVehiculo.Clicked += delegate { 
                Console.WriteLine("Opción: Registrar Vehículo seleccionada"); 
                InterfazURV interfazURV = new InterfazURV(idUsuario, listaVehiculos);
                interfazURV.ShowAll();
            };
            buttonsGrid.Attach(buttonRegistrarVehiculo, 0, 1, 0, 1);

            Button buttonVerServicios = CreateStyledButton("Servicios Activos");
            buttonVerServicios.Clicked += delegate { 
                Console.WriteLine("Opción: Servicios Activos seleccionada"); 
                InterfazUSA interfazUSA = new InterfazUSA(idUsuario, listaVehiculos, arbolServicios);
                interfazUSA.ShowAll();
            };
            buttonsGrid.Attach(buttonVerServicios, 1, 2, 0, 1);

            Button buttonMisFacturas = CreateStyledButton("Facturas Activas");
            buttonMisFacturas.Clicked += delegate { 
                Console.WriteLine("Opción: Facturas Activas seleccionada"); 
                InterfazUFA interfazUFA = new InterfazUFA(idUsuario, listaVehiculos, arbolFacturas, arbolServicios);
                interfazUFA.ShowAll();
            };
            buttonsGrid.Attach(buttonMisFacturas, 0, 1, 1, 2);

            Button buttonCancelarFactura = CreateStyledButton("Cancelar Factura");
            buttonCancelarFactura.Clicked += delegate { 
                Console.WriteLine("Opción: Cancelar Factura seleccionada"); 
                InterfazUCF interfazUCF = new InterfazUCF(idUsuario, listaVehiculos, arbolFacturas, arbolServicios);
                interfazUCF.ShowAll();
            };
            buttonsGrid.Attach(buttonCancelarFactura, 1, 2, 1, 2);
            
            // Añadir el grid de botones al contenedor principal
            contentBox.PackStart(buttonsGrid, true, true, 0);
            
            // Separador antes del botón de cerrar sesión
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 10);

            // Botón para cerrar sesión con estilo diferenciado
            Button buttonCerrarSesion = CreateStyledButton("Cerrar Sesión");
            buttonCerrarSesion.ModifyBg(StateType.Normal, new Gdk.Color(255, 200, 200));
            buttonCerrarSesion.Clicked += delegate {
                Console.WriteLine("Cerrando sesión de usuario...");
                
                // Registrar salida del usuario
                if (!string.IsNullOrEmpty(correoUsuarioActual))
                {
                    RegistrosLoginManager.RegistrarSalida(correoUsuarioActual);
                    Console.WriteLine($"Salida de usuario {correoUsuarioActual} registrada");
                }
                
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
        
        // Método para convertir char* a string
        private unsafe string GetFixedString(char* fixedStr)
        {
            string str = "";
            for (int i = 0; fixedStr[i] != '\0'; i++)
            {
                str += fixedStr[i];
            }
            return str;
        }
    }
}