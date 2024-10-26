using Library_MVC.Data;
using Library_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using Library_MVC.Data.Static;
using System.Linq;

namespace Library_MVC.Controllers
{
	public class LibraryController : Controller
	{
		private readonly LibDBContext _context;

		public LibraryController(LibDBContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string sortOrder, string? searchString = null, int pageNumber = 1, int pageSize = 10)
		{
			ViewBag.CurrentSort = sortOrder;

			var books = from b in _context.Books.Include(b => b.Author) select b;

			ViewData["CurrentFilter"] = searchString;

			// Поиск (сначала фильтруем по значению строки поиска)
			if (!string.IsNullOrEmpty(searchString))
			{
				books = books.Where(u => u.Id!.ToString().Contains(searchString)
												|| u.Title.Contains(searchString)
												|| u.Author!.FullName.Contains(searchString)
												|| u.PublishedYear.ToString().Contains(searchString));
			}

			// Применение сортировки (на те элементы, которые остались)
			if (!string.IsNullOrEmpty(sortOrder))
			{
				var sortParams = sortOrder.Split('_');
				string sortProperty = sortParams[0];
				string sortDirection = sortParams[1];

				var param = Expression.Parameter(typeof(BookModel), "b");
				var property = Expression.Property(param, sortProperty);
				var sortExpression = Expression.Lambda(property, param);

				string methodName = sortDirection == "asc" ? "OrderBy" : "OrderByDescending";
				var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(BookModel), property.Type },
					books.Expression, Expression.Quote(sortExpression));

				books = books.Provider.CreateQuery<BookModel>(resultExpression);
			}
			else
			{
				books = books.OrderBy(b => b.Id);
			}

			// Пагинация
			var totalItems = await books.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

			var booksOnPage = await books
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			ViewBag.CurrentPage = pageNumber;
			ViewBag.TotalPages = totalPages;

			return View(booksOnPage);
		}

		[Authorize(Policy = Policies.CreateBook)]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Policies.CreateBook)]
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

		[Authorize(Policy = Policies.EditBook)]
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
		[Authorize(Policy = Policies.EditBook)]
		public async Task<IActionResult> Edit(int id, BookModel book)
		{
			if (id != book.Id)
				return NotFound();

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
						return NotFound();
					throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(book);
		}

		[Authorize(Policy = Policies.DeleteBook)]
		public async Task<IActionResult> Delete(int id)
		{
			var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
			if (book == null)
				return NotFound();

			return View(book);
		}

		[Authorize(Policy = Policies.DeleteBook)]
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
