public class FocusQueryResult
{
    public bool Sucesso { get; private set; }
    public string Mensagem { get; private set; }
    public long UltimoNSU { get; private set; }
    public IReadOnlyCollection<string> LoteXmls { get; private set; }

    public FocusQueryResult(bool sucesso, string mensagem, long ultimoNSU, List<string> loteXmls)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
        UltimoNSU = ultimoNSU;
        LoteXmls = loteXmls.AsReadOnly();
    }
}