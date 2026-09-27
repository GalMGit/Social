namespace Social.EmailWorker.Email;

public sealed record EmailResult(
    string ToEmail, 
    string Subject, 
    string Body);