using System;

namespace Servicios
{
    public class Servicio
    {
        public int ID { get; set; }
        public int Id_Vehiculo { get; set; }
        public int Id_Repuesto { get; set; }
        public string Detalles { get; set; }
        public double Costo { get; set; }
        public string MetodoPago { get; set; } // Nuevo atributo para método de pago

        public Servicio(int id, int idVehiculo, int idRepuesto, string detalles, double costo, string metodoPago = "Efectivo")
        {
            ID = id;
            Id_Vehiculo = idVehiculo;
            Id_Repuesto = idRepuesto;
            Detalles = detalles;
            Costo = costo;
            MetodoPago = metodoPago;
        }

        public override string ToString()
        {
            return $"[ID:{ID}, Vehículo:{Id_Vehiculo}, Repuesto:{Id_Repuesto}, Costo:${Costo:F2}, Método Pago:{MetodoPago}]";
        }
    }
}