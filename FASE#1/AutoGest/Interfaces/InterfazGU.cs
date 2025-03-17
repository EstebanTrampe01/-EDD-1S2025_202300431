using Gtk;
using System;
using Usuarios;
using Vehiculos;


namespace AutoGest.Interfaces
{
    public unsafe class InterfazGU : Window
    {
        private ListaSimple<Usuario> listaUsuarios;
        private ListaDoblementeEnlazada listaVehiculos;


        public InterfazGU(ListaSimple<Usuario> listaUsuarios, ListaDoblementeEnlazada listaVehiculos) : base("Gestión de Usuarios")
        {
            this.listaUsuarios = listaUsuarios;
            this.listaVehiculos = listaVehiculos;


            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);

            // Crear un contenedor para las pestañas
            Notebook notebook = new Notebook();

            // Crear las pestañas
            VBox vboxVerUsuarios = new VBox();
            VBox vboxEditarUsuario = new VBox();
            VBox vboxEliminarUsuario = new VBox();

            // Contenido de la pestaña "Ver Usuarios"
            TextView textViewUsuarios = new TextView();
            textViewUsuarios.Editable = false;
            Button buttonVerUsuarios = new Button("Ver Usuarios");
            buttonVerUsuarios.Clicked += delegate {
                textViewUsuarios.Buffer.Text = listaUsuarios.ObtenerLista();
            };
            vboxVerUsuarios.PackStart(textViewUsuarios, true, true, 10);
            vboxVerUsuarios.PackStart(buttonVerUsuarios, false, false, 10);

            // Contenido de la pestaña "Editar Usuario"
            Entry entryIdEditar = new Entry();
            Button buttonBuscarUsuario = new Button("Buscar");
            Label labelNombresActual = new Label("Nombres: ");
            Entry entryNombresEditar = new Entry { Sensitive = false };
            Label labelApellidosActual = new Label("Apellidos: ");
            Entry entryApellidosEditar = new Entry { Sensitive = false };
            Label labelCorreoActual = new Label("Correo: ");
            Entry entryCorreoEditar = new Entry { Sensitive = false };
            Label labelContraseniaActual = new Label("Contrasenia: ");
            Entry entryContraseniaEditar = new Entry { Sensitive = false };
            Button buttonEditarUsuario = new Button("Actualizar") { Sensitive = false };

            buttonBuscarUsuario.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    Usuario* usuario = listaUsuarios.Buscar(id);
                    if (usuario != null)
                    {
                        labelNombresActual.Text = $"Nombres: {GetFixedString(usuario->name)}";
                        labelApellidosActual.Text = $"Apellidos: {GetFixedString(usuario->lastName)}";
                        labelCorreoActual.Text = $"Correo: {GetFixedString(usuario->correo)}";
                        labelContraseniaActual.Text = $"Contrasenia: {GetFixedString(usuario->contrasena)}";

                        entryNombresEditar.Text = GetFixedString(usuario->name);
                        entryApellidosEditar.Text = GetFixedString(usuario->lastName);
                        entryCorreoEditar.Text = GetFixedString(usuario->correo);
                        entryContraseniaEditar.Text = GetFixedString(usuario->contrasena);

                        entryNombresEditar.Sensitive = true;
                        entryApellidosEditar.Sensitive = true;
                        entryCorreoEditar.Sensitive = true;
                        entryContraseniaEditar.Sensitive = true;
                        buttonEditarUsuario.Sensitive = true;
                    }
                    else
                    {
                        Console.WriteLine("Usuario no encontrado.");
                    }
                }
                else
                {
                    Console.WriteLine("ID inválido.");
                }
            };

            buttonEditarUsuario.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEditar.Text, out id))
                {
                    listaUsuarios.ModificarUsuario(id, entryNombresEditar.Text, entryApellidosEditar.Text, entryCorreoEditar.Text, entryContraseniaEditar.Text);

                    // Limpiar los inputs después de actualizar
                    entryIdEditar.Text = "";
                    entryNombresEditar.Text = "";
                    entryApellidosEditar.Text = "";
                    entryCorreoEditar.Text = "";
                    entryContraseniaEditar.Text = "";

                    entryNombresEditar.Sensitive = false;
                    entryApellidosEditar.Sensitive = false;
                    entryCorreoEditar.Sensitive = false;
                    entryContraseniaEditar.Sensitive = false;
                    buttonEditarUsuario.Sensitive = false;
                }
            };

            HBox hboxIdEditar = new HBox();
            hboxIdEditar.PackStart(new Label("ID:"), false, false, 10);
            hboxIdEditar.PackStart(entryIdEditar, true, true, 10);
            hboxIdEditar.PackStart(buttonBuscarUsuario, false, false, 10);

            vboxEditarUsuario.PackStart(hboxIdEditar, false, false, 10);
            vboxEditarUsuario.PackStart(CreateLabeledEntryWithLabel("Nombres:", labelNombresActual, entryNombresEditar), false, false, 10);
            vboxEditarUsuario.PackStart(CreateLabeledEntryWithLabel("Apellidos:", labelApellidosActual, entryApellidosEditar), false, false, 10);
            vboxEditarUsuario.PackStart(CreateLabeledEntryWithLabel("Correo:", labelCorreoActual, entryCorreoEditar), false, false, 10);
            vboxEditarUsuario.PackStart(CreateLabeledEntryWithLabel("Contrasenia:", labelContraseniaActual, entryContraseniaEditar), false, false, 10);
            vboxEditarUsuario.PackStart(buttonEditarUsuario, false, false, 10);

            // Contenido de la pestaña "Eliminar Usuario"
            Entry entryIdEliminar = new Entry();
            Button buttonEliminarUsuario = new Button("Eliminar Usuario");
            buttonEliminarUsuario.Clicked += delegate {
                int id;
                if (int.TryParse(entryIdEliminar.Text, out id))
                {
                    listaUsuarios.Eliminar(id);
                }
            };
            vboxEliminarUsuario.PackStart(CreateLabeledEntry("ID:", entryIdEliminar), false, false, 10);
            vboxEliminarUsuario.PackStart(buttonEliminarUsuario, false, false, 10);

            // Agregar las pestañas al notebook
            notebook.AppendPage(vboxVerUsuarios, new Label("Ver Usuarios"));
            notebook.AppendPage(vboxEditarUsuario, new Label("Editar Usuario"));
            notebook.AppendPage(vboxEliminarUsuario, new Label("Eliminar Usuario"));

            // Agregar el notebook a la ventana
            Add(notebook);
            ShowAll();
        }

        private HBox CreateLabeledEntry(string labelText, Entry entry)
        {
            HBox hbox = new HBox();
            Label label = new Label(labelText);
            hbox.PackStart(label, false, false, 10);
            hbox.PackStart(entry, true, true, 10);
            return hbox;
        }

        private HBox CreateLabeledEntryWithLabel(string labelText, Label labelActual, Entry entry)
        {
            HBox hbox = new HBox();
            Label label = new Label(labelText);
            hbox.PackStart(label, false, false, 10);
            hbox.PackStart(entry, true, true, 10);
            return hbox;
        }

        private string GetFixedString(char* fixedStr)
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