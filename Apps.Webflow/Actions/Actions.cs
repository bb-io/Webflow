using Apps.Webflow.Invocables;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Actions;

[ActionList]
public class Actions : WebflowInvocable
{
    public Actions(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}