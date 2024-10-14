using Library_MVC.Data;
using Library_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Library_MVC.Controllers
{
	public class LibraryController : Controller
	{
		private readonly LibDBContext _context;

		public LibraryController(LibDBContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var books = await _context.Books.ToListAsync();
			return View(books);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BookModel book)
		{
			if (ModelState.IsValid)
			{
				_context.Books.Add(book);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(book);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id)
		{
			var book = await _context.Books.FindAsync(id);
			if (book == null)
			{
				return NotFound();
			}
			return View(book);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id, BookModel book)
		{
			if (id != book.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(book);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!BookExists(book.Id))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(book);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
			if (book == null)
			{
				return NotFound();
			}
			return View(book);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var book = await _context.Books.FindAsync(id);
			if (book != null)
			{
				_context.Books.Remove(book);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}

		private bool BookExists(int id)
		{
			return _context.Books.Any(e => e.Id == id);
		}
	}
}
