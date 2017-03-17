# Pass Global Data in Service Fabric

When you use stateful service or stateless service in service fabric platform, you may want to pass global data from client to service, this Repository supports this.

## Note

1.  In Service Project AssembleInfo.cs File, you should add this line `[assembly: CustomFabricTransportServiceRemotingProvider]` at the end.
2.  In Service Implementation class, replace `return new ServiceReplicaListener[0]` with `return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) }`

## Environment:

1. Visual Studio 2017
2. .Net Core 1.0.1

## Acknowledgments

Thanks for yoape's help from this link: 
http://stackoverflow.com/questions/41629755/passing-user-and-auditing-information-in-calls-to-reliable-services-in-service-f

## Change Log

###v0.1.1.020170317

1. Add WebApi Project
2. Support Https in ASP.Net Core and WebApi(Use Default IIS Express Development Certificate)
