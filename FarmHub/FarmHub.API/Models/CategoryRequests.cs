namespace FarmHub.API.Models
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }


        public CategoryCreateRequest(string name)
        {
            Name = name;
        }

        public CategoryCreateRequest()
        {

        }
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }
    }



}
