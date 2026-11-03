using System;
using System.Reflection;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}