using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PessoasAPI.Data;
using PessoasAPI.Models;
using PessoasAPI.Services;

namespace PessoasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PessoaService _pessoaService;
        
        public PessoasController(AppDbContext context, PessoaService pessoaService)
        {
            _context = context;
            _pessoaService = pessoaService;
        }
        
        // GET: api/pessoas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas()
        {
            return await _context.Pessoas.OrderBy(p => p.Nome).ToListAsync();
        }
        
        // GET: api/pessoas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pessoa>> GetPessoa(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            
            if (pessoa == null)
            {
                return NotFound(new { message = "Pessoa não encontrada" });
            }
            
            return pessoa;
        }
        
        // POST: api/pessoas
        [HttpPost]
        public async Task<ActionResult<Pessoa>> PostPessoa(Pessoa pessoa)
        {
            // Validações de negócio
            var validationErrors = await ValidarPessoa(pessoa);
            if (validationErrors.Any())
            {
                return BadRequest(new { errors = validationErrors });
            }
            
            pessoa.DataCriacao = DateTime.Now;
            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetPessoa), new { id = pessoa.Id }, pessoa);
        }
        
        // PUT: api/pessoas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPessoa(int id, Pessoa pessoa)
        {
            if (id != pessoa.Id)
            {
                return BadRequest(new { message = "ID não confere" });
            }
            
            // Validações de negócio
            var validationErrors = await ValidarPessoa(pessoa, id);
            if (validationErrors.Any())
            {
                return BadRequest(new { errors = validationErrors });
            }
            
            pessoa.DataAtualizacao = DateTime.Now;
            _context.Entry(pessoa).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PessoaExists(id))
                {
                    return NotFound(new { message = "Pessoa não encontrada" });
                }
                throw;
            }
            
            return NoContent();
        }
        
        // DELETE: api/pessoas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePessoa(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound(new { message = "Pessoa não encontrada" });
            }
            
            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        private bool PessoaExists(int id)
        {
            return _context.Pessoas.Any(e => e.Id == id);
        }
        
        private async Task<List<string>> ValidarPessoa(Pessoa pessoa, int? idExcluir = null)
        {
            var erros = new List<string>();
            
            // Validar CPF
            if (!_pessoaService.ValidarCpf(pessoa.Cpf))
            {
                erros.Add("CPF inválido");
            }
            else if (!await _pessoaService.ValidarCpfUnico(pessoa.Cpf, idExcluir))
            {
                erros.Add("CPF já cadastrado");
            }
            
            // Validar Email único
            if (!await _pessoaService.ValidarEmailUnico(pessoa.Email, idExcluir))
            {
                erros.Add("Email já cadastrado");
            }
            
            // Validar idade
            if (!_pessoaService.ValidarIdade(pessoa.DataNascimento))
            {
                erros.Add("Pessoa deve ser maior de idade");
            }
            
            return erros;
        }
    }
}