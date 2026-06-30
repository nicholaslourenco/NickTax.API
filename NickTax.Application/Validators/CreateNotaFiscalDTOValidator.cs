using FluentValidation;
using NickTax.Application.DTOs;
using NickTax.Application.DTOs.NotaFiscal;

namespace NickTax.Application.Validators
{
    public class CreateNotaFiscalDTOValidator : AbstractValidator<CreateNotaFiscalDTO>
    {
        public CreateNotaFiscalDTOValidator()
        {
            RuleFor(x => x.NSU)
                .GreaterThanOrEqualTo(0).WithMessage("O NSU deve ser um número maior ou igual a zero.");

            RuleFor(x => x.ChaveAcesso)
                .NotEmpty().WithMessage("A Chave de Acesso é obrigatória.")
                .Length(44).WithMessage("A Chave de Acesso deve conter exatamente 44 caracteres.")
                .Matches(@"^\d+$").WithMessage("A Chave de Acesso deve conter apenas números.");

            RuleFor(x => x.ConteudoXML)
                .NotEmpty().WithMessage("O conteúdo XML da nota fiscal é obrigatório.");

            RuleFor(x => x.DataEmissao)
                .NotEmpty().WithMessage("A data de emissão é obrigatória.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data de emissão não pode ser uma data futura.");

            RuleFor(x => x.ValorTotal)
                .GreaterThanOrEqualTo(0).WithMessage("O valor total da nota não pode ser negativo.");
        }
    }
}