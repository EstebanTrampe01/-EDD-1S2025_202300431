using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Servicios;
using Facturas;
using Repuestos;
using Vehiculos;

namespace AutoGest.Utils
{
    public unsafe class ServiceLoader
    {
        // Estructura para almacenar errores de validación
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
            public ValidationResult(bool isValid, string errorMessage = "")
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }
        }

        // Clase para representar un servicio desde JSON
        public class ServiceDTO
        {
            public int Id { get; set; }
            public int Id_repuesto { get; set; }
            public int Id_vehiculo { get; set; }
            public string Detalles { get; set; }
            public double Costo { get; set; }
            public string MetodoPago { get; set; } = "Efectivo"; // Valor por defecto
        }

        public static List<ServiceDTO> ParseServicesFromJson(string jsonFilePath)
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            List<ServiceDTO> services = new List<ServiceDTO>();
            
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in root.EnumerateArray())
                        {
                            ServiceDTO service = new ServiceDTO
                            {
                                Id = element.GetProperty("Id").GetInt32(),
                                Id_repuesto = element.GetProperty("Id_repuesto").GetInt32(),
                                Id_vehiculo = element.GetProperty("Id_vehiculo").GetInt32(),
                                Detalles = element.GetProperty("Detalles").GetString(),
                                Costo = element.GetProperty("Costo").GetDouble()
                            };
                            
                            // Intentar obtener método de pago si existe
                            if (element.TryGetProperty("MetodoPago", out JsonElement metodoPagoElement))
                            {
                                service.MetodoPago = metodoPagoElement.GetString();
                            }
                            
                            services.Add(service);
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error al parsear JSON: {ex.Message}");
                throw;
            }
            
            return services;
        }

        public static ValidationResult ValidateService(
            ServiceDTO service, 
            ArbolBinario arbolServicios, 
            ListaDoblementeEnlazada listaVehiculos, 
            ArbolAVL arbolRepuestos)
        {
            // Validación 1: ID único
            if (arbolServicios.Buscar(service.Id) != null)
            {
                return new ValidationResult(false, $"Ya existe un servicio con ID {service.Id}");
            }
            
            // Validación 2: Existencia del vehículo
            Vehiculo* vehiculo = listaVehiculos.Buscar(service.Id_vehiculo);
            if (vehiculo == null)
            {
                return new ValidationResult(false, $"No existe un vehículo con ID {service.Id_vehiculo}");
            }
            
            // Validación 3: Existencia del repuesto
            LRepuesto* repuesto = arbolRepuestos.Buscar(service.Id_repuesto);
            if (repuesto == null)
            {
                return new ValidationResult(false, $"No existe un repuesto con ID {service.Id_repuesto}");
            }
            
            // Validación 4: Costo positivo
            if (service.Costo <= 0)
            {
                return new ValidationResult(false, "El costo del servicio debe ser un valor positivo");
            }
            
            // Validación 5: Método de pago válido
            if (string.IsNullOrWhiteSpace(service.MetodoPago))
            {
                service.MetodoPago = "Efectivo"; // Asignar valor por defecto
            }
            
            return new ValidationResult(true);
        }

        // Método para cargar servicios masivamente y crear sus facturas correspondientes
        public static int CargarServiciosMasivamente(
            string jsonFilePath,
            ArbolBinario arbolServicios,
            ArbolM arbolFacturas,
            ListaDoblementeEnlazada listaVehiculos,
            ArbolAVL arbolRepuestos,
            out string errorMessages)
        {
            List<string> errors = new List<string>();
            int contadorCargados = 0;
            
            try
            {
                List<ServiceDTO> services = ParseServicesFromJson(jsonFilePath);
                
                foreach (var serviceDto in services)
                {
                    ValidationResult validationResult = ValidateService(
                        serviceDto, arbolServicios, listaVehiculos, arbolRepuestos);
                    
                    if (validationResult.IsValid)
                    {
                        // Crear y guardar el servicio
                        Servicio servicio = new Servicio(
                            serviceDto.Id, 
                            serviceDto.Id_vehiculo, 
                            serviceDto.Id_repuesto, 
                            serviceDto.Detalles, 
                            serviceDto.Costo,
                            serviceDto.MetodoPago);
                        
                        arbolServicios.Insertar(servicio);
                        
                        // Crear y guardar la factura correspondiente
                        LRepuesto* repuesto = arbolRepuestos.Buscar(serviceDto.Id_repuesto);
                        double total = serviceDto.Costo + repuesto->Costo;
                        
                        Factura factura = new Factura(serviceDto.Id + 100, serviceDto.Id, total);
                        factura.MetodoPago = serviceDto.MetodoPago;
                        arbolFacturas.Insertar(factura);
                        
                        contadorCargados++;
                    }
                    else
                    {
                        errors.Add($"Error en servicio ID={serviceDto.Id}: {validationResult.ErrorMessage}");
                    }
                }
                
                errorMessages = string.Join("\n", errors);
                return contadorCargados;
            }
            catch (Exception ex)
            {
                errorMessages = $"Error general: {ex.Message}";
                return 0;
            }
        }
    }
}
