namespace BookManagementAPI.BLL.Models.Dtos;

public class CreateBookRequest
{
    public string Title { get; set; }
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; }
    public int Views { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
}