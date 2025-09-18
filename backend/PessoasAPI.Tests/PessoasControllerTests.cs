using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PessoasAPI.Controllers;
using PessoasAPI.Data;
using PessoasAPI.Models;
using PessoasAPI.Services;
using FluentAssertions;

namespace PessoasAPI.Tests
{
    public class PessoasControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PessoaService _service;
        private readonly PessoasController _controller;
        
        public PessoasControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new AppDbContext(options);
            _service = new PessoaService(_context);
            _controller = new PessoasController(_context, _service);
        }
        
        [Fact]
        public async Task GetPessoas_RetornaListaVazia_QuandoNaoHaPessoas()
        {
            // Act
            var result = await _controller.GetPessoas();
            
            // Assert
            result.Should().NotBeNull();
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().BeNull(); // Retorna ActionResult<IEnumerable<Pessoa>>
            
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetPessoas_RetornaListaOrdenada_QuandoHaPessoas()
        {
            // Arrange
            var pessoas = new List<Pessoa>
            {
                new Pessoa { Nome = "Zé", Email = "ze@teste.com", Cpf = "12345678909", DataNascimento = DateTime.Now.AddYears(-30) },
                new Pessoa { Nome = "Ana", Email = "ana@teste.com", Cpf = "98765432100", DataNascimento = DateTime.Now.AddYears(-25) }
            };
            
            _context.Pessoas.AddRange(pessoas);
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _controller.GetPessoas();
            
            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value!.First().Nome.Should().Be("Ana"); // Deve estar ordenado por nome
        }
        
        [Fact]
        public async Task GetPessoa_RetornaPessoa_QuandoExiste()
        {
            // Arrange
            var pessoa = new Pessoa 
            { 
                Nome = "Teste", 
                Email = "teste@teste.com", 
                Cpf = "12345678909", 
                DataNascimento = DateTime.Now.AddYears(-25) 
            };
            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _controller.GetPessoa(pessoa.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value!.Nome.Should().Be("Teste");
        }
        
        [Fact]
        public async Task GetPessoa_RetornaNotFound_QuandoNaoExiste()
        {
            // Act
            var result = await _controller.GetPessoa(999);
            
            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        
        [Fact]
        public async Task PostPessoa_CriaPessoa_ComDadosValidos()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Cpf = "12345678909",
                DataNascimento = DateTime.Now.AddYears(-30)
            };
            
            // Act
            var result = await _controller.PostPessoa(pessoa);
            
            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            
            // Verificar se foi salvo no banco
            var pessoaSalva = await _context.Pessoas.FindAsync(pessoa.Id);
            pessoaSalva.Should().NotBeNull();
            pessoaSalva!.Nome.Should().Be("João Silva");
        }
        
        [Fact]
        public async Task PostPessoa_RetornaBadRequest_ComCpfInvalido()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Cpf = "12345678901", // CPF inválido
                DataNascimento = DateTime.Now.AddYears(-30)
            };
            
            // Act
            var result = await _controller.PostPessoa(pessoa);
            
            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Fact]
        public async Task PostPessoa_RetornaBadRequest_ComMenorIdade()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Cpf = "12345678909",
                DataNascimento = DateTime.Now.AddYears(-15) // Menor de idade
            };
            
            // Act
            var result = await _controller.PostPessoa(pessoa);
            
            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Fact]
        public async Task PutPessoa_AtualizaPessoa_ComDadosValidos()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Cpf = "12345678909",
                DataNascimento = DateTime.Now.AddYears(-30)
            };
            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();
            
            // Modificar dados
            pessoa.Nome = "João Santos";
            
            // Act
            var result = await _controller.PutPessoa(pessoa.Id, pessoa);
            
            // Assert
            result.Should().BeOfType<NoContentResult>();
            
            // Verificar se foi atualizado
            var pessoaAtualizada = await _context.Pessoas.FindAsync(pessoa.Id);
            pessoaAtualizada!.Nome.Should().Be("João Santos");
            pessoaAtualizada.DataAtualizacao.Should().NotBeNull();
        }
        
        [Fact]
        public async Task DeletePessoa_RemovePessoa_QuandoExiste()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Cpf = "12345678909",
                DataNascimento = DateTime.Now.AddYears(-30)
            };
            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _controller.DeletePessoa(pessoa.Id);
            
            // Assert
            result.Should().BeOfType<NoContentResult>();
            
            // Verificar se foi removido
            var pessoaRemovida = await _context.Pessoas.FindAsync(pessoa.Id);
            pessoaRemovida.Should().BeNull();
        }
        
        [Fact]
        public async Task DeletePessoa_RetornaNotFound_QuandoNaoExiste()
        {
            // Act
            var result = await _controller.DeletePessoa(999);
            
            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}