using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DPWebDemo.Interop.Cards;
using DPWebDemo.Interop.Fingerprint;
using DPWebDemo.Services;
using DPWebDemo.Services.Biometric;

namespace DPWebDemo
{
    /// <summary>
    /// Interaction logic for CredentialsWindow.xaml
    /// </summary>
    public partial class AuthenticateCredentialsWindow : Window
    {
        public IEnumerable<Tuple<LiveQuestion, LiveAnswer>> LiveQuestion { get; set; }

        private IAuthenticationCredential credential;

        private FingerprintEngine fingerprintEngine;

        private FingerprintImage fingerprintImage;

        private CardEngine cardEngine;

        private AuthenticateCredentialsWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Password

        private void PasswordOkButton_Click(object sender, RoutedEventArgs e)
        {
            credential = Credential.Create(new PasswordAuthenticationData(PasswordPasswordTextBox.Password));
            DialogResult = true;
        }

        #endregion

        #region Fingerprint

        protected override void OnSourceInitialized(EventArgs e)
        {
            var source = PresentationSource.FromVisual(this) as HwndSource;

            if (source != null)
            {
                fingerprintEngine.Initialize(source.Handle);
                fingerprintEngine.SampleCollected += FingerprintEngineSampleCollected;
                source.AddHook(WndProc);
            }

            base.OnSourceInitialized(e);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            fingerprintEngine.ProcessSample(hwnd, msg, wParam, lParam, ref handled);
            return IntPtr.Zero;
        }

        private void FingerprintEngineSampleCollected(object sender, ImageCapturedEventArgs e)
        {

            if (fingerprintImage != null)
                fingerprintImage.Dispose();

            fingerprintImage = e.FingerprintImage;

            var writableBitmap = new WriteableBitmap(e.FingerprintImage.Width, e.FingerprintImage.Height, e.FingerprintImage.DpiX, e.FingerprintImage.DpiY, PixelFormats.Gray8, BitmapPalettes.Gray256);
            writableBitmap.Lock();

            writableBitmap.WritePixels(new Int32Rect(0, 0, e.FingerprintImage.Width, e.FingerprintImage.Height), e.FingerprintImage.ImageData, e.FingerprintImage.Width * e.FingerprintImage.BitPerPixel / 8, 0);

            writableBitmap.Unlock();

            FingerprintImage.Source = writableBitmap;

        }

        private void FingerprintStartCapture_Click(object sender, RoutedEventArgs e)
        {
            StartFingerprintButton.IsEnabled = false;
            StopFingerprintButton.IsEnabled = true;
            fingerprintEngine.Start();
        }

        private void FingerprintStopCapture_Click(object sender, RoutedEventArgs e)
        {
            StartFingerprintButton.IsEnabled = true;
            StopFingerprintButton.IsEnabled = false;
            fingerprintEngine.Stop();
        }

        private void FingerprintOkButton_Click(object sender, RoutedEventArgs e)
        {
            if (fingerprintImage == null)
                return;

            using (var ffe = new FingerprintFeatureExtractor())
            {
                ffe.Initialize();
                var featuries = ffe.Extract(fingerprintImage, ExtractionType.FeatureSetForAuthentication);

                var fpFeatures = new BiometricSample(featuries, BiometricSampleHeaderType.DigitalPersonaFingerprintFeatureSet);

                credential = Credential.Create(new FingerprintAuthenticationData(fpFeatures));
            }

            DialogResult = true;
            Close();
        }

        #endregion

        #region PIN

        private void PinOkButton_Click(object sender, RoutedEventArgs e)
        {
            credential = Credential.Create(new PinAuthenticationData(PinPasswordTextBox.Text));
            DialogResult = true;
        }
        #endregion

        #region Live question

        private void LiveQuestionOkButton_Click(object sender, RoutedEventArgs e)
        {
            credential = Credential.Create(new LiveQuestionAuthenticationData(LiveQuestion.Select(p => p.Item2)));
            DialogResult = true;
        }

        #endregion

        #region Cards

        private void RefreshCardsButton_Click(object sender, RoutedEventArgs e)
        {
            cardEngine.Initialize();

            CardsListBox.ItemsSource = null;

            var cards = new List<Tuple<string, Card>>();

            var readers = cardEngine.GetReaders();
            if (readers == null)
            {
                CardsListBox.ItemsSource = null;
                return;
            }

            foreach (var readerName in readers)
            {
                var card = cardEngine.GetCard(readerName);

                if (card.Name == null)
                    continue;

                if(card.CardType == CardType.Smartcard || card.CardType == CardType.None)
                    continue;

                cards.Add(new Tuple<string, Card>(readerName, card));
            }

            CardsListBox.ItemsSource = cards;
        }

        private void CardOkButton_Click(object sender, RoutedEventArgs e)
        {
            var card = CardsListBox.SelectedItem as Tuple<string, Card>;
            if (card == null)
                return;

            credential = Credential.Create(new CardAuthenticationData(card.Item2.Id));
            DialogResult = true;
        }

        #endregion

        #region OTP

        private void OtpOkButton_Click(object sender, RoutedEventArgs e)
        {
            credential = Credential.Create(new OtpAuthenticationData(OtpTextBox.Text));
            DialogResult = true;
        }

        #endregion

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static IAuthenticationCredential ShowDialog(Window owner, IEnumerable<LiveQuestion> liveQuestions, bool isAuthenticationMode)
        {
            ICredential credential = null;
            var win = new AuthenticateCredentialsWindow();
            
            if (liveQuestions != null)
                win.LiveQuestion = liveQuestions.Select(p => new Tuple<LiveQuestion, LiveAnswer>(p, new LiveAnswer(p.Number))).ToArray();
            else
                win.LiveQuestionsTabItem.Visibility = Visibility.Collapsed;

            if (isAuthenticationMode)
            {
                win.PassworTabItem.Visibility = Visibility.Collapsed;
                win.PinTabItem.Visibility = Visibility.Collapsed;
                win.OtpTabItem.Visibility = Visibility.Collapsed;

            }

            win.Owner = owner;

            using (win.fingerprintEngine = new FingerprintEngine())
            using (win.fingerprintImage)
            using (win.cardEngine = new CardEngine())
            {
                if (win.ShowDialog() == true)
                    return win.credential;
                return null;
            }

        }

        public static IAuthenticationCredential ShowDialog(Window owner)
        {
            return ShowDialog(owner, null, false);
        }

        public static IAuthenticationCredential ShowDialog(Window owner, IEnumerable<LiveQuestion> liveQuestions)
        {
            return ShowDialog(owner, liveQuestions, false);
        }

        public static IAuthenticationCredential ShowDialog(Window owner, bool isAuthenticationMode)
        {
            return ShowDialog(owner, null, isAuthenticationMode);
        }

    }
}
