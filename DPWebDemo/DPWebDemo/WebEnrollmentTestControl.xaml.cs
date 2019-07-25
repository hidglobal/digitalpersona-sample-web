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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataObject = System.Windows.DataObject;

namespace DPWebDemo
{
    /// <summary>
    /// Interaction logic for WebEnrollmentTestControl.xaml
    /// </summary>
    public partial class WebEnrollmentTestControl : UserControl
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
            typeof(WebEnrollmentTestControl), new PropertyMetadata(WebApiAddressChanged));


        private static void WebApiAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as WebEnrollmentTestControl;
            if (ctrl == null)
                return;

            ctrl.enrollmentService.ServerName = ctrl.WebApiAddress;
            ctrl.authenticationService.ServerName = ctrl.WebApiAddress;
        }

        #endregion


        private Ticket officerTicket;
        private Ticket userTicket;

        private EnrollmentService enrollmentService = new EnrollmentService();

        private AuthenticationService authenticationService = new AuthenticationService();

        public WebEnrollmentTestControl()
        {
            InitializeComponent();

            GetUserCredentialsTextBox.Text = Environment.UserDomainName + "\\" + Environment.UserName;
            GetEnrollmentDataTextBox.Text = Environment.UserDomainName + "\\" + Environment.UserName;
            AutenticateOfficerName.Text = Environment.UserDomainName + "\\" + Environment.UserName;
        }

        private async void EnrollmentPingButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var res = await enrollmentService.Ping();

                    EnrollmentPingResult.Text = res.ToString();
                }
                catch (Exception ex)
                {
                    EnrollmentPingResult.Text = ex.Message;
                }
            }
        }

        private async void GetUserCredentialsButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {

                    var user = UserName.Parse(GetUserCredentialsTextBox.Text);

                    var enrolledCreds = await enrollmentService.GetEnrolledCredentials(user);

                    GetUserCredentialsResult.Text = string.Join(Environment.NewLine, enrolledCreds.Select(p => p.ToString()));
                }
                catch (Exception ex)
                {
                    GetUserCredentialsResult.Text = ex.Message;
                }
            }
        }

        private async void GetEnrollmentDataButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var user = UserName.Parse(GetEnrollmentDataTextBox.Text);

                    var credentialType = (CredentialType)GetEnrollmentDataCredentialTypeComboBox.SelectedItem;

                    if (credentialType == CredentialType.Fingerprint)
                    {
                        var data = await enrollmentService.GetEnrollmentDataFingerprint(user);
                        GetEnrollmentDataResult.Text = string.Join(Environment.NewLine, data.Select(p => p.ToString()));
                    }
                    else if (credentialType == CredentialType.LiveQuestions)
                    {
                        var data = await enrollmentService.GetEnrollmentDataLiveQuestion(user);
                        if (data != null)
                            GetEnrollmentDataResult.Text = string.Join(Environment.NewLine, data.Select(p => p.Text));
                        else GetEnrollmentDataResult.Text = "Empty.";
                    }
                    else
                        GetEnrollmentDataResult.Text = "Not support";
                }
                catch (Exception ex)
                {
                    GetEnrollmentDataResult.Text = ex.Message;
                }
            }
        }

        private async void AutenticatOfficerButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    var user = UserName.Parse(AutenticateOfficerName.Text);
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

                    officerTicket = await authenticationService.Authenticate(user, cred);
                    AuthenticateOfficerUserResult.Text = "Offecer token accepted.";

                }
                catch (Exception ex)
                {
                    AuthenticateOfficerUserResult.Text = ex.Message;
                }
            }
        }

        private async void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    if (officerTicket == null)
                    {
                        MessageBox.Show("Authenticate officer first!");
                        return;
                    }

                    if (CreateUserPasswordTextBox.Password != ConfirmCreateUserPasswordTextBox.Password)
                    {
                        MessageBox.Show("Passsword and Confirm password must be identical.");
                        return;
                    }

                    var user = UserName.Parse(CreateUserNameTextBox.Text);

                    await enrollmentService.CreateUser(officerTicket, user, CreateUserPasswordTextBox.Password);
                    CreateUserResult.Text = "OK";
                }
                catch (Exception ex)
                {
                    CreateUserResult.Text = ex.Message;
                }
            }
        }

        private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
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

                    var user = UserName.Parse(DeleteUserNameTextBox.Text);

                    await enrollmentService.DeleteUser(officerTicket, user);

                    CreateUserResult.Text = "Ok";

                }
                catch (Exception ex)
                {
                    DeleteUserResult.Text = ex.Message;
                }
            }
        }

        private async void AutenticatUserButton_Click(object sender, RoutedEventArgs e)
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

                    userTicket = await authenticationService.Authenticate(user, cred);

                    AuthenticateUserResult.Text = "User token accepted.";
                }
                catch (Exception ex)
                {
                    AuthenticateUserResult.Text = ex.Message;
                }
            }
        }

        private async void EnrollUserCredential_Button(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    if (userTicket == null)
                    {
                        MessageBox.Show("Authenticate first!");
                        return;
                    }
                    var user = UserName.Parse(AutenticateUserName.Text);
                    var cred = EnrollCredentialsWindow.ShowDialog(Application.Current.MainWindow, user);

                    if (cred == null)
                        return;

                    await enrollmentService.EnrollCredential(officerTicket, userTicket, cred);
                    EnrollUserCredentialUserResult.Text = "Ok";
                }
                catch (Exception ex)
                {
                    EnrollUserCredentialUserResult.Text = ex.Message;
                }
            }
        }

        private async void DeleteUserCredential_Button(object sender, RoutedEventArgs e)
        {
            using (Logger.TraceMehtod())
            {
                try
                {
                    if (officerTicket == null || userTicket == null)
                    {
                        MessageBox.Show("Authenticate first!");
                        return;
                    }

                    CredentialType type = (CredentialType)DeleteCredentialTypeComboBox.SelectedValue;
                    var cred = Credential.CreateEmpty(type);
                    await enrollmentService.DeleteCredential(officerTicket, userTicket, cred);

                    DeleteUserCredentialResult.Text = "Ok";
                }
                catch (Exception ex)
                {
                    DeleteUserCredentialResult.Text = ex.Message;
                }
            }
        }

        private async void ReadAttribute_Button(object sender, RoutedEventArgs e)
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

                    var user = UserName.Parse(AttributeUserNameTextBox.Text);


                    var value = await enrollmentService.GetSingleValueUserAttribute<string>(officerTicket, user, AttributeNameTextBox.Text);

                    ReadAttributeResult.Text = string.Join(Environment.NewLine, value);
                }
                catch (Exception ex)
                {
                    ReadAttributeResult.Text = ex.Message;
                }
            }
        }

        private async void WriteAttribute_Button(object sender, RoutedEventArgs e)
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

                    var user = UserName.Parse(WriteAttributeUserNameTextBox.Text);


                    await enrollmentService.UpdateSingleValueUserAttribute(officerTicket, user, WriteAttributeNameTextBox.Text, WriteAttributeValueTextBox.Text);

                    WriteAttributeResult.Text = "Ok";
                }
                catch (Exception ex)
                {
                    WriteAttributeResult.Text = ex.Message;
                }
            }
        }

    }
}
