using Login;
using Login.Models;
using Login.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Login.Controllers
{

	public class LoginController : Controller
	{
		[HttpPost]
		[Route("~/Autenticacao")]
		public async Task<IActionResult> Autenticacao([FromBody] AuxSenha request)
		{
			using (var context = new MyDbContext())
			{
				var tg = (from t in context.User
						  where t.Email == request.Email
						  where t.Key == request.Key
						  where t.DateDelete == null
                          where t.SystemId == 3
						  select new TableUser
						  {
							  Id = t.Id,
							  Name = t.Name,
							  Type = t.Type,
							  Email = t.Email,
                          }).ToArray().FirstOrDefault();

				var claims = new List<Claim>();

				if (tg != null)
				{					
					claims.Add(new Claim(ClaimTypes.Name, tg.Name));
					claims.Add(new Claim(ClaimTypes.Sid, Convert.ToString(tg.Id)));
					claims.Add(new Claim(ClaimTypes.Email, tg.Email));
					claims.Add(new Claim(ClaimTypes.System, Convert.ToString(tg.TypeId)));
                    var id = new ClaimsIdentity(claims, "Basic");

                    try
                    {
						await HttpContext.SignInAsync(
						CookieAuthenticationDefaults.AuthenticationScheme,
						new ClaimsPrincipal(id),
						new AuthenticationProperties
						{
							IsPersistent = true,
							ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
						});
					}
                    catch (Exception ex)
                    {
						BadRequest(ex);
                        throw;
                    }
				}
				return Ok(tg);
			}
		}

		[HttpPost]
		[Route("~/Logout")]
		public async Task<IActionResult> Logout()
		{
			try
			{
				await HttpContext.SignOutAsync();
			}
			catch (Exception ex)
			{
				BadRequest(ex);
				throw;
			}
			return Ok();
		}
    

        [HttpPut]
        [Route("~/CadastroSenha")]
        public IActionResult CadastroSenha([FromBody] TableUser request)
        {
            using (var context = new MyDbContext())
            {
                var usuario = (from t in context.User
                               where t.Hash.Equals(request.Hash)
                               select t).ToArray().FirstOrDefault();
                if (usuario != null)
                {
                    usuario.Key = request.Key;
                    usuario.Status = true;
                    context.SaveChanges();
                }
            };
            return Ok();
        }

		[HttpPost]
		[Route("~/EsqueceuSenha")]
		public IActionResult EsqueceuSenha([FromBody] TableUser request)
		{
            using (var context = new MyDbContext())
            {
                var usuario = (from t in context.User
                               where t.Email.Equals(request.Email)
                               select t).ToArray().FirstOrDefault();                
          
				if(usuario != null)
				{
                    var link = HttpContext.Request.Host.Host + ':' + HttpContext.Request.Host.Port + "/Home/RecuperarSenha?pass=" + usuario.Hash;

                    EmailTask.SendFormatedMail("Novo usuário solicitado - Você tem um convite para utilizar os sistemas GP Audax",
                    request.Name, request.Email,
                    "Foi solicitado o cadastro de uma nova senha de acesso. Para continuar com a solicitação, Use o link a seguir:" +
                    "<br/>" +
                    "Copie o link abaixo e cole no seu navegador:" +
                    "<br/>" + link
                    );
				}
				else
				{
					return BadRequest();
				}      
				return Ok();
            };
        }

        [HttpPost]
        [Route("~/AuthExterna")]
        public async Task <IActionResult> AuthExterna([FromBody] AuxSenha request)
        {
            using (var context = new MyDbContext())
            {
                var user = (from t in context.User
                          where t.Email == request.Email
                          where t.Key == request.Key
                          where t.DateDelete == null
                          select new TableUser
                          {
                              Id = t.Id,
                              Name = t.Name,
                              Type = t.Type,
                              System = t.System,
                              Email = t.Email,
                              Status = t.Status,
                              Hash = t.Hash,
                              Key = t.Key,
                              SubCategory = t.SubCategory,
                              DateDelete = t.DateDelete,
                              AddInfo = t.AddInfo,

                          }).ToArray().FirstOrDefault();

                if (user == null)
                {
                    return Unauthorized();
                }
                else
                {
                    string UserName = user.Name.ToString();
                    string token = AuthTools.GenerateToken(UserName);

                    var objeto = new 
                    {
                        Token = token,
                        User = user,
                    };
                   
                    return Ok(objeto);
                }                
            }
        }
    }
}

