namespace Servicios
{
    public class NodoBinario
    {
        public Servicio Data { get; set; }
        public NodoBinario Izquierda { get; set; }
        public NodoBinario Derecha { get; set; }

        public NodoBinario(Servicio data)
        {
            Data = data;
            Izquierda = null;
            Derecha = null;
        }
    }
}