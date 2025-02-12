using AutoFixture;
using Moq;
using Project.Application.Models;
using Project.Application.UseCases.AuthUseCases.SetNewPassword;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AuthUseCases
{
    public class SetNewPasswordUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICreateVerifyHash> _createVerifyHashMock;
        private readonly Fixture _fixture;

        public SetNewPasswordUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _createVerifyHashMock = new Mock<ICreateVerifyHash>();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handle_ValidToken_UpdatesPasswordSuccessfully()
        {
            // Arrange
            var request = _fixture.Create<SetNewPasswordRequest>();
            var cancellationToken = new CancellationToken();

            byte[] hashPassword = new byte[32];
            byte[] saltPassword = new byte[16];

            var user = new User(
                fullname: "Test User",
                username: "testuser",
                email: "test@mail.com",
                hashPassword: hashPassword,
                saltPassword: saltPassword
            );

            user.GeneratePasswordResetToken();
            request.Token = user.PasswordResetToken;

            _userRepositoryMock.Setup(r => r.GetByPasswordResetToken(request.Token, cancellationToken))
                .ReturnsAsync(user);

            _createVerifyHashMock.Setup(h => h.CreateHashPassword(
                request.NewPassword,
                out It.Ref<byte[]>.IsAny,
                out It.Ref<byte[]>.IsAny
            ));

            var handler = new SetNewPasswordHandler(_userRepositoryMock.Object, _createVerifyHashMock.Object);

            // Act
            var response = await handler.Handle(request, cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Status.ShouldBeTrue();
            response.Data.ShouldBe("Your password has been successfully updated.");
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
        }


        [Fact]
        public async Task Handle_InvalidToken_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = _fixture.Create<SetNewPasswordRequest>();

            _userRepositoryMock.Setup(r => r.GetByPasswordResetToken(request.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var handler = new SetNewPasswordHandler(_userRepositoryMock.Object, _createVerifyHashMock.Object);
            var cancellationToken = new CancellationToken();

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => handler.Handle(request, cancellationToken));
        }
    }
}
