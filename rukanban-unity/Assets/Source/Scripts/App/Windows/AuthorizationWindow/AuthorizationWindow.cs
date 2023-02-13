namespace RuKanban.App.Window
{
    public class AuthorizationWindow : BaseAppWindow
    {
        public LoginPanel loginPanel;
        public RegistrationPanel registrationPanel;

        protected override void HideWindow(bool force)
        {
            ResetElements();
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            loginPanel.ResetElements();
            registrationPanel.ResetElements();
        }
    }
}