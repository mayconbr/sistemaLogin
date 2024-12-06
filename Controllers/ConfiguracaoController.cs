using Login;
using Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace Login.Controllers
{

	public class ConfiguracaoController : Controller
	{
        [Authorize]

        public IActionResult Configuracao()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Categories = GetCategory();
            mymodel.SubCategories = GetSubcategory();
            return View(mymodel);
        }

        [HttpPost]
        [Route("~/InsertSubCategory")]
        public IActionResult InsertSubCategory([FromBody] TableSubCategory request)
        {
            using (var context = new MyDbContext())
            {
                context.SubCategories.Add(new TableSubCategory
                {
                    Id = request.Id,
                    CategoryId = request.CategoryId,
                    Nome = request.Nome,
                });
                context.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        [Route("~/InsertCategory")]
        public IActionResult InsertCategory([FromBody] TableCategory request)
        {
            using (var context = new MyDbContext())
            {
                context.Category.Add(new TableCategory
                {
                    Nome = request.Nome,
                });
                context.SaveChanges();
            }
            return Ok();
        }

        [HttpGet]
        [Route("~/GetCategory")]
        public IEnumerable<TableCategory> GetCategory()
        {

            using (var context = new MyDbContext())
            {
                var Categorys = (from category in context.Category
                                select category).ToList();

                return Categorys;
            }
        }

        [HttpGet]
        [Route("~/GetSubcategory")]
        public IEnumerable<TableSubCategory> GetSubcategory()
        {

            using (var context = new MyDbContext())
            {
                var sub = (from t in context.SubCategories                          
                             select new TableSubCategory
                             {
                                 Id = t.Id,
                                 Category = t.Category,
                                 Nome = t.Nome,

                             }).ToList();

                return sub;
            }
        }
    }
}

