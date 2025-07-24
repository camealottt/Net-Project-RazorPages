using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using Razor_Tutorial_Test.Model;
using Razor_Tutorial_Test.Data;

public class AddModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public AddModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Items Item { get; set; }

    [BindProperty]
    public List<ItemImages> ItemImages { get; set; }

    public List<Category> Category { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Category = await _db.Category.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            Category = await _db.Category.ToListAsync();
            return Page();
        }

        // Set owner from session
        Item.Owner = HttpContext.Session.GetInt32("UserID") ?? 0;
        Item.CreatedAt = DateTime.Now;
        _db.Items.Add(Item);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Item Added Successfully!";

        return RedirectToPage("Index");
    }
}
