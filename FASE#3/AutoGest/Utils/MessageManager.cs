using Gtk;
using System;

namespace AutoGest.Utils
{
    /// <summary>
    /// Clase utilitaria para mostrar mensajes de estado de forma uniforme en la aplicación,
    /// evitando los diálogos modales que pueden causar problemas con GTK.
    /// </summary>
    public static class MessageManager
    {
        public enum MessageType
        {
            Info,
            Success,
            Warning,
            Error
        }

        /// <summary>
        /// Muestra un mensaje en una etiqueta de estado en lugar de usar un diálogo modal.
        /// </summary>
        /// <param name="statusLabel">La etiqueta donde mostrar el mensaje</param>
        /// <param name="message">El mensaje a mostrar</param>
        /// <param name="type">El tipo de mensaje (define el color)</param>
        /// <param name="autoHide">Si es true, el mensaje desaparecerá automáticamente después de un tiempo</param>
        public static void ShowMessage(Label statusLabel, string message, MessageType type, bool autoHide = false)
        {
            string color = GetColorForMessageType(type);
            statusLabel.Markup = $"<span foreground='{color}'>{message}</span>";
            statusLabel.Visible = true;

            if (autoHide)
            {
                // Programar para que el mensaje desaparezca después de 5 segundos
                GLib.Timeout.Add(5000, () => {
                    statusLabel.Text = "";
                    statusLabel.Visible = false;
                    return false; // para que se ejecute solo una vez
                });
            }
        }

        /// <summary>
        /// Obtiene el código de color adecuado para cada tipo de mensaje
        /// </summary>
        private static string GetColorForMessageType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    return "blue";
                case MessageType.Success:
                    return "green";
                case MessageType.Warning:
                    return "orange";
                case MessageType.Error:
                    return "red";
                default:
                    return "black";
            }
        }

        /// <summary>
        /// Crea una etiqueta para mostrar mensajes de estado.
        /// </summary>
        public static Label CreateStatusLabel()
        {
            Label label = new Label("");
            label.UseMarkup = true;
            label.Visible = false;
            label.SetAlignment(0.5f, 0.5f);
            label.Wrap = true;
            label.WidthRequest = 400;
            return label;
        }

        /// <summary>
        /// Pide confirmación usando una etiqueta y botones en lugar de un diálogo modal.
        /// </summary>
        /// <param name="container">El contenedor donde mostrar la confirmación</param>
        /// <param name="message">El mensaje de confirmación</param>
        /// <param name="onYes">Acción a ejecutar si se confirma</param>
        /// <param name="onNo">Acción a ejecutar si se cancela</param>
        public static void AskConfirmation(Box container, string message, System.Action onYes, System.Action onNo = null)
        {
            // Crear frame para la confirmación
            Frame confirmFrame = new Frame("Confirmación requerida");
            VBox confirmBox = new VBox(false, 5);
            confirmBox.BorderWidth = 10;

            // Mensaje
            Label confirmLabel = new Label(message);
            confirmLabel.Wrap = true;
            confirmBox.PackStart(confirmLabel, false, false, 5);

            // Botones
            HBox buttonBox = new HBox(true, 10);
            Button yesButton = new Button("Sí");
            Button noButton = new Button("No");

            yesButton.Clicked += (sender, e) => {
                container.Remove(confirmFrame);
                onYes?.Invoke();
            };

            noButton.Clicked += (sender, e) => {
                container.Remove(confirmFrame);
                onNo?.Invoke();
            };

            buttonBox.PackStart(yesButton, true, true, 5);
            buttonBox.PackStart(noButton, true, true, 5);
            confirmBox.PackStart(buttonBox, false, false, 5);

            confirmFrame.Add(confirmBox);
            container.PackStart(confirmFrame, false, false, 10);
            container.ShowAll();
        }
    }
}
