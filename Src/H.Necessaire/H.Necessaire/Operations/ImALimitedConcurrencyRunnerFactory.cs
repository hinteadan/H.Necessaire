namespace H.Necessaire
{
    public interface ImALimitedConcurrencyRunnerFactory
    {
        ImALimitedConcurrencyRunner New();
        ImALimitedConcurrencyRunner New(int maxConcurrency = 150);
    }
}
