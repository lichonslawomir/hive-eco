using System;
using System.Collections.Generic;
using System.Text;
using ChartJs.Blazor.Common.Handlers;
using ChartJs.Blazor.Interop;

namespace BeeHive.Client.Shared.Helpers;

public class CustomLegendClickHandler : IMethodHandler<LegendItemMouseEvent>
{
    public string MethodName => "OnClick";
}