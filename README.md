# Pass Global Data in Service Fabric
When you use stateful service or stateless service in service fabric platform, you may want to pass global data from client to service, this Repository supports this.
## Acknowledgments
Thanks for yoape's help from this link: 
http://stackoverflow.com/questions/41629755/passing-user-and-auditing-information-in-calls-to-reliable-services-in-service-f
## Note
1.  In Service Project AssembleInfo.cs File, you should add this line `[assembly: CustomFabricTransportServiceRemotingProvider]` at the end.
2.  In Service Implementation class, replace `return new ServiceReplicaListener[0]` with `return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) }`
## Environment:
1. Visual Studio 2017
2. .Net Core 1.0.1
