using Moq;
using Project.Application.UseCases.AuthUseCases.Enable2FA;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AuthUseCases
{
    public class Enable2FAUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly EnableTwoFactorHandler _handler;

        public Enable2FAUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new EnableTwoFactorHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidUser_EnablesTwoFactorSuccessfully()
        {
            // Arrange
            var request = new EnableTwoFactorRequest { Email = "test@mail.com" };
            var cancellationToken = new CancellationToken();

            var user = new User(
                fullname: "Test User",
                username: "testuser",
                email: request.Email,
                hashPassword: new byte[32],
                saltPassword: new byte[16]
            );

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, cancellationToken))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(r => r.Update(user))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Status.ShouldBeTrue();
            response.Data.ShouldBe("Two-factor authentication enabled.");
            user.IsTwoFactorEnabled.ShouldBeTrue();
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var request = new EnableTwoFactorRequest { Email = "nonexistent@mail.com" };
            var cancellationToken = new CancellationToken();

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, cancellationToken))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(() => _handler.Handle(request, cancellationToken));
        }
    }

}