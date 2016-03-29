#Rsc.HttpClient - A wrapper library to extend normal HttpClient functionality.

## Introduction
In a microservice world, we are relying on using external Http services more and more. There are a number of things that make this more difficult than it should be - for example there is no interface for HttpClient which can make unit testing... _testing_.

Couple this with a cloud based world where failures are actually more common then we need to be able to react to and compensate for network partitions, service availability and so on. Inspired by the wonderful Polly library, I have created a number of wrappers around the basic HttpClient that add some simple retry strategies.

HttpClient is intended to be reused for multiple requests, however since it implements _IDisposable_ you often see it in _using_ statement firing off only one request. I suspect that this is mainly because it is good practice to use _using_ but also because the examples show this. Let's face it, we often crib from the examples. For a desktop application this might not make a difference (what's an extra 50ms in the scheme of things?) but at scale- for example, server applications (including web) this can be an issue.

Typically us .Net developers run and hide at the singleton pattern, but quite often use our IOC/DI system to provide that functionality. I actually think that's fine in a way, but since I don't want to force anyone to use a particular container, I have included a register interface  _IHttpClientRegister_ and accompanying class *HttpClientRegister* to be used as a singleton by your IOC/DI (perhaps in future I will create a true singleton for this)..

Developers are often an optimistic bunch (just ask any project manager about how well we estimate the time taken for a feature...) and we often act as if all external dependencies are always working in a timely manner. As such, we tend to not think about what if something fails? Or if it takes a long time? As such the classes I have created *have* to have a timeout specified- you need to think about how long you want to wait for a service to respond. By exposing the concept of retries - even if you choose not to use them- it is explicitly stating that at some point the external service _will_ fail. To paraphrase Scott Guthrie; I want developers to fall into the pit of success.

###Features:
* Fully asynchronous and threadsafe.
* **IHttpClient** - abstraction around HttpClient to aid mocking and injection.
* IHttpClient implementations all require a timeout to be set on creation - so you don't forget and use the terrible 100 second default!
* Multiple retry strategies - no retry, simple, fixed back off, exponential back off, circuit breaker.
* Retry strategies can be specified on a per request basis, or you can simply use the default for that client.
* **HttpClientRegister** - used to register clients for services, aiding reuse.


###Supported Frameworks
.Net 4.5, 4.5.1, 4.6.1
Support for .net core (or whatever it will be called) will be added once it reaches it's first full release (too many things change between releases currently).

## A quick tour

####Retry strategies and circuits

Retry strategies will use some basic logic to determine whether they should retry, and when. This decision is made on a *per-request* basis.

In contrast, a circuit breaker keeps track of all requests made through it. If the number of failures meets a certain threshold, then the circuit is considered broken (or "_open_") and no further requests are allowed to be made for a given period of time.

Circuit breakers will allow an occasional request through- to test the water and see if the service is back up. If it is, then the circuit breaker resets and allows further requests to come through. Obviously, the circuit breaker object is only useful if configures as a singleton for your application! 

####Available clients

The following clients with default retry strategies have been created for you to use:
* **NoRetryClient** - never retries, simple wrapper around HttpClient that implements IHttpClient and can therefore be easily mocked. It is also used as the base class for other implementations.
* **RetryClient** - retries _n_ times where _n_ is a number you specify in the constructor.
* **FixedBackoffRetryClient** - as RetryClient but waits _x_ milliseconds between each attempt (again, specified in the constructor)
* **ExponentialBackoffRetryClient** - as RetryClient but waits _x_ milliseconds for the second attempt, then _2x_, then _4x_ and so on.
* **CircuitBreakerClient** - a client that uses an ICircuitBreaker to determine whether it should attempt to make a request.

####Extensibility

All of the components are referenced by interface, so you should be able to extend by implementing your own versions of those interfaces. If these interfaces aren't sufficient, please consider submitting a pull request.

####Supporting structures

* **CircuitBreaker** - a configurable, threadsafe implementation of the circuit breaker pattern.
* **HttpClientRegister** - a threadsafe service register, promoting reuse of the client objects.
* **IRetryStrategy<T>** - an interface for you to implement your own strategies. You could be specific with certain Exception types, add in reporting and so on.

###Rules of thumb

Abide by these rules of thumb, and your journey will be a happy one...

