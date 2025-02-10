using AutoFixture;
using Microsoft.Extensions.Configuration;
using Moq;
using Project.Application.Interfaces;
using Project.Application.UseCases.AuthUseCases.ResetPassword;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AuthUseCases
{
    public class ResetPasswordUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Fixture _fixture;

        public ResetPasswordUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _configurationMock = new Mock<IConfiguration>();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handle_ValidUser_SendsResetEmail()
        {
            // Arrange
            var request = _fixture.Create<ResetPasswordRequest>();

            var cancellationToken = new CancellationToken();

            var user = new User(
                fullname: "Test User",
                username: "testuser",
                email: request.Email,
                hashPassword: new byte[32],
                saltPassword: new byte[16]
            );

            user.GeneratePasswordResetToken();

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, cancellationToken))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.Update(user)).Returns(Task.CompletedTask);
            _configurationMock.Setup(c => c["Links:PasswordResetUrl"]).Returns("https://example.com/reset-password?token=");
            _emailServiceMock.Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var handler = new ResetPasswordHandler(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object);

            // Act
            var response = await handler.Handle(request, cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Status.ShouldBeTrue();
            response.Data.ShouldBe("Password reset instructions have been sent to your email.");

            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _emailServiceMock.Verify(e => e.SendEmailAsync(user.Email, "Password Reset Request", It.Is<string>(s => s.Contains(user.PasswordResetToken))), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = _fixture.Create<ResetPasswordRequest>();

            var cancellationToken = new CancellationToken();

            _userRepositoryMock.Setup(r => r.GetByEmail(request.Email, cancellationToken))
                .ReturnsAsync((User)null);

            var handler = new ResetPasswordHandler(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => handler.Handle(request, cancellationToken));
        }
    }
}