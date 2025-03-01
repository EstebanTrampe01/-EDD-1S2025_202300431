using System;
using System.Runtime.InteropServices;

namespace Usuarios
{
    // se define la estructura usuario pero tipo unsafe
    public unsafe struct Usuario
    {
        public int id;
        public fixed char name[50];
        public fixed char lastName[50];
        public fixed char correo[100];
        public fixed char contrasena[50]; // Nuevo campo para la contraseña
        public Usuario* sig;

        // Constructor que inicializa un nuevo Usuario con sus datos.
        public Usuario(int id, string name, string lastName, string correo, string contrasena)
        {
            this.id = id;
            sig = null;
            fixed (char* n = this.name, l = this.lastName, c = this.correo, p = this.contrasena)
            {
                setFixedString(name, n, 50);
                setFixedString(lastName, l, 50);
                setFixedString(correo, c, 100);
                setFixedString(contrasena, p, 50);
            }
        }

        private static void setFixedString(string str, char* fixedStr, int maxLength)
        {
            int i;
            for (i = 0; i < str.Length && i < maxLength - 1; i++)
            {
                fixedStr[i] = str[i];
            }
            fixedStr[i] = '\0';
        }

        private static string getFixedString(char* fixedStr)
        {
            string str = "";
            for (int i = 0; fixedStr[i] != '\0'; i++)
            {
                str += fixedStr[i];
            }
            return str;
        }

        public override string ToString()
        {
            fixed (char* n = name, l = lastName, c = correo, p = contrasena)
            {
                return $"ID: {id}, Nombre: {getFixedString(n)}, Apellido: {getFixedString(l)}, Correo: {getFixedString(c)}, Contraseña: {getFixedString(p)}";
            }
        }
    }
}