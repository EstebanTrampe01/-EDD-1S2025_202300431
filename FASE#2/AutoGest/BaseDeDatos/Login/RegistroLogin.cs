using Gtk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Usuarios;

namespace AutoGest.Interfaces
{
    // Estructura para almacenar los registros de login/logout
    public class RegistroLogin
    {
        public string Usuario { get; set; }
        public string Entrada { get; set; }
        public string Salida { get; set; }

        public RegistroLogin(string usuario, string entrada, string salida = null)
        {
            Usuario = usuario;
            Entrada = entrada;
            Salida = salida;
        }
    }

    // Clase estática para gestionar los registros
    public static class RegistrosLoginManager
    {
        private static List<RegistroLogin> registros = new List<RegistroLogin>();

        // Registra la entrada de un usuario
        public static void RegistrarEntrada(string usuario)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
            registros.Add(new RegistroLogin(usuario, timestamp));
            Console.WriteLine($"Entrada registrada para {usuario} a las {timestamp}");
        }

        // Registra la salida de un usuario
        public static void RegistrarSalida(string usuario)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
            
            // Buscar el último registro del usuario sin salida
            for (int i = registros.Count - 1; i >= 0; i--)
            {
                if (registros[i].Usuario == usuario && registros[i].Salida == null)
                {
                    registros[i].Salida = timestamp;
                    Console.WriteLine($"Salida registrada para {usuario} a las {timestamp}");
                    return;
                }
            }
            
            // Si no se encontró un registro de entrada, crear uno nuevo con salida
            Console.WriteLine($"No se encontró registro de entrada para {usuario}, creando registro completo");
            registros.Add(new RegistroLogin(usuario, "Desconocido", timestamp));
        }

        // Exporta los registros a un archivo JSON
        public static void ExportarRegistros(string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(registros, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Registros exportados a {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar registros: {ex.Message}");
            }
        }

        // Obtiene todos los registros
        public static List<RegistroLogin> ObtenerRegistros()
        {
            return registros;
        }
    }

    // Interfaz para el Control de Logueo
    public class InterfazCL : Window
    {
        private ListaSimple<Usuario> listaUsuarios;
        private TextView textViewRegistros;

        public InterfazCL(ListaSimple<Usuario> listaUsuarios) : base("Control de Logueo")
        {
            this.listaUsuarios = listaUsuarios;

            SetDefaultSize(500, 400);
            SetPosition(WindowPosition.Center);

            VBox vbox = new VBox(false, 10);
            vbox.BorderWidth = 10;

            // Título
            Label labelTitulo = new Label("Control de Logueo - Registro de Entradas y Salidas");
            labelTitulo.ModifyFont(Pango.FontDescription.FromString("Sans Bold 14"));
            vbox.PackStart(labelTitulo, false, false, 10);

            // Área para mostrar los registros
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            textViewRegistros = new TextView();
            textViewRegistros.Editable = false;
            textViewRegistros.WrapMode = WrapMode.Word;
            scrolledWindow.Add(textViewRegistros);
            vbox.PackStart(scrolledWindow, true, true, 0);

            // Botones
            HBox hboxBotones = new HBox(true, 10);
            
            Button buttonActualizar = new Button("Actualizar Registros");
            buttonActualizar.Clicked += delegate
            {
                ActualizarVistaRegistros();
            };
            hboxBotones.PackStart(buttonActualizar, true, true, 0);
            
            Button buttonExportar = new Button("Exportar a JSON");
            buttonExportar.Clicked += delegate
            {
                ExportarRegistros();
            };
            hboxBotones.PackStart(buttonExportar, true, true, 0);
            
            vbox.PackStart(hboxBotones, false, false, 0);

            Add(vbox);
            ActualizarVistaRegistros();
            ShowAll();
        }

        private void ActualizarVistaRegistros()
        {
            var registros = RegistrosLoginManager.ObtenerRegistros();
            
            string texto = "";
            foreach (var registro in registros)
            {
                texto += $"Usuario: {registro.Usuario}\n";
                texto += $"Entrada: {registro.Entrada}\n";
                texto += $"Salida: {(registro.Salida != null ? registro.Salida : "Sesión activa")}\n";
                texto += "-------------------------------------\n";
            }
            
            if (string.IsNullOrEmpty(texto))
            {
                texto = "No hay registros de inicio de sesión.";
            }
            
            textViewRegistros.Buffer.Text = texto;
        }

        private void ExportarRegistros()
        {
            FileChooserDialog filechooser = new FileChooserDialog(
                "Guardar Registros", 
                this, 
                FileChooserAction.Save,
                "Cancelar", ResponseType.Cancel,
                "Guardar", ResponseType.Accept);

            filechooser.CurrentName = "registros_login.json";

            if (filechooser.Run() == (int)ResponseType.Accept)
            {
                RegistrosLoginManager.ExportarRegistros(filechooser.Filename);
                
                // Mostrar mensaje de éxito
                MessageDialog md = new MessageDialog(
                    this,
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