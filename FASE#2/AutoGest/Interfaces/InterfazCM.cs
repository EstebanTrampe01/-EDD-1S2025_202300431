using Gtk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Usuarios;
using Vehiculos;
using Repuestos;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazCM : Window
    {
        private ListaSimple<Usuario> listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;
        private ListaCircular listaRepuestos;


        public InterfazCM(ListaSimple<Usuario> listaUsuarios, ListaDoblementeEnlazada listaVehiculos, ListaCircular listaRepuestos) : base("Cargas Masivas")
        {
            this.listaUsuarios = listaUsuarios;
            this.listaVehiculos = listaVehiculos;
            this.listaRepuestos = listaRepuestos;

            SetDefaultSize(300, 200);
            SetPosition(WindowPosition.Center);

            VBox vbox = new VBox();
            vbox.BorderWidth = 20;

            ComboBoxText comboBox = new ComboBoxText();
            comboBox.AppendText("Usuarios");
            comboBox.AppendText("Vehículos");
            comboBox.AppendText("Repuestos");
            vbox.PackStart(comboBox, false, false, 10);

            Button buttonCargar = new Button("Cargar");
            buttonCargar.Clicked += delegate {
                string selectedOption = comboBox.ActiveText;
                Console.WriteLine("Opción seleccionada: " + selectedOption);

                FileChooserDialog filechooser = new FileChooserDialog("Seleccione un archivo",
                    this,
                    FileChooserAction.Open,
                    "Cancelar", ResponseType.Cancel,
                    "Abrir", ResponseType.Accept);

                if (filechooser.Run() == (int)ResponseType.Accept)
                {
                    try
                    {
                        Console.WriteLine($"Archivo seleccionado: {filechooser.Filename}");

                        string contenido = File.ReadAllText(filechooser.Filename);

                        JsonDocument doc = JsonDocument.Parse(contenido);
                        JsonElement root = doc.RootElement;

                        if (root.ValueKind == JsonValueKind.Array)
                        {
                            if (selectedOption == "Usuarios")
                            {
                                foreach (JsonElement element in root.EnumerateArray())
                                {
                                    int id = element.GetProperty("ID").GetInt32();
                                    string name = element.GetProperty("Nombres").GetString();
                                    string lastName = element.GetProperty("Apellidos").GetString();
                                    string correo = element.GetProperty("Correo").GetString();
                                    string contrasena = element.GetProperty("Contrasenia").GetString(); // Nuevo campo

                                    Usuario usuario = new Usuario(id, name, lastName, correo, contrasena);
                                    Console.WriteLine($"ID: {usuario.id}, Nombres: {name}, Apellidos: {lastName}, Correo: {correo}, Contraseña: {contrasena}");
                                    listaUsuarios.Insertar(usuario);
                                }
                            }
                            else if (selectedOption == "Vehículos")
                            {
                                foreach (JsonElement element in root.EnumerateArray())
                                {
                                    int id = element.GetProperty("ID").GetInt32();
                                    int idUsuario = element.GetProperty("ID_Usuario").GetInt32();
                                    string marca = element.GetProperty("Marca").GetString();
                                    int modelo = element.GetProperty("Modelo").GetInt32();
                                    string placa = element.GetProperty("Placa").GetString();

                                    Vehiculo vehiculo = new Vehiculo(id, idUsuario, marca, modelo, placa);
                                    Console.WriteLine($"ID: {vehiculo.Id}, ID Usuario: {idUsuario}, Marca: {marca}, Modelo: {modelo}, Placa: {placa}");
                                    listaVehiculos.Insertar(id, idUsuario, marca, modelo, placa);
                                }
                            }
                            else if (selectedOption == "Repuestos")
                            {
                                foreach (JsonElement element in root.EnumerateArray())
                                {
                                    int id = element.GetProperty("ID").GetInt32();
                                    string repuesto = element.GetProperty("Repuesto").GetString();
                                    string detalles = element.GetProperty("Detalles").GetString();
                                    double costo = element.GetProperty("Costo").GetDouble();

                                    LRepuesto nuevoRepuesto = new LRepuesto(id, repuesto, detalles, costo);
                                    Console.WriteLine($"ID: {nuevoRepuesto.Id}, Repuesto: {repuesto}, Detalles: {detalles}, Costo: {costo}");
                                    listaRepuestos.Insertar(id, repuesto, detalles, costo);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("El archivo no contiene datos válidos.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar el archivo: {ex.Message}");
                    }
                }

                filechooser.Destroy();

                if (selectedOption == "Usuarios")
                {
                    listaUsuarios.Imprimir();
                }
                else if (selectedOption == "Vehículos")
                {
                    listaVehiculos.Mostrar();
                }
                else if (selectedOption == "Repuestos")
                {
                    listaRepuestos.Mostrar();
                }
            };
            vbox.PackStart(buttonCargar, false, false, 10);

            Add(vbox);
            ShowAll();
        }
    }
}