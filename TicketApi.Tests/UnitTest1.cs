using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Moq;
using TicketAPI.CustomExceptions;
using TicketAPI.DataTransferObjects;
using TicketAPI.Helpers;
using TicketAPI.Interfaces;
using TicketAPI.Models.Tickets;
using TicketAPI.Models.TicketComments;
using TicketAPI.Models.Users;
using TicketAPI.Services;
using TicketAPI.Validators;
using CustomValidationException = TicketAPI.CustomExceptions.ValidationException;
using Xunit;

namespace TicketApi.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<CadastraUsuarioRequest>> _cadastraValidatorMock;
        private readonly Mock<IValidator<AtualizaUsuarioRequest>> _atualizaValidatorMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _cadastraValidatorMock = new Mock<IValidator<CadastraUsuarioRequest>>();
            _atualizaValidatorMock = new Mock<IValidator<AtualizaUsuarioRequest>>();

            _cadastraValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CadastraUsuarioRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _atualizaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<AtualizaUsuarioRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _userService = new UserService(_userRepositoryMock.Object, _cadastraValidatorMock.Object, _atualizaValidatorMock.Object);
        }

        [Fact]
        public async Task GetUsuarioPorIdAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var userDto = new UsersDto
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                Role = "User",
                CreatedAt = DateTime.Now,
                PasswordHash = "hashed"
            };

            _userRepositoryMock.Setup(r => r.UserExists(userId)).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _userService.GetUsuarioPorIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.IdUsuario);
            Assert.Equal("Test User", result.Nome);
            Assert.Equal("test@example.com", result.EmailUsuario);
            Assert.Equal("User", result.Funcao);
        }

        [Fact]
        public async Task GetUsuarioPorIdAsync_WhenUserNotExists_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 999;
            _userRepositoryMock.Setup(r => r.UserExists(userId)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUsuarioPorIdAsync(userId));
            Assert.Contains("Usuario ID 999", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_WhenValidRequest_CreatesUser()
        {
            // Arrange
            var request = new CadastraUsuarioRequest
            {
                Nome = "New User",
                Email = "new@example.com",
                Senha = "password123",
                Funcao = "User"
            };

            var createdUser = new UsersDto
            {
                Id = 1,
                Name = request.Nome,
                Email = request.Email,
                Role = request.Funcao,
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha)
            };

            _userRepositoryMock.Setup(r => r.CreateUserAsync(It.IsAny<UsersDto>()))
                .Callback<UsersDto>(u => u.Id = 1)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.CreateUserAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New User", result.Nome);
            Assert.Equal("new@example.com", result.EmailUsuario);
            Assert.Equal("User", result.Funcao);
            _userRepositoryMock.Verify(r => r.CreateUserAsync(It.IsAny<UsersDto>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WhenValidationFails_ThrowsValidationException()
        {
            // Arrange
            var request = new CadastraUsuarioRequest
            {
                Nome = "",
                Email = "invalid",
                Senha = "123",
                Funcao = "InvalidRole"
            };

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Nome", "Nome é obrigatório"),
                new FluentValidation.Results.ValidationFailure("Email", "Email inválido")
            });

            _cadastraValidatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(validationResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomValidationException>(() => _userService.CreateUserAsync(request));
            Assert.Contains("Nome é obrigatório", exception.Message);
            Assert.Contains("Email inválido", exception.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenValidRequest_UpdatesUser()
        {
            // Arrange
            var userId = 1;
            var existingUser = new UsersDto
            {
                Id = userId,
                Name = "Old Name",
                Email = "old@example.com",
                Role = "User",
                CreatedAt = DateTime.Now.AddDays(-10),
                PasswordHash = "oldhash"
            };

            var request = new AtualizaUsuarioRequest
            {
                Id = userId,
                Nome = "New Name",
                Email = "new@example.com",
                Funcao = "Support",
                DataCriacao = existingUser.CreatedAt.Value
            };

            _userRepositoryMock.Setup(r => r.UserExists(userId)).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.UpdateUserAsync(userId, It.IsAny<UsersDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result.Nome);
            Assert.Equal("new@example.com", result.EmailUsuario);
            Assert.Equal("Support", result.Funcao);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(userId, It.IsAny<UsersDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserExists_DeletesUser()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(r => r.UserExists(userId)).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            _userRepositoryMock.Verify(r => r.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetListaUsuariosAsync_ReturnsPagedResponse()
        {
            // Arrange
            var users = new List<UsersDto>
            {
                new UsersDto { Id = 1, Name = "User 1", Email = "user1@example.com", Role = "User", CreatedAt = DateTime.Now, PasswordHash = "hash" },
                new UsersDto { Id = 2, Name = "User 2", Email = "user2@example.com", Role = "Support", CreatedAt = DateTime.Now, PasswordHash = "hash" }
            };

            var pagedList = new PagedList<UsersDto>(users, users.Count, 1, 10);

            _userRepositoryMock.Setup(r => r.GetUserListAsync(1, 10)).ReturnsAsync(pagedList);

            // Act
            var result = await _userService.GetListaUsuariosAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.Data.Count);
        }
    }

    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepositoryMock;
        private readonly Mock<ITicketQueryRepository> _ticketQueryRepositoryMock;
        private readonly Mock<ITicketCommentRepository> _ticketCommentRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<CadastraTicketRequest>> _cadastraTicketValidatorMock;
        private readonly Mock<IValidator<AtualizaTicketRequest>> _atualizaTicketValidatorMock;
        private readonly Mock<IValidator<CadastraComentarioTicket>> _cadastraComentarioValidatorMock;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _ticketQueryRepositoryMock = new Mock<ITicketQueryRepository>();
            _ticketCommentRepositoryMock = new Mock<ITicketCommentRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _cadastraTicketValidatorMock = new Mock<IValidator<CadastraTicketRequest>>();
            _atualizaTicketValidatorMock = new Mock<IValidator<AtualizaTicketRequest>>();
            _cadastraComentarioValidatorMock = new Mock<IValidator<CadastraComentarioTicket>>();

            _cadastraTicketValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CadastraTicketRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _atualizaTicketValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<AtualizaTicketRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _cadastraComentarioValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CadastraComentarioTicket>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _ticketService = new TicketService(
                _ticketRepositoryMock.Object,
                _ticketQueryRepositoryMock.Object,
                _ticketCommentRepositoryMock.Object,
                _userRepositoryMock.Object,
                _cadastraTicketValidatorMock.Object,
                _atualizaTicketValidatorMock.Object,
                _cadastraComentarioValidatorMock.Object);
        }

        [Fact]
        public async Task GetSingleTicketAsync_WhenTicketExists_ReturnsTicket()
        {
            // Arrange
            var ticketId = 1;
            var ticketDto = new TicketsDto
            {
                Id = ticketId,
                Title = "Test Ticket",
                Description = "Test Description",
                Priority = "High",
                Status = "Open",
                CategoryId = 1,
                RequesterId = 1,
                AssignedToId = 2,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                ClosedAt = null
            };

            _ticketRepositoryMock.Setup(r => r.TicketExists(ticketId)).ReturnsAsync(true);
            _ticketRepositoryMock.Setup(r => r.GetSingleTicketAsync(ticketId)).ReturnsAsync(ticketDto);

            // Act
            var result = await _ticketService.GetSingleTicketAsync(ticketId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticketId, result.Id);
            Assert.Equal("Test Ticket", result.Titulo);
            Assert.Equal("High", result.Prioridade);
        }

        [Fact]
        public async Task PostNewTicket_WhenValidRequest_CreatesTicket()
        {
            // Arrange
            var request = new CadastraTicketRequest
            {
                Titulo = "New Ticket",
                Descricao = "New Description",
                Prioridade = "High",
                Status = "Open",
                IdCategoria = 1,
                IdSolicitante = 1,
                IdVinculado = 2
            };

            _ticketRepositoryMock.Setup(r => r.CreateTicketAsync(It.IsAny<TicketsDto>())).Returns(Task.CompletedTask);

            // Act
            await _ticketService.PostNewTicket(request);

            // Assert
            _ticketRepositoryMock.Verify(r => r.CreateTicketAsync(It.Is<TicketsDto>(t =>
                t.Title == request.Titulo &&
                t.Description == request.Descricao &&
                t.Priority == request.Prioridade &&
                t.Status == request.Status)), Times.Once);
        }

        [Fact]
        public async Task PostNewTicketComment_WhenValidRequest_CreatesComment()
        {
            // Arrange
            var request = new CadastraComentarioTicket
            {
                TicketId = 1,
                UserId = 1,
                Content = "Test comment"
            };

            _ticketRepositoryMock.Setup(r => r.TicketExists(request.TicketId)).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.UserExists(request.UserId)).ReturnsAsync(true);
            _ticketCommentRepositoryMock.Setup(r => r.CreateTicketCommentAsync(It.IsAny<TicketCommentsDto>())).Returns(Task.CompletedTask);

            // Act
            await _ticketService.PostNewTicketComment(request);

            // Assert
            _ticketCommentRepositoryMock.Verify(r => r.CreateTicketCommentAsync(It.Is<TicketCommentsDto>(c =>
                c.TicketId == request.TicketId &&
                c.UserId == request.UserId &&
                c.Content == request.Content)), Times.Once);
        }

        [Fact]
        public async Task DeleteTicket_WhenTicketExists_DeletesTicket()
        {
            // Arrange
            var ticketId = 1;
            _ticketRepositoryMock.Setup(r => r.TicketExists(ticketId)).ReturnsAsync(true);
            _ticketRepositoryMock.Setup(r => r.DeleteTicketsAsync(ticketId)).Returns(Task.CompletedTask);

            // Act
            await _ticketService.DeleteTicket(ticketId);

            // Assert
            _ticketRepositoryMock.Verify(r => r.DeleteTicketsAsync(ticketId), Times.Once);
        }
    }

    public class ValidatorTests
    {
        private readonly CadastraUsuarioRequestValidator _cadastraUsuarioValidator;
        private readonly AtualizaUsuarioRequestValidator _atualizaUsuarioValidator;
        private readonly CadastraTicketRequestValidator _cadastraTicketValidator;
        private readonly CadastraComentarioTicketValidator _cadastraComentarioValidator;

        public ValidatorTests()
        {
            _cadastraUsuarioValidator = new CadastraUsuarioRequestValidator();
            _atualizaUsuarioValidator = new AtualizaUsuarioRequestValidator();
            _cadastraTicketValidator = new CadastraTicketRequestValidator();
            _cadastraComentarioValidator = new CadastraComentarioTicketValidator();
        }

        [Fact]
        public void CadastraUsuarioRequestValidator_ValidRequest_PassesValidation()
        {
            var request = new CadastraUsuarioRequest
            {
                Nome = "Valid User",
                Email = "valid@example.com",
                Senha = "password123",
                Funcao = "User"
            };

            var result = _cadastraUsuarioValidator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void CadastraUsuarioRequestValidator_InvalidEmail_FailsValidation()
        {
            var request = new CadastraUsuarioRequest
            {
                Nome = "Valid User",
                Email = "invalid-email",
                Senha = "password123",
                Funcao = "User"
            };

            var result = _cadastraUsuarioValidator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void CadastraUsuarioRequestValidator_ShortPassword_FailsValidation()
        {
            var request = new CadastraUsuarioRequest
            {
                Nome = "Valid User",
                Email = "valid@example.com",
                Senha = "123",
                Funcao = "User"
            };

            var result = _cadastraUsuarioValidator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Senha");
        }

        [Fact]
        public void CadastraUsuarioRequestValidator_InvalidRole_FailsValidation()
        {
            var request = new CadastraUsuarioRequest
            {
                Nome = "Valid User",
                Email = "valid@example.com",
                Senha = "password123",
                Funcao = "InvalidRole"
            };

            var result = _cadastraUsuarioValidator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Funcao");
        }

        [Fact]
        public void CadastraTicketRequestValidator_ValidRequest_PassesValidation()
        {
            var request = new CadastraTicketRequest
            {
                Titulo = "Valid Ticket",
                Descricao = "Valid Description",
                Prioridade = "High",
                Status = "Open",
                IdCategoria = 1,
                IdSolicitante = 1
            };

            var result = _cadastraTicketValidator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void CadastraTicketRequestValidator_InvalidPriority_FailsValidation()
        {
            var request = new CadastraTicketRequest
            {
                Titulo = "Valid Ticket",
                Descricao = "Valid Description",
                Prioridade = "Invalid",
                Status = "Open",
                IdCategoria = 1,
                IdSolicitante = 1
            };

            var result = _cadastraTicketValidator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Prioridade");
        }

        [Fact]
        public void CadastraComentarioTicketValidator_ValidRequest_PassesValidation()
        {
            var request = new CadastraComentarioTicket
            {
                TicketId = 1,
                UserId = 1,
                Content = "Valid comment"
            };

            var result = _cadastraComentarioValidator.Validate(request);
            Assert.True(result.IsValid);
        }
    }

    public class PaginationHelperTests
    {
        [Fact]
        public void PagedList_Constructor_CreatesCorrectPage()
        {
            // Arrange
            var items = Enumerable.Range(1, 100).Select(i => new TestItem { Id = i, Name = $"Item {i}" }).ToList();
            var pageNumber = 2;
            var pageSize = 10;
            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Act
            var result = new PagedList<TestItem>(pagedItems, items.Count, pageNumber, pageSize);

            // Assert
            Assert.Equal(pageNumber, result.CurrentPage);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(100, result.TotalCount);
            Assert.Equal(10, result.TotalPages);
            Assert.Equal(10, result.Count);
            Assert.Equal(11, result.First().Id);
            Assert.Equal(20, result.Last().Id);
        }

        [Fact]
        public void PagedResponse_FromPagedList_CopiesProperties()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Id = 1, Name = "Item 1" },
                new TestItem { Id = 2, Name = "Item 2" }
            };
            var pagedList = new PagedList<TestItem>(items, 50, 2, 10);

            // Act
            var response = new PagedResponse<TestItem>(pagedList);

            // Assert
            Assert.Equal(2, response.CurrentPage);
            Assert.Equal(5, response.TotalPages);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(50, response.TotalCount);
            Assert.Equal(2, response.Data.Count);
            Assert.True(response.HasPrevious);
            Assert.True(response.HasNext);
        }

        private class TestItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }
    }

    public class PropertiesHelperTests
    {
        [Fact]
        public void AreAllPropertiesNull_AllNull_ReturnsTrue()
        {
            var obj = new TestObject { Prop1 = null, Prop2 = null, Prop3 = null };
            var result = PropertiesHelper.AreAllPropertiesNull(obj);
            Assert.True(result);
        }

        [Fact]
        public void AreAllPropertiesNull_SomeNotNull_ReturnsFalse()
        {
            var obj = new TestObject { Prop1 = "value", Prop2 = null, Prop3 = null };
            var result = PropertiesHelper.AreAllPropertiesNull(obj);
            Assert.False(result);
        }

        [Fact]
        public void AreAllPropertiesNull_ObjectNull_ReturnsFalse()
        {
            TestObject? obj = null;
            var result = PropertiesHelper.AreAllPropertiesNull(obj!);
            Assert.False(result);
        }

        [Fact]
        public void IsNullOrEmpty_NullString_ReturnsTrue()
        {
            string? value = null;
            var result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_EmptyString_ReturnsTrue()
        {
            var value = "";
            var result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_ValidString_ReturnsFalse()
        {
            var value = "test";
            var result = value.IsNullOrEmpty();
            Assert.False(result);
        }

        private class TestObject
        {
            public string? Prop1 { get; set; }
            public string? Prop2 { get; set; }
            public string? Prop3 { get; set; }
        }
    }
}