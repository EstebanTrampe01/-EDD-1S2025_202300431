using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace Usuarios
{
    public unsafe class ListaSimple<T> where T : unmanaged
    {
        private Nodo<T>* cabeza;

        public ListaSimple()
        {
            cabeza = null;
        }

        public void Insertar(T data)
        {
            Nodo<T>* nuevo = (Nodo<T>*)Marshal.AllocHGlobal(sizeof(Nodo<T>));
            nuevo->Data = data;
            nuevo->Sig = null;

            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                Nodo<T>* temp = cabeza;
                while (temp->Sig != null)
                {
                    temp = temp->Sig;
                }
                temp->Sig = nuevo;
            }

        }

        public void Eliminar(int id)
        {
            if (typeof(T) != typeof(Usuario))
            {
                throw new InvalidOperationException("El método Eliminar solo puede ser utilizado con listas de usuarios.");
            }

            if (cabeza == null) return;

            Usuario* usuarioCabeza = (Usuario*)(&cabeza->Data);
            if (usuarioCabeza->id == id)
            {
                Nodo<T>* temp = cabeza;
                cabeza = cabeza->Sig;
                Marshal.FreeHGlobal((IntPtr)temp);
                return;
            }

            Nodo<T>* actual = cabeza;
            while (actual->Sig != null)
            {
                Usuario* usuarioActual = (Usuario*)(&actual->Sig->Data);
                if (usuarioActual->id == id)
                {
                    Nodo<T>* temp = actual->Sig;
                    actual->Sig = actual->Sig->Sig;
                    Marshal.FreeHGlobal((IntPtr)temp);
                    return;
                }
                actual = actual->Sig;
            }
        }

        public void ModificarUsuario(int id, string nuevoNombre, string nuevoApellido, string nuevoCorreo, string nuevaContrasena)
        {
            if (typeof(T) != typeof(Usuarios.Usuario))
            {
                throw new InvalidOperationException("El método ModificarUsuario solo puede ser utilizado con listas de usuarios.");
            }

            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                Usuarios.Usuario* pUsuario = (Usuarios.Usuario*)(&temp->Data);
                if (pUsuario->id == id)
                {
                    char* n = pUsuario->name;
                    char* l = pUsuario->lastName;
                    char* c = pUsuario->correo;
                    char* p = pUsuario->contrasena;
                    int i;

                    for (i = 0; i < nuevoNombre.Length && i < 50 - 1; i++)
                    {
                        n[i] = nuevoNombre[i];
                    }
                    n[i] = '\0';

                    for (i = 0; i < nuevoApellido.Length && i < 50 - 1; i++)
                    {
                        l[i] = nuevoApellido[i];
                    }
                    l[i] = '\0';

                    for (i = 0; i < nuevoCorreo.Length && i < 100 - 1; i++)
                    {
                        c[i] = nuevoCorreo[i];
                    }
                    c[i] = '\0';

                    for (i = 0; i < nuevaContrasena.Length && i < 50 - 1; i++)
                    {
                        p[i] = nuevaContrasena[i];
                    }
                    p[i] = '\0';

                    Console.WriteLine("Usuario modificado exitosamente.");
                    return;
                }
                temp = temp->Sig;
            }
            Console.WriteLine("Usuario no encontrado.");
        }

        public T* Buscar(int id)
        {
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                if (typeof(T) == typeof(Usuario))
                {
                    Usuario* usuario = (Usuario*)(&temp->Data);
                    if (usuario->id == id)
                    {
                        return (T*)usuario;
                    }
                }
                temp = temp->Sig;
            }
            return null;
        }

        // En el método BuscarCorreo
        public T* BuscarCorreo(string id)
        {
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                if (typeof(T) == typeof(Usuario))
                {
                    Usuario* usuario = (Usuario*)(&temp->Data);
                    // Convertir el puntero char* a string antes de comparar
                    string usuarioCorreo = GetFixedString(usuario->correo);
                    if (usuarioCorreo == id)  // Ahora comparamos correctamente dos strings
                    {
                        return (T*)usuario;
                    }
                }
                temp = temp->Sig;
            }
            return null;
        }

        public void Imprimir()
        {
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                Console.WriteLine(temp->Data);
                temp = temp->Sig;
            }
        }

     // Método corregido en la clase ListaSimple<T>
        public unsafe bool VerificarCredenciales(string correo, string contrasena)
        {
            if (cabeza == null)
                return false;
        
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                Usuario* usuario = (Usuario*)(&temp->Data);
                string usuarioCorreo = GetFixedString(usuario->correo);
                string usuarioContrasena = GetFixedString(usuario->contrasena);
                
                if (usuarioCorreo == correo && usuarioContrasena == contrasena)
                {
                    return true;
                }
                
                temp = temp->Sig;  // Corregido: usar temp en lugar de actual
            }
            
            return false;
        }

    // Método a agregar en la clase ListaSimple<T>
        public unsafe (bool, int) VerificarCredencialesConId(string correo, string contrasena)
        {
            if (cabeza == null)
                return (false, -1);
        
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                Usuario* usuario = (Usuario*)(&temp->Data);
                string usuarioCorreo = GetFixedString(usuario->correo);
                string usuarioContrasena = GetFixedString(usuario->contrasena);
                
                if (usuarioCorreo == correo && usuarioContrasena == contrasena)
                {
                    return (true, usuario->id);
                }
                
                temp = temp->Sig;
            }
            
            return (false, -1);
        }

        public string ObtenerLista()
        {
            if (typeof(T) != typeof(Usuario))
            {
                throw new InvalidOperationException("El método ObtenerLista solo puede ser utilizado con listas de usuarios.");
            }

            string lista = "";
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                Usuario* usuario = (Usuario*)(&temp->Data);
                lista += $"ID: {usuario->id}, Nombre: {GetFixedString(usuario->name)}, Apellido: {GetFixedString(usuario->lastName)}, Correo: {GetFixedString(usuario->correo)}, Contraseña: {GetFixedString(usuario->contrasena)}\n";
                temp = temp->Sig;
            }
            return lista;
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

        public void GenerarGrafico(string fileName)
        {
            string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reports");
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            string filePath = Path.Combine(carpeta, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("digraph G {");
                writer.WriteLine("rankdir=LR;"); // Orientación horizontal
                writer.WriteLine("node [shape=record];");
                writer.WriteLine("splines=false;"); // Flechas rectas

                Nodo<T>* temp = cabeza;
                int index = 0;
                while (temp != null)
                {
                    if (typeof(T) == typeof(Usuario))
                    {
                        Usuario* usuario = (Usuario*)(&temp->Data);
                        writer.WriteLine($"node{index} [label=\"ID: {usuario->id} \\n Nombre: {GetFixedString(usuario->name)} \\n Apellido: {GetFixedString(usuario->lastName)} \\n Correo: {GetFixedString(usuario->correo)} \\n Contraseña: {GetFixedString(usuario->contrasena)}\"]");
                    }
                    else
                    {
                        writer.WriteLine($"node{index} [label=\"<data> {temp->Data} \\n <next>\"]");
                    }

                    if (temp->Sig != null)
                    {
                        writer.WriteLine($"node{index} -> node{index + 1} [tailport=next];");
                    }
                    temp = temp->Sig;
                    index++;
                }

                writer.WriteLine("}");
            }

            // Generar el archivo PNG usando Graphviz
            var process = new Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {filePath} -o {Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        ~ListaSimple()
        {
            while (cabeza != null)
            {
                Nodo<T>* temp = cabeza;
                cabeza = cabeza->Sig;
                Marshal.FreeHGlobal((IntPtr)temp);
            }
        }
    }
}