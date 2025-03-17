﻿using Gtk;
using System;

public class Login : Window
{
    public Login() : base("Juan Esteban CHACÓN TRAMPE 202300431")
    {
        SetDefaultSize(300, 200);
        SetPosition(WindowPosition.Center);

        VBox vbox = new VBox(false, 10);

        // Crear el label superior
        Label labelTitulo = new Label("JUAN ESTEBAN CHACÓN TRAMPE 202300431");
        vbox.PackStart(labelTitulo, false, false, 0);

        // Crear el label y el entry para el correo
        HBox hboxCorreo = new HBox(false, 5);
        Label labelCorreo = new Label("Correo:");
        Entry entryCorreo = new Entry();
        hboxCorreo.PackStart(labelCorreo, false, false, 10);
        hboxCorreo.PackStart(entryCorreo, true, true, 10);

        // Crear el label y el entry para la contraseña
        HBox hboxContrasena = new HBox(false, 5);
        Label labelContrasena = new Label("Contraseña:");
        Entry entryContrasena = new Entry();
        entryContrasena.Visibility = false; // Ocultar la contraseña
        hboxContrasena.PackStart(labelContrasena, false, false, 10);
        hboxContrasena.PackStart(entryContrasena, true, true, 10);

        // Crear el botón de validar
        Button buttonValidar = new Button("Validar");
        buttonValidar.Clicked += (sender, e) => {
            // Aquí puedes agregar la lógica de validación
            Console.WriteLine("Correo: " + entryCorreo.Text);
            Console.WriteLine("Contraseña: " + entryContrasena.Text);
        };

        vbox.PackStart(hboxCorreo, false, false, 10);
        vbox.PackStart(hboxContrasena, false, false, 10);
        vbox.PackStart(buttonValidar, false, false, 10);

        Add(vbox);
        ShowAll();
    }
}

class Program
{
    public static void Main(string[] args)
    {
        Application.Init();

        // Crear la ventana de login
        Login login = new Login();
        login.ShowAll();

        Application.Run();
    }
}