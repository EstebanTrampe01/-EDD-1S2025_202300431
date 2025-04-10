namespace Facturas
{
    public class Nodo
    {
        public Factura Data { get; set; }
        public Nodo Sig { get; set; }

        public Nodo(Factura data)
        {
            Data = data;
            Sig = null;
        }
    }
}