using _0122.Models;
using _0122.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace _0122.Controllers
{
	public class ApiController : Controller
	{
		private readonly MyDbContext _dbContext;
		private readonly IWebHostEnvironment _host;

		public ApiController(MyDbContext dbContext, IWebHostEnvironment hostingEnvironment)
		{
			_dbContext = dbContext;
			_host= hostingEnvironment;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult First() 
		{
			return View();
		}

		public IActionResult Register(Member member, IFormFile Avatar)
		{
			//if (string.IsNullOrEmpty(_user.Name)) 
			//{
			//	_user.Name = "Guest";
			//}
			//return Content($"Hello {_user.Name}, you are {_user.Age} years old. 您電子郵件是 {_user.Email}");

			//return Content($"{_user.Avatar?.FileName}-{_user.Avatar?.Length}-{_user.Avatar?.ContentType}");
			string fileName="empty.jpg";
			if (Avatar != null) 
			{ 
				fileName=Avatar.FileName;
			}
			//取得檔案上傳的實際路徑
			string uploadPath=Path.Combine(_host.WebRootPath, "uploads", fileName);
			//檔案上傳
			using (var fileStream = new FileStream(uploadPath, FileMode.Create))
			{
				Avatar?.CopyTo(fileStream);
			}

			//轉成二進制
			byte[]? imgByte =null;
			using (var memoryStream=new MemoryStream()) 
			{
				Avatar?.CopyTo(memoryStream);
				imgByte = memoryStream.ToArray();
			}

			member.FileName = fileName;
			member.FileData = imgByte;

			//新增
			_dbContext.Members.Add(member);
			_dbContext.SaveChanges();

			return Content("新增成功");
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
