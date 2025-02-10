using AutoFixture;
using Moq;
using Project.Application.Interfaces;
using Project.Application.UseCases.AuthUseCases.Login;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AuthUseCases
{
    public class LoginUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ICreateVerifyHash> _hashServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Fixture _fixture;

        public LoginUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _hashServiceMock = new Mock<ICreateVerifyHash>();
            _emailServiceMock = new Mock<IEmailService>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handle_ValidUser_ReturnsTokenResponse()
        {
            // Arrange
            var request = _fixture.Create<LoginUserRequest>();

            byte[] hashPassword = new byte[32];
            byte[] saltPassword = new byte[16];

            var user = new User(
                email: request.Email,
                fullname: "testname",
                username: "testusername",
                hashPassword: hashPassword,
                saltPassword: saltPassword
            );

            var cancellationToken = new CancellationToken();

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, cancellationToken))
                .ReturnsAsync(user);
            _hashServiceMock.Setup(h => h.PasswordVerify(request.Password, user.HashPassword, user.SaltPassword))
                .Returns(true);
            _tokenServiceMock.Setup(t => t.GenerateToken(user, user.Roles))
                .Returns("mocked-jwt-token");

            _refreshTokenRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<RefreshToken>(), cancellationToken))
                .Returns(Task.CompletedTask);

            var handler = new LoginHandler(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _hashServiceMock.Object,
                _emailServiceMock.Object,
                _refreshTokenRepositoryMock.Object);

            // Act
            var response = await handler.Handle(request, cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Token.ShouldBe("mocked-jwt-token");
            response.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = _fixture.Create<LoginUserRequest>();
            var user = _fixture.Create<User>();

            var cancellationToken = new CancellationToken();

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, cancellationToken))
                .ReturnsAsync(user);
            _hashServiceMock.Setup(h => h.PasswordVerify(request.Password, user.HashPassword, user.SaltPassword))
                .Returns(false);

            var handler = new LoginHandler(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _hashServiceMock.Object,
                _emailServiceMock.Object,
                _refreshTokenRepositoryMock.Object);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(() => handler.Handle(request, cancellationToken));
        }
    }
}
    