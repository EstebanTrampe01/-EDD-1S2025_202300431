using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Servicios
{
    public class ArbolBinario
    {
        private NodoBinario raiz;
        private Random random = new Random(); // Para inserción semi-aleatoria

        public ArbolBinario()
        {
            raiz = null;
        }

        // Método para insertar un servicio en el árbol
        public void Insertar(Servicio servicio)
        {
            // Método 1: Inserción con factor aleatorio para balancear mejor
            raiz = InsertarBalanceado(raiz, servicio);
            GenerarGrafico("Servicios.dot");
        }

        private NodoBinario InsertarBalanceado(NodoBinario actual, Servicio servicio)
        {
            // Si el nodo actual es nulo, crear un nuevo nodo
            if (actual == null)
            {
                return new NodoBinario(servicio);
            }

            // Usar un factor aleatorio para decidir izquierda o derecha
            // Esto ayuda a crear un árbol más balanceado
            bool irIzquierda;
            
            if (actual.Data.ID == servicio.ID)
            {
                // Si el ID ya existe, simplemente actualizamos la información
                actual.Data = servicio;
                return actual;
            }
            else if (servicio.ID < actual.Data.ID)
            {
                // Si el ID es menor, tendencia a ir a la izquierda (70% de probabilidad)
                irIzquierda = random.Next(100) < 70;
            }
            else
            {
                // Si el ID es mayor, tendencia a ir a la derecha (70% de probabilidad)
                irIzquierda = random.Next(100) < 30;
            }

            // Insertar en el subárbol correspondiente
            if (irIzquierda)
            {
                actual.Izquierda = InsertarBalanceado(actual.Izquierda, servicio);
            }
            else
            {
                actual.Derecha = InsertarBalanceado(actual.Derecha, servicio);
            }

            return actual;
        }

        // Alternativa: Método para insertar utilizando la altura para balancear
        public void InsertarPorAltura(Servicio servicio)
        {
            raiz = InsertarPorAlturaRecursivo(raiz, servicio);
            GenerarGrafico("Servicios.dot");
        }

        private NodoBinario InsertarPorAlturaRecursivo(NodoBinario actual, Servicio servicio)
        {
            if (actual == null)
            {
                return new NodoBinario(servicio);
            }

            // Calcular las alturas de los subárboles
            int alturaIzq = ObtenerAltura(actual.Izquierda);
            int alturaDer = ObtenerAltura(actual.Derecha);

            // Insertar en el subárbol con menor altura para mantener el árbol balanceado
            if (alturaIzq <= alturaDer)
            {
                actual.Izquierda = InsertarPorAlturaRecursivo(actual.Izquierda, servicio);
            }
            else
            {
                actual.Derecha = InsertarPorAlturaRecursivo(actual.Derecha, servicio);
            }

            return actual;
        }

        private int ObtenerAltura(NodoBinario nodo)
        {
            if (nodo == null)
                return -1;
            
            int alturaIzq = ObtenerAltura(nodo.Izquierda);
            int alturaDer = ObtenerAltura(nodo.Derecha);
            
            return 1 + Math.Max(alturaIzq, alturaDer);
        }

        // Para mantener la compatibilidad con el código anterior, la función Encolar ahora usa InsertarPorAltura
        public void Encolar(Servicio servicio)
        {
            InsertarPorAltura(servicio);
        }

        // Método para buscar un servicio por ID
        public Servicio Buscar(int id)
        {
            return BuscarRecursivo(raiz, id);
        }

        private Servicio BuscarRecursivo(NodoBinario actual, int id)
        {
            if (actual == null)
            {
                return null;
            }
            
            if (id == actual.Data.ID)
            {
                return actual.Data;
            }

            // Buscar en ambos subárboles
            Servicio izq = BuscarRecursivo(actual.Izquierda, id);
            if (izq != null)
                return izq;
            
            return BuscarRecursivo(actual.Derecha, id);
        }

        // El resto del código permanece igual...
        // Método para eliminar un servicio por ID
        public Servicio Eliminar(int id)
        {
            Servicio servicioEliminado = Buscar(id);
            if (servicioEliminado != null)
            {
                raiz = EliminarRecursivo(raiz, id);
                GenerarGrafico("Servicios.dot");
            }
            return servicioEliminado;
        }

        private NodoBinario EliminarRecursivo(NodoBinario actual, int id)
        {
            if (actual == null)
            {
                return null;
            }

            // Si encontramos el nodo a eliminar
            if (actual.Data.ID == id)
            {
                // Caso 1: Nodo sin hijos
                if (actual.Izquierda == null && actual.Derecha == null)
                {
                    return null;
                }
                // Caso 2: Nodo con un solo hijo
                if (actual.Izquierda == null)
                {
                    return actual.Derecha;
                }
                if (actual.Derecha == null)
                {
                    return actual.Izquierda;
                }
                
                // Caso 3: Nodo con dos hijos
                // Encontrar el sucesor inorden (el menor valor en el subárbol derecho)
                actual.Data = EncontrarMinimo(actual.Derecha);
                // Eliminar el sucesor inorden
                actual.Derecha = EliminarRecursivo(actual.Derecha, actual.Data.ID);
            }
            else
            {
                // Buscar en ambos subárboles
                actual.Izquierda = EliminarRecursivo(actual.Izquierda, id);
                actual.Derecha = EliminarRecursivo(actual.Derecha, id);
            }

            return actual;
        }

        // Método para encontrar el servicio con el ID mínimo en un subárbol
        private Servicio EncontrarMinimo(NodoBinario nodo)
        {
            Servicio minValor = nodo.Data;
            while (nodo.Izquierda != null)
            {
                nodo = nodo.Izquierda;
                minValor = nodo.Data;
            }
            return minValor;
        }

        // Los métodos para visualización y otros permanecen igual
        public void Print()
        {
            if (raiz == null)
            {
                Console.WriteLine("Árbol vacío.");
                return;
            }

            Console.WriteLine("Servicios en el árbol:");
            RecorridoInOrden(raiz);
            GenerarGrafico("Servicios.dot");
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

        public Servicio Desencolar()
        {
            if (raiz == null)
            {
                return null;
            }

            // Encontrar el servicio con el ID mínimo (o el más a la izquierda)
            Servicio servicioMinimo = EncontrarServicioMasIzquierda(raiz);
            
            // Eliminar y devolver ese servicio
            raiz = EliminarRecursivo(raiz, servicioMinimo.ID);
            GenerarGrafico("Servicios.dot");
            
            return servicioMinimo;
        }

        private Servicio EncontrarServicioMasIzquierda(NodoBinario nodo)
        {
            if (nodo.Izquierda == null)
                return nodo.Data;
            return EncontrarServicioMasIzquierda(nodo.Izquierda);
        }

        // Los métodos para generar gráficos se mantienen iguales
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