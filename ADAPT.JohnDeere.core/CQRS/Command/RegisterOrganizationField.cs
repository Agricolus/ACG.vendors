using System;
using ACG.Common.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class RegisterOrganizationField : IRequest<Field>
    {
        public Dto.JohnDeereApiResponse.Field Field { get; set; }
    }
}
