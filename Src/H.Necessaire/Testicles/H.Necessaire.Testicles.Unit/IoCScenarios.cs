using FluentAssertions;
using H.Necessaire.Testicles.Unit.Model.IoC;
using System.Linq;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class IoCScenarios
    {
        readonly ImADependencyRegistry dependencyRegistry = IoC.NewDependencyRegistry();

        [Fact(DisplayName = "IoC API Can Correctly Register A Singleton Dependency Via Type")]
        public void IoC_API_Can_Correctly_Register_A_Singleton_Dependency_Via_Type()
        {
            dependencyRegistry.Register(typeof(PureDependency), () => new PureDependency());
            dependencyRegistry.Get(typeof(PureDependency)).Should().BeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The pure dependecy was registered as singleton");

            dependencyRegistry.Register(typeof(ComposedDependency), () => new ComposedDependency());
            dependencyRegistry.Get(typeof(ComposedDependency)).Should().BeSameAs(dependencyRegistry.Get(typeof(ComposedDependency)), "The composed dependecy was registered as singleton");
            (dependencyRegistry.Get(typeof(ComposedDependency)) as ComposedDependency).PureDependency.Should().BeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The composed dependecy refers the pure dependency, registred as singleton");
        }

        [Fact(DisplayName = "IoC API Can Correctly Register A Singleton Dependency Via GenericType")]
        public void IoC_API_Can_Correctly_Register_A_Singleton_Dependency_Via_GenericType()
        {
            dependencyRegistry.Register<PureDependency>(() => new PureDependency());
            dependencyRegistry.Get<PureDependency>().Should().BeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The pure dependecy was registered as singleton");

            dependencyRegistry.Register<ComposedDependency>(() => new ComposedDependency());
            dependencyRegistry.Get<ComposedDependency>().Should().BeSameAs(dependencyRegistry.Get(typeof(ComposedDependency)), "The composed dependecy was registered as singleton");
            dependencyRegistry.Get<ComposedDependency>().PureDependency.Should().BeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The composed dependecy refers the pure dependency, registred as singleton");
        }

        [Fact(DisplayName = "IoC API Can Correctly Register A Transient Dependency Via Type")]
        public void IoC_API_Can_Correctly_Register_A_Transient_Dependency_Via_Type()
        {
            dependencyRegistry.RegisterAlwaysNew(typeof(PureDependency), () => new PureDependency());
            dependencyRegistry.Get(typeof(PureDependency)).Should().NotBeNull();
            dependencyRegistry.Get(typeof(PureDependency)).Should().NotBeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The pure dependecy was registered as transient");

            dependencyRegistry.RegisterAlwaysNew(typeof(ComposedDependency), () => new ComposedDependency());
            dependencyRegistry.Get(typeof(ComposedDependency)).Should().NotBeNull();
            dependencyRegistry.Get(typeof(ComposedDependency)).Should().NotBeSameAs(dependencyRegistry.Get(typeof(ComposedDependency)), "The composed dependecy was registered as transient");
            (dependencyRegistry.Get(typeof(ComposedDependency)) as ComposedDependency).PureDependency.Should().NotBeNull();
            (dependencyRegistry.Get(typeof(ComposedDependency)) as ComposedDependency).PureDependency.Should().NotBeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The composed dependecy refers the pure dependency, registred as transient");
        }

        [Fact(DisplayName = "IoC API Can Correctly Register A Transient Dependency Via GenericType")]
        public void IoC_API_Can_Correctly_Register_A_Transient_Dependency_Via_GenericType()
        {
            dependencyRegistry.RegisterAlwaysNew<PureDependency>(() => new PureDependency());
            dependencyRegistry.Get<PureDependency>().Should().NotBeNull();
            dependencyRegistry.Get<PureDependency>().Should().NotBeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The pure dependecy was registered as transient");

            dependencyRegistry.RegisterAlwaysNew<ComposedDependency>(() => new ComposedDependency());
            dependencyRegistry.Get<ComposedDependency>().Should().NotBeNull();
            dependencyRegistry.Get<ComposedDependency>().Should().NotBeSameAs(dependencyRegistry.Get(typeof(ComposedDependency)), "The composed dependecy was registered as transient");
            dependencyRegistry.Get<ComposedDependency>().PureDependency.Should().NotBeNull();
            dependencyRegistry.Get<ComposedDependency>().PureDependency.Should().NotBeSameAs(dependencyRegistry.Get(typeof(PureDependency)), "The composed dependecy refers the pure dependency, registred as transient");
        }

        [Fact(DisplayName = "IoC API Can Correctly Unregister A Dependency Via Type")]
        public void IoC_API_Can_Correctly_Unregister_A_Dependency_Via_Type()
        {
            dependencyRegistry.Register<PureDependency>(() => new PureDependency());
            dependencyRegistry.Unregister(typeof(PureDependency));
            dependencyRegistry.Get(typeof(PureDependency)).Should().BeNull("The singleton pure dependency was unregistered");

            dependencyRegistry.RegisterAlwaysNew<PureDependency>(() => new PureDependency());
            dependencyRegistry.Unregister(typeof(PureDependency));
            dependencyRegistry.Get(typeof(PureDependency)).Should().BeNull("The transient pure dependency was unregistered");

            dependencyRegistry.Register<ComposedDependency>(() => new ComposedDependency());
            dependencyRegistry.Unregister(typeof(ComposedDependency));
            dependencyRegistry.Get(typeof(ComposedDependency)).Should().BeNull("The singleton composed dependency was unregistered");

            dependencyRegistry.RegisterAlwaysNew<ComposedDependency>(() => new ComposedDependency());
            dependencyRegistry.Unregister(typeof(ComposedDependency));
            dependencyRegistry.Get(typeof(ComposedDependency)).Should().BeNull("The transient composed dependency was unregistered");
        }

        [Fact(DisplayName = "IoC API Can Correctly Unregister A Dependency Via GenericType")]
        public void IoC_API_Can_Correctly_Unregister_A_Dependency_Via_GenericType()
        {
            dependencyRegistry.Register<PureDependency>(() => new PureDependency());
            dependencyRegistry.Unregister<PureDependency>();
            dependencyRegistry.Get(typeof(PureDependency)).Should().BeNull("The singleton pure dependency was unregistered");

            dependencyRegistry.RegisterAlwaysNew<PureDependency>(() => new PureDependency());
            dependencyRegistry.Unregister<PureDependency>();
            dependencyRegistry.Get(typeof(PureDependency)).Should().BeNull("The transient pure dependency was unregistered");

            dependencyRegistry.Register<ComposedDependency>(() => new ComposedDependency());
            dependencyRegistry.Unregister<ComposedDependency>();
            dependencyRegistry.Get(typeof(ComposedDependency)).Should().BeNull("The singleton composed dependency was unregistered");

            dependencyRegistry.RegisterAlwaysNew<ComposedDependency>(() => new ComposedDependency());
            dependencyRegistry.Unregister<ComposedDependency>();
            dependencyRegistry.Get(typeof(ComposedDependency)).Should().BeNull("The transient composed dependency was unregistered");
        }

        [Fact(DisplayName = "IoC API Can Correctly Browse Registered Dependencies")]
        public void IoC_API_Can_Correctly_Browse_Registered_Dependencies()
        {
            dependencyRegistry.Register<PureDependency>(() => new PureDependency());
            dependencyRegistry.RegisterAlwaysNew<ComposedDependency>(() => new ComposedDependency());

            dependencyRegistry.GetAllOneTimeTypes().Should().HaveCount(1, "we registred one singleton");
            dependencyRegistry.GetAllAlwaysNewTypes().Should().HaveCount(1, "we registred one transient");
            dependencyRegistry.GetAllOneTimeTypes().Single().Value().Should().Be(dependencyRegistry.Get<PureDependency>(), "singletons always have the same instance");
            dependencyRegistry.GetAllAlwaysNewTypes().Single().Value().Should().NotBeNull("calling the factory should resolve");
            dependencyRegistry.GetAllAlwaysNewTypes().Single().Value().Should().NotBe(dependencyRegistry.Get<ComposedDependency>(), "transients always resolve to new instances");
        }
    }
}
