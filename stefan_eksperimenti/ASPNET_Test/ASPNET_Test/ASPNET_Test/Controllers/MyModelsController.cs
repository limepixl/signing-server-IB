using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPNET_Test.Data;
using ASPNET_Test.Models;

namespace ASPNET_Test.Controllers
{
    public class MyModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MyModels
        public async Task<IActionResult> Index()
        {
              return _context.Model != null ? 
                          View() :
                          Problem("Entity set 'ApplicationDbContext.MyModel'  is null.");
        }

        // GET: MyModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MyModel == null)
            {
                return NotFound();
            }

            var myModel = await _context.MyModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myModel == null)
            {
                return NotFound();
            }

            return View(myModel);
        }

        // GET: MyModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,surname")] MyModel myModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(myModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(myModel);
        }

        // GET: MyModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MyModel == null)
            {
                return NotFound();
            }

            var myModel = await _context.MyModel.FindAsync(id);
            if (myModel == null)
            {
                return NotFound();
            }
            return View(myModel);
        }

        // POST: MyModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,surname")] MyModel myModel)
        {
            if (id != myModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyModelExists(myModel.Id))
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
            return View(myModel);
        }

        // GET: MyModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MyModel == null)
            {
                return NotFound();
            }

            var myModel = await _context.MyModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myModel == null)
            {
                return NotFound();
            }

            return View(myModel);
        }

        // POST: MyModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MyModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MyModel'  is null.");
            }
            var myModel = await _context.MyModel.FindAsync(id);
            if (myModel != null)
            {
                _context.MyModel.Remove(myModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyModelExists(int id)
        {
          return (_context.MyModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
