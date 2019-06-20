using System;
using System.Collections.Generic;
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
    public class ProjectInvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectInvitesController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: ProjectInvites
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            var invites = _context.ProjectInvite.Include(p => p.Project).Include(p => p.Receiver).Include(p => p.Sender).Where(p => p.ReceiverId == user.Id).ToList();
            return View(invites);
        }

        // GET: ProjectInvites/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectInvite = await _context.ProjectInvite
                .Include(p => p.Project)
                .Include(p => p.Receiver)
                .Include(p => p.Sender)
                .FirstOrDefaultAsync(m => m.ProjectInviteId == id);
            if (projectInvite == null)
            {
                return NotFound();
            }

            return View(projectInvite);
        }

        // GET: ProjectInvites/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ArtistName");
            ViewData["ReceiverId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["SenderId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: ProjectInvites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectInviteId,DateInvited,DateAccepted,IsActive,SenderId,ReceiverId,ProjectId")] ProjectInvite projectInvite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectInvite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ArtistName", projectInvite.ProjectId);
            ViewData["ReceiverId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", projectInvite.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", projectInvite.SenderId);
            return View(projectInvite);
        }

        // GET: ProjectInvites/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectInvite = await _context.ProjectInvite.FindAsync(id);
            if (projectInvite == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ArtistName", projectInvite.ProjectId);
            ViewData["ReceiverId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", projectInvite.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", projectInvite.SenderId);
            return View(projectInvite);
        }

        // POST: ProjectInvites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectInviteId,DateInvited,DateAccepted,IsActive,SenderId,ReceiverId,ProjectId")] ProjectInvite projectInvite)
        {
            if (id != projectInvite.ProjectInviteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectInvite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectInviteExists(projectInvite.ProjectInviteId))
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
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ArtistName", projectInvite.ProjectId);
            ViewData["ReceiverId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", projectInvite.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", projectInvite.SenderId);
            return View(projectInvite);
        }

        // GET: ProjectInvites/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectInvite = await _context.ProjectInvite
                .Include(p => p.Project)
                .Include(p => p.Receiver)
                .Include(p => p.Sender)
                .FirstOrDefaultAsync(m => m.ProjectInviteId == id);
            if (projectInvite == null)
            {
                return NotFound();
            }

            return View(projectInvite);
        }

        // POST: ProjectInvites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectInvite = await _context.ProjectInvite.FindAsync(id);
            _context.ProjectInvite.Remove(projectInvite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AcceptInvite(int id)
        {
            var projectInvite = await _context.ProjectInvite.FindAsync(id);

            var projectUser = new ProjectUser();
            projectUser.DateAdded = DateTime.Now;
            projectUser.UserId = projectInvite.ReceiverId;
            projectUser.ProjectId = projectInvite.ProjectId;
            projectUser.IsAdmin = false;
            projectUser.MemberRoleId = 2;

            _context.Add(projectUser);
            _context.ProjectInvite.Remove(projectInvite);
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", "Projects", new { id = projectInvite.ProjectId });
        }

        [Authorize]
        public async Task<IActionResult> DeclineInvite(int id)
        {
            var projectInvite = await _context.ProjectInvite.FindAsync(id);
            _context.ProjectInvite.Remove(projectInvite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectInviteExists(int id)
        {
            return _context.ProjectInvite.Any(e => e.ProjectInviteId == id);
        }
    }
}
