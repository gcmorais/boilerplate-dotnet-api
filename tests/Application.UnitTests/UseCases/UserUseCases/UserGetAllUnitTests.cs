using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Application.UseCases.UserUseCases.GetAll;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.UserUseCases
{
    public class UserGetAllUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public UserGetAllUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetAllUsers_RepositoryReturnsUsers_ReturnsMappedUserResponses()
        {
            // Arrange
            byte[] hashPassword = new byte[32];
            byte[] saltPassword = new byte[16];

            var users = new List<User>
            {
                new User(
                    email: "user1@mail.com",
                    fullname: "user one",
                    username: "username1",
                    hashPassword: hashPassword,
                    saltPassword: saltPassword
                ),
                new User(
                    email: "user2@mail.com",
                    fullname: "user two",
                    username: "username2",
                    hashPassword: hashPassword,
                    saltPassword: saltPassword
                ),
                new User(
                    email: "user3@mail.com",
                    fullname: "user three",
                    username: "username3",
                    hashPassword: hashPassword,
                    saltPassword: saltPassword
                )
            };

            var userResponses = new List<UserResponse>
            {
                new UserResponse { Email = "user1@mail.com", FullName = "user one", UserName = "username1", HashPassword = hashPassword, SaltPassword = saltPassword},
                new UserResponse { Email = "user2@mail.com", FullName = "user two", UserName = "username2", HashPassword = hashPassword, SaltPassword = saltPassword},
                new UserResponse { Email = "user3@mail.com", FullName = "user three", UserName = "username3", HashPassword = hashPassword, SaltPassword = saltPassword},
            };

            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetAll(cancellationToken))
                .ReturnsAsync(users);

            _mapperMock
                .Setup(m => m.Map<List<UserResponse>>(users))
                .Returns(userResponses);

            var getAllUserHandler = new GetAllUserHandler(_userRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var response = await getAllUserHandler.Handle(new GetAllUserRequest(), cancellationToken);
            response.Data.ShouldBe(userResponses);

            _userRepositoryMock.Verify(repo => repo.GetAll(cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<List<UserResponse>>(users), Times.Once);
        }

        [Fact]
        public async Task GetAllUsers_RepositoryReturnsNull_ReturnsEmptyList()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetAll(cancellationToken))
                .ReturnsAsync((List<User>)null);

            var getAllUserHandler = new GetAllUserHandler(_userRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var response = await getAllUserHandler.Handle(new GetAllUserRequest(), cancellationToken);
            response.Data.ShouldBeEmpty();

            _userRepositoryMock.Verify(repo => repo.GetAll(cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>()), Times.Never);
        }

        [Fact]
        public async Task GetAllUsers_RepositoryReturnsEmptyList_ReturnsEmptyUserResponses()
        {
            // Arrange
            var users = new List<User>();
            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetAll(cancellationToken))
                .ReturnsAsync(users);

            _mapperMock
                .Setup(m => m.Map<List<UserResponse>>(users))
                .Returns(new List<UserResponse>());

            var getAllUserHandler = new GetAllUserHandler(_userRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var response = await getAllUserHandler.Handle(new GetAllUserRequest(), cancellationToken);
            response.Data.ShouldBeEmpty();

            _userRepositoryMock.Verify(repo => repo.GetAll(cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>()), Times.Never);

        }
    }
}
