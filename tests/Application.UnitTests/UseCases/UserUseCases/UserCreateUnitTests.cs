using AutoFixture;
using AutoMapper;
using Moq;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Application.UseCases.UserUseCases.Create;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.UserUseCases
{
    public class UserCreateUnitTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICreateVerifyHash> _createVerifyHashMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserCreateUnitTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _createVerifyHashMock = new Mock<ICreateVerifyHash>();
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task ValidUser_CreateIsCalled_ReturnValidResponseUser()
        {
            // Arrange
            var userCreateRequest = new Fixture().Create<UserCreateRequest>();

            byte[] hashPassword = new byte[32];
            byte[] saltPassword = new byte[16];

            var user = new User(
                email: userCreateRequest.Email,
                fullname: userCreateRequest.FullName,
                username: userCreateRequest.UserName,
                hashPassword: hashPassword,
                saltPassword: saltPassword
            );

            _mapperMock.Setup(m => m.Map<User>(userCreateRequest)).Returns(user);
            _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(new UserResponse
            {
                Email = userCreateRequest.Email,
                UserName = userCreateRequest.UserName,
                FullName = userCreateRequest.FullName,
                HashPassword = hashPassword,
                SaltPassword = saltPassword
            });

            var userCreateService = new UserCreateHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _mapperMock.Object, _createVerifyHashMock.Object);
            var cancellationToken = new CancellationToken();

            // Act & Assert
            var response = await userCreateService.Handle(userCreateRequest, cancellationToken);

            response.Data.Email.ShouldBe(userCreateRequest.Email);
            response.Data.FullName.ShouldBe(userCreateRequest.FullName);
            response.Data.UserName.ShouldBe(userCreateRequest.UserName);
            response.Data.HashPassword.ShouldBe(hashPassword);
            response.Data.SaltPassword.ShouldBe(saltPassword);

            _userRepositoryMock.Verify(er => er.Create(It.Is<User>(u => u.Email == userCreateRequest.Email)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task InvalidUser_CreateIsCalled_ReturnsErrorResponse()
        {
            // Arrange
            var userCreateRequest = new UserCreateRequest
            {
                Email = "",
                FullName = "",
                UserName = ""
            };

            _createVerifyHashMock
                .Setup(h => h.CreateHashPassword(It.IsAny<string>(), out It.Ref<byte[]>.IsAny, out It.Ref<byte[]>.IsAny))
                .Throws(new ArgumentException("Invalid password data"));

            var userCreateService = new UserCreateHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _mapperMock.Object, _createVerifyHashMock.Object);
            var cancellationToken = new CancellationToken();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => userCreateService.Handle(userCreateRequest, cancellationToken));
            exception.Message.ShouldBe("Invalid password data");

            _userRepositoryMock.Verify(er => er.Create(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
