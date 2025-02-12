using MediatR;
using Moq;
using Project.Application.UseCases.AdminUseCases.RevokeTokens;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AdminUseCases
{
    public class RevokeTokensUnitTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly RevokeTokensHandler _handler;

        public RevokeTokensUnitTests()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _handler = new RevokeTokensHandler(_refreshTokenRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_UserHasTokens_RevokesTokensSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tokens = new List<RefreshToken>
        {
            new RefreshToken { Id = Guid.NewGuid(), UserId = userId, IsRevoked = false },
            new RefreshToken { Id = Guid.NewGuid(), UserId = userId, IsRevoked = false }
        };

            _refreshTokenRepositoryMock
                .Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokens);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var request = new RevokeTokensRequest { UserId = userId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.ShouldBe(Unit.Value);
            tokens.All(t => t.IsRevoked).ShouldBeTrue();
            _refreshTokenRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserHasNoTokens_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _refreshTokenRepositoryMock
                .Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RefreshToken>());

            var request = new RevokeTokensRequest { UserId = userId };

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () =>
                await _handler.Handle(request, CancellationToken.None));
        }
    }
}
