using PessoasAPI.Models;
using PessoasAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace PessoasAPI.Services
{
    public class PessoaService
    {
        private readonly AppDbContext _context;
        
        public PessoaService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<bool> ValidarCpfUnico(string cpf, int? idExcluir = null)
        {
            return !await _context.Pessoas
                .Where(p => p.Cpf == cpf && (idExcluir == null || p.Id != idExcluir))
                .AnyAsync();
        }
        
        public async Task<bool> ValidarEmailUnico(string email, int? idExcluir = null)
        {
            return !await _context.Pessoas
                .Where(p => p.Email == email && (idExcluir == null || p.Id != idExcluir))
                .AnyAsync();
        }
        
        public bool ValidarIdade(DateTime dataNascimento)
        {
            var idade = DateTime.Now.Year - dataNascimento.Year;
            if (DateTime.Now.DayOfYear < dataNascimento.DayOfYear)
                idade--;
                
            return idade >= 18; // Apenas maiores de idade
        }
        
        public bool ValidarCpf(string cpf)
        {
            // Remover caracteres especiais
            cpf = cpf.Replace(".", "").Replace("-", "");
            
            // Verificar se tem 11 dígitos
            if (cpf.Length != 11)
                return false;
                
            // Verificar se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;
                
            // Validar dígitos verificadores
            return ValidarDigitosCpf(cpf);
        }
        
        private bool ValidarDigitosCpf(string cpf)
        {
            // Lógica de validação de CPF
            var multiplicadores1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicadores2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            
            var soma1 = cpf.Take(9).Select((c, i) => int.Parse(c.ToString()) * multiplicadores1[i]).Sum();
            var resto1 = soma1 % 11;
            var digito1 = resto1 < 2 ? 0 : 11 - resto1;
            
            var soma2 = cpf.Take(10).Select((c, i) => int.Parse(c.ToString()) * multiplicadores2[i]).Sum();
            var resto2 = soma2 % 11;
            var digito2 = resto2 < 2 ? 0 : 11 - resto2;
            
            return cpf[9] == digito1.ToString()[0] && cpf[10] == digito2.ToString()[0];
        }
    }
}