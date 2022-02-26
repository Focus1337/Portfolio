namespace Portfolio.Misc.Services.EmailSender;

public interface IEmailService
{
    void SendEmail(Message message);
}