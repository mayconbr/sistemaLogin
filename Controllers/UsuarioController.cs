using Login;
using Login.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Login.Tools;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;

namespace Login.Controllers
{
    //[Authorize]
    public class UsuarioController : Controller
    {
        public IActionResult TodosCadastros()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Users = GetUsers();
            return View(mymodel);
        }

        public IActionResult NovoUsuario()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Types = GetTypeSystem();
            mymodel.SubCategories = new ConfiguracaoController().GetSubcategory();
            return View(mymodel);
        }

        [HttpPost]
        [Route("~/InsertUser")]
        public IActionResult InsertUser([FromBody] TableUser request)
        {
            using (var context = new MyDbContext())
            {
                try
                {
                    string hashString = string.Empty;

                    using (var sha1 = SHA1.Create())
                    {
                        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(request.Email));

                        foreach (byte x in hash)
                        {
                            hashString += String.Format("{0:x2}", x);
                        }
                    }

                    context.User.Add(new TableUser
                    {
                        Name = request.Name,
                        Email = request.Email,
                        Key = request.Key,
                        TypeId = request.TypeId,
                        Status = false,
                        Hash = hashString,
                        SystemId = request.SystemId,
                        SubCategoryId = request.SubCategoryId,
                        DateDelete = null,
                        AddInfo = request.AddInfo,

                    });

                    context.SaveChanges();

                    var link = HttpContext.Request.Host.Host + ':' + HttpContext.Request.Host.Port + "/Home/RecuperarSenha?pass=" + hashString;

                    EmailTask.SendFormatedMail("Novo usuário solicitado - Você tem um convite para utilizar os sistemas GP Audax",
                    request.Name, request.Email,
                    "Foi solicitado o cadastro de uma nova senha de acesso. Para continuar com a solicitação, Use o link a seguir:" +
                    "<br/>" +
                    "Copie o link abaixo e cole no seu navegador:" +
                    "<br/>" + link
                    );

                    return Ok();
                }

                catch (Exception)
                {
                    return BadRequest();
                    throw;
                }
            }
        }

        [HttpPut]
        [Route("~/UpdateUser")]
        public IActionResult UpdateUser([FromBody] TableUser request)
        {
            using (var context = new MyDbContext())
            {
                var User = (from user in context.User
                            where user.Id == request.Id
                            select user).ToArray().FirstOrDefault();

                if (User != null)
                {
                    User.Name = request.Name;
                    User.Email = request.Email;
                    User.Status = request.Status;
                    User.TypeId = request.TypeId;
                }
                return Ok();
            }
        }

        [HttpGet]
        [Route("~/GetUsers")]
        public IEnumerable<TableUser> GetUsers()
        {
            using (var context = new MyDbContext())
            {
                var User = (from user in context.User
                            where user.DateDelete == null
                            select new TableUser
                            {
                                Id = user.Id,
                                Name = user.Name,
                                Email = user.Email,
                                Status = user.Status,
                                TypeId = user.TypeId,
                                Type = user.Type,
                                System = user.System,
                            }).ToArray();
                return User;
            }
        }

        [HttpGet("~/GetUserId{Id}")]
        public IEnumerable<TableUser> GetUserId(long Id)
        {
            using (var context = new MyDbContext())
            {
                var users = (from user in context.User
                             where user.Id == Id
                             select user).ToList();
                return users;
            }
        }

        [HttpGet("/GetTypeSystem")]
        public IEnumerable<TableType> GetTypeSystem()
        {
            using (var context = new MyDbContext())
            {
                var Types = (from type in context.Type
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

        [HttpPut]
        [Route("~/DeleteUser/{Id}")]
        public IActionResult DeleteUser(long Id)
        {
            using (var context = new MyDbContext())
            {
                var User = (from user in context.User
                            where user.Id == Id
                            select user).FirstOrDefault();

                if (User != null)
                {
                    User.DateDelete = DateTime.UtcNow;
                }
                context.SaveChanges();
                return Ok();
            }
        }

    }
}
