using Microsoft.AspNetCore.Mvc;
using FileProcessor.Models;
using FileProcessor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FileProcessor.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        //get the context for items so you can perfrom CRUD
        private readonly FileProcessorContext Context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemsController(FileProcessorContext context, IWebHostEnvironment environment, UserManager<IdentityUser> userManager)
        {
            Context = context;
            _userManager = userManager;

        }


        public async Task<IActionResult> Overview(string searchTerm, string fileType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // Start building query
            var query = Context.Items
                .Include(f => f.User) // Always include User data to prevent null issues
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(f => f.UserId == user.Id); // Regular users only see their files
            }

            // Apply Search & Filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.Name.Contains(searchTerm) || f.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(fileType))
            {
                query = query.Where(f => f.ContentType.Contains(fileType));
            }

          

            var items = await query.ToListAsync();
            return View(items);
        }



        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost, ActionName("Create")]
        public async Task<ActionResult> Create(Item item, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }

            var user = await _userManager.GetUserAsync(User); // Get logged-in user

            //read the file using stream without saving it on disk
            //but saving it in memory.
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                item.File = file.FileName;
                item.ContentType = file.ContentType;
                //store the entire file content as binary data in the database.
                item.FileData = memoryStream.ToArray();
                item.UserId = user.Id;
                item.UploadedAt = DateTime.UtcNow;

            }

            Context.Items.Add(item);
            await Context.SaveChangesAsync();

            return RedirectToAction("Overview");

        }

        [ActionName("Download")]
        public IActionResult Download(int id)
        {
            var item = Context.Items.Find(id);
            if (item == null || item.FileData == null)
            {
                return NotFound();
            }

            // Check if the file is a PDF or an image
            if (item.ContentType == "application/pdf" || item.ContentType.StartsWith("image/"))
            {
                // Set Content-Disposition to 'inline' for PDF or image
                Response.Headers.Add("Content-Disposition", $"inline; filename={item.File}");
            }
            else
            {
                // Default behavior: Download the file instead of opening it in a new tab
                Response.Headers.Add("Content-Disposition", $"attachment; filename={item.File}");
            }

            return File(item.FileData, item.ContentType);
        }
             


        
        public async Task<ActionResult> Edit(int id)
        {
            ///allows the app to continue doing other things while waiting for the database query.
            var item = await Context.Items.FindAsync(id);
            return View(item);
        }

        
        [HttpPost]
        public async Task<ActionResult> Edit(Item item, IFormFile file)
        {
            //find the item from the database and replace it with the new ITEM up here.
            var existingItem = await Context.Items.FindAsync(item.Id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Update text fields
            existingItem.Name = item.Name;
            existingItem.Description = item.Description;

            // If a new file is uploaded, replace the existing one
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    //save the file into memory
                    await file.CopyToAsync(memoryStream);

                    existingItem.File = file.FileName;
                    existingItem.ContentType = file.ContentType;
                    existingItem.FileData = memoryStream.ToArray();
                }
            }

            await Context.SaveChangesAsync();
            return RedirectToAction("Overview");
        }

        
        public async Task<ActionResult> Delete(int id)
        {
            var item = await Context.Items.FindAsync(id);
            return View(item);
        }

        
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> ConfiremedDelete(int id)
        {
            var item = await Context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            Context.Items.Remove(item);
            await Context.SaveChangesAsync();

            return RedirectToAction("Overview");
        }
    }
}
