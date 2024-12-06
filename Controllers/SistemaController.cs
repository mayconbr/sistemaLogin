using Login;
using Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace CRMAudax.Controllers
{
	public class SistemaController : Controller
	{
        [Authorize]
        public IActionResult NovoSistema()
        {
            return View();
        }

        public IActionResult GerenciadorSistema()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Systems = GetSystems();
            return View(mymodel);
        }

        public IActionResult Tipos(long id)
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Types = GetTypesId(id);
            mymodel.Systems = GetSystems();
            return View(mymodel);
        }

        [HttpPost]
        [Route("~/InsertSystem")]
        public IActionResult InsertSystem([FromBody] TableSystem request)
        {
            using (var context = new MyDbContext())
            {
                context.System.Add(new TableSystem
                {
                    Name = request.Name,
                    Domain = request.Domain
                }); 
                context.SaveChanges();
            }               
            return Ok();
        }

        [HttpPut]
        [Route("~/UpdateSystem")]
        public IActionResult UpdateSystem([FromBody] TableSystem request)
        {
            using (var context = new MyDbContext())
            {

                var System = (from system in context.System
                              where system.Id == request.Id
                              select system).ToArray().FirstOrDefault();

                if(System != null)
                {
                    System.Name = request.Name;
                    System.Domain = request.Domain;

                    context.SaveChanges();
                }
            }
            return Ok();
        }

        [HttpPut]
        [Route("~/DeleteSystem/{Id}")]
        public IActionResult DeleteSystem(long Id)
        {
            using (var context = new MyDbContext())
            {

                var System = (from system in context.System
                              where system.Id == Id
                              select system).ToArray().FirstOrDefault();

                if (System != null)
                {
                    System.DataDelete = DateTime.UtcNow;

                    context.SaveChanges();
                }
            }
            return Ok();
        }

        [HttpGet]
        [Route("~/GetSystems")]
        public IEnumerable<TableSystem> GetSystems() {

            using (var context = new MyDbContext())
            {
                var Systems  = (from system in context.System
                                where system.DataDelete == null
                                select system).ToList();

                return Systems;
            }         
        }

        [HttpGet]
        [Route("~/GetSystemId/{Id}")]
        public IEnumerable<TableSystem> GetSystemsId(long Id)
        {

            using (var context = new MyDbContext())
            {
                var Systems = (from system in context.System
                               where system.Id == Id
                               select system).ToList();

                return Systems;
            }
        }

        [HttpGet("/GetTypesId/{Id}")]
        public IEnumerable<TableType> GetTypesId(long Id)
        {
            using ( var context = new MyDbContext())
            {
                var Types = (from type in context.Type
                             where type.SystemId == Id
                             where type.DataDelete == null
                             select new TableType
                             {
                                 Id = type.Id,
                                 Name = type.Name,
                                 SystemId = type.SystemId,
                                 System = type.System,
                             }).ToList();

                return Types;
            }
        }

        [HttpPost]
        [Route("~/InsertType")]
        public IActionResult InsertType([FromBody] TableType request)
        {
            using (var context = new MyDbContext())
            {
                context.Type.Add(new TableType
                {
                    Name = request.Name,
                    SystemId = request.SystemId,
                });
                context.SaveChanges();
            }
            return Ok();
        }

        [HttpPut]
        [Route("~/DeleteType/{Id}")]
        public IActionResult DeleteType(long Id)
        {
            using (var context = new MyDbContext())
            {
                var Type = (from type in context.Type
                              where type.Id == Id
                              select type).ToArray().FirstOrDefault();
                if (Type != null)
                {
                    Type.DataDelete = DateTime.UtcNow;
                    context.SaveChanges();
                }
            }
            return Ok();
        }

    }
}


