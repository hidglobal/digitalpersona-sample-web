using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for SecretTestControl.xaml
    /// </summary>
    public partial class SecretTestControl : UserControl
    {

        #region WebApiAddress Dependency Property

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
            typeof(SecretTestControl), new PropertyMetadata(WebApiAddressChanged));


        private static void WebApiAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as SecretTestControl;
            if (ctrl == null)
                return;

            ctrl.authenticationService.ServerName = ctrl.WebApiAddress;
            ctrl.secretService.ServerName = ctrl.WebApiAddress;
        }

        #endregion



        private Ticket officerTicket;

        private AuthenticationService authenticationService = new AuthenticationService();

        private SecretService secretService = new SecretService();

        public SecretTestControl()
        {
            InitializeComponent();
            AutenticateOfficerUserName.Text = Environment.UserDomainName + "\\" + Environment.UserName;
        }

        private async void PingButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var res = await secretService.Ping();

                    PingResult.Text = res.ToString();
                }
                catch (Exception ex)
                {
                    PingResult.Text = ex.Message;
                }
            }
        }

        private async void ExistButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var user = UserName.Parse(ExistUserNameTextBox.Text);

                    var res = await secretService.CheckSecretExist(user, ExistSectertNameTextBox.Text);

                    ExistResult.Text = res.ToString();
                }
                catch (Exception ex)
                {
                    ExistResult.Text = ex.Message;
                }
            }
        }

        private async void AutenticatOfficerButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var cred = AuthenticateCredentialsWindow.ShowDialog(Application.Current.MainWindow);
                    var user = UserName.Parse(AutenticateOfficerUserName.Text);

                    if (cred == null)
                        return;

                    officerTicket = await authenticationService.Authenticate(user, cred);
                    AuthenticateOfficerUserResult.Text = "Token accepted.";

                }
                catch (Exception ex)
                {
                    AuthenticateOfficerUserResult.Text = ex.Message;
                }
            }
        }

        private async void ReadSecretButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {

                    if (officerTicket == null)
                    {
                        MessageBox.Show("Authenticate first!");
                        return;
                    }

                    var secret = await secretService.ReadSecret(officerTicket, ReadSecretNameTextBox.Text);
                    ReadSecretResult.Text = secret;

                }
                catch (Exception ex)
                {
                    ReadSecretResult.Text = ex.Message;
                }
            }
        }

        private async void WriteSecretButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {

                    if (officerTicket == null)
                    {
                        MessageBox.Show("Authenticate first!");
                        return;
                    }

                    await secretService.WriteSecret(officerTicket, WriteSectertNameTextBox.Text, WriteDataTextBox.Text);
                    WriteResult.Text = "ok";

                }
                catch (Exception ex)
                {
                    WriteResult.Text = ex.Message;
                }
            }
        }

        private async void DeleteSecretButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {

                    if (officerTicket == null)
                    {
                        MessageBox.Show("Authenticate first!");
                        return;
                    }

                    await secretService.DeleteSecret(officerTicket, DeleteSectertNameTextBox.Text);
                    DeleteResult.Text = "ok";

                }
                catch (Exception ex)
                {
                    DeleteResult.Text = ex.Message;
                }
            }
        }
    }
}
