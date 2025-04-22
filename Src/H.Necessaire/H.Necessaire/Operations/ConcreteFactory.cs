using H.Necessaire.Operations.Concrete;

namespace H.Necessaire
{
    static class ConcreteFactory
    {
        public static ImAPeriodicAction BuildNewPeriodicAction() => new TaskDelayPeriodicAction();
    }
}
