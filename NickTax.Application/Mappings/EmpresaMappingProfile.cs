using AutoMapper;
using NickTax.Application.DTOs.Empresa;
using NickTax.Domain.Entities;

namespace NickTax.Application.Mappings
{
    public class EmpresaMappingProfile : Profile
    {
        public EmpresaMappingProfile()
        {

            CreateMap<EmpresaEntity, EmpresaDTO>();

            CreateMap<CreateEmpresaDTO, EmpresaEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.UltimoNSU, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.NotasFiscais, opt => opt.Ignore());

            CreateMap<UpdateEmpresaDTO, EmpresaEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CNPJ, opt => opt.Ignore()) // CNPJ não se altera na atualização
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.UltimoNSU, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.NotasFiscais, opt => opt.Ignore());
        }
    }
}