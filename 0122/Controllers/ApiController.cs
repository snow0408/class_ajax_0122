using _0122.Models;
using _0122.Models.DTO;
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

		public IActionResult Register(UsersDTO _user)
		{
			if (string.IsNullOrEmpty(_user.Name)) 
			{
				_user.Name = "Guest";
			}
			//return Content($"Hello {_user.Name}, you are {_user.Age} years old. 您電子郵件是 {_user.Email}");
			return Content($"{_user.Avatar?.FileName}-{_user.Avatar?.Length}-{_user.Avatar?.ContentType}");

		}

		public IActionResult Cities()
		{
			var cities = _dbContext.Addresses.Select(a => a.City).Distinct();
			return Json(cities);
		}

        public IActionResult Districts(string city)
        {
            var districts = _dbContext.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }

        public IActionResult Content()
		{
			System.Threading.Thread.Sleep(2000); //停2000毫秒
			//return Content("Hello Content");
			//return Content("<h2>Hello Content</h2>", "text/html");
			return Content("<h2>Hello Content, 早安安安安安</h2>", "text/plain", System.Text.Encoding.UTF8);

			// text/plain 純文字
		}

		public IActionResult Address() 
		{
			return View();
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
