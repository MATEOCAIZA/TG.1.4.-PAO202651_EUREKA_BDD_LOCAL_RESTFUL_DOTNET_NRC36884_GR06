using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Areas.HelpPage.ModelDescriptions
{
    public class EnumTypeModelDescription : ModelDescription
    {
        public EnumTypeModelDescription()
        {
            Values = new Collection<EnumValueDescription>();
        }

        public Collection<EnumValueDescription> Values { get; private set; }
    }
}