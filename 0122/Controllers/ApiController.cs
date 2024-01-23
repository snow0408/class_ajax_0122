using _0122.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _0122.Controllers
{
	public class ApiController : Controller
	{
		private readonly MyDbContext _dbContext;
		public ApiController(MyDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult First() 
		{
			return View();
		}

		public IActionResult Cities()
		{
			var cities = _dbContext.Addresses.Select(a => a.City).Distinct();
			return Json(cities);
		}

		public IActionResult Content()
		{
			//return Content("Hello Content");
			//return Content("<h2>Hello Content</h2>", "text/html");
			return Content("<h2>Hello Content, 早安安安安安</h2>", "text/plain", System.Text.Encoding.UTF8);

			// text/plain 純文字
		}

		public IActionResult Avatar(int id = 1)
		{
			Member? member = _dbContext.Members.Find(id);

			if (member != null)
			{
				byte[] img = member.FileData;
				return File(img, "image/jpeg");
			}

			return NotFound();
		}
	}
}
