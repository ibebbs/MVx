# MVx - Model-View eXtensions
A suite of libraries to simplify functional, declarative and reactive implementations of the [MVVM](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel), [MVP](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93presenter) & [MVC](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller) patterns.

## MVx.Observable
A (mostly) unopinionated, light-weight alternative to ReactiveUI provided as a library _not a framework_.

### Getting Started

Install the [nuget package](https://www.nuget.org/packages/MVx.Observable/); net5.0 and .NETStandard 2.0 supported out of the box.

### Example

The example below shows an MVVM style ViewModel for a typical log in page which exhibits the following behaviour:

* When the user has entered a value in both the user name and password text boxes then enable the "Log in" button
* When the user clicks the "Log in" button, asynchronously attempt to log in with the supplied credentials.
* When a successful login attempt is made, emit a LogInSuccessful event
* When an unsuccessful login attempt is made, display an error
* When the user clicks the "Cancel" button, emit a LogInCancelled event

```c#
public record LogInSuccessful(AuthenticationResponse AuthenticationResponse) {}
public record LogInCancelled {}

public class LogInPageViewModel : INotifyPropertyChanged
{
    private readonly IAuthenticationService _authenticationService;
    private readonly MVx.Observable.IBus _eventBus;

    private readonly MVx.Observable.Property<string> _username;
    private readonly MVx.Observable.Property<string> _password;
    private readonly MVx.Observable.Property<string> _error;
    private readonly MVx.Observable.Command _logInCommand;
    private readonly MVx.Observable.Command _cancelCommand;

    private readonly Subject<AuthenticationResponse> _logInResponse;
    
    public event PropertyChangedEventHandler PropertyChanged;

    public LogInPageViewModel(IAuthenticationService authenticationService, MVx.Observable.IBus eventBus)
    {
        _authenticationService = authenticationService;
        _eventBus = eventBus;

        _username = new MVx.Observable.Property<string>(nameof(Username), args => PropertyChanged?.Invoke(this, args));
        _password = new MVx.Observable.Property<string>(nameof(Password), args => PropertyChanged?.Invoke(this, args));
        _error = new MVx.Observable.Property<string>(nameof(Error), args => PropertyChanged?.Invoke(this, args));
        _logInCommand = new MVx.Observable.Command(false);
        _cancelCommand = new MVx.Observable.Command(true);

        _logInResponse = new Subject<AuthenticationResponse>();
    }

    private IDisposable WhenTheUserHasEnteredBothUsernameAndPasswordThenEnableLogInButton()
    {
        return Observable
            .CombineLatest(_username, _password, (username, password) => !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            .Subscribe(_logInCommand);
    }

    private IDisposable WhenTheUserClicksTheLogInButtonAttemptToLogIn()
    {
        return _logInCommand
            .SelectMany(_ => Observable.CombineLatest(_username, _password, (username, password) => new AuthenticationRequest(username, password)).Take(1))
            .SelectMany(request => _authenticationService.AuthenticateAsync(request))
            .Subscribe(_logInResponse);
    }

    private IDisposable WhenASuccessfulLogInAttemptIsMadeEmitALogInSuccessfulEvent()
    {
        return _logInResponse
            .Where(response => response.Successful)
            .Select(response => new LogInSuccessful(response))
            .Subscribe(_eventBus.Publish);
    }

    private IDisposable WhenAnUnsuccessfulLogInAttemptIsMadeDisplayAnError()
    {
        return _logInResponse
            .Where(response => !response.Successful)
            .Select(response => response.Error.Message)
            .Subscribe(_error);
    }

    private IDisposable WhenTheUserClicksTheCancelButtonEmitALogInCancelledEvent()
    {
        return _cancelCommand
            .Select(_ => new LogInCancelled())
            .Subscribe(_eventBus.Publish);
    }

    public IDisposable Activate()
    {
        return new CompositeDisposable(
            WhenTheUserHasEnteredBothUsernameAndPasswordThenEnableLogInButton(),
            WhenTheUserClicksTheLogInButtonAttemptToLogIn(),
            WhenASuccessfulLogInAttemptIsMadeEmitALogInSuccessfulEvent(),
            WhenAnUnsuccessfulLogInAttemptIsMadeDisplayAnError(),
            WhenTheUserClicksTheCancelButtonEmitALogInCancelledEvent()
        );
    }

    public string Username
    {
        get { return _username.Get(); }
        set { _username.Set(value); }
    }

    public string Password
    {
        get { return _password.Get(); }
        set { _password.Set(value); }
    }

    public string Error => _error.Get();

    public ICommand LogInCommand => _logInCommand;

    public ICommand CancelCommand => _cancelCommand;
}
```

Note the following:

1. There is no shared state as all variables are readonly. State is encapsulated in the underlying `MVx.Observable.Property` instances and composed through reactive pipelines.
2. All behaviour is encapsulated in discrete "WhenXThenY" methods which can be easily modified without effecting other behaviours.
3. Behaviour is not tied to the lifetime of the view model but to the lifetime of the `IDisposable` instance returned by the `Initialize` method.
4. The boiler plate code required by `INotifyPropertyChanged` implementations is confined to a single point in the view models constructor.
5. The view model is a POCO with no untoward constraints (i.e. a base class or specific interface implementation) placed on it by MVx.Observable
