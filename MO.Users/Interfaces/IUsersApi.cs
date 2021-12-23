using Microsoft.AspNetCore.Mvc;
using MO.Auth.Data;

namespace MO.Auth.Interfaces;

public interface IUsersApi
{
    const string GetByIdEndpoint = "{id}";
    Task<ActionResult<GetByIdResponse>> GetById(Guid id);
    record GetByIdResponse(UserResponse UserResponse);


    const string GetByUsernameEndpoint = "GetByUsername";
    Task<ActionResult<GetByUsernameResponse>> GetByUsername(string username);
    record GetByUsernameResponse(UserResponse UserResponse);

    const string GetAllUsersEndpoint = "getAll";
    Task<ActionResult<GetAllUsersResponse>> GetAllUsers();
    record GetAllUsersResponse(List<UserResponse> Users);


    class UserResponse
    {
        public UserResponse() { }
        public UserResponse(Guid UserId, string Username, string Email, bool EmailConfirmed)
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

        public void Deconstruct(out Guid UserId, out string Username, out string Email, out bool EmailConfirmed)
        {
            UserId = this.UserId;
            Username = this.Username;
            Email = this.Email;
            EmailConfirmed = this.EmailConfirmed;
        }
    }
}