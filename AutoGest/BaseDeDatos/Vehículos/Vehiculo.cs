using System;
using System.Runtime.InteropServices;

namespace Vehiculos
{
    // se define la estructura usuario pero tipo unsafe
    public unsafe struct Vehiculo
    {
        public int Id;
        public int ID_Usuario;
        public fixed char Marca[50];
        public int Modelo;
        public fixed char Placa[100];

        // Constructor que inicializa un nuevo Vehiculo con sus datos.
        public Vehiculo(int id, int idUsuario, string marca, int modelo, string placa)
        {
            this.Id = id;
            this.ID_Usuario = idUsuario;
            this.Modelo = modelo;

            fixed (char* m = this.Marca, p = this.Placa)
            {
                setFixedString(marca, m, 50);
                setFixedString(placa, p, 100);
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
            fixed (char* m = Marca, p = Placa)
            {
                return $"ID: {Id}, ID Usuario: {ID_Usuario}, Marca: {getFixedString(m)}, Modelo: {Modelo}, Placa: {getFixedString(p)}";
            }
        }
    }
}