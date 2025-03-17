namespace Servicios
{
    public class Nodo
    {
        public Servicio Data { get; set; }
        public Nodo Sig { get; set; }

        public Nodo(Servicio data)
        {
            Data = data;
            Sig = null;
        }
    }
}