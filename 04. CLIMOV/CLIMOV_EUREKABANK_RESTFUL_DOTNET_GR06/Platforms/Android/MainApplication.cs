using Android.App;
using Android.Runtime;

namespace CLIMOV_EUREKABANK_RESTFUL_DOTNET_GR06;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
