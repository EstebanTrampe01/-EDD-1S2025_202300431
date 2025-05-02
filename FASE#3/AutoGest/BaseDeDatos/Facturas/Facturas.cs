using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Facturas
{
    public class Factura
    {
        public int ID { get; set; }           
        public int ID_Orden { get; set; }  // Mantenemos este campo para compatibilidad
        public int ID_Servicio { get; set; }  
        public double Total { get; set; }     
        public string Fecha { get; set; }     
        public string MetodoPago { get; set; }

        // Constructor completo
        public Factura(int id, int idOrden, int idServicio, double total, string fecha, string metodoPago)
        {
            ID = id;
            ID_Orden = idOrden;
            ID_Servicio = idServicio;
            Total = total;
            Fecha = fecha;
            MetodoPago = metodoPago;
        }

        // Constructor simple para compatibilidad con código existente
        public Factura(int id, int idOrden, double total)
        {
            ID = id;
            ID_Orden = idOrden;
            Total = total;
            ID_Servicio = 0;
            Fecha = DateTime.Now.ToString("yyyy-MM-dd");
            MetodoPago = "No especificado";
        }

        // Método para obtener el hash de la factura
        public string GetHash()
        {
            string data = JsonConvert.SerializeObject(this); // Serializar la factura a JSON
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convertir a hexadecimal
                }
                return builder.ToString();
            }
        }

        public override string ToString()
        {
            return $"[ID:{ID}, Orden:{ID_Orden}, Total:${Total:F2}]";
        }
    }
}
