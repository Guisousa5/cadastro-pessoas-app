using Microsoft.EntityFrameworkCore;
using PessoasAPI.Data;
using PessoasAPI.Models;
using PessoasAPI.Services;
using FluentAssertions;

namespace PessoasAPI.Tests
{
    public class PessoaServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PessoaService _service;
        
        public PessoaServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new AppDbContext(options);
            _service = new PessoaService(_context);
        }
        
        [Theory]
        [InlineData("12345678909")] // CPF válido
        [InlineData("11144477735")] // CPF válido
        public void ValidarCpf_CpfValido_RetornaTrue(string cpfValido)
        {
            // Act
            var resultado = _service.ValidarCpf(cpfValido);
            
            // Assert
            resultado.Should().BeTrue();
        }
        
        [Theory]
        [InlineData("12345678901")] // CPF inválido
        [InlineData("00000000000")] // CPF com todos zeros
        [InlineData("11111111111")] // CPF com todos números iguais
        [InlineData("123456789")]   // CPF com menos dígitos
        public void ValidarCpf_CpfInvalido_RetornaFalse(string cpfInvalido)
        {
            // Act
            var resultado = _service.ValidarCpf(cpfInvalido);
            
            // Assert
            resultado.Should().BeFalse();
        }
        
        [Fact]
        public void ValidarIdade_MaiorIdade_RetornaTrue()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-25);
            
            // Act
            var resultado = _service.ValidarIdade(dataNascimento);
            
            // Assert
            resultado.Should().BeTrue();
        }
        
        [Fact]
        public void ValidarIdade_MenorIdade_RetornaFalse()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-15);
            
            // Act
            var resultado = _service.ValidarIdade(dataNascimento);
            
            // Assert
            resultado.Should().BeFalse();
        }
        
        [Fact]
        public void ValidarIdade_Exatos18Anos_RetornaTrue()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-18);
            
            // Act
            var resultado = _service.ValidarIdade(dataNascimento);
            
            // Assert
            resultado.Should().BeTrue();
        }
        
        [Fact]
        public async Task ValidarEmailUnico_EmailNovo_RetornaTrue()
        {
            // Arrange
            var emailNovo = "novo@teste.com";
            
            // Act
            var resultado = await _service.ValidarEmailUnico(emailNovo);
            
            // Assert
            resultado.Should().BeTrue();
        }
        
        [Fact]
        public async Task ValidarEmailUnico_EmailJaExiste_RetornaFalse()
        {
            // Arrange
            var emailExistente = "existente@teste.com";
            _context.Pessoas.Add(new Pessoa 
            { 
                Nome = "Teste",
                Email = emailExistente,
                Cpf = "12345678909",
                DataNascimento = DateTime.Now.AddYears(-25)
            });
            await _context.SaveChangesAsync();
            
            // Act
            var resultado = await _service.ValidarEmailUnico(emailExistente);
            
            // Assert
            resultado.Should().BeFalse();
        }
        
        [Fact]
        public async Task ValidarCpfUnico_CpfNovo_RetornaTrue()
        {
            // Arrange
            var cpfNovo = "12345678909";
            
            // Act
            var resultado = await _service.ValidarCpfUnico(cpfNovo);
            
            // Assert
            resultado.Should().BeTrue();
        }
        
        [Fact]
        public async Task ValidarCpfUnico_CpfJaExiste_RetornaFalse()
        {
            // Arrange
            var cpfExistente = "12345678909";
            _context.Pessoas.Add(new Pessoa 
            { 
                Nome = "Teste",
                Email = "teste@teste.com",
                Cpf = cpfExistente,
                DataNascimento = DateTime.Now.AddYears(-25)
            });
            await _context.SaveChangesAsync();
            
            // Act
            var resultado = await _service.ValidarCpfUnico(cpfExistente);
            
            // Assert
            resultado.Should().BeFalse();
        }
        
        [Fact]
        public async Task ValidarEmailUnico_EmailExistenteComExclusao_RetornaTrue()
        {
            // Arrange
            var emailExistente = "existente@teste.com";
            var pessoa = new Pessoa 
            { 
                Id = 1,
                Nome = "Teste",
                Email = emailExistente,
                Cpf = "12345678909",
                DataNascimento = DateTime.Now.AddYears(-25)
            };
            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();
            
            // Act - validar email da própria pessoa (excluindo ela da validação)
            var resultado = await _service.ValidarEmailUnico(emailExistente, pessoa.Id);
            
            // Assert
            resultado.Should().BeTrue();
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}