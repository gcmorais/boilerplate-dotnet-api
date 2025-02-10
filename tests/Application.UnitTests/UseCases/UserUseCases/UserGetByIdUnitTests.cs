using AutoFixture;
using AutoMapper;
using Moq;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Application.UseCases.UserUseCases.GetById;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.UserUseCases
{
    public class UserGetByIdUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetByIdUserHandler _handler;
        private readonly Fixture _fixture;

        public UserGetByIdUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetByIdUserHandler(_userRepositoryMock.Object, _mapperMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task UserExists_ReturnsUserResponse()
        {
            // Arrange
            var request = _fixture.Create<GetByIdUserRequest>();
            var user = _fixture.Create<User>();
            var userResponse = _fixture.Create<UserResponse>();
            var cancellationToken = new CancellationToken();

            _userRepositoryMock.Setup(repo => repo.GetById(request.id, cancellationToken))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserResponse>(user))
                .Returns(userResponse);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Data.ShouldBe(userResponse);
            response.Status.ShouldBeTrue();

            _userRepositoryMock.Verify(repo => repo.GetById(request.id, cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<UserResponse>(user), Times.Once);
        }

        [Fact]
        public async Task UserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<GetByIdUserRequest>();
            var cancellationToken = new CancellationToken();

            _userRepositoryMock.Setup(repo => repo.GetById(request.id, cancellationToken))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () =>
                await _handler.Handle(request, cancellationToken));

            _userRepositoryMock.Verify(repo => repo.GetById(request.id, cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<UserResponse>(It.IsAny<User>()), Times.Never);
        }
    }
}
