using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DPWebDemo.Services;

namespace DPWebDemo
{
    /// <summary>
    /// Interaction logic for WebAuthTester.xaml
    /// </summary>
    public partial class WebAuthTesterControl : UserControl
    {

        #region WebApiAddress Dependency Property
        private Ticket userTicket = null;

        /// <summary>
        /// 
        /// </summary>
        public string WebApiAddress
        {
            get { return (string)GetValue(WebApiAddressProperty); }
            set { SetValue(WebApiAddressProperty, value); }
        }

        public static readonly DependencyProperty WebApiAddressProperty =
            DependencyProperty.Register("WebApiAddress", typeof(string),
            typeof(WebAuthTesterControl), new PropertyMetadata(WebApiAddressChanged));

        #endregion

        private static void WebApiAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as WebAuthTesterControl;
            if (ctrl == null)
                return;

            ctrl.authenticationService.ServerName = ctrl.WebApiAddress;
        }

        private AuthenticationService authenticationService = new AuthenticationService();

        public WebAuthTesterControl()
        {
            InitializeComponent();
            GetUserCredentialsUserNameTextBox.Text = Environment.UserDomainName + "\\" + Environment.UserName;
            AutenticateUserName.Text = Environment.UserDomainName + "\\" + Environment.UserName;
        }

        private async void AuthenticationPingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                authenticationService.ServerName = WebApiAddress;

                var result = await authenticationService.Ping();

                PingResult.Text = result ? "Ok" : "Error";
            }
            catch (Exception ex)
            {
                PingResult.Text = ex.Message;
            }
        }

        private async void GetUserCredentialsButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var userName = UserName.Parse(GetUserCredentialsUserNameTextBox.Text);

                    var enrolledCreds = await authenticationService.GetEnrolledCredentials(userName);

                    GetUserCredentialsResult.Text = string.Join(Environment.NewLine, enrolledCreds);
                }
                catch (Exception ex)
                {
                    GetUserCredentialsResult.Text = ex.Message;
                }
            }
        }

        private async void IdentifyUserButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var cred = AuthenticateCredentialsWindow.ShowDialog(App.Current.MainWindow, true);
                    if (cred == null)
                        return;

                    userTicket = await authenticationService.Identify(cred);

                    var token = new JwtSecurityToken(userTicket.Data);
                    IdentifyUserResult.Text = token.Subject;

                }
                catch (Exception ex)
                {
                    IdentifyUserResult.Text = ex.Message;
                }
            }
        }

        private async void AutenticateUserButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var user = UserName.Parse(AutenticateUserName.Text);
                    IEnumerable<LiveQuestion> liveQuestions = null;
                    try
                    {
                        liveQuestions = await authenticationService.GetEnrollmentDataLiveQuestion(user);
                    }
                    catch (Exception ex)
                    {

                    }

                    var cred = AuthenticateCredentialsWindow.ShowDialog(Application.Current.MainWindow, liveQuestions);

                    if (cred == null)
                        return;

                    if (userTicket == null)
                        userTicket = await authenticationService.Authenticate(user, cred);
                    else
                        userTicket = await authenticationService.AuthenticateTicket(userTicket, cred);

                    AuthenticateUserResult.Text = "Ticket accepted";
                }
                catch (Exception ex)
                {
                    AuthenticateUserResult.Text = ex.Message;
                }
            }
        }

    }
}
