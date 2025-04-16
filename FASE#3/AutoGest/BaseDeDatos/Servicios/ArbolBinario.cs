using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Servicios
{
    public class ArbolBinario
    {
        private NodoBinario raiz;

        public ArbolBinario()
        {
            raiz = null;
        }

        public void Insertar(Servicio servicio)
        {
            raiz = InsertarRecursivo(raiz, servicio);
            ;
        }

        private NodoBinario InsertarRecursivo(NodoBinario nodo, Servicio servicio)
        {
            // Si el nodo es null, crear un nuevo nodo
            if (nodo == null)
            {
                return new NodoBinario(servicio);
            }

            // Si ya existe un servicio con el mismo ID, actualizamos los datos
            if (servicio.ID == nodo.Data.ID)
            {
                nodo.Data = servicio;
                return nodo;
            }

            // Insertar en el subárbol correspondiente según el ID
            if (servicio.ID < nodo.Data.ID)
            {
                nodo.Izquierda = InsertarRecursivo(nodo.Izquierda, servicio);
            }
            else
            {
                nodo.Derecha = InsertarRecursivo(nodo.Derecha, servicio);
            }

            return nodo;
        }

        /// <summary>
        /// Busca un servicio por su ID.
        /// </summary>
        public Servicio Buscar(int id)
        {
            return BuscarRecursivo(raiz, id);
        }
        public List<Servicio> ObtenerServicios()
        {
            return RecorridoInOrden();
        }

        private Servicio BuscarRecursivo(NodoBinario nodo, int id)
        {
            // Si el nodo es null o encontramos el ID
            if (nodo == null)
            {
                return null;
            }
            
            // Si encontramos el ID
            if (id == nodo.Data.ID)
            {
                return nodo.Data;
            }
            
            // Buscar en el subárbol correspondiente
            if (id < nodo.Data.ID)
            {
                return BuscarRecursivo(nodo.Izquierda, id);
            }
            else
            {
                return BuscarRecursivo(nodo.Derecha, id);
            }
        }

        /// <summary>
        /// Elimina un servicio por su ID y devuelve el servicio eliminado.
        /// </summary>
        public Servicio Eliminar(int id)
        {
            Servicio servicioEliminado = Buscar(id);
            if (servicioEliminado != null)
            {
                raiz = EliminarRecursivo(raiz, id);
                ;
            }
            return servicioEliminado;
        }

        private NodoBinario EliminarRecursivo(NodoBinario nodo, int id)
        {
            // Si el nodo es null, no hay nada que eliminar
            if (nodo == null)
            {
                return null;
            }

            // Buscar el nodo a eliminar
            if (id < nodo.Data.ID)
            {
                nodo.Izquierda = EliminarRecursivo(nodo.Izquierda, id);
            }
            else if (id > nodo.Data.ID)
            {
                nodo.Derecha = EliminarRecursivo(nodo.Derecha, id);
            }
            else
            {
                // Caso 1: Nodo sin hijos
                if (nodo.Izquierda == null && nodo.Derecha == null)
                {
                    return null;
                }
                // Caso 2: Nodo con un solo hijo
                if (nodo.Izquierda == null)
                {
                    return nodo.Derecha;
                }
                if (nodo.Derecha == null)
                {
                    return nodo.Izquierda;
                }
                
                // Caso 3: Nodo con dos hijos
                // Encontrar el sucesor inorden (el menor valor en el subárbol derecho)
                Servicio sucesor = EncontrarMinimo(nodo.Derecha);
                nodo.Data = sucesor;
                
                // Eliminar el sucesor inorden
                nodo.Derecha = EliminarRecursivo(nodo.Derecha, sucesor.ID);
            }

            return nodo;
        }

        /// <summary>
        /// Encuentra el servicio con el ID mínimo en un subárbol.
        /// </summary>
        private Servicio EncontrarMinimo(NodoBinario nodo)
        {
            if (nodo == null)
            {
                return null;
            }
            
            if (nodo.Izquierda == null)
            {
                return nodo.Data;
            }
            
            return EncontrarMinimo(nodo.Izquierda);
        }

        /// <summary>
        /// Imprime todos los servicios en el árbol en orden.
        /// </summary>
        public void Print()
        {
            if (raiz == null)
            {
                Console.WriteLine("Árbol vacío.");
                return;
            }

            Console.WriteLine("Servicios en el árbol:");
            RecorridoInOrden(raiz);
            ;
        }

        private void RecorridoInOrden(NodoBinario nodo)
        {
            if (nodo != null)
            {
                RecorridoInOrden(nodo.Izquierda);
                Console.WriteLine(nodo.Data.ToString());
                RecorridoInOrden(nodo.Derecha);
            }
        }

        /// <summary>
        /// Alias para buscar un servicio por su ID. Mantenido por compatibilidad.
        /// </summary>
        public Servicio BuscarServicioPorId(int id)
        {
            return Buscar(id);
        }
        
        /// <summary>
        /// Obtiene una lista con todos los servicios en recorrido InOrden.
        /// </summary>
        public List<Servicio> RecorridoInOrden()
        {
            List<Servicio> servicios = new List<Servicio>();
            RecorridoInOrdenALista(raiz, servicios);
            return servicios;
        }
        
        private void RecorridoInOrdenALista(NodoBinario nodo, List<Servicio> servicios)
        {
            if (nodo != null)
            {
                RecorridoInOrdenALista(nodo.Izquierda, servicios);
                servicios.Add(nodo.Data);
                RecorridoInOrdenALista(nodo.Derecha, servicios);
            }
        }
        
        /// <summary>
        /// Obtiene una lista con todos los servicios en recorrido PreOrden.
        /// </summary>
        public List<Servicio> RecorridoPreOrden()
        {
            List<Servicio> servicios = new List<Servicio>();
            RecorridoPreOrdenALista(raiz, servicios);
            return servicios;
        }
        
        private void RecorridoPreOrdenALista(NodoBinario nodo, List<Servicio> servicios)
        {
            if (nodo != null)
            {
                servicios.Add(nodo.Data);
                RecorridoPreOrdenALista(nodo.Izquierda, servicios);
                RecorridoPreOrdenALista(nodo.Derecha, servicios);
            }
        }
        
        /// <summary>
        /// Obtiene una lista con todos los servicios en recorrido PostOrden.
        /// </summary>
        public List<Servicio> RecorridoPostOrden()
        {
            List<Servicio> servicios = new List<Servicio>();
            RecorridoPostOrdenALista(raiz, servicios);
            return servicios;
        }
        
        private void RecorridoPostOrdenALista(NodoBinario nodo, List<Servicio> servicios)
        {
            if (nodo != null)
            {
                RecorridoPostOrdenALista(nodo.Izquierda, servicios);
                RecorridoPostOrdenALista(nodo.Derecha, servicios);
                servicios.Add(nodo.Data);
            }
        }
        
        /// <summary>
        /// Obtiene los servicios de un vehículo específico en recorrido InOrden.
        /// </summary>
        public List<Servicio> ObtenerServiciosPorVehiculoInOrden(int idVehiculo)
        {
            List<Servicio> servicios = new List<Servicio>();
            ObtenerServiciosPorVehiculoInOrdenRecursivo(raiz, idVehiculo, servicios);
            return servicios;
        }
        
        private void ObtenerServiciosPorVehiculoInOrdenRecursivo(NodoBinario nodo, int idVehiculo, List<Servicio> servicios)
        {
            if (nodo != null)
            {
                // Recorrido In-Orden: izquierda, raíz, derecha
                ObtenerServiciosPorVehiculoInOrdenRecursivo(nodo.Izquierda, idVehiculo, servicios);
                
                if (nodo.Data.Id_Vehiculo == idVehiculo)
                {
                    servicios.Add(nodo.Data);
                }
                
                ObtenerServiciosPorVehiculoInOrdenRecursivo(nodo.Derecha, idVehiculo, servicios);
            }
        }
        
        /// <summary>
        /// Obtiene los servicios de un vehículo específico en recorrido PreOrden.
        /// </summary>
        public List<Servicio> ObtenerServiciosPorVehiculoPreOrden(int idVehiculo)
        {
            List<Servicio> servicios = new List<Servicio>();
            ObtenerServiciosPorVehiculoPreOrdenRecursivo(raiz, idVehiculo, servicios);
            return servicios;
        }
        
        private void ObtenerServiciosPorVehiculoPreOrdenRecursivo(NodoBinario nodo, int idVehiculo, List<Servicio> servicios)
        {
            if (nodo != null)
            {
                // Recorrido Pre-Orden: raíz, izquierda, derecha
                if (nodo.Data.Id_Vehiculo == idVehiculo)
                {
                    servicios.Add(nodo.Data);
                }
                
                ObtenerServiciosPorVehiculoPreOrdenRecursivo(nodo.Izquierda, idVehiculo, servicios);
                ObtenerServiciosPorVehiculoPreOrdenRecursivo(nodo.Derecha, idVehiculo, servicios);
            }
        }
        
        /// <summary>
        /// Obtiene los servicios de un vehículo específico en recorrido PostOrden.
        /// </summary>
        public List<Servicio> ObtenerServiciosPorVehiculoPostOrden(int idVehiculo)
        {
            List<Servicio> servicios = new List<Servicio>();
            ObtenerServiciosPorVehiculoPostOrdenRecursivo(raiz, idVehiculo, servicios);
            return servicios;
        }
        
        private void ObtenerServiciosPorVehiculoPostOrdenRecursivo(NodoBinario nodo, int idVehiculo, List<Servicio> servicios)
        {
            if (nodo != null)
            {
                // Recorrido Post-Orden: izquierda, derecha, raíz
                ObtenerServiciosPorVehiculoPostOrdenRecursivo(nodo.Izquierda, idVehiculo, servicios);
                ObtenerServiciosPorVehiculoPostOrdenRecursivo(nodo.Derecha, idVehiculo, servicios);
                
                if (nodo.Data.Id_Vehiculo == idVehiculo)
                {
                    servicios.Add(nodo.Data);
                }
            }
        }

        /// <summary>
        /// Genera una visualización gráfica del árbol binario.
        /// </summary>
        public void GenerarGrafico(string fileName)
        {
            string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reports");
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }
        
            string filePath = Path.Combine(carpeta, fileName);
        
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("digraph G {");
                writer.WriteLine("node [shape=circle, style=filled, fillcolor=lightblue];");
                writer.WriteLine("rankdir=TB;");
                
                if (raiz != null)
                {
                    GenerarNodosGrafico(writer, raiz);
                    GenerarConexionesGrafico(writer, raiz);
                }
                
                writer.WriteLine("}");
            }
        
            var process = new Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {filePath} -o {Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        
            Console.WriteLine("La gráfica del árbol binario se ha guardado en: " + Path.GetFullPath(Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))));
        }
        
        private void GenerarNodosGrafico(StreamWriter writer, NodoBinario nodo)
        {
            if (nodo != null)
            {
                Servicio servicio = nodo.Data;
                writer.WriteLine($"node{servicio.ID} [label=\"ID: {servicio.ID}\\nRepuesto: {servicio.Id_Repuesto}\\nVehículo: {servicio.Id_Vehiculo}\\nCosto: ${servicio.Costo:F2}\"];");
                
                GenerarNodosGrafico(writer, nodo.Izquierda);
                GenerarNodosGrafico(writer, nodo.Derecha);
            }
        }
        
        private void GenerarConexionesGrafico(StreamWriter writer, NodoBinario nodo)
        {
            if (nodo != null)
            {
                if (nodo.Izquierda != null)
                {
                    writer.WriteLine($"node{nodo.Data.ID} -> node{nodo.Izquierda.Data.ID} [label=\"L\"];");
                }
                
                if (nodo.Derecha != null)
                {
                    writer.WriteLine($"node{nodo.Data.ID} -> node{nodo.Derecha.Data.ID} [label=\"R\"];");
                }
                
                GenerarConexionesGrafico(writer, nodo.Izquierda);
                GenerarConexionesGrafico(writer, nodo.Derecha);
            }
        }
    }
}