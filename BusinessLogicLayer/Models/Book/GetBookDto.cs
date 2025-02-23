namespace BookManagementAPI.BLL.Models.Dtos;

public class GetBookDto
{
    public string Title { get; set; }
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; }
    public int Views { get; set; }
    public float PopularityScore { get; set; }
}