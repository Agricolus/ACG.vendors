using System;
using System.Threading;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Dto;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ADAPT.JohnDeere.handlers.Handler.Query
{
    public class GetUserTokenHandler : IRequestHandler<GetUserToken, UserToken>
    {
        private readonly JohnDeereContext db;

        public GetUserTokenHandler(JohnDeereContext db)
        {
            this.db = db;
        }

        public async Task<UserToken> Handle(GetUserToken request, CancellationToken cancellationToken)
        {
            var recordedUserToken = await (from um in db.UsersTokens where um.Id == request.UserId select um).FirstOrDefaultAsync();

            if (recordedUserToken == null)
                return null;

            return new UserToken()
            {
                AccessToken = recordedUserToken.AccessToken,
                ExpiresIn = recordedUserToken.ExpiresIn,
                ExternalUserId = recordedUserToken.ExternalId,
                RefreshToken = recordedUserToken.RefreshToken,
                RegistrationTime = recordedUserToken.RegistrationTime,
                UserId = recordedUserToken.Id
            };
        }
    }
}
