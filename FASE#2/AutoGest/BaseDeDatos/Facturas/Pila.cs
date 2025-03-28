using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Facturas
{
    public class NodoArbolB
    {
        private const int ORDEN = 5;
        public const int MAX_CLAVES = ORDEN - 1;
        public const int MIN_CLAVES = (ORDEN / 2) - 1;
        
        public List<Factura> Claves { get; set; }
        public List<NodoArbolB> Hijos { get; set; }
        public bool EsHoja { get; set; }
        public string Tag { get; set; } // Agregar propiedad Tag para identificación en el gráfico

        public NodoArbolB()
        {
            Claves = new List<Factura>(MAX_CLAVES);
            Hijos = new List<NodoArbolB>(ORDEN);
            EsHoja = true;
            Tag = "";
        }

        // Verifica si el nodo está lleno
        public bool EstaLleno()
        {
            return Claves.Count >= MAX_CLAVES;
        }

        // Verifica si el nodo tiene el mínimo de claves requerido
        public bool TieneMinimoClaves()
        {
            return Claves.Count >= MIN_CLAVES;
        }
    }
    
    // El resto del código del ArbolB permanece igual...


    public class ArbolB
    {
        private NodoArbolB raiz;
        private const int ORDEN = 5;
        private const int MAX_CLAVES = ORDEN - 1;
        private const int MIN_CLAVES = (ORDEN / 2) - 1;

        public ArbolB()
        {
            raiz = new NodoArbolB();
        }

        // Método para insertar una nueva factura (equivalente a Push)
        public void Push(Factura factura)
        {
            // Si la raíz está llena, se crea una nueva raíz
            if (raiz.EstaLleno())
            {
                NodoArbolB nuevaRaiz = new NodoArbolB();
                nuevaRaiz.EsHoja = false;
                nuevaRaiz.Hijos.Add(raiz);
                DividirHijo(nuevaRaiz, 0);
                raiz = nuevaRaiz;
            }
            
            InsertarNoLleno(raiz, factura);
            GenerarGrafico("Facturacion.dot");
        }

        // Divide un hijo cuando está lleno durante la inserción
        private void DividirHijo(NodoArbolB padre, int indiceHijo)
        {
            NodoArbolB hijoCompleto = padre.Hijos[indiceHijo];
            NodoArbolB nuevoHijo = new NodoArbolB();
            nuevoHijo.EsHoja = hijoCompleto.EsHoja;

            // Elemento del medio que se promoverá al padre
            Factura facturaMedio = hijoCompleto.Claves[MIN_CLAVES];

            // Mover la mitad de las claves al nuevo hijo
            for (int i = MIN_CLAVES + 1; i < MAX_CLAVES; i++)
            {
                nuevoHijo.Claves.Add(hijoCompleto.Claves[i]);
            }

            // Si no es hoja, mover también los hijos correspondientes
            if (!hijoCompleto.EsHoja)
            {
                for (int i = (ORDEN / 2); i < ORDEN; i++)
                {
                    if (i < hijoCompleto.Hijos.Count)
                    {
                        nuevoHijo.Hijos.Add(hijoCompleto.Hijos[i]);
                    }
                }
                if (hijoCompleto.Hijos.Count > (ORDEN / 2))
                {
                    hijoCompleto.Hijos.RemoveRange((ORDEN / 2), hijoCompleto.Hijos.Count - (ORDEN / 2));
                }
            }

            // Eliminar las claves movidas del hijo original
            hijoCompleto.Claves.RemoveRange(MIN_CLAVES, hijoCompleto.Claves.Count - MIN_CLAVES);

            // Insertar el nuevo hijo en el padre
            padre.Hijos.Insert(indiceHijo + 1, nuevoHijo);

            // Insertar la clave media en el padre
            int j = 0;
            while (j < padre.Claves.Count && padre.Claves[j].ID < facturaMedio.ID)
            {
                j++;
            }
            padre.Claves.Insert(j, facturaMedio);
        }

        // Inserta una factura en un nodo que no está lleno
        private void InsertarNoLleno(NodoArbolB nodo, Factura factura)
        {
            int i = nodo.Claves.Count - 1;

            // Si es hoja, simplemente inserta la factura en orden
            if (nodo.EsHoja)
            {
                // Buscar la posición correcta para insertar
                while (i >= 0 && factura.ID < nodo.Claves[i].ID)
                {
                    i--;
                }
                nodo.Claves.Insert(i + 1, factura);
            }
            else
            {
                // Encuentra el hijo donde debe estar la factura
                while (i >= 0 && factura.ID < nodo.Claves[i].ID)
                {
                    i--;
                }
                i++;

                // Si el hijo está lleno, divídelo primero
                if (nodo.Hijos[i].EstaLleno())
                {
                    DividirHijo(nodo, i);
                    if (factura.ID > nodo.Claves[i].ID)
                    {
                        i++;
                    }
                }
                InsertarNoLleno(nodo.Hijos[i], factura);
            }
        }

        // Método para obtener y eliminar la factura del árbol (equivalente a Pop)
        public Factura Pop()
        {
            if (raiz == null || raiz.Claves.Count == 0)
            {
                return null;
            }

            // En un Árbol B, Pop podría ser eliminar la factura con el ID más alto
            // para simular un comportamiento LIFO similar a una ArbolB
            Factura ultimaFactura = ObtenerFacturaMasReciente();
            Eliminar(ultimaFactura.ID);
            GenerarGrafico("Facturacion.dot");
            return ultimaFactura;
        }

        // Obtiene la factura más reciente (mayor ID, asumiendo que IDs más altos son más recientes)
        private Factura ObtenerFacturaMasReciente()
        {
            NodoArbolB actual = raiz;
            
            // Navegar hasta el nodo más a la derecha
            while (!actual.EsHoja)
            {
                actual = actual.Hijos[actual.Hijos.Count - 1];
            }
            
            // Retorna la última clave (factura con mayor ID)
            return actual.Claves[actual.Claves.Count - 1];
        }

        // Busca una factura por su ID
        public Factura Buscar(int id)
        {
            return BuscarRecursivo(raiz, id);
        }

        private Factura BuscarRecursivo(NodoArbolB nodo, int id)
        {
            int i = 0;
            // Buscar la primera clave mayor o igual que id
            while (i < nodo.Claves.Count && id > nodo.Claves[i].ID)
            {
                i++;
            }

            // Si encontramos el id, devolvemos la factura
            if (i < nodo.Claves.Count && id == nodo.Claves[i].ID)
            {
                return nodo.Claves[i];
            }

            // Si es una hoja y no encontramos el id, no existe
            if (nodo.EsHoja)
            {
                return null;
            }

            // Si no es hoja, buscamos en el hijo correspondiente
            return BuscarRecursivo(nodo.Hijos[i], id);
        }

        // Método para eliminar una factura por su ID
        public void Eliminar(int id)
        {
            EliminarRecursivo(raiz, id);
            
            // Si la raíz quedó vacía pero tiene hijos, el primer hijo se convierte en la nueva raíz
            if (raiz.Claves.Count == 0 && !raiz.EsHoja && raiz.Hijos.Count > 0)
            {
                NodoArbolB antiguaRaiz = raiz;
                raiz = raiz.Hijos[0];
            }

            GenerarGrafico("Facturacion.dot");
        }

        private void EliminarRecursivo(NodoArbolB nodo, int id)
        {
            int indice = EncontrarIndice(nodo, id);

            // Caso 1: La clave está en este nodo
            if (indice < nodo.Claves.Count && nodo.Claves[indice].ID == id)
            {
                // Si es hoja, simplemente eliminamos
                if (nodo.EsHoja)
                {
                    nodo.Claves.RemoveAt(indice);
                }
                else
                {
                    // Si no es hoja, usamos estrategias más complejas
                    EliminarDeNodoInterno(nodo, indice);
                }
            }
            else
            {
                // Caso 2: La clave no está en este nodo
                if (nodo.EsHoja)
                {
                    Console.WriteLine($"La factura con ID {id} no existe en el árbol");
                    return;
                }

                // Determinar si el último hijo fue visitado
                bool ultimoHijo = (indice == nodo.Claves.Count);

                // Verificar que el índice está dentro del rango válido
                if (indice < nodo.Hijos.Count)
                {
                    // Si el hijo tiene el mínimo de claves, rellenarlo
                    if (!nodo.Hijos[indice].TieneMinimoClaves())
                    {
                        RellenarHijo(nodo, indice);
                    }

                    // Si el último hijo se fusionó, recurrimos al hijo anterior
                    if (ultimoHijo && indice > nodo.Hijos.Count - 1)
                    {
                        EliminarRecursivo(nodo.Hijos[indice - 1], id);
                    }
                    else
                    {
                        EliminarRecursivo(nodo.Hijos[indice], id);
                    }
                }
            }
        }

        // Encuentra el índice de la primera clave mayor o igual a id
        private int EncontrarIndice(NodoArbolB nodo, int id)
        {
            int indice = 0;
            while (indice < nodo.Claves.Count && nodo.Claves[indice].ID < id)
            {
                indice++;
            }
            return indice;
        }

        // Elimina un elemento de un nodo interno
        private void EliminarDeNodoInterno(NodoArbolB nodo, int indice)
        {
            Factura clave = nodo.Claves[indice];

            // Caso 2a: Si el hijo anterior tiene más del mínimo de claves
            if (indice < nodo.Hijos.Count && nodo.Hijos[indice].Claves.Count > MIN_CLAVES)
            {
                // Reemplazar clave con el predecesor
                Factura predecesor = ObtenerPredecesor(nodo, indice);
                nodo.Claves[indice] = predecesor;
                EliminarRecursivo(nodo.Hijos[indice], predecesor.ID);
            }
            // Caso 2b: Si el hijo siguiente tiene más del mínimo de claves
            else if (indice + 1 < nodo.Hijos.Count && nodo.Hijos[indice + 1].Claves.Count > MIN_CLAVES)
            {
                // Reemplazar clave con el sucesor
                Factura sucesor = ObtenerSucesor(nodo, indice);
                nodo.Claves[indice] = sucesor;
                EliminarRecursivo(nodo.Hijos[indice + 1], sucesor.ID);
            }
            // Caso 2c: Si ambos hijos tienen el mínimo de claves
            else if (indice + 1 < nodo.Hijos.Count)
            {
                // Fusionar el hijo actual con el siguiente
                FusionarNodos(nodo, indice);
                EliminarRecursivo(nodo.Hijos[indice], clave.ID);
            }
        }

        // Obtiene el predecesor de una clave (la clave más grande en el subárbol izquierdo)
        private Factura ObtenerPredecesor(NodoArbolB nodo, int indice)
        {
            NodoArbolB actual = nodo.Hijos[indice];
            while (!actual.EsHoja)
            {
                if (actual.Hijos.Count > 0)
                    actual = actual.Hijos[actual.Hijos.Count - 1];
                else
                    break;
            }
            return actual.Claves[actual.Claves.Count - 1];
        }

        // Obtiene el sucesor de una clave (la clave más pequeña en el subárbol derecho)
        private Factura ObtenerSucesor(NodoArbolB nodo, int indice)
        {
            NodoArbolB actual = nodo.Hijos[indice + 1];
            while (!actual.EsHoja)
            {
                if (actual.Hijos.Count > 0)
                    actual = actual.Hijos[0];
                else
                    break;
            }
            return actual.Claves[0];
        }

        // Rellena un hijo que tiene menos del mínimo de claves
        private void RellenarHijo(NodoArbolB nodo, int indice)
        {
            // Si el hermano izquierdo existe y tiene más del mínimo de claves
            if (indice > 0 && nodo.Hijos[indice - 1].Claves.Count > MIN_CLAVES)
            {
                TomaPrestadoDelAnterior(nodo, indice);
            }
            // Si el hermano derecho existe y tiene más del mínimo de claves
            else if (indice + 1 < nodo.Hijos.Count && nodo.Hijos[indice + 1].Claves.Count > MIN_CLAVES)
            {
                TomaPrestadoDelSiguiente(nodo, indice);
            }
            // Si no se puede tomar prestado, fusionar con un hermano
            else if (indice < nodo.Hijos.Count - 1)
            {
                FusionarNodos(nodo, indice);
            }
            else if (indice > 0)
            {
                FusionarNodos(nodo, indice - 1);
            }
        }

        // Toma prestado una clave del hermano anterior
        private void TomaPrestadoDelAnterior(NodoArbolB nodo, int indice)
        {
            if (indice >= nodo.Hijos.Count) return;
            
            NodoArbolB hijo = nodo.Hijos[indice];
            NodoArbolB hermano = nodo.Hijos[indice - 1];

            // Desplazar todas las claves e hijos para hacer espacio para la nueva clave
            hijo.Claves.Insert(0, nodo.Claves[indice - 1]);

            // Si no es hoja, mover también el hijo correspondiente
            if (!hijo.EsHoja && hermano.Hijos.Count > 0)
            {
                hijo.Hijos.Insert(0, hermano.Hijos[hermano.Hijos.Count - 1]);
                hermano.Hijos.RemoveAt(hermano.Hijos.Count - 1);
            }

            // Actualizar la clave del padre
            nodo.Claves[indice - 1] = hermano.Claves[hermano.Claves.Count - 1];
            hermano.Claves.RemoveAt(hermano.Claves.Count - 1);
        }

        // Toma prestado una clave del hermano siguiente
        private void TomaPrestadoDelSiguiente(NodoArbolB nodo, int indice)
        {
            if (indice >= nodo.Hijos.Count) return;
            
            NodoArbolB hijo = nodo.Hijos[indice];
            if (indice + 1 >= nodo.Hijos.Count) return;
            
            NodoArbolB hermano = nodo.Hijos[indice + 1];

            // Añadir la clave del padre al hijo
            hijo.Claves.Add(nodo.Claves[indice]);

            // Si no es hoja, mover también el hijo correspondiente
            if (!hijo.EsHoja && hermano.Hijos.Count > 0)
            {
                hijo.Hijos.Add(hermano.Hijos[0]);
                hermano.Hijos.RemoveAt(0);
            }

            // Actualizar la clave del padre
            nodo.Claves[indice] = hermano.Claves[0];
            hermano.Claves.RemoveAt(0);
        }

        // Fusiona dos nodos hijo
        private void FusionarNodos(NodoArbolB nodo, int indice)
        {
            if (indice >= nodo.Hijos.Count - 1) return;
            
            NodoArbolB hijo = nodo.Hijos[indice];
            NodoArbolB hermano = nodo.Hijos[indice + 1];

            // Añadir la clave del padre al hijo
            if (indice < nodo.Claves.Count)
                hijo.Claves.Add(nodo.Claves[indice]);

            // Añadir todas las claves del hermano al hijo
            for (int i = 0; i < hermano.Claves.Count; i++)
            {
                hijo.Claves.Add(hermano.Claves[i]);
            }

            // Si no es hoja, mover también los hijos
            if (!hijo.EsHoja)
            {
                for (int i = 0; i < hermano.Hijos.Count; i++)
                {
                    hijo.Hijos.Add(hermano.Hijos[i]);
                }
            }

            // Remover la clave y el hijo del nodo padre
            if (indice < nodo.Claves.Count)
                nodo.Claves.RemoveAt(indice);
                
            if (indice + 1 < nodo.Hijos.Count)
                nodo.Hijos.RemoveAt(indice + 1);
        }

        // Imprime las facturas (similar al método Print original)
        public void Print()
        {
            List<Factura> facturas = RecorridoInOrden();
            foreach (var factura in facturas)
            {
                Console.Write(factura + " -> ");
            }
            Console.WriteLine("NULL");
            GenerarGrafico("Facturacion.dot");
        }

        // Recorrido InOrden del árbol para obtener las facturas en orden
        public List<Factura> RecorridoInOrden()
        {
            List<Factura> resultado = new List<Factura>();
            RecorridoInOrdenRecursivo(raiz, resultado);
            return resultado;
        }

        private void RecorridoInOrdenRecursivo(NodoArbolB nodo, List<Factura> resultado)
        {
            if (nodo == null)
                return;

            int i;
            for (i = 0; i < nodo.Claves.Count; i++)
            {
                // Recorrer el hijo izquierdo
                if (!nodo.EsHoja && i < nodo.Hijos.Count)
                    RecorridoInOrdenRecursivo(nodo.Hijos[i], resultado);

                // Agregar la clave actual
                resultado.Add(nodo.Claves[i]);
            }

            // Recorrer el último hijo
            if (!nodo.EsHoja && i < nodo.Hijos.Count)
                RecorridoInOrdenRecursivo(nodo.Hijos[i], resultado);
        }

        // Método para generar el gráfico del árbol B
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
                writer.WriteLine("digraph BTree {");
                writer.WriteLine("    node [shape=record];");
                writer.WriteLine("    rankdir=TB;");
                
                int contadorNodos = 0;
                GraficarNodos(raiz, writer, ref contadorNodos);
                GraficarConexiones(raiz, writer);
                
                writer.WriteLine("}");
            }

            // Generar el archivo PNG usando Graphviz
            var process = new Process();
            process.StartInfo.FileName = "dot";
            process.StartInfo.Arguments = $"-Tpng {filePath} -o {Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            // Mostrar la URL en la consola
            Console.WriteLine("La gráfica del árbol B se ha guardado en: " + Path.GetFullPath(Path.Combine(carpeta, Path.ChangeExtension(fileName, ".png"))));
        }

                private void GraficarNodos(NodoArbolB nodo, StreamWriter writer, ref int contadorNodos)
        {
            if (nodo == null)
                return;
        
            string nodeId = $"node{contadorNodos++}";
            
            // Construir la etiqueta del nodo
            StringBuilder label = new StringBuilder("{");
            
            for (int i = 0; i < nodo.Claves.Count; i++)
            {
                if (i > 0) label.Append("|");
                label.Append($"<p{i}> ID: {nodo.Claves[i].ID}, Orden: {nodo.Claves[i].ID_Orden}, Total: {nodo.Claves[i].Total:F2}");
            }
            
            label.Append("}");
            
            writer.WriteLine($"    {nodeId} [label=\"{label}\"];");
            
            // Asignar un ID único a este nodo usando la propiedad Tag
            nodo.Tag = nodeId;
            
            // Generar nodos para los hijos
            for (int i = 0; i < nodo.Hijos.Count; i++)
            {
                GraficarNodos(nodo.Hijos[i], writer, ref contadorNodos);
            }
        }
        
        private void GraficarConexiones(NodoArbolB nodo, StreamWriter writer)
        {
            if (nodo == null || nodo.EsHoja)
                return;
                
            // Generar conexiones a los hijos
            for (int i = 0; i < nodo.Hijos.Count; i++)
            {
                if (nodo.Hijos[i] != null)
                {
                    int portIndex = Math.Min(i, nodo.Claves.Count - 1);
                    writer.WriteLine($"    {nodo.Tag}:p{portIndex} -> {nodo.Hijos[i].Tag};");
                    GraficarConexiones(nodo.Hijos[i], writer);
                }
            }
        }
    }
}