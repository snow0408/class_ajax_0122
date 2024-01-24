using _0122.Models.DTO;
using _0122.Models.EFModels;
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
			_host = hostingEnvironment;
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
			string fileName = "empty.jpg";
			if (Avatar != null)
			{
				fileName = Avatar.FileName;
			}
			//取得檔案上傳的實際路徑
			string uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);
			//檔案上傳
			using (var fileStream = new FileStream(uploadPath, FileMode.Create))
			{
				Avatar?.CopyTo(fileStream);
			}

			//轉成二進制
			byte[]? imgByte = null;
			using (var memoryStream = new MemoryStream())
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

		[HttpPost]
		public IActionResult spots([FromBody] SearchDTO _search)
		{
			//根據分類取景點資料
			var spots = _search.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == _search.categoryId);

			//根據關鍵字搜尋
			if (!string.IsNullOrEmpty(_search.keyword))
			{
				spots = spots.Where(s => s.SpotTitle.Contains(_search.keyword) || s.SpotDescription.Contains(_search.keyword));
			}

			//排序
			switch (_search.sortBy)
			{
				case "spotTitle":
					spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
					break;
				case "categoryId":
					spots=_search.sortType=="asc"?spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s=> s.CategoryId);
					break;
				default:
					spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
					break;
			}

			//分頁
			int TotalCount = spots.Count(); //總共有多少筆資料
			int pageSize = _search.pageSize ?? 9; //設定每頁顯示多少筆資料 ??預設9
			int page = _search.page ?? 1; //目前要顯示第幾頁 ??預設1
			int TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize); //計算總共有幾頁

			spots = spots.Skip((int)((page - 1) * pageSize)).Take(pageSize);
			//跳過((頁數-1)*單頁資料數)取(單頁資料數)

			return Json(spots);
		}
	}
}
