using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Application.UseCases.UserUseCases.Create;
using Project.Application.UseCases.UserUseCases.Delete;
using Project.Application.UseCases.UserUseCases.GetAll;
using Project.Application.UseCases.UserUseCases.GetById;
using Project.Application.UseCases.UserUseCases.Update;

namespace Project.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Create(UserCreateRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllUserRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetByIdUserRequest(id), cancellationToken);

            if (response == null) return NotFound();

            return Ok(response);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<UserResponse>> Update(Guid id, UserUpdateRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest();

            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid? id, CancellationToken cancellationToken)
        {
            if (id is null) return BadRequest();

            var deleteUserRequest = new DeleteUserRequest(id.Value);

            var response = await _mediator.Send(deleteUserRequest, cancellationToken);
            return Ok(response);
        }
    }
}
