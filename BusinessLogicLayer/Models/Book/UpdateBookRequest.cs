namespace BookManagementAPI.BLL.Models.Dtos;

public class UpdateBookRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; }
    public int Views { get; set; }
    public DateTime UpdateDate { get; set; } = DateTime.Now;
}