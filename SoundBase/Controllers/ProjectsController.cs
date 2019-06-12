using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoundBase.Data;
using SoundBase.Models;

namespace SoundBase.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            var projects = _context.Project.Include(p => p.ProjectUsers).ToList();
            var projectsToShow = new List<Project>();
            foreach (Project project in projects)
            {
                project.ProjectUsers = project.ProjectUsers.Where(pu => pu.UserId == user.Id).ToList();
                if (project.ProjectUsers.Count > 0)
                {
                    projectsToShow.Add(project);
                }
            }

            ViewBag.Name = user.FirstName;
            return View(projectsToShow);
        }

        // GET: Projects/Chat/5
        public async Task<IActionResult> Chat(int id)
        {

            var user = await GetCurrentUserAsync();

            ViewBag.Page = "Chat";

            ProjectChatViewModel viewproject = new ProjectChatViewModel();

            viewproject.ProjectId = id;

            viewproject.Project = await _context.Project
                .Include(p => p.Creator)
                .Include(p => p.ChatMessages)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            viewproject.Project.ChatMessages = viewproject.Project.ChatMessages.OrderBy(cm => cm.DatePosted).ToList();

            if (viewproject.Project == null)
            {
                return NotFound();
            }

            ViewBag.ArtistName = viewproject.Project.ArtistName;
            ViewBag.ProjectTitle = viewproject.Project.Title;
            ViewBag.ImagePath = viewproject.Project.ImagePath;
            ViewBag.UserId = user.Id;

            return View(viewproject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chat(ProjectChatViewModel viewproject)
        {
            var user = await GetCurrentUserAsync();

            ChatMessage chatMessage = new ChatMessage();
            chatMessage.UserId = user.Id;
            chatMessage.Content = viewproject.ChatMessage;
            chatMessage.ProjectId = viewproject.ProjectId;
            chatMessage.DatePosted = DateTime.Now;

            _context.Add(chatMessage);

            await _context.SaveChangesAsync();

            viewproject.Project = await _context.Project
                .Include(p => p.Creator)
                .Include(p => p.ChatMessages)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(m => m.ProjectId == viewproject.ProjectId);

            viewproject.Project.ChatMessages = viewproject.Project.ChatMessages.OrderBy(cm => cm.DatePosted).ToList();

            ViewBag.ArtistName = viewproject.Project.ArtistName;
            ViewBag.ProjectTitle = viewproject.Project.Title;
            ViewBag.ImagePath = viewproject.Project.ImagePath;
            ViewBag.UserId = user.Id;

            return View(viewproject);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ProjectCreateViewModel viewproject = new ProjectCreateViewModel();
            viewproject.Project = new Project();
            ViewData["MemberRoleId"] = new SelectList(_context.MemberRole, "MemberRoleId", "Title");
            ViewData["CreatorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View(viewproject);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel viewproject)
        {
            viewproject.Project.DateCreated = DateTime.Now;
            ModelState.Remove("Project.CreatorId");

            var user = await GetCurrentUserAsync();
            viewproject.Project.CreatorId = user.Id;

            if (viewproject.MemberRoleId == 0)
            {
                ViewBag.Message = string.Format("Please select your role");

                ProjectCreateViewModel newviewproject = new ProjectCreateViewModel();
                viewproject.Project = new Project();
                ViewData["MemberRoleId"] = new SelectList(_context.MemberRole, "MemberRoleId", "Title");
                ViewData["CreatorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
                return View(newviewproject);
            }

            if (ModelState.IsValid)
            {
                if (viewproject.ImageFile != null)
                {
                    var fileName = Path.GetFileName(viewproject.ImageFile.FileName);
                    Path.GetTempFileName();
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewproject.ImageFile.CopyToAsync(stream);
                    }

                    viewproject.Project.ImagePath = viewproject.ImageFile.FileName;
                }

                _context.Add(viewproject.Project);

                var initialUser = new ProjectUser();
                initialUser.UserId = user.Id;
                initialUser.ProjectId = viewproject.Project.ProjectId;
                initialUser.MemberRoleId = viewproject.MemberRoleId;
                initialUser.IsAdmin = true;
                initialUser.DateAdded = DateTime.Now;

                _context.Add(initialUser);

                await _context.SaveChangesAsync();
                return RedirectToAction("Chat", new { id = viewproject.Project.ProjectId });
            }
            viewproject = new ProjectCreateViewModel();
            viewproject.Project = new Project();
            ViewData["MemberRoleId"] = new SelectList(_context.MemberRole, "MemberRoleId", "Title");
            ViewData["CreatorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View(viewproject);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", project.CreatorId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,DateCreated,CreatorId,ArtistName,Title,Description,ImagePath")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", project.CreatorId);
            return View(project);
        }

        public async Task<IActionResult> Members(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);

            ViewBag.ProjectId = id;
            ViewBag.ArtistName = project.ArtistName;
            ViewBag.ProjectTitle = project.Title;
            ViewBag.ImagePath = project.ImagePath;
            ViewBag.Title = "Members";

            var user = await GetCurrentUserAsync();
            var members = _context.ProjectUser.Include(pu => pu.User).Where(pu => pu.ProjectId == id);
            return View(await members.ToListAsync());
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }
    }
}
