namespace DTO
{
    public class DishPagedListDto
    {
        public List<DishDto>? Dishes { get; set; }
        public PageInfoModel Paginatin { get; set; }
    }

    public class PageInfoModel 
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public int Current { get; set; } 
    }
}