* Create a client for each service you expect to call in your application. Use IHttpClientRegister to register each client.
* Set an appropriate timeout for each client. For some services you may expect a 5 second call, for others a one second call is too much.
* Set an appropriate retry count, if any. Consider using the *ExponentialBackoffClient* to not _denial of service_ the service you are calling!
* In regards to the above two, think of your _users_. Is it fair to make them wait 30 seconds for something to complete (probably not) ?
* If you use a circuit breaker, use *one* per service, and use that for the *whole application*. If you have a new circuit breaker for each web request/thread/_per instance_ you are missing out on the main benefit of a circuit breaker.
* Always check to see if AllowRequest() is true (see below) before attempting a call.

##Shut up and show us the code!

I have included some usage examples in the test library, but for those that just want to use the nuget package and don't want to pull the source code down...


####Basic use - Creating a registry

As previously mentioned, I have not forced you down the route of using any particular IOC container. Use the HttpClientRegister to register each service, and then use this as a kind of factory to get a client and use it.

```csharp
    var service1=new NoRetryClient(TimeSpan.FromSeconds(1));
    var service2 = new NoRetryClient(TimeSpan.FromSeconds(1));
    var circuit = new Retry.CircuitBreaker(new TimeService());
    var service3=new CircuitBreakerClient(TimeSpan.FromSeconds(1),circuit);
    
    // To get ultimate benefit, register the HttpClientRegister as a singleton!
    var register=new HttpClientRegister();
    register.RegisterClient("Google",service1);
    register.RegisterClient("Bing",service2);
    register.RegisterClient("DuckDuckGo",service3);

    var google=register.GetClient("Google");
    var bing=register.GetClient("Bing"); 
    var duckDuckGo=register.GetClient("DuckDuckGo");
```

####Using a client

Below is an example of getting a client from the register, checking to see if the service is allowing requests, and the using one of the HttpClient methods to get a web page as a string.

* Always check AllowRequest() first. This is most important if you're using a circuit breaker, but you could use a custom retry strategy that defines this based on another parameter- throttling, certain times of day and so on.

```csharp
    var myService=register.GetClient("MyService");
    if (myService.AllowRequest())
    {
        var webPage=await myService.GetStringAsync("http://www.myservice.com");
        //Do something with the web page
    }
    else
    {
        Debug.WriteLine("Oh no! The service isn't allowing requests so I won't even try to fetch the page!");
    }
```

####Specifying a retry stategy per request

Here we know that we want a different retry strategy than the default for this single call - perhaps if it doesn't work first time we know to give up. To do this, we simply pass in the strategy to use for this one call.

```csharp
    var myService=register.GetClient("MyService");
    if (myService.AllowRequest())
    {
        var tempStrategy=new NoRetry<string>(); 
        var webPage=await myService.GetStringAsync("http://www.myservice.com",tempStrategy);
        //Do something with the web page
    }
    else
    {
        Debug.WriteLine("Oh no! The service isn't allowing requests so I won't even try to fetch the page!");
    }
```

####Using a circuit breaker

Included is a default implementation for ICircuitBreaker. This is entirely configurable and you can always implement your own version if you wish.
The main properties you would want to configure are as follows:-

* *FailureThreshold* - The number of failures before the circuit is considered broken.
* *CircuitLockout* - The amount of time the circuit will be broken. After this period, it will allow requests through until it breaks again.
* *CircuitRetry* - The period of time before the circuit will allow a single request through to test if the circuit should be closed again. For example, if set to a minute, it will allow a single request every minute until it gets a success. On success, the circuit will reset.

A circuitbreaker should be shared where it makes sense- you may have one for Google, one for Bing and one for DuckDuckGo. It does not make sense for Bing and Google to share the same circuit. Neither does it make sense for the circuit to be short-lived- per web request, for example. On the other hand, you may well have two client instances for Google that share the same circuit.

The *TimeService* is there for unit testing's sake mainly, I don't imagine you would need any but the default other than for unit tests.
Things to note:-

* When the circuit breaks due to making a web request, it will throw a *CircuitBrokenException* - try to handle this.
* _Always_ check the AllowRequest() method first. If you don't and the circuit is already broken, you will get a *CircuitBrokenException*.

```csharp
    var timeService = new TimeService();
    _googleCircuitBreaker = new Retry.CircuitBreaker(timeService);
    _googleHttpClient = new CircuitBreakerClient(TimeSpan.FromSeconds(1),_googleCircuitBreaker);
```



##Contributing

The library isn't fully battle-tested so all bugs can be reported on the issue list. If you can, please follow up with a pull request fixing the bug!
If you want a feature, again, please submit a pull request.

On the roadmap is an extension (probably a different interface) to allow header injection per request. For example, you may want to inject a transaction ID or security token along with the request. I _suspect_ that this could be extended to change the timeout value per request too, which is possible but not simple/convenient to do with the existing HttpClient. At the same time, it would be possible to set a timeout per request as well.