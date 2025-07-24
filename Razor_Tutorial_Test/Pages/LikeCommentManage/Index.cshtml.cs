using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Razor_Tutorial_Test.Pages.LikeCommentManage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<ItemDisplayModel> ItemList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task OnGet()
        {
            var itemsQuery = _db.Items.AsQueryable();

            // Apply search filter if SearchString is not empty
            if (!string.IsNullOrEmpty(SearchString))
            {
                itemsQuery = itemsQuery.Where(i => i.Name.Contains(SearchString) ||
                                                  i.Description.Contains(SearchString) ||
                                                  i.Status.Contains(SearchString));
            }

            ItemList = await itemsQuery
                .Select(i => new ItemDisplayModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description ?? "Unknown Description",
                    Price = i.Price,
                    Status = i.Status ?? "Unknown Status",
                    CreatedAt = i.CreatedAt,
                    Category = _db.Category.Where(c => c.Id == i.Category).Select(c => c.Name).FirstOrDefault() ?? "Unknown Category",
                    Owner = _db.User.Where(u => u.Id == i.Owner).Select(u => u.Username).FirstOrDefault() ?? "Unknown User"
                })
                .ToListAsync();
        }

        public class ItemDisplayModel
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public required string Description { get; set; }
            public decimal Price { get; set; }
            public required string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public required string Category { get; set; }
            public required string Owner { get; set; }
        }
    }
}