using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using Facturas;
using AutoGest;
using AutoGest.Utils;

namespace AutoGest.Interfaces
{
    public unsafe class InterfazCF : Box
    {
        private InterfazMain mainWindow;
        private ArbolM arbolFacturas;
        private VBox facturaBox;
        private Frame frameFactura;
        private Label statusLabel;

        public InterfazCF(InterfazMain mainWindow, ArbolM arbolFacturas)
        {
            this.mainWindow = mainWindow;
            this.arbolFacturas = arbolFacturas;

            // Configurar espaciado y bordes
            BorderWidth = 20;
            Spacing = 15;
            
            // Contenedor principal centralizado
            VBox contentBox = new VBox(false, 15);
            contentBox.BorderWidth = 20;
            
            // Título del panel con mejor formato
            Label labelTitulo = new Label();
            labelTitulo.Markup = "<span font='16' weight='bold'>CANCELACIÓN DE FACTURA</span>";
            labelTitulo.SetAlignment(0.5f, 0.5f);
            contentBox.PackStart(labelTitulo, false, false, 20);
            
            // Añadir etiqueta de estado para mensajes
            statusLabel = MessageManager.CreateStatusLabel();
            contentBox.PackStart(statusLabel, false, false, 10);
            
            // Separador después del título
            HSeparator separator = new HSeparator();
            contentBox.PackStart(separator, false, false, 10);
            
            // Frame para mostrar la información de la última factura
            frameFactura = new Frame("Última Factura Generada");
            facturaBox = new VBox(false, 10);
            facturaBox.BorderWidth = 15;
            
            // Actualizar la información de la factura
            ActualizarInfoFactura();
            
            frameFactura.Add(facturaBox);
            contentBox.PackStart(frameFactura, true, true, 10);
            
            // Separador antes del botón de volver
            HSeparator separator2 = new HSeparator();
            contentBox.PackStart(separator2, false, false, 10);
            
            // Botón para volver al menú principal
            Button buttonVolver = new Button();
            Label volverLabel = new Label();
            volverLabel.Markup = "<span font='10'>Volver al Menú</span>";
            buttonVolver.Add(volverLabel);
            buttonVolver.SetSizeRequest(150, 35);
            buttonVolver.Clicked += delegate {
                Console.WriteLine("Volviendo al menú principal...");
                mainWindow.ShowMenuPanel();
            };
            
            // Centrar el botón de volver
            HBox volverBox = new HBox();
            volverBox.PackStart(new Label(""), true, true, 0);
            volverBox.PackStart(buttonVolver, false, false, 0);
            volverBox.PackStart(new Label(""), true, true, 0);
            contentBox.PackStart(volverBox, false, false, 10);
            
            // Centrar el contenido en el panel
            HBox centeringBox = new HBox();
            centeringBox.PackStart(new Label(""), true, true, 0);
            centeringBox.PackStart(contentBox, false, false, 0);
            centeringBox.PackStart(new Label(""), true, true, 0);
            
            PackStart(centeringBox, true, true, 0);
        }
        private void ActualizarInfoFactura()
        {
            Application.Invoke(delegate {
                try {
                    // Crear un nuevo VBox en lugar de limpiar el existente
                    VBox nuevoFacturaBox = new VBox(false, 10);
                    nuevoFacturaBox.BorderWidth = 15;
                    
                    // Obtener la última factura generada
                    Factura ultimaFactura = ObtenerUltimaFactura(arbolFacturas);
                    
                    if (ultimaFactura != null)
                    {
                        // Información de la factura en color ROJO
                        AddInfoRow(nuevoFacturaBox, "ID:", ultimaFactura.ID.ToString(), true);
                        AddInfoRow(nuevoFacturaBox, "ID Orden:", ultimaFactura.ID_Orden.ToString(), true);
                        AddInfoRow(nuevoFacturaBox, "Total:", $"Q{ultimaFactura.Total:F2}", true);
                        
                        // Botón para cancelar la factura
                        Button buttonCancelar = new Button();
                        Label buttonLabel = new Label();
                        buttonLabel.Markup = "<span font='11' weight='bold'>Cancelar Factura</span>";
                        buttonCancelar.Add(buttonLabel);
                        buttonCancelar.SetSizeRequest(180, 40);
                        buttonCancelar.ModifyBg(StateType.Normal, new Gdk.Color(255, 200, 200));
                        
                        // Copia local del ID para evitar problemas de captura
                        int idFactura = ultimaFactura.ID;
                        buttonCancelar.Clicked += delegate {
                            CancelarFactura(idFactura);
                        };
                        
                        // Centrar el botón
                        HBox buttonBox = new HBox();
                        buttonBox.PackStart(new Label(""), true, true, 0);
                        buttonBox.PackStart(buttonCancelar, false, false, 0);
                        buttonBox.PackStart(new Label(""), true, true, 0);
                        nuevoFacturaBox.PackStart(buttonBox, false, false, 15);
                    }
                    else
                    {
                        // Mensaje si no hay facturas
                        Label labelNoFacturas = new Label("No se encontraron facturas en el sistema.");
                        labelNoFacturas.SetAlignment(0.5f, 0.5f);
                        nuevoFacturaBox.PackStart(labelNoFacturas, false, false, 20);
                    }
                    
                    // Primero mostrar el nuevo contenedor
                    nuevoFacturaBox.ShowAll();
                    
                    // Eliminar el contenedor anterior y agregar el nuevo
                    if (frameFactura != null)
                    {
                        if (facturaBox != null)
                        {
                            frameFactura.Remove(facturaBox);
                        }
                        frameFactura.Add(nuevoFacturaBox);
                        facturaBox = nuevoFacturaBox;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error al actualizar información de factura: {ex.Message}");
                }
            });
        }
        // Método auxiliar para añadir filas de información (con opción de color rojo)
        private void AddInfoRow(VBox container, string label, string value, bool useRedColor = false)
        {
            HBox hbox = new HBox();
            
            Label labelField = new Label(label);
            labelField.SetAlignment(0, 0.5f);
            labelField.WidthRequest = 80;
            
            Label labelValue = new Label(value);
            labelValue.SetAlignment(0, 0.5f);
            
            // Cambiar a color ROJO en lugar de azul
            if (useRedColor)
            {
                labelValue.ModifyFg(StateType.Normal, new Gdk.Color(200, 0, 0));
            }
            else
            {
                labelValue.ModifyFg(StateType.Normal, new Gdk.Color(0, 0, 150));
            }
            
            hbox.PackStart(labelField, false, false, 0);
            hbox.PackStart(labelValue, true, true, 5);
            
            container.PackStart(hbox, false, false, 5);
        }

        // Método para cancelar una factura
        private void CancelarFactura(int idFactura)
        {
            try
            {
                MessageManager.AskConfirmation(
                    this, // Este es el contenedor (Box) donde se mostrará la confirmación
                    $"¿Está seguro que desea cancelar la factura #{idFactura}?",
                    () => {
                        // Intentar eliminar la factura
                        bool eliminacionExitosa = arbolFacturas.Eliminar(idFactura);
                        
                        if (eliminacionExitosa)
                        {
                            // Mostrar mensaje de éxito
                            MessageManager.ShowMessage(statusLabel, $"La factura #{idFactura} ha sido cancelada exitosamente.", MessageManager.MessageType.Success);
                            
                            // Actualizar la información de la factura en la misma interfaz
                            ActualizarInfoFactura();
                        }
                        else
                        {
                            // Mostrar mensaje de error
                            MessageManager.ShowMessage(statusLabel, $"Error: No se pudo cancelar la factura #{idFactura}.", MessageManager.MessageType.Error);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cancelar factura: {ex.Message}");
                MessageManager.ShowMessage(statusLabel, $"Error al cancelar factura: {ex.Message}", MessageManager.MessageType.Error);
            }
        }

        private Factura ObtenerUltimaFactura(ArbolM arbolFacturas)
        {
            try
            {
                // Obtener todas las facturas en orden
                List<Factura> facturas = arbolFacturas.RecorridoInOrden();

                if (facturas != null && facturas.Count > 0)
                {
                    // Obtener la factura con el ID más alto
                    return facturas.OrderByDescending(f => f.ID).First();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la última factura: {ex.Message}");
            }

            return null;
        }
    }
}