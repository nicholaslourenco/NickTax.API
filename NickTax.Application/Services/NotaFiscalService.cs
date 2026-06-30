using AutoMapper;
using NickTax.Application.DTOs;
using NickTax.Application.DTOs.NotaFiscal;
using NickTax.Application.Interfaces;
using NickTax.Domain.Entities;
using NickTax.Domain.Interfaces;

namespace NickTax.Application.Services
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly INotaFiscalRepository _notaFiscalRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;
        private readonly IFocusClient _focusNfeClient;
        private readonly INfeParserService _nfeParserService;

        public NotaFiscalService(
            INotaFiscalRepository notaFiscalRepository,
            IEmpresaRepository empresaRepository,
            IMapper mapper,
            IFocusClient focusClient,
            INfeParserService nfeParserService)
        {
            _notaFiscalRepository = notaFiscalRepository;
            _empresaRepository = empresaRepository;
            _mapper = mapper;
            _focusNfeClient = focusClient;
            _nfeParserService = nfeParserService;
        }

        public async Task<SincronizacaoResultadoDTO> SincronizarNotasSefazAsync(int empresaId, Guid usuarioId)
        {
            // 1. Garante que a empresa existe e pertence ao usuário autenticado
            var empresa = await _empresaRepository.GetByIdAsync(empresaId, usuarioId);
            if (empresa == null)
            {
                throw new UnauthorizedAccessException("Acesso negado à empresa informada.");
            }

            // 2. Recupera apenas o necessário (Focus só precisa do CNPJ e do último NSU)
            string cnpjEmpresa = empresa.CNPJ;
            long ultimoNsuSalvo = empresa.UltimoNSU;

            // 3. Consulta a Focus NFe usando o cliente atualizado (passando apenas o que importa)
            await _focusNfeClient.DispararPesquisaNotasAsync(cnpjEmpresa);

            var focusResult = await _focusNfeClient.ConsultarNotasAsync(cnpjEmpresa, ultimoNsuSalvo);

            int notasNovasInseridas = 0;

            // Se o cliente reportar que a sincronização foi bem-sucedida e trouxe notas
            if (focusResult.Sucesso && focusResult.LoteXmls.Any())
            {
                foreach (var xmlNota in focusResult.LoteXmls)
                {
                    // 4. O Parser processa o XML limpo retornado pela Focus
                    var dadosParseados = _nfeParserService.ParseResumoNfe(xmlNota);

                    // 5. Evita duplicidade no banco
                    var jaExiste = await _notaFiscalRepository.ChaveAcessoExistsAsync(dadosParseados.ChaveAcesso);
                    if (!jaExiste)
                    {
                        var novaNota = new NotaFiscalEntity
                        {
                            EmpresaId = empresaId,
                            NSU = dadosParseados.NSU,
                            ChaveAcesso = dadosParseados.ChaveAcesso,
                            CnpjEmitente = dadosParseados.CnpjEmitente,
                            NomeEmitente = dadosParseados.NomeEmitente,
                            ConteudoXML = xmlNota,
                            DataEmissao = dadosParseados.DataEmissao,
                            ValorTotal = dadosParseados.ValorTotal
                        };

                        await _notaFiscalRepository.AddAsync(novaNota);
                        notasNovasInseridas++;
                    }
                }

                // 6. Atualiza o ponteiro do NSU na Empresa baseado no maior retornado pela API
                empresa.UltimoNSU = focusResult.UltimoNSU;
                _empresaRepository.Update(empresa);

                // 7. Salva as mudanças
                await _notaFiscalRepository.SaveChangesAsync();
            }

            // Mantemos o DTO de saída para não quebrar o Controller, mapeando o status de forma amigável
            return new SincronizacaoResultadoDTO
            {
                CStat = focusResult.Sucesso ? "100" : "500", // Códigos genéricos ou mensagens diretas
                XMotivo = focusResult.Mensagem,
                QuantidadeNovasNotas = notasNovasInseridas,
                NovoUltimoNSU = empresa.UltimoNSU
            };
        }

        public async Task<IEnumerable<NotaFiscalDTO>> GetAllByEmpresaAsync(int empresaId, Guid usuarioId)
        {
            var empresa = await _empresaRepository.GetByIdAsync(empresaId, usuarioId);
            if (empresa == null) throw new UnauthorizedAccessException("Acesso negado à empresa informada.");

            var notas = await _notaFiscalRepository.GetAllByEmpresaAsync(empresaId, usuarioId);
            return _mapper.Map<IEnumerable<NotaFiscalDTO>>(notas);
        }

        public async Task<NotaFiscalDTO?> GetByIdAsync(int id, Guid usuarioId)
        {
            var nota = await _notaFiscalRepository.GetByIdAsync(id, usuarioId);
            return _mapper.Map<NotaFiscalDTO>(nota);
        }

        public async Task<NotaFiscalDTO> CreateAsync(int empresaId, CreateNotaFiscalDTO dto, Guid usuarioId)
        {
            var empresa = await _empresaRepository.GetByIdAsync(empresaId, usuarioId);
            if (empresa == null) throw new UnauthorizedAccessException("Acesso negado à empresa informada.");

            var chaveExiste = await _notaFiscalRepository.ChaveAcessoExistsAsync(dto.ChaveAcesso);
            if (chaveExiste) throw new InvalidOperationException("Esta Chave de Acesso de NF-e já está cadastrada no sistema.");

            var notaFiscalEntity = _mapper.Map<NotaFiscalEntity>(dto);
            notaFiscalEntity.EmpresaId = empresaId;

            await _notaFiscalRepository.AddAsync(notaFiscalEntity);
            await _notaFiscalRepository.SaveChangesAsync();

            return _mapper.Map<NotaFiscalDTO>(notaFiscalEntity);
        }

        public async Task<bool> DeleteAsync(int id, Guid usuarioId)
        {
            var notaFiscalEntity = await _notaFiscalRepository.GetByIdAsync(id, usuarioId);
            if (notaFiscalEntity == null) return false;

            _notaFiscalRepository.Delete(notaFiscalEntity);
            await _notaFiscalRepository.SaveChangesAsync();
            return true;
        }
    }
}