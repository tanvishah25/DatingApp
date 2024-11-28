using Microsoft.EntityFrameworkCore;

namespace DatingApp.Helpers
{
    public class PagedList<T> :List<T>
    {
        //public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        //{
        //    CurrentPage = pageNumber;
        //    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        //    TotalCount = count;
        //    PageSize = pageSize;
        //    AddRange(items);
        //}

        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,int pagenumber,int pagesize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToListAsync();
            //return new PagedList<T>(items, count, pagenumber, pagesize);
            //Alternative of above line
            var v1 = new PagedList<T>()
            {
                CurrentPage = pagenumber,
                TotalPages = (int)Math.Ceiling(count / (double)pagesize),
                TotalCount = count,
                PageSize = pagesize,
            };
            v1.AddRange(items);
            return v1;
        }
    }
}
