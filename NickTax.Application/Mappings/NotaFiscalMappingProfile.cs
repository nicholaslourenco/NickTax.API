using AutoMapper;
using NickTax.Application.DTOs;
using NickTax.Application.DTOs.NotaFiscal;
using NickTax.Domain.Entities;

namespace NickTax.Application.Mappings
{
    public class NotaFiscalMappingProfile : Profile
    {
        public NotaFiscalMappingProfile()
        {

            CreateMap<NotaFiscalEntity, NotaFiscalDTO>();

            CreateMap<CreateNotaFiscalDTO, NotaFiscalEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Empresa, opt => opt.Ignore())
                .ForMember(dest => dest.CnpjEmitente, opt => opt.MapFrom(src => "00000000000000"))
                .ForMember(dest => dest.NomeEmitente, opt => opt.MapFrom(src => "EMISSÃO MANUAL"));
        }
    }
}