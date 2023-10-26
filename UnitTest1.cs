using System.Diagnostics;
using System.Windows.Automation;
namespace BrokenAutomation
{


    public class Setting
    {
        public string ApplicationDisplayName { get; set; } = "WinUIGallery";
        public TimeSpan ApplicationStartWait { get; set; } = TimeSpan.FromSeconds(50);
    }

    public class RepoTest
    {
        public AutomationElement WindowRootElement { get; protected set; }
        private Process _applicationProcess;
        private Setting _setting = new();
        public RepoTest()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            while (null == _applicationProcess && stopwatch.Elapsed < _setting.ApplicationStartWait)
            {
                Thread.Sleep(100);
                _applicationProcess = Process.GetProcessesByName(_setting.ApplicationDisplayName).SingleOrDefault();
            }


            _applicationProcess.WaitForInputIdle((int)(_setting.ApplicationStartWait.TotalMilliseconds - stopwatch.ElapsedMilliseconds));

            do
            {
                Thread.Sleep(100);

                if (IntPtr.Zero != _applicationProcess.MainWindowHandle)
                    WindowRootElement = AutomationElement.FromHandle(_applicationProcess.MainWindowHandle);

            }
            while (null == WindowRootElement && stopwatch.Elapsed < _setting.ApplicationStartWait);

        }

        [Theory]
        [InlineData("TextBox")]
        void When_combobox_is_opened_Expect_find_method_fails(string controlId)
        {
            var element = WindowRootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, controlId));
            Assert.True(element != null);
        }
    }
}
