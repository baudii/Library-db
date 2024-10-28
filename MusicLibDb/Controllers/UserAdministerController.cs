using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Library_MVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library_MVC.Controllers
{
	[Authorize(Roles = "Admin")] // Доступ только для администраторов
	public class UserAdministerController : Controller
	{
		private readonly UserManager<UserModel> _userManager;

		public UserAdministerController(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
		}

		// GET: UserAdminister
		public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber = 1, int pageSize = 10)
		{
			ViewData["CurrentSort"] = sortOrder;
			ViewData["UserNameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "UserName_desc" : "";
			ViewData["EmailSortParm"] = sortOrder == "Email" ? "Email_desc" : "Email";
			ViewData["RoleSortParm"] = sortOrder == "Role" ? "Role_desc" : "Role";
			ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
			ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "LastName_desc" : "LastName";

			ViewData["CurrentFilter"] = searchString;

			// Получаем всех пользователей
			var usersQuery = _userManager.Users;

			// Поиск
			if (!string.IsNullOrEmpty(searchString))
			{
				usersQuery = usersQuery.Where(u => u.UserName!.Contains(searchString)
												|| u.Email!.Contains(searchString)
												|| u.FirstName!.Contains(searchString)
												|| u.LastName!.Contains(searchString)
												|| u.PhoneNumber!.Contains(searchString));
			}



			var usersList = await _userManager.Users.ToListAsync();
			var userRoles = new Dictionary<string, IList<string>>();

			foreach (var user in usersList)
			{
				var roles = await _userManager.GetRolesAsync(user);
				userRoles[user.Id] = roles;
			}

			// Сортировка
			switch (sortOrder)
			{
				case "UserName_desc":
					usersList = usersList.OrderByDescending(u => u.UserName).ToList();
					break;
				case "Email":
					usersList = usersList.OrderBy(u => u.Email).ToList();
					break;
				case "Email_desc":
					usersList = usersList.OrderByDescending(u => u.Email).ToList();
					break;
				case "Role":
					usersList = usersList.OrderBy(u => userRoles[u.Id].FirstOrDefault()).ToList();
					break;
				case "Role_desc":
					usersList = usersList.OrderByDescending(u => userRoles[u.Id].FirstOrDefault()).ToList();
					break;
				case "FirstName":
					usersList = usersList.OrderBy(u => u.FirstName).ToList();
					break;
				case "FirstName_desc":
					usersList = usersList.OrderByDescending(u => u.FirstName).ToList();
					break;
				case "LastName":
					usersList = usersList.OrderBy(u => u.LastName).ToList();
					break;
				case "LastName_desc":
					usersList = usersList.OrderByDescending(u => u.LastName).ToList();
					break;
				default:
					usersList = usersList.OrderBy(u => u.UserName).ToList();
					break;
			}

			// Пагинация
			int totalUsers = usersList.Count();
			var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

			var usersOnPage = usersList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

			ViewBag.UserRoles = userRoles;
			ViewBag.CurrentPage = pageNumber;
			ViewBag.TotalPages = totalPages;

			return View(usersOnPage);
		}


		// GET: UserAdminister/Details/5
		public async Task<IActionResult> Details(string id)
		{
			if (id == null)
				return NotFound();

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
				return NotFound();

			return View(user);
		}

		// GET: UserAdminister/CreateUser
		public IActionResult CreateUser()
		{
			return View();
		}

		// POST: UserAdminister/CreateUser
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateUser(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new UserModel
				{
					UserName = model.UserName,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
					FirstName = model.FirstName,
					LastName = model.LastName
				};
				var result = await _userManager.CreateAsync(user, model.Password!);
				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user, AccountController.MemberRole);
					return RedirectToAction(nameof(Index));
				}
				AddErrors(result);
			}
			return View(model);
		}

		// GET: UserAdminister/CreateModerator
		public IActionResult CreateModerator()
		{
			return View();
		}

		// POST: UserAdminister/CreateModerator
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateModerator(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new UserModel
				{
					UserName = model.UserName,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
					FirstName = model.FirstName,
					LastName = model.LastName
				};
				var result = await _userManager.CreateAsync(user, model.Password!);
				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user, AccountController.ModeratorRole);
					return RedirectToAction(nameof(Index));
				}
				AddErrors(result);
			}
			return View(model);
		}

		// Остальные действия (Edit, Delete) аналогичны предыдущим, но с учетом новых полей

		// GET: UserAdminister/Edit/5
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
				return NotFound();

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
				return NotFound();

			var role = await _userManager.GetRolesAsync(user);

			var model = new EditUserViewModel
			{
				Id = user.Id,
				UserName = user.UserName!,
				Email = user.Email!,
				PhoneNumber = user.PhoneNumber!,
				FirstName = user.FirstName!,
				LastName = user.LastName!,
				NewRole = role.FirstOrDefault()
			};

			return View(model);
		}

		// POST: UserAdminister/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, EditUserViewModel model)
		{
			if (id != model.Id)
				return NotFound();


			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(id);
				if (user == null)
					return NotFound();

				user.UserName = model.UserName;
				user.Email = model.Email;
				user.PhoneNumber = model.PhoneNumber;
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;

				if (string.IsNullOrEmpty(model.NewRole))
				{
					model.NewRole = "Member";
				}
				var roles = await _userManager.GetRolesAsync(user);
				await _userManager.RemoveFromRolesAsync(user, roles);
				try
				{
					var roleAddResult = await _userManager.AddToRoleAsync(user, model.NewRole!);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Не получилось добавить роль. Ошибка: " + ex.Message);
				}

				var updateUserReult = await _userManager.UpdateAsync(user);
				if (updateUserReult.Succeeded)
				{
					if (!string.IsNullOrEmpty(model.NewPassword))
					{
						var token = await _userManager.GeneratePasswordResetTokenAsync(user);
						var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
						if (!passwordResult.Succeeded)
						{
							AddErrors(passwordResult);
						}
					}
					if (ModelState.ErrorCount == 0)
						return RedirectToAction(nameof(Index));
				}
				AddErrors(updateUserReult);
			}

			return View(model);
		}

		// GET: UserAdminister/Delete/5
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null)
				return NotFound();

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
				return NotFound();

			return View(user);
		}

		// POST: UserAdminister/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			if (id == null)
				return NotFound();

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
				return NotFound();

			var result = await _userManager.DeleteAsync(user);
			if (!result.Succeeded)
			{
				AddErrors(result);
				return View(user);
			}

			return RedirectToAction(nameof(Index));
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
		}
	}
}
