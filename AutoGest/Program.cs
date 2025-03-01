﻿using Gtk;

class Program
{
    public static void Main(string[] args)
    {
        Application.Init();

        // Crear la ventana principal
        InterfazMenu interfazMenu = new InterfazMenu();
        interfazMenu.ShowAll();

        Application.Run();
    }
}