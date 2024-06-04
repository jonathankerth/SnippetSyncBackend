using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetSyncBackend.Data;
using SnippetSyncBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetSyncBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SnippetsController : ControllerBase
    {
        private readonly SnippetContext _context;

        public SnippetsController(SnippetContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CodeSnippet>>> GetSnippets()
        {
            return await _context.CodeSnippets.Include(cs => cs.Tags).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CodeSnippet>> GetSnippet(int id)
        {
            var snippet = await _context.CodeSnippets.Include(cs => cs.Tags).FirstOrDefaultAsync(cs => cs.Id == id);
            if (snippet == null) return NotFound();
            return snippet;
        }

        [HttpPost]
        public async Task<ActionResult<CodeSnippet>> CreateSnippet(CodeSnippet snippet)
        {
            _context.CodeSnippets.Add(snippet);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSnippet), new { id = snippet.Id }, snippet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSnippet(int id, CodeSnippet snippet)
        {
            if (id != snippet.Id) return BadRequest();
            _context.Entry(snippet).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CodeSnippets.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSnippet(int id)
        {
            var snippet = await _context.CodeSnippets.FindAsync(id);
            if (snippet == null) return NotFound();
            _context.CodeSnippets.Remove(snippet);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CodeSnippet>>> SearchSnippets(string query)
        {
            return await _context.CodeSnippets
                .Where(s => s.Title.Contains(query) || s.Code.Contains(query) || s.Tags.Any(t => t.Name.Contains(query)))
                .Include(cs => cs.Tags)
                .ToListAsync();
        }
    }
}
