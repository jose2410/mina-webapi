using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("cliente")]
    public class ClienteController : ControllerBase
    {
        [HttpGet]
        [Route("listar")]
        public dynamic listarClientes()
        {

            List<Cliente> lista = new List<Cliente>
            {
                new Cliente
                {
                    id = "1",
                    correo = "joseacb24@gmail.com",
                    edad = "26",
                    nombre = "José Cruz"
                },
                new Cliente
                {
                    id = "2",
                    correo = "jossy@gmail.com",
                    edad = "23",
                    nombre = "Josselyn Flores"
                }
            };
            return lista;
            //return new
            //{
            //    nombre = "José",
            //    edad = "26"
            //};

        }

        [HttpGet]
        [Route("listarxid")]
        public dynamic listarClientesxid(int codigo)
        {

            return new Cliente { 
                id = codigo.ToString(),
                correo = "google@gmail.com",
                edad="19",
                nombre="Dan Vidal"
            };

        }

        [HttpPost]
        [Route("guardar")]
        public dynamic guardarClientes(Cliente cliente)
        {
            cliente.id = "3";

            return new
            {
                success = true,
                message = "Cliente registrado",
                result = cliente
            };

        }
    }
}
