using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentValidation;
using Moq;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Application.UseCases.UserUseCases.Update;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.UserUseCases
{
    public class UserUpdateUnitTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<UserUpdateRequest>> _userValidatorMock;

        public UserUpdateUnitTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userValidatorMock = new Mock<IValidator<UserUpdateRequest>>();
        }

        [Fact]
        public async Task ValidUser_UpdateIsCalled_ReturnValidResponseUser()
        {
            // Arrange
            var updateUserRequest = new Fixture().Create<UserUpdateRequest>();

            byte[] hashPassword = new byte[32];
            byte[] saltPassword = new byte[16];

            var existingUser = new User(
                email: "test@mail.com",
                fullname: "test",
                username: "usertest",
                hashPassword: hashPassword,
                saltPassword: saltPassword
            )
            {
                Id = updateUserRequest.Id,
                DateCreated = DateTimeOffset.UtcNow
            };

            var updatedUser = new User(
                email: updateUserRequest.Email,
                fullname: updateUserRequest.FullName,
                username: updateUserRequest.UserName,
                hashPassword: existingUser.HashPassword,
                saltPassword: existingUser.SaltPassword
            )
            {
                Id = updateUserRequest.Id,
                DateCreated = existingUser.DateCreated,
                DateUpdated = DateTimeOffset.UtcNow
            };

            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetById(updateUserRequest.Id, cancellationToken))
                .ReturnsAsync(existingUser);

            _userRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<User>()))
                .Callback<User>(user =>
                {
                    user.ShouldNotBeNull();
                    user.Id.ShouldBe(updateUserRequest.Id);
                    user.Fullname.ShouldBe(updateUserRequest.FullName);
                    user.UserName.ShouldBe(updateUserRequest.UserName);
                    user.Email.ShouldBe(updateUserRequest.Email);
                    user.DateCreated.ShouldBe(existingUser.DateCreated);
                });

            _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(new UserResponse
            {
                Email = updateUserRequest.Email,
                UserName = updateUserRequest.UserName,
                FullName = updateUserRequest.FullName,
                HashPassword = hashPassword,
                SaltPassword = saltPassword
            });

            _userValidatorMock.Setup(v => v.ValidateAsync(updateUserRequest, cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var userUpdateService = new UserUpdateHandler(
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _userValidatorMock.Object
            );

            // Act
            var response = await userUpdateService.Handle(updateUserRequest, cancellationToken);

            // Assert
            response.Data.FullName.ShouldBe(updateUserRequest.FullName);
            response.Data.Email.ShouldBe(updateUserRequest.Email);
            response.Data.UserName.ShouldBe(updateUserRequest.UserName);

            _userRepositoryMock.Verify(repo => repo.Update(It.Is<User>(u =>
                u.Id == updateUserRequest.Id &&
                u.Fullname == updateUserRequest.FullName &&
                u.UserName == updateUserRequest.UserName &&
                u.Email == updateUserRequest.Email)), Times.Once);

            _unitOfWorkMock.Verify(u => u.Commit(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task InvalidUserId_UpdateIsCalled_ReturnsNullOrThrowsException()
        {
            // Arrange
            var updateUserRequest = new Fixture().Create<UserUpdateRequest>();
            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetById(updateUserRequest.Id, cancellationToken))
                .ReturnsAsync((User)null);

            var userUpdateService = new UserUpdateHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object, _mapperMock.Object, _userValidatorMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () =>
             await userUpdateService.Handle(updateUserRequest, cancellationToken));

            _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(cancellationToken), Times.Never);
        }

        [Fact]
        public async Task ExistingUser_InvalidData_ReturnsValidationErrors()
        {
            // Arrange
            var invalidUpdateUserRequest = new UserUpdateRequest
            {
                Id = Guid.NewGuid(),
                Email = "invalid-email",
                FullName = "Valid Name",
                UserName = "us" // invalid username
            };

            var existingUser = new User(
                email: "existing@mail.com",
                fullname: "Existing User",
                username: "existingUsername",
                hashPassword: new byte[32],
                saltPassword: new byte[16]
            )
            {
                Id = invalidUpdateUserRequest.Id,
                DateCreated = DateTimeOffset.UtcNow
            };

            var cancellationToken = new CancellationToken();

            _userRepositoryMock
                .Setup(repo => repo.GetById(invalidUpdateUserRequest.Id, cancellationToken))
                .ReturnsAsync(existingUser);

            _userValidatorMock.Setup(v => v.ValidateAsync(invalidUpdateUserRequest, cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult
                {
                    Errors = {
                        new FluentValidation.Results.ValidationFailure("Email", "Invalid email format."),
                        new FluentValidation.Results.ValidationFailure("UserName", "Username must be between 3 and 50 characters.")
                    }
                });

            var userUpdateService = new UserUpdateHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object, _mapperMock.Object, _userValidatorMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
                await userUpdateService.Handle(invalidUpdateUserRequest, cancellationToken));

            exception.Errors.ShouldContain(e => e.PropertyName == nameof(UserUpdateRequest.Email));
            exception.Errors.ShouldContain(e => e.PropertyName == nameof(UserUpdateRequest.UserName));

            _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(cancellationToken), Times.Never);
        }
    }
}
