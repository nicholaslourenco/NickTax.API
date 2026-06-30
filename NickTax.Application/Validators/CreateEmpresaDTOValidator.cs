using FluentValidation;
using NickTax.Application.DTOs.Empresa;

namespace NickTax.Application.Validators
{
    public class CreateEmpresaDTOValidator : AbstractValidator<CreateEmpresaDTO>
    {
        public CreateEmpresaDTOValidator()
        {
            RuleFor(x => x.CNPJ)
                .NotEmpty().WithMessage("O CNPJ é obrigatório.")
                .Length(14).WithMessage("O CNPJ deve conter exatamente 14 dígitos numéricos.")
                .Matches(@"^\d+$").WithMessage("O CNPJ deve conter apenas números.");

            RuleFor(x => x.RazaoSocial)
                .NotEmpty().WithMessage("A Razão Social é obrigatória.")
                .MaximumLength(200).WithMessage("A Razão Social deve ter no máximo 200 caracteres.");

            RuleFor(x => x.CertificadoDigital)
                .NotEmpty().WithMessage("O arquivo do Certificado Digital é obrigatório.")
                .Must(x => x.Length > 0).WithMessage("O Certificado Digital não pode ser um arquivo vazio.");

            RuleFor(x => x.CertificadoSenha)
                .NotEmpty().WithMessage("A senha do Certificado Digital é obrigatória.");
        }
    }
}