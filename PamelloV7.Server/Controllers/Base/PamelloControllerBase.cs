using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Model;
using PamelloV7.Server.Extensions;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Repositories.Database;

namespace PamelloV7.Server.Controllers.Base
{
    public class PamelloControllerBase : ControllerBase {
        private readonly IPamelloUserRepository _users;

        protected new IPamelloUser? User;

        public PamelloControllerBase(IServiceProvider services) {
            _users = services.GetRequiredService<IPamelloUserRepository>();
        }

        [MemberNotNull(nameof(User))]
        protected void RequireUser() {
            var userStringToken = Request.Headers["user"].FirstOrDefault();
            if (userStringToken is null || userStringToken.Length == 0)
                throw new PamelloControllerException(Unauthorized("user is required to make this request"));

            if (!Guid.TryParse(userStringToken, out var userToken))
                throw new PamelloControllerException(BadRequest("invalid token format"));

            User = _users.GetByToken(userToken) ??
                throw new PamelloControllerException(BadRequest($"user \"{userToken}\" doesnt exist"));
        }

        protected void TryGetUser() {
            if (User is not null) return;

            try {
                RequireUser();
            }
            catch {
                //ignore
            }
        }
    }
}
