namespace BookManagementAPI.BLL.Common;

public class Pagination<T>
{
    public List<T> Items { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public Pagination(List<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public static Task<Pagination<T>> CreateAsync(IQueryable<T> Source, int pageIndex, int pageSize)
    {
        var totalCount = Source.Count();
        var items = Source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        return Task.Run(() => new Pagination<T>(items, totalCount, pageIndex, pageSize));
    }
}