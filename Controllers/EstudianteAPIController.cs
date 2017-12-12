using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using EstudianteNS;

namespace RegistroEstudiantesMVC.Controllers{

    [Route("api/Estudiante")]
    public class EstudianteAPIController:Controller{
        private DALMySQLContext _contextoMySQL;
        private DALContext _contextoSqlite;

        public EstudianteAPIController(DALMySQLContext mySQLContext, DALContext SqliteContext){
            _contextoMySQL = mySQLContext;
            _contextoSqlite = SqliteContext;
        }

        [Route("listado")]
        public DbSet<Estudiante> ListarEstudiantes() => _contextoMySQL.estudiante;

        [HttpGet("{nombre}", Name = "GetEstudianteByName")]
        public Estudiante GetOneEstudiante(string nombre){
            var estudiante = _contextoMySQL.estudiante.Find(nombre);
            if (estudiante == null){
                Response.StatusCode = 404;
                return null;
            }
            else{
                return estudiante;
            }
        }

        [HttpPost]
        public IActionResult InsertarEstudiante([FromBody]Estudiante estudiante){
            if (!ModelState.IsValid){
                BadRequest();
            }
            var transaccion = new Transaccion(){
                Fecha = DateTime.Now,
                Hora = DateTime.Now,
                ip = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            };
            try{
                _contextoMySQL.estudiante.Add(estudiante);
                _contextoMySQL.SaveChanges();
                transaccion.Aprobado = true;
            }
            catch{
                transaccion.Aprobado = false;
            }
            finally{
                _contextoSqlite.transaccion.Add(transaccion);
                _contextoSqlite.SaveChanges();
            }
            Response.StatusCode = 201;
            return CreatedAtRoute("GetEstudianteByName",
                new {nombre = estudiante.Nombre},
                estudiante);
        }

        [HttpPut("{nombre}")]
        public IActionResult ActualizarEstudiante(string nombre, [FromBody]Estudiante estudiante){
            if (string.IsNullOrWhiteSpace(nombre) || 
                estudiante.Nombre != nombre ||
                !ModelState.IsValid){
                return BadRequest();
            }
            
            var transaccion = new Transaccion(){
                Fecha = DateTime.Now,
                Hora = DateTime.Now,
                ip = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            };
            try{
                _contextoMySQL.estudiante.Update(estudiante);
                _contextoMySQL.SaveChanges();
                transaccion.Aprobado = true;
            }
            catch{
                transaccion.Aprobado = false;
            }
            finally{
                _contextoSqlite.transaccion.Add(transaccion);
                _contextoSqlite.SaveChanges();
            }
            Response.StatusCode = 201;
            return NoContent();
        }
    }
}