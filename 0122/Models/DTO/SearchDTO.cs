namespace _0122.Models.DTO
{
	public class SearchDTO
	{
		//搜尋相關
		public string? keyword { get; set; }
		public int? categoryId { get; set; } = 0; //不根據分類編號抓資料

		//排序相關
		public string? sortBy { get; set; }
		public string? sortType { get; set; } = "asc";


		//分頁相關
		public int? page { get; set; } = 1;//第一頁
		public int? pageSize { get; set; } = 9;//一頁顯示9筆
	}
}
