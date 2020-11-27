using ACG.Common.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class RegisterMachine : IRequest<Machine>
    {

        public Machine Machine { get; set; }
    }
}
