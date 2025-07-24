using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Linq;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public OrderRecord OrderRecord { get; set; }

    public Items BuyerItem { get; set; }
    public Items SellerItem { get; set; }

    public List<ItemImages> BuyerItemImages { get; set; } = new List<ItemImages>();
    public List<ItemImages> SellerItemImages { get; set; } = new List<ItemImages>();

    public IActionResult OnGet(int id)
    {
        OrderRecord = _db.OrderRecord.FirstOrDefault(o => o.Id == id);
        if (OrderRecord == null)
        {
            return NotFound();
        }

        if (OrderRecord.OfferedItemID.HasValue)
        {
            BuyerItem = _db.Items.FirstOrDefault(i => i.Id == OrderRecord.OfferedItemID.Value);
            BuyerItemImages = _db.ItemImages.Where(img => img.ItemId == OrderRecord.OfferedItemID.Value).ToList();
        }

        if (OrderRecord.RequestedItemID != 0)
        {
            SellerItem = _db.Items.FirstOrDefault(i => i.Id == OrderRecord.RequestedItemID);
            SellerItemImages = _db.ItemImages.Where(img => img.ItemId == OrderRecord.RequestedItemID).ToList();
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        var order = _db.OrderRecord.FirstOrDefault(o => o.Id == OrderRecord.Id);
        if (order == null)
        {
            return NotFound();
        }

        // Only update the OrderStatus
        order.OrderStatus = OrderRecord.OrderStatus;

        _db.SaveChanges();

        return RedirectToPage("Index"); // Or return to another page as needed
    }
}
