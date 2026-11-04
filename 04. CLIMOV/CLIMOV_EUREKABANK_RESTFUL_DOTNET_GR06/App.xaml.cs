using Microsoft.Extensions.DependencyInjection;
using EurekaMovil;

namespace CLIMOV_EUREKABANK_RESTFUL_DOTNET_GR06;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new NavigationPage(new LoginPage()));
	}
}