using Microsoft.AspNetCore.Mvc;
using MO.Auth.Data;

namespace MO.Auth.Interfaces;

public interface IUsersApi
{
    const string GetByIdEndpoint = "GetById";
    Task<ActionResult<GetByIdResponse>> GetById(Guid id);
    record GetByIdResponse(UserDto UserDto);

    const string GetByUsernameEndpoint = "GetByUsername";
    Task<ActionResult<GetByUsernameResponse>> GetByUsername(string username);
    record GetByUsernameResponse(UserDto UserDto);

    const string GetAllUsersEndpoint = "GetAll";
    Task<ActionResult<GetAllUsersResponse>> GetAllUsers();
    record GetAllUsersResponse(List<UserDto> Users);


    class UserDto
    {
        public UserDto() { }
        public UserDto(Guid UserId, string Username, string Email, bool EmailConfirmed)
        {
            this.UserId = UserId;
            this.Username = Username;
            this.Email = Email;
            this.EmailConfirmed = EmailConfirmed;
        }

        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}