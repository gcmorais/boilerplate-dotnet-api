using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Models;
using Project.Application.UseCases.AuthUseCases.Enable2FA;
using Project.Application.UseCases.AuthUseCases.Login;
using Project.Application.UseCases.AuthUseCases.RefreshTokens;
using Project.Application.UseCases.AuthUseCases.SetNewPassword;
using Project.Application.UseCases.AuthUseCases.Verify2FA;

namespace Project.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RefreshTokenCommand
            {
                RefreshToken = request.Token,
            }, cancellationToken);

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ResponseModel<string>>> ForgotPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ResponseModel<string>>> ResetPassword([FromBody] SetNewPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("verify-2fa")]
        public async Task<ActionResult<LoginResponse>> VerifyTwoFactor(VerifyTwoFactorRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("enable-2fa")]
        public async Task<ActionResult<ResponseModel<string>>> EnableTwoFactorAuthentication(EnableTwoFactorRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(request, cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while enabling 2FA.");
            }
        }
    }
}
