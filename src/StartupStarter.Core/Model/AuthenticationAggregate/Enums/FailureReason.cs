namespace StartupStarter.Core.Model.AuthenticationAggregate.Enums;

public enum FailureReason
{
    InvalidCredentials,
    AccountLocked,
    AccountDisabled,
    MFAFailed
}
