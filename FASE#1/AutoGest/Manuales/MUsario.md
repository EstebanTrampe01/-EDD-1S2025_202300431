# Manual de Usuario

## Introducción
AutoGest Pro es un sistema diseñado para la gestión eficiente de talleres de reparación de vehículos, permitiendo la administración de vehículos, repuestos, servicios y facturación.

## Requisitos del Sistema
- **Sistema Operativo**: Linux
- **Lenguaje de Programación**: C#
- **Bibliotecas Requeridas**: GTK para interfaces gráficas, Graphviz para reportes

## Instalación
1. Clonar el repositorio desde GitHub.
2. Compilar el código fuente en un entorno compatible con C#.
3. Ejecutar la aplicación.

## Inicio de Sesión
- **Usuario Administrador**: root@gmail.com
- **Contraseña**: root123

## Funcionalidades
### Gestión de Usuarios
- Agregar, editar y eliminar usuarios.
- Listar usuarios y ver sus vehículos registrados.

### Gestión de Vehículos
- Registrar vehículos en el sistema.
- Consultar y modificar información de los vehículos.

### Gestión de Repuestos
- Agregar y listar repuestos disponibles.

### Servicios y Facturación
- Generar órdenes de servicio.
- Crear facturas automáticamente.
- Cancelar facturas al recibir pago.

### Reportes
- Listado de usuarios, vehículos, repuestos y servicios.
- Top 5 vehículos con más servicios.
- Top 5 vehículos más antiguos.

# Manual Técnico

## Descripción General
El sistema implementa estructuras de datos avanzadas en C# para gestionar la información de manera eficiente.

## Arquitectura del Sistema
- **Usuarios**: Lista simplemente enlazada.
- **Vehículos**: Lista doblemente enlazada.
- **Repuestos**: Lista circular.
- **Servicios**: Cola.
- **Facturas**: Pila.
- **Bitácora**: Matriz dispersa.

## Lógica del Programa
- Uso de punteros y `unsafe code` para la manipulación eficiente de memoria.
- Integración con Graphviz para la visualización de estructuras de datos.
- Implementación de carga masiva desde archivos JSON.

## Clases y Métodos Principales
```csharp
class Usuario {
    int ID;
    string Nombre;
    string Apellido;
    string Correo;
    string Contrasenia;
}

class Vehiculo {
    int ID;
    int ID_Usuario;
    string Marca;
    string Modelo;
    string Placa;
}

class Factura {
    int ID;
    int ID_Orden;
    double Total;
}
```

## Seguridad y Validaciones
- Validación de existencia de usuarios, vehículos y repuestos antes de generar servicios.
- Restricción de acceso a funcionalidades administrativas.

## Entrega y Repositorio
- **Repositorio en GitHub**: [EDD]1S2025_carnet
- **Fecha de Entrega**: 28/02/2025 23:59 PM

---
