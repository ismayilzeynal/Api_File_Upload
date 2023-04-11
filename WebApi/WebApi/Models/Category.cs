namespace WebApi.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public bool IsDelete { get; set; }
    }
}
