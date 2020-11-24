using System;
using System.Threading;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.handlers.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class CreateOrUpdateUserRegistrationHandler : IRequestHandler<CreateOrUpdateUserRegistration, bool>
    {
        private readonly JohnDeereContext db;
        private readonly IMediator mediator;

        public CreateOrUpdateUserRegistrationHandler(JohnDeereContext db, IMediator mediator)
        {
            this.db = db;
            this.mediator = mediator;
        }


        public async Task<bool> Handle(CreateOrUpdateUserRegistration request, CancellationToken cancellationToken)
        {
            var recordedUserToken = await (from um in db.UsersTokens where um.Id == request.UserId select um).FirstOrDefaultAsync();
            if (recordedUserToken != null) {
                var expired = (DateTime.UtcNow - recordedUserToken.RegistrationTime).TotalSeconds > recordedUserToken.ExpiresIn ? true : false;
                if (expired) return false;
            }

            recordedUserToken = new UserToken()
            {
                ExternalId = request.ExternalUserId,
                Id = request.UserId,
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken,
                ExpiresIn = request.ExpiresIn,
                RegistrationTime = DateTime.UtcNow
            };

            db.UsersTokens.Add(recordedUserToken);
            await db.SaveChangesAsync();

            return true;
        }

    }
}
