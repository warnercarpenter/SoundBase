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
        [Authorize]
        public async Task<IActionResult> Chat(int id)
        {

            var user = await GetCurrentUserAsync();

            ViewBag.PageTitle = "Chat";

            ProjectChatViewModel viewproject = new ProjectChatViewModel();

            viewproject.ProjectId = id;

            viewproject.Project = await _context.Project
                .Include(p => p.Creator)
                .Include(p => p.ChatMessages)
                .Include(p => p.ProjectUsers)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            bool isAuth = false;

            foreach (ProjectUser projectUser in viewproject.Project.ProjectUsers)
            {
                if (projectUser.UserId == user.Id)
                {
                    isAuth = true;
                }
            }

            if (isAuth == false)
            {
                return RedirectToAction("Index");
            }

            viewproject.Project.ChatMessages = viewproject.Project.ChatMessages.OrderBy(cm => cm.DatePosted).ToList();

            if (viewproject.Project == null)
            {
                return NotFound();
            }

            ViewBag.ArtistName = viewproject.Project.ArtistName;
            ViewBag.ProjectTitle = viewproject.Project.Title;
            ViewBag.ImagePath = viewproject.Project.ImagePath;
            ViewBag.UserId = user.Id;
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(id);

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
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(viewproject.Project.ProjectId);

            return View(viewproject);
        }

        [Authorize]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var user = await GetCurrentUserAsync();
            var chatMessage = await _context.ChatMessage.FirstOrDefaultAsync(c => c.ChatMessageId == id);

            if (user.Id == chatMessage.UserId)
            {
                _context.ChatMessage.Remove(chatMessage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Chat", new { id = chatMessage.ProjectId });
        }

        // GET: Projects/Create
        [Authorize]
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
                initialUser.MemberRoleId = 1;
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
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Project.FindAsync(id);

            if (await IsProjectOwnerAsync(id) == false)
            {
                return RedirectToAction("Chat", new { id = id });
            }

            var viewproject = new ProjectEditViewModel();
            viewproject.Project = project;
            viewproject.ImagePath = project.ImagePath;

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.ArtistName = viewproject.Project.ArtistName;
            ViewBag.ProjectTitle = viewproject.Project.Title;
            ViewBag.ImagePath = viewproject.Project.ImagePath;
            ViewBag.ProjectId = id;
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(id);

            ViewBag.PageTitle = "Edit";
            ViewData["CreatorId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", project.CreatorId);
            return View(viewproject);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectEditViewModel viewproject)
        {
            var project = viewproject.Project;

            if (project.ImagePath == null)
            {
                project.ImagePath = viewproject.ImagePath;
            }

            if (ModelState.IsValid)
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Chat", new { id = project.ProjectId });
            }
            return RedirectToAction("Edit", new { id = project.ProjectId });
        }

        [Authorize]
        public async Task<IActionResult> Info(int id)
        {
            var user = await GetCurrentUserAsync();
            var project = await _context.Project.Include(p => p.ProjectUsers).Include(p => p.ProjectUsers).Include(p => p.Creator).FirstOrDefaultAsync(m => m.ProjectId == id);

            bool isAuth = false;

            foreach (ProjectUser projectUser in project.ProjectUsers)
            {
                if (projectUser.UserId == user.Id)
                {
                    isAuth = true;
                }
            }

            if (isAuth == false)
            {
                return RedirectToAction("Index");
            }

            ViewBag.PageTitle = "Info";
            ViewBag.ArtistName = project.ArtistName;
            ViewBag.ProjectTitle = project.Title;
            ViewBag.ImagePath = project.ImagePath;
            ViewBag.ProjectId = id;
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize]
        public async Task<IActionResult> Members(int id)
        {
            var project = await _context.Project
                .Include(p => p.Creator)
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.MemberRole)
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            bool isAuth = false;

            var user = await GetCurrentUserAsync();

            foreach (ProjectUser projectUser in project.ProjectUsers)
            {
                if (projectUser.UserId == user.Id)
                {
                    isAuth = true;
                }
            }

            if (isAuth == false)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = id;
            ViewBag.ArtistName = project.ArtistName;
            ViewBag.ProjectTitle = project.Title;
            ViewBag.ImagePath = project.ImagePath;
            ViewBag.PageTitle = "Members";
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(id);

            return View(project);
        }

        [Authorize]
        public async Task<IActionResult> Tracks(int id)
        {
            var project = await _context.Project.Include(p => p.ProjectUsers)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            var user = await GetCurrentUserAsync();

            bool isAuth = false;

            foreach (ProjectUser projectUser in project.ProjectUsers)
            {
                if (projectUser.UserId == user.Id)
                {
                    isAuth = true;
                }
            }

            if (isAuth == false)
            {
                return RedirectToAction("Index");
            }

            var tracks = _context.Track.Include(t => t.Project).Include(t => t.User).Where(t => t.ProjectId == id).OrderByDescending(t => t.DateUploaded).ToList();

            ViewBag.ProjectId = id;
            ViewBag.ArtistName = project.ArtistName;
            ViewBag.ProjectTitle = project.Title;
            ViewBag.ImagePath = project.ImagePath;
            ViewBag.PageTitle = "Tracks";
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(id);
            ViewBag.UserId = user.Id;

            return View(tracks);
        }

        [Authorize]
        public async Task<IActionResult> DeleteTrack(int id)
        {
            var user = await GetCurrentUserAsync();
            var track = await _context.Track.FirstOrDefaultAsync(t => t.TrackId == id);

            if (user.Id == track.UserId)
            {
                _context.Track.Remove(track);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Tracks", new { id = track.ProjectId });
        }

        [Authorize]
        public async Task<IActionResult> UploadTrack(int id)
        {

            var user = await GetCurrentUserAsync();
            var project = await _context.Project.Include(p => p.ProjectUsers)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            bool isAuth = false;

            foreach (ProjectUser projectUser in project.ProjectUsers)
            {
                if (projectUser.UserId == user.Id)
                {
                    isAuth = true;
                }
            }

            if (isAuth == false)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = id;
            ViewBag.ArtistName = project.ArtistName;
            ViewBag.ProjectTitle = project.Title;
            ViewBag.ImagePath = project.ImagePath;
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(id);

            var viewmodel = new TrackCreateViewModel
            {
                Track = new Track
                {
                    ProjectId = id
                }
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadTrack(TrackCreateViewModel viewmodel)
        {
            var user = await GetCurrentUserAsync();

            var fileName = Path.GetFileName(viewmodel.AudioFile.FileName);
            Path.GetTempFileName();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\audio", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await viewmodel.AudioFile.CopyToAsync(stream);
            }

            viewmodel.Track.FilePath = viewmodel.AudioFile.FileName;

            viewmodel.Track.UserId = user.Id;
            _context.Add(viewmodel.Track);
            await _context.SaveChangesAsync();
            return RedirectToAction("Tracks", new { id = viewmodel.Track.ProjectId });
        }

        [Authorize]
        public async Task<IActionResult> InviteMembers(int id)
        {

            var project = await _context.Project
                .Include(p => p.Creator)
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            var userToCheck = await GetCurrentUserAsync();

            bool isAuth = false;

            foreach (ProjectUser projectUser in project.ProjectUsers)
            {
                if (projectUser.UserId == userToCheck.Id)
                {
                    isAuth = true;
                }
            }

            if (isAuth == false)
            {
                return RedirectToAction("Index");
            }

            var users = _context.ApplicationUsers.Include(u => u.ProjectUsers).Include(u => u.ProjectInvitesReceived).ToList();

            var viewmodel = new InviteUserViewModel();
            viewmodel.ProjectId = id;
            viewmodel.Users = new List<ApplicationUser>();

            foreach (ApplicationUser user in users)
            {
                user.ProjectUsers = user.ProjectUsers.Where(pu => pu.ProjectId == id).ToList();
                user.ProjectInvitesReceived = user.ProjectInvitesReceived.Where(pi => pi.ProjectId == id).ToList();
                if (user.ProjectUsers.Count < 1 && user.ProjectInvitesReceived.Count < 1)
                {
                    viewmodel.Users.Add(user);
                };
            }

            ViewBag.ProjectId = id;
            ViewBag.ArtistName = project.ArtistName;
            ViewBag.ProjectTitle = project.Title;
            ViewBag.ImagePath = project.ImagePath;
            ViewBag.PageTitle = "Members";
            ViewBag.IsProjectOwner = await IsProjectOwnerAsync(id);

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteMembers(InviteUserViewModel viewmodel)
        {
            var user = await GetCurrentUserAsync();

            var projectInvite = new ProjectInvite();
            projectInvite.DateInvited = DateTime.Now;
            projectInvite.IsActive = true;
            projectInvite.SenderId = user.Id;
            projectInvite.ReceiverId = viewmodel.UserId;
            projectInvite.ProjectId = viewmodel.ProjectId;

            _context.Add(projectInvite);
            await _context.SaveChangesAsync();
            return RedirectToAction("InviteMembers", new { id = viewmodel.ProjectId });
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }

        public async Task<bool> IsProjectOwnerAsync(int id)
        {
            var user = await GetCurrentUserAsync();

            var project = await _context.Project
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project.CreatorId == user.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
