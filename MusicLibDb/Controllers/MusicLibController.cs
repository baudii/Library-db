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
	public class MusicLibController : Controller
	{
		private readonly MusicLibDBContext _context;

		public MusicLibController(MusicLibDBContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string sortOrder, string? searchString = null, int pageNumber = 1, int pageSize = 10)
		{
			ViewBag.CurrentSort = sortOrder;

			var songs = _context.Songs.AsQueryable();

			ViewData["CurrentFilter"] = searchString;

			// Поиск (сначала фильтруем по значению строки поиска)
			if (!string.IsNullOrEmpty(searchString))
			{
				songs = songs.Where(u => u.Id!.ToString().Contains(searchString)
												|| u.Title.Contains(searchString)
												|| u.Author!.Contains(searchString)
												|| u.PublishedYear.ToString().Contains(searchString));
			}

			// Применение сортировки (на те элементы, которые остались)
			if (!string.IsNullOrEmpty(sortOrder))
			{
				var sortParams = sortOrder.Split('_');
				string sortProperty = sortParams[0];
				string sortDirection = sortParams[1];

				var param = Expression.Parameter(typeof(SongModel), "b");
				var property = Expression.Property(param, sortProperty);
				var sortExpression = Expression.Lambda(property, param);

				string methodName = sortDirection == "asc" ? "OrderBy" : "OrderByDescending";
				var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(SongModel), property.Type },
					songs.Expression, Expression.Quote(sortExpression));

				songs = songs.Provider.CreateQuery<SongModel>(resultExpression);
			}
			else
			{
				songs = songs.OrderBy(b => b.Id);
			}

			// Пагинация
			var totalItems = await songs.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

			var songsOnPage = await songs
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			ViewBag.CurrentPage = pageNumber;
			ViewBag.TotalPages = totalPages;

			return View(songsOnPage);
		}

		[Authorize(Policy = Policies.CreateBook)]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Policies.CreateBook)]
		public async Task<IActionResult> Create(SongModel song)
		{
			if (ModelState.IsValid)
			{
				_context.Songs.Add(song);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(song);
		}

		[Authorize(Policy = Policies.EditBook)]
		public async Task<IActionResult> Edit(int id)
		{
			var song = await _context.Songs.FindAsync(id);
			if (song == null)
			{
				return NotFound();
			}
			return View(song);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Policies.EditBook)]
		public async Task<IActionResult> Edit(int id, SongModel song)
		{
			if (id != song.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(song);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					bool songExist = _context.Songs.Any(e => e.Id == id);
					if (!songExist)
						return NotFound();
					throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(song);
		}

		[Authorize(Policy = Policies.DeleteBook)]
		public async Task<IActionResult> Delete(int id)
		{
			var song = await _context.Songs.FirstOrDefaultAsync(m => m.Id == id);
			if (song == null)
				return NotFound();

			return View(song);
		}

		[Authorize(Policy = Policies.DeleteBook)]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var song = await _context.Songs.FindAsync(id);
			if (song != null)
			{
				_context.Songs.Remove(song);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Listen(int id)
		{
			var song = await _context.Songs.FindAsync(id);
			return View(song);
		}
	}
}
