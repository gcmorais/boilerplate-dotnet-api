using Moq;
using Project.Application.UseCases.AdminUseCases.UnbanUser;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AdminUseCases
{
    public class UnbanUserUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UnbanUserHandler _handler;

        public UnbanUserUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UnbanUserHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_UserExists_UnbansUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("Test User", "testuser", "test@mail.com", new byte[32], new byte[16])
            {
                Id = userId
            };
            user.BanUser(DateTimeOffset.UtcNow.AddDays(7)); // Simulando um usuário banido

            var request = new UnbanUserRequest { UserId = userId };

            _userRepositoryMock.Setup(r => r.GetById(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(r => r.Update(user))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.Commit(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.ShouldNotBeNull();
            response.UserId.ShouldBe(userId);
            response.IsBanned.ShouldBeFalse();
            response.BannedUntil.ShouldBeNull();
            response.Description.ShouldBe("Successfully unbanned!!");

            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UnbanUserRequest { UserId = userId };

            _userRepositoryMock.Setup(r => r.GetById(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () =>
                await _handler.Handle(request, CancellationToken.None));

            _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
