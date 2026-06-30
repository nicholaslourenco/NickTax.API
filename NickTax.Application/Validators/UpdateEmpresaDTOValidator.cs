using FluentValidation;
using NickTax.Application.DTOs.Empresa;

namespace NickTax.Application.Validators
{
    public class UpdateEmpresaDTOValidator : AbstractValidator<UpdateEmpresaDTO>
    {
        public UpdateEmpresaDTOValidator()
        {
            RuleFor(x => x.RazaoSocial)
                .NotEmpty().WithMessage("A Razão Social é obrigatória.")
                .MaximumLength(200).WithMessage("A Razão Social deve ter no máximo 200 caracteres.");

            // Só valida o tamanho se o arquivo for enviado
            RuleFor(x => x.CertificadoDigital)
                .Must(x => x == null || x.Length > 0)
                .WithMessage("O Certificado Digital não pode ser um arquivo vazio.");

            // A senha só passa a ser obrigatória SE o certificado digital for fornecido
            RuleFor(x => x.CertificadoSenha)
                .NotEmpty()
                .When(x => x.CertificadoDigital != null && x.CertificadoDigital.Length > 0)
                .WithMessage("A senha do Certificado Digital é obrigatória ao atualizar o arquivo do certificado.");
        }
    }
}