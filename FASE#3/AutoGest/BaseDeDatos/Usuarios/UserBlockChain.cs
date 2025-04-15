using System;
using System.IO;
using System.Collections.Generic;

namespace Usuarios
{
    public class UserBlockchain
    {
        private UsuarioBlockchain blockchain;
        private Dictionary<int, int> idToBlockIndex;  // Mapeo de ID de usuario a índice del bloque

        public UserBlockchain()
        {
            blockchain = new UsuarioBlockchain();
            idToBlockIndex = new Dictionary<int, int>();
        }

        public void Insertar(Usuario usuario)
        {
            idToBlockIndex[usuario.Id] = blockchain.Chain.Count;
            blockchain.AddBlock(usuario);
        }

        public void Eliminar(int id)
        {
            Usuario usuario = Buscar(id);
            if (usuario != null)
            {
                Usuario marcadorEliminacion = usuario;
                marcadorEliminacion.Correo = "ELIMINADO_" + marcadorEliminacion.Correo;
                blockchain.AddBlock(marcadorEliminacion);
                idToBlockIndex[id] = blockchain.Chain.Count - 1;
            }
        }

        public void ModificarUsuario(int id, string nuevoNombre, string nuevoApellido, string nuevoCorreo, string nuevaContrasena)
        {
            Usuario usuario = Buscar(id);
            if (usuario != null)
            {
                Usuario usuarioModificado = usuario;
                usuarioModificado.Name = nuevoNombre;
                usuarioModificado.LastName = nuevoApellido;
                usuarioModificado.Correo = nuevoCorreo;
                usuarioModificado.Contrasena = nuevaContrasena;
                blockchain.AddBlock(usuarioModificado);
                idToBlockIndex[id] = blockchain.Chain.Count - 1;
                Console.WriteLine("Usuario modificado exitosamente.");
            }
            else
            {
                Console.WriteLine("Usuario no encontrado.");
            }
        }

        public Usuario Buscar(int id)
        {
            if (idToBlockIndex.TryGetValue(id, out int blockIndex))
            {
                Block block = blockchain.Chain[blockIndex];
                Usuario usuario = block.Data;
                if (usuario.Correo.StartsWith("ELIMINADO_"))
                {
                    return null; // Usuario eliminado
                }
                return usuario;
            }
            return null;
        }

        public Usuario BuscarCorreo(string correo)
        {
            foreach (var block in blockchain.Chain)
            {
                if (block.Index == 0) continue; // Saltar el bloque génesis
                Usuario usuario = block.Data;
                if (!usuario.Correo.StartsWith("ELIMINADO_") && usuario.Correo == correo)
                {
                    return usuario;
                }
            }
            return null;
        }

        public void Imprimir()
        {
            foreach (var block in blockchain.Chain)
            {
                if (block.Index == 0) continue; // Saltar el bloque génesis
                Usuario usuario = block.Data;
                if (!usuario.Correo.StartsWith("ELIMINADO_"))
                {
                    Console.WriteLine($"ID: {usuario.Id}, Nombre: {usuario.Name}, Apellido: {usuario.LastName}, Correo: {usuario.Correo}, Contraseña: {usuario.Contrasena}");
                }
            }
        }

        public bool VerificarCredenciales(string correo, string contrasena)
        {
            foreach (var block in blockchain.Chain)
            {
                if (block.Index == 0) continue; // Saltar el bloque génesis
                Usuario usuario = block.Data;
                if (!usuario.Correo.StartsWith("ELIMINADO_") && usuario.Correo == correo && usuario.Contrasena == contrasena)
                {
                    return true;
                }
            }
            return false;
        }

        public (bool, int) VerificarCredencialesConId(string correo, string contrasena)
        {
            foreach (var block in blockchain.Chain)
            {
                if (block.Index == 0) continue; // Saltar el bloque génesis
                Usuario usuario = block.Data;
                if (!usuario.Correo.StartsWith("ELIMINADO_") && usuario.Correo == correo && usuario.Contrasena == contrasena)
                {
                    return (true, usuario.Id);
                }
            }
            return (false, -1);
        }

        public string ObtenerLista()
        {
            string lista = "";
            foreach (var block in blockchain.Chain)
            {
                if (block.Index == 0) continue; // Saltar el bloque génesis
                Usuario usuario = block.Data;
                if (!usuario.Correo.StartsWith("ELIMINADO_"))
                {
                    lista += $"ID: {usuario.Id}, Nombre: {usuario.Name}, Apellido: {usuario.LastName}, Correo: {usuario.Correo}, Contraseña: {usuario.Contrasena}\n";
                }
            }
            return lista;
        }

        public void GenerarGrafico(string fileName)
        {
            blockchain.GenerarGrafico(fileName);
        }
    }
}