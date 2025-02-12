using AutoMapper;
using Moq;
using Project.Application.UseCases.AdminUseCases.BanUser;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Shouldly;

namespace Application.UnitTests.UseCases.AdminUseCases
{
    public class BanUserUnitTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly BanUserHandler _handler;

        public BanUserUnitTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<BanUserMapper>());
            _mapper = mapperConfig.CreateMapper();

            _handler = new BanUserHandler(_userRepositoryMock.Object, _mapper, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_UserExists_BansUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var banUntil = DateTimeOffset.UtcNow.AddDays(7);
            var request = new BanUserRequest { UserId = userId, BannedUntil = banUntil };

            var user = new User("Test User", "testuser", "test@mail.com", new byte[32], new byte[16])
            {
                Id = userId,
            };

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
            response.IsBanned.ShouldBeTrue();
            response.BannedUntil.ShouldBe(banUntil);

            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new BanUserRequest { UserId = userId, BannedUntil = DateTimeOffset.UtcNow.AddDays(7) };

            _userRepositoryMock.Setup(r => r.GetById(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(() => _handler.Handle(request, CancellationToken.None));

            _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
