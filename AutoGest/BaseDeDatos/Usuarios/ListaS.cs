using System;
using System.Runtime.InteropServices;

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
            // se valida que sea tipo usuario
            if (typeof(T) != typeof(Usuarios.Usuario))
            {
                throw new InvalidOperationException("El método ModificarUsuario solo puede ser utilizado con listas de usuarios.");
            }
            // Recorre la lista enlazada.
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                // Obtiene el puntero al dato y lo interpreta como un Usuario.
                Usuarios.Usuario* pUsuario = (Usuarios.Usuario*)(&temp->Data);
                if (pUsuario->id == id)
                {
                    // Como la memoria es no administrada, los buffers ya tienen una dirección fija.
                    // Simplemente se asignan a punteros locales.
                    char* n = pUsuario->name;
                    char* l = pUsuario->lastName;
                    char* c = pUsuario->correo;
                    char* p = pUsuario->contrasena;
                    int i;
                    // Actualiza el campo name 
                    for (i = 0; i < nuevoNombre.Length && i < 50 - 1; i++)
                    {
                        n[i] = nuevoNombre[i];
                    }
                    n[i] = '\0';

                    // Actualiza el campo lastName
                    for (i = 0; i < nuevoApellido.Length && i < 50 - 1; i++)
                    {
                        l[i] = nuevoApellido[i];
                    }
                    l[i] = '\0';

                    // Actualiza el campo correo
                    for (i = 0; i < nuevoCorreo.Length && i < 100 - 1; i++)
                    {
                        c[i] = nuevoCorreo[i];
                    }
                    c[i] = '\0';

                    // Actualiza el campo contrasena
                    for (i = 0; i < nuevaContrasena.Length && i < 50 - 1; i++)
                    {
                        p[i] = nuevaContrasena[i];
                    }
                    p[i] = '\0';

                    Console.WriteLine("Usuario modificado exitosamente.");
                    return; // finalizar tras modificar
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

        public void Imprimir()
        {
            Nodo<T>* temp = cabeza;
            while (temp != null)
            {
                Console.WriteLine(temp->Data);
                temp = temp->Sig;
            }
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