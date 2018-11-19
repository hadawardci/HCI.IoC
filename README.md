# HCI.IoC
![](https://img.shields.io/nuget/v/HCI.IoC.svg)

## Features
HCI.IoC is a [NuGet library](https://www.nuget.org/api/v2/package/HCI.IoC) to help you perform the inversion of control with Simple Injector easily automatically.

## Getting started
The easiest way to get started is by [installing the available NuGet packages](https://www.nuget.org/packages/HCI.IoC) and if you're not a NuGet fan then follow these steps:

Download the latest runtime library from: https://www.nuget.org/api/v2/package/HCI.IoC.
Or install the latest package:
PM> **Install-Package HCI.IoC**

## A Quick Example

### Register services on container
Take advantage through the static class ```Register```.

#### Register services for an namespace
- whenever the prefix is not declared, "I" will be adopted by default;

> Return a new container:
```csharp
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
 var container = Register.RegisterNamespace<Foo>(lifestyle, patternSuffix);
```

------------

> Register services into a existing container:
```csharp
var container = Activator.CreateInstance<Container>();
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
 var container = Register.RegisterNamespace<Foo>(container, lifestyle, patternSuffix);
```

------------

> Register services into a existing container declaring service prefix:
```csharp
var container = Activator.CreateInstance<Container>();
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
string servicePrefix = "I";
 var container = Register.RegisterNamespace<Foo>(container, lifestyle, patternSuffix, servicePrefix);
```

#### Register services for an assembly
- whenever the prefix is not declared, "I" will be adopted by default;

> Return a new container:
```csharp
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
 var container = Register.RegisterAssembly<Foo>(lifestyle, patternSuffix);
```

------------

> Register services into a existing container:
```csharp
var container = Activator.CreateInstance<Container>();
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
 var container = Register.RegisterAssembly<Foo>(container, lifestyle, patternSuffix);
```

------------

> Register services into a existing container declaring service prefix:
```csharp
var container = Activator.CreateInstance<Container>();
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
string servicePrefix = "I";
 var container = Register.RegisterAssembly<Foo>(container, lifestyle, patternSuffix, servicePrefix);
```

------------

> Register services into a existing container declaring service prefix and service types to be ignored:
```csharp
var container = Activator.CreateInstance<Container>();
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
string servicePrefix = "I";
var notWorking = typeof(IBar);
 var container = Register.RegisterAssembly<Foo>(container, lifestyle, patternSuffix, servicePrefix, notWorking);
```

#### Register service factory
- TFactory (Dictionary): must be represented by the concrete type of your service factory;
- TService: Represents a generic contract type inherited by all services from your factory.
> Return factory with service deployments:
```csharp
var container = Activator.CreateInstance<Container>();
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
var factory = Register.RegisterFactory<Factory, IRepository>(container, lifestyle, patternSuffix, ProductNameCallback);
container.RegisterInstance<IFactory>(factory);
```

------------

> Return factory with service deployments allowing to ignore certain types through callback:
```csharp
var container = Activator.CreateInstance<Container>();
Lifestyle lifestyle = Lifestyle.Singleton;
string patternSuffix = "Repository";
var factory = Register.RegisterFactory<Factory, IRepository>(container, lifestyle, patternSuffix, ProductNameCallback);
container.RegisterInstance<IFactory>(factory);
 ...
 private static string ProductNameCallback(Type type)
{
	var name = type.Name.Replace("Repository", string.Empty);
	var project = type.Namespace.split(' ')[0];
	var key = $"{project}.{name}".ToUpper();
	return key;
}
```

## Report Support
To report errors, questions and suggestions go to the [link](https://www.nuget.org/packages/HCI.IoC/1.0.0/ReportMyPackage)
