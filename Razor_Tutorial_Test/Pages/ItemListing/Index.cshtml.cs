using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.ItemListing
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedCategoryId { get; set; }

        public List<Category> Categories { get; set; } = new();
        public List<ItemDisplayModel> ItemList { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _context.Category.ToListAsync();
            var currentUserId = HttpContext.Session.GetInt32("UserID");

            var query = from item in _context.Items
                        join user in _context.User on item.Owner equals user.Id
                        where item.Status != "Unavailable"
                          && item.Status != "Traded"
                          && (currentUserId == null || item.Owner != currentUserId)
                        select new ItemDisplayModel
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            Price = item.Price,
                            CategoryId = item.Category,
                            FirstImageUrl = _context.ItemImages
                                .Where(img => img.ItemId == item.Id)
                                .Select(img => img.ImageUrl)
                                .FirstOrDefault(),
                            LikeCount = _context.LikeRecord.Count(l => l.Item == item.Id),
                            UserLiked = currentUserId != null && _context.LikeRecord.Any(l => l.Item == item.Id && l.User == currentUserId),
                            OwnerUsername = user.Username,
                            OwnerProfilePicture = user.ProfilePicture,
                            OwnerId = item.Owner,
                            Comments = _context.CommentRecord
                                .Where(c => c.Item == item.Id)
                                .Join(_context.User, c => c.User, u => u.Id, (c, u) => new CommentDisplayModel
                                {
                                    Username = u.Username,
                                    ProfilePicture = u.ProfilePicture,
                                    CommentText = c.Content
                                }).ToList(),
                            ImageUrls = _context.ItemImages
                                .Where(img => img.ItemId == item.Id)
                                .Select(img => img.ImageUrl)
                                .ToList()
                        };

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(i => i.Name.Contains(Search));
            }

            if (SelectedCategoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == SelectedCategoryId.Value);
            }

            ItemList = await query.OrderByDescending(i => i.LikeCount).ToListAsync();
        }


        public async Task<IActionResult> OnPostLikeAsync(int itemId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) 
            {
                TempData["ErrorMessage"] = "You Need to Login First!";
                return RedirectToPage();
            }

            var alreadyLiked = await _context.LikeRecord.AnyAsync(l => l.Item == itemId && l.User == userId);
            if (alreadyLiked)
            {
                TempData["ErrorMessage"] = "You already liked this item!";
                return RedirectToPage();
            }

            _context.LikeRecord.Add(new LikeRecord { Item = itemId, User = userId.Value });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "You liked the item!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleLikeAsync(int itemId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You need to login first!";
                return RedirectToPage();
            }

            var existingLike = await _context.LikeRecord
                .FirstOrDefaultAsync(l => l.Item == itemId && l.User == userId);

            if (existingLike != null)
            {
                // Unlike
                _context.LikeRecord.Remove(existingLike);
                TempData["SuccessMessage"] = "You unliked the item!";
            }
            else
            {
                // Like
                _context.LikeRecord.Add(new LikeRecord { Item = itemId, User = userId.Value });
                TempData["SuccessMessage"] = "You liked the item!";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int itemId, string CommentText)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You Need to Login First!";
                return RedirectToPage();
            }

            if (string.IsNullOrWhiteSpace(CommentText))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty!";
                return RedirectToPage();
            }

            _context.CommentRecord.Add(new CommentRecord
            {
                Item = itemId,
                User = userId.Value,
                Content = CommentText
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment added!";
            return RedirectToPage();
        }

        public class ItemDisplayModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string? Description { get; set; }
            public string? FirstImageUrl { get; set; }
            public int LikeCount { get; set; }
            public bool UserLiked { get; set; }
            public string OwnerUsername { get; set; }
            public string? OwnerProfilePicture { get; set; }
            public int OwnerId { get; set; } 
            public int? CategoryId { get; set; }
            public List<string> ImageUrls { get; set; } = new();
            public List<CommentDisplayModel> Comments { get; set; } = new();
        }



        public class CommentDisplayModel
        {
            public string Username { get; set; }
            public string? ProfilePicture { get; set; }
            public string CommentText { get; set; }
        }
    }
}
