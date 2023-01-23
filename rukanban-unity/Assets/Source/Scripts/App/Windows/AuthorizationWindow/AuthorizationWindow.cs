namespace RuKanban.App.Window
{
    public class AuthorizationWindow : BaseAppWindow
    {
        public SignInTab signInTab;
        public SignUpTab signUpTab;

        protected override void HideWindow(bool force = false)
        {
            ResetElements();
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            signInTab.ResetElements();
            signUpTab.ResetElements();
        }
    }
}