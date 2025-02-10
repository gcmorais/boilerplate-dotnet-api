using AutoFixture;
using Moq;
using Project.Application.UseCases.UserUseCases.Delete;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.UserUseCases
{
    public class UserDeleteUnitTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserDeleteUnitTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task UserExists_DeleteIsCalled_ReturnValidResponseUser()
        {
            // Arrange
            var deleteUserRequest = new Fixture().Create<DeleteUserRequest>();

            byte[] hashPassword = new byte[10];
            byte[] saltPassword = new byte[5];

            var user = new User(
                email: "test@mail.com",
                fullname: "test",
                username: "usertest",
                hashPassword: hashPassword,
                saltPassword: saltPassword
            );
            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetById(deleteUserRequest.id, cancellationToken))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(repo => repo.Delete(user))
                .Verifiable();

            var userDeleteService = new DeleteUserHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);

            // Act & Assert
            var response = await userDeleteService.Handle(deleteUserRequest, cancellationToken);

            response.Data.ShouldNotBeNull();

            _userRepositoryMock.Verify(repo => repo.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UserDoesNotExist_DeleteIsCalled_ReturnDefault()
        {
            // Arrange
            var deleteUserRequest = new Fixture().Create<DeleteUserRequest>();
            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetById(deleteUserRequest.id, cancellationToken))
                .ReturnsAsync((User)null);

            var userDeleteService = new DeleteUserHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => userDeleteService.Handle(deleteUserRequest, cancellationToken));
            exception.Message.ShouldBe("User not found.");

            _userRepositoryMock.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(cancellationToken), Times.Never);
        }
    }
}
