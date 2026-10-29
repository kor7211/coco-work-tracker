namespace Recorder
{
    public partial class AppShell : Shell
    {
        public bool IsPopEnabled {  get; set; } = false;
        public AppShell()
        {
            InitializeComponent();
        }

        protected override void OnNavigating(
        ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (IsPopEnabled)
                return;

            if (args.Source is ShellNavigationSource.Pop
                or ShellNavigationSource.PopToRoot)
            {
                args.Cancel();
            }
        }
    }
}
