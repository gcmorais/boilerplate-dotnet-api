using AutoFixture;
using Moq;
using Project.Application.Interfaces;
using Project.Application.UseCases.AuthUseCases.RefreshTokens;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AuthUseCases
{
    public class RefreshTokenUnitTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Fixture _fixture;

        public RefreshTokenUnitTests()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handle_ValidRefreshToken_ReturnsNewTokens()
        {
            // Arrange
            var request = _fixture.Create<RefreshTokenRequest>();
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = request.Token,
                UserId = Guid.NewGuid(),
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            var user = new User("Test User", "testuser", "test@example.com", new byte[32], new byte[16]);
            var cancellationToken = new CancellationToken();

            _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(request.Token, cancellationToken))
                .ReturnsAsync(refreshToken);
            _userRepositoryMock.Setup(r => r.GetById(refreshToken.UserId, cancellationToken))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateToken(user, user.Roles))
                .Returns("new-jwt-token");
            _refreshTokenRepositoryMock.Setup(r => r.RevokeAsync(refreshToken.Id, cancellationToken))
                .Returns(Task.CompletedTask);
            _refreshTokenRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<RefreshToken>(), cancellationToken))
                .Returns(Task.CompletedTask);

            var handler = new RefreshTokenHandler(
                _refreshTokenRepositoryMock.Object,
                _userRepositoryMock.Object,
                _tokenServiceMock.Object);

            // Act
            var response = await handler.Handle(request, cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Token.ShouldBe("new-jwt-token");
            response.RefreshToken.ShouldNotBeNullOrWhiteSpace();

            _refreshTokenRepositoryMock.Verify(r => r.RevokeAsync(refreshToken.Id, cancellationToken), Times.Once);
            _refreshTokenRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<RefreshToken>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidOrExpiredRefreshToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = _fixture.Create<RefreshTokenRequest>();
            var cancellationToken = new CancellationToken();

            _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(request.Token, cancellationToken))
                .ReturnsAsync((RefreshToken)null);

            var handler = new RefreshTokenHandler(
                _refreshTokenRepositoryMock.Object,
                _userRepositoryMock.Object,
                _tokenServiceMock.Object);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(() => handler.Handle(request, cancellationToken));
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = _fixture.Create<RefreshTokenRequest>();
            var refreshToken = _fixture.Build<RefreshToken>()
                .With(rt => rt.Token, request.Token)
                .With(rt => rt.ExpiryDate, DateTime.UtcNow.AddDays(1))
                .With(rt => rt.IsRevoked, false)
                .Create();
            var cancellationToken = new CancellationToken();

            _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(request.Token, cancellationToken))
                .ReturnsAsync(refreshToken);
            _userRepositoryMock.Setup(r => r.GetById(refreshToken.UserId, cancellationToken))
                .ReturnsAsync((User)null);

            var handler = new RefreshTokenHandler(
                _refreshTokenRepositoryMock.Object,
                _userRepositoryMock.Object,
                _tokenServiceMock.Object);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(() => handler.Handle(request, cancellationToken));
        }
    }

}
