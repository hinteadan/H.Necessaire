# Create a H MAUI Control

## Super Basic

```csharp
public class MyControl : HMauiComponentBase
{
	protected override View ConstructDefaultContent()
	{
		//Construct the view here
	}
}
```

OR if you can't extend `HMauiComponentBase`, you can use composition with `HUiToolkit.Current` to access the stuff that `HMauiComponentBase` would give you

```csharp
internal class HButton : Button
    {
        public HButton()
        {
            FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily;
            FontSize = HUiToolkit.Current.Branding.Typography.FontSize;
            BackgroundColor = HUiToolkit.Current.Branding.PrimaryColor.ToMaui();
            TextColor = HUiToolkit.Current.Branding.BackgroundColor.ToMaui();
        }
    }
```

## With custom deps (e.g. dependencies that are needed for ConstructDefaultContent)

```csharp
public class MyControl : HMauiComponentBase
{
	protected MyControl(object arg) : base(arg){}

	protected override void EnsureDependencies(params object[] constructionArgs)
    {
        base.EnsureDependencies(constructionArgs);

        hUIComponent = constructionArgs[0] as ImAnHUIComponent;
        propertyComponentBuilder = Get<ImAHMauiPropertyComponentBuilder>();
		//etc.
    }
}

```

## For an H UI runtime component (aka with ViewModel)

### Create the data structure for the view model 
```csharp
public class LoginCommand
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

### Create and register The Core H UI Component w/ ViewModel for the data structure

```csharp
[ID("login")]
public class HUILoginComponent : HUIComponentBase
{
    public HUILoginComponent() : base(new LoginCommand().ToHViewModel())
    {
    }

    public HUILoginComponent(string title) : base(new LoginCommand().ToHViewModel(title))
    {
    }
    public HUILoginComponent(string title, string description) : base(new LoginCommand().ToHViewModel(title, description))
    {
    }

    public HUILoginComponent(string title, string description, params Note[] notes) : base(new LoginCommand().ToHViewModel(title, description, notes))
    {
    }
}

internal class DependencyGroup : ImADependencyGroup
{
    public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
    {
        dependencyRegistry

            .RegisterAlwaysNew<HUILoginComponent>(() => new HUILoginComponent("Login"))

            ;
    }
}

```

### Create the View Components

```csharp
class MyControl : HMauiHUIComponentBase
{
	public MyControl(ImAnHUIComponent hUIComponent) : base(hUIComponent) { }

	//Rest is same as before
}

//OR

class MyControl : HMauiHUIGenericComponent<ConcreteHUIComponent> //where THUIComponent : ImAnHUIComponent
{
	public MyControl(ConcreteHUIComponent hUIComponent) : base(hUIComponent) { }

	//Rest is same as before
}
```

BUT this can also be extented by implementing-or-extending custom
 - `ImAHMauiHUIComponentBuilder` (with base `HMauiHUIComponentBuilderBase`)
 - `ImAHMauiHUIPropertyComponentBuilder` (with base `HMauiHUIPropertyComponentBuilderBase`)

 and register the custom implements in the DI Container

 ```csharp
class DependencyGroup : ImADependencyGroup
{
    public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
    {
        dependencyRegistry
    
            .RegisterAlwaysNew<HMauiHUIGenericComponent<HUILoginComponent>>(() => dependencyRegistry.Get<HUILoginComponent>().ToHMauiComponent())
    
            ;
    }
}
 ```

### Use it

In any View that extends `HMauiComponentBase` or via `HUiToolkit.Current`

```csharp
View loginComponent = Get<HMauiHUIGenericComponent<HUILoginComponent>>();
new VerticalStackLayout().And(layout =>
{
    layout.Add(loginComponent);
});
```