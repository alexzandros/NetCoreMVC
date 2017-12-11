using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using EstudianteNS;

namespace RegistroEstudiantesMVC.Controllers{

    public class EstudianteController:Controller{

        private DALMySQLContext _contextoMySQL;
        private DALContext _contextoSqlite;

        public EstudianteController(DALMySQLContext mySQLContext, DALContext SqliteContext){
            _contextoMySQL = mySQLContext;
            _contextoSqlite = SqliteContext;
        }

        public IActionResult ListarEstudiantes(){
            var listaEstudiantes = _contextoMySQL.estudiante;
            return View(listaEstudiantes);

        }

        public IActionResult InsertarEstudiante()
        {
            return View("Views\\Estudiante\\InsertarEstudiante.cshtml");
        }

        [HttpPost]
        public IActionResult InsertarEstudiante(Estudiante estudiante)
        {
            if(!ModelState.IsValid){
                return BadRequest(new {Error="Datos no vàlidos"});       
            }
            Transaccion transaccion = new Transaccion();
            transaccion.Fecha = DateTime.Now;
            transaccion.Hora = DateTime.Now;
            transaccion.ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            
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
            return RedirectToAction("ListarEstudiantes");
        }

        public IActionResult EditarEstudiante(string nombreEstudiante){
            var estudianteAEditar = _contextoMySQL.estudiante.Find(nombreEstudiante);
            if (estudianteAEditar == null){
                return NotFound();
            }
            return View("Views\\Estudiante\\EditarEstudiante.cshtml",estudianteAEditar);
        }

        [HttpPost]
        public IActionResult EditarEstudiante(Estudiante estudiante)
        {
            if(!ModelState.IsValid){
                return BadRequest(new {Error="Datos no válidos"});       
            }
            Transaccion transaccion = new Transaccion();
            transaccion.Fecha = DateTime.Now;
            transaccion.Hora = DateTime.Now;
            transaccion.ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            
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
            return RedirectToAction("ListarEstudiantes");
        }
    }
}
