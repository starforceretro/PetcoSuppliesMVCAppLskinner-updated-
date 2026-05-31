using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class AdviceArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdviceArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // public articles view 

        public async Task<IActionResult> Index(string category)
        {
            var articles = _context.AdviceArticles.AsQueryable();
             
            if (!string.IsNullOrEmpty(category)) // filter by category if provided
            {
                articles = articles.Where(a =>
                    a.AnimalCategory == category); // filter articles by category
            }

            return View(await articles
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync());
        }

        // view article details

        public async Task<IActionResult> Details(int id)
        {
            var article = await _context.AdviceArticles
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // create article

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdviceArticle article)
        {
            if (ModelState.IsValid) // if the model state is valid, save the article to the database
            {
                article.CreatedDate = DateTime.Now;

                _context.AdviceArticles.Add(article); // add the new article to the database context

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }

        // edit article

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.AdviceArticles.FindAsync(id); // find the article by id

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdviceArticle article) // handle the form submission for editing an article
        {
            if (id != article.Id) // if the id in the URL does not match the id of the article being edited, return a 404 not found
            {
                return NotFound();
            }

            if (ModelState.IsValid) // if the model state is valid, update the article in the database
            {
                _context.Update(article);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }

        // delete article

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.AdviceArticles // find the article by id to confirm deletion
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // handle the form submission for deleting an article
        {
            var article = await _context.AdviceArticles.FindAsync(id);

            if (article != null)
            {
                _context.AdviceArticles.Remove(article); // remove the article from the database context

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}