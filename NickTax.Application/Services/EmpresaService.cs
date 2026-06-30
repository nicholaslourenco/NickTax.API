using AutoMapper;
using NickTax.Application.DTOs.Empresa;
using NickTax.Application.Interfaces;
using NickTax.Domain.Entities;
using NickTax.Domain.Interfaces;

namespace NickTax.Application.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;
        private readonly IFocusClient _focusNfeClient;
        private readonly ICertificadoService _certificadoService;

        public EmpresaService(
            IEmpresaRepository empresaRepository,
            IMapper mapper,
            IFocusClient focusNfeClient,
            ICertificadoService certificadoService)
        {
            _empresaRepository = empresaRepository;
            _mapper = mapper;
            _focusNfeClient = focusNfeClient;
            _certificadoService = certificadoService;
        }

        public async Task<IEnumerable<EmpresaDTO>> GetAllByUserAsync(Guid usuarioId) =>
            _mapper.Map<IEnumerable<EmpresaDTO>>(await _empresaRepository.GetAllByUserAsync(usuarioId));

        public async Task<EmpresaDTO?> GetByIdAsync(int id, Guid usuarioId) =>
            _mapper.Map<EmpresaDTO>(await _empresaRepository.GetByIdAsync(id, usuarioId));

        public async Task<EmpresaDTO> CreateAsync(CreateEmpresaDTO dto, Guid usuarioId)
        {
            if (dto.CertificadoDigital == null || dto.CertificadoDigital.Length == 0)
                throw new InvalidOperationException("O certificado digital enviado é inválido ou está vazio.");

            if (string.IsNullOrWhiteSpace(dto.CertificadoSenha))
                throw new InvalidOperationException("A senha do certificado digital não pode ser vazia.");

            // Validação local do certificado
            bool certificadoValido = _certificadoService.ValidarCertificado(
                dto.CertificadoDigital, dto.CertificadoSenha, dto.CNPJ, out string msgErro);

            if (!certificadoValido)
                throw new InvalidOperationException($"Certificado inválido: {msgErro}");

            var cnpjExiste = await _empresaRepository.CnpjExistsAsync(dto.CNPJ, usuarioId);
            if (cnpjExiste)
                throw new InvalidOperationException("Este CNPJ já está cadastrado para a sua conta.");

            // 2. [NOVO] Integração com a Focus NFe
            bool cadastrouNaFocus = await _focusNfeClient.CadastrarEmpresaAsync(dto.CNPJ, dto.RazaoSocial, dto.CertificadoDigital, dto.CertificadoSenha);
            if (!cadastrouNaFocus)
                throw new InvalidOperationException("Falha ao registrar a empresa nos servidores da Focus NFe. Verifique os dados.");

            var empresaEntity = _mapper.Map<EmpresaEntity>(dto);
            empresaEntity.UsuarioId = usuarioId;
            empresaEntity.UltimoNSU = 0;

            await _empresaRepository.AddAsync(empresaEntity);
            await _empresaRepository.SaveChangesAsync();

            return _mapper.Map<EmpresaDTO>(empresaEntity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEmpresaDTO dto, Guid usuarioId)
        {
            var empresaEntity = await _empresaRepository.GetByIdAsync(id, usuarioId);
            if (empresaEntity == null) return false;

            var certificadoAntigo = empresaEntity.CertificadoDigital;
            var senhaAntiga = empresaEntity.CertificadoSenha;

            _mapper.Map(dto, empresaEntity);

            if (dto.CertificadoDigital != null && dto.CertificadoDigital.Length > 0)
            {
                if (string.IsNullOrWhiteSpace(dto.CertificadoSenha))
                    throw new InvalidOperationException("Para atualizar o certificado digital, você deve fornecer a senha correspondente.");

                // Valida o novo certificado
                if (!_certificadoService.ValidarCertificado(dto.CertificadoDigital, dto.CertificadoSenha, empresaEntity.CNPJ, out string msgErro))
                    throw new InvalidOperationException($"Novo certificado inválido: {msgErro}");

                // Update na Focus NFe
                // await _focusNfeClient.AtualizarCertificadoNaFocusAsync(empresaEntity.CNPJ, dto.CertificadoDigital, dto.CertificadoSenha);

                empresaEntity.UltimoNSU = 0; // Reseta ponteiro para buscar histórico
            }
            else
            {
                empresaEntity.CertificadoDigital = certificadoAntigo;
                empresaEntity.CertificadoSenha = senhaAntiga;
            }

            _empresaRepository.Update(empresaEntity);
            await _empresaRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, Guid usuarioId)
        {
            var empresaEntity = await _empresaRepository.GetByIdAsync(id, usuarioId);
            if (empresaEntity == null) return false;

            _empresaRepository.Delete(empresaEntity);
            await _empresaRepository.SaveChangesAsync();
            return true;
        }
    }
}