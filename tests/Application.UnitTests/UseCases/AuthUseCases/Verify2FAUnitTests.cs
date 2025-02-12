using Moq;
using Project.Application.Interfaces;
using Project.Application.UseCases.AuthUseCases.Verify2FA;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;


namespace Application.UnitTests.UseCases.AuthUseCases
{
    public class Verify2FAUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly VerifyTwoFactorHandler _handler;

        public Verify2FAUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new VerifyTwoFactorHandler(_userRepositoryMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidTwoFactorCode_ReturnsToken()
        {
            // Arrange
            var request = new VerifyTwoFactorRequest { Email = "test@mail.com", Code = "123456" };
            var user = new User("Test User", "testuser", request.Email, new byte[32], new byte[16]);

            user.EnableTwoFactorAuthentication();

            user.GenerateTwoFactorCode(); // Generates 2FA code
            var validCode = user.TwoFactorCode; // Gets the correct code

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _tokenServiceMock.Setup(t => t.GenerateToken(user, user.Roles))
                .Returns("mocked-jwt-token");

            // Mock to simulate that validation always returns true when the correct code is used
            _userRepositoryMock.Setup(u => u.GetByEmail(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var response = await _handler.Handle(new VerifyTwoFactorRequest { Email = "test@mail.com", Code = validCode }, CancellationToken.None);

            // Assert
            response.ShouldNotBeNull();
            response.Token.ShouldBe("mocked-jwt-token");
        }

        [Fact]
        public async Task Handle_InvalidTwoFactorCode_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new VerifyTwoFactorRequest { Email = "test@mail.com", Code = "wrong-code" };
            var user = new User("Test User", "testuser", request.Email, new byte[32], new byte[16]);
            user.EnableTwoFactorAuthentication();

            user.GenerateTwoFactorCode(); // Generates valid code, but the test will use the wrong one

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new VerifyTwoFactorRequest { Email = "nonexistent@mail.com", Code = "123456" };

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
        }
    }
}
