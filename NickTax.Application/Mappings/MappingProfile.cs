using AutoMapper;
using NickTax.Domain.Entities;
using NickTax.Application.DTOs;

namespace NickTax.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<UsuarioEntity, UsuarioPerfilResponse>();

        // Para outros DTOs futuramente
        // CreateMap<EmpresaEntity, EmpresaResponse>();
    }
}