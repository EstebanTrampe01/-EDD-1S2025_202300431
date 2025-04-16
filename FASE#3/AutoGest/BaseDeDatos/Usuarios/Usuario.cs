namespace Usuarios
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }

        public int Edad { get; set; } 

        // Constructor requerido
        public Usuario(int id, string name, string lastName, string correo, int edad,string contrasena)
        {
            Id = id;
            Name = name;
            LastName = lastName;
            Correo = correo;
            Edad = edad;
            Contrasena = contrasena;
        }
    }
}