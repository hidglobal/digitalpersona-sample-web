using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
using Base32;
using DPWebDemo.Interop.Cards;
using DPWebDemo.Interop.Fingerprint;
using DPWebDemo.Services;
using DPWebDemo.Services.Biometric;
using DPWebDemo.Services.Fingerprints;
using MessagingToolkit.QRCode.Codec;
using Microsoft.Win32;
using FingerprintImage = DPWebDemo.Interop.Fingerprint.FingerprintImage;

namespace DPWebDemo
{
    /// <summary>
    /// Interaction logic for EnrollCredentialsWindow.xaml
    /// </summary>
    public partial class EnrollCredentialsWindow : Window
    {
        public List<LiveQuestion> LiveQuestions { get; set; }

        public List<string> LiveAnswers { get; set; }

        public List<LiveQuestion> SelectedQuestions { get; set; }

        private readonly UserName userName;

        private CardEngine cardEngine;

        private IEnrollmentCredential credential;

        private FingerprintEngine fingerprintEngine;

        private FingerprintImage fingerprintImage;

        private List<BiometricSample> fingerprintEnrollmentData = new List<BiometricSample>();

        private byte[] otpEnrollmentKey;

        private Bitmap qrCode;


        private EnrollCredentialsWindow(UserName userName)
        {
            this.userName = userName;
            InitializeComponent();
            InitiailizeOtp();
            InitializeLiveQuestions();
            InitializeFingerprints();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #region Password

        private void PasswordOkButton_Click(object sender, RoutedEventArgs e)
        {
            credential = Credential.Create(new PasswordEnrollmentData(OldPasswordTextBox.Password, NewPasswordTextBox.Password));

            DialogResult = true;
            Close();
        }

        #endregion

        #region PIN

        private void PinOkButton_Click(object sender, RoutedEventArgs e)
        {
            credential = Credential.Create(new PinEnrollmentData(PinTextBox.Text));
            DialogResult = true;
            Close();
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

            var writableBitmap = new WriteableBitmap(fingerprintImage.Width, fingerprintImage.Height, fingerprintImage.DpiX, fingerprintImage.DpiY, PixelFormats.Gray8, BitmapPalettes.Gray256);
            writableBitmap.Lock();

            writableBitmap.WritePixels(new Int32Rect(0, 0, fingerprintImage.Width, fingerprintImage.Height), fingerprintImage.ImageData, fingerprintImage.Width * fingerprintImage.BitPerPixel / 8, 0);

            writableBitmap.Unlock();

            FingerprintImage.Source = writableBitmap;

            //var fpImage = new Services.Fingerprints.FingerprintImage();
            //fpImage.Header.DeviceType = 49264417347272704;
            //fpImage.Format.Bpp = PixelFormats.Gray8.BitsPerPixel;
            //fpImage.Format.ColorRepresentation = FingerprintImageColorRepresantation.NoColorRepresentation;
            //fpImage.Format.Height = fingerprintImage.Height;
            //fpImage.Format.Width = fingerprintImage.Width;
            //fpImage.Format.DpiX = fingerprintImage.DpiX;
            //fpImage.Format.DpiY = fingerprintImage.DpiY;
            //fpImage.Format.ImageType = FingerprintImageType.GrayScale;
            //fpImage.Format.Padding = FingerprintImagePadding.RightPadding;
            //fpImage.Format.Polarity = FingerprintImagePolarity.PositivePolarity;
            //fpImage.Format.SignificantBpp = 8;
            //fpImage.Format.Planes = 1;
            //fpImage.Data = fingerprintImage.ImageData;
            //fingerprintEnrollmentData.Add(new BiometricSample(fpImage,
            //    BiometricSampleHeaderType.DigitalPersonaFingerprintImage));
            using (var fe = new FingerprintFeatureExtractor())
            {
                fe.Initialize();
                var featuries = fe.Extract(fingerprintImage, ExtractionType.FeatureSetForEnrollment);


                fingerprintEnrollmentData.Add(new BiometricSample(featuries,
                    BiometricSampleHeaderType.DigitalPersonaFingerprintFeatureSet));
            }
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

            FingerPosition position = FingerPosition.Unknown;
            if(FingerprintPosition.SelectedIndex >= 0)
            {
                position = (FingerPosition)(FingerprintPosition.SelectedIndex + 1);
            }
            credential = Credential.Create(new FingerprintEnrollmentData(position, fingerprintEnrollmentData));

            DialogResult = true;
            Close();
        }

        private void InitializeFingerprints()
        {
            FingerprintPosition.Items.Add("Right thumb finger");
            FingerprintPosition.Items.Add("Right index finger");
            FingerprintPosition.Items.Add("Right middle finger");
            FingerprintPosition.Items.Add("Right ring finger");
            FingerprintPosition.Items.Add("Right little finger");
            FingerprintPosition.Items.Add("Left thumb finger");
            FingerprintPosition.Items.Add("Left index finger");
            FingerprintPosition.Items.Add("Left middle finger");
            FingerprintPosition.Items.Add("Left ring finger");
            FingerprintPosition.Items.Add("Left little finger");
            FingerprintPosition.SelectedIndex = 1;
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

                if (card.CardType == CardType.Smartcard || card.CardType == CardType.None)
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

            credential = Credential.Create(new CardEnrollmentData(card.Item2.Id));
            DialogResult = true;
        }

        #endregion

        #region OTP

        private void InitiailizeOtp()
        {
            otpEnrollmentKey = new byte[20];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
                randomNumberGenerator.GetBytes(otpEnrollmentKey);


            var encodedKey = Base32Encoder.Encode(otpEnrollmentKey);
            var enrollmentUrl = String.Format("otpauth://totp/{0}?secret={1}&issuer=Altus", userName.Name, encodedKey);

            var qrEncoder = new QRCodeEncoder
            {
                QRCodeBackgroundColor = System.Drawing.Color.Transparent,
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H,
                QRCodeForegroundColor = System.Drawing.Color.Black
            };

            qrCode = qrEncoder.Encode(enrollmentUrl);

            OtpImage.Source = Imaging.CreateBitmapSourceFromHBitmap(qrCode.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

        }


        private void OtpOkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(OneTimePasswordTextBox.Text))
                return;

            credential = Credential.Create(new OtpEnrollmentData(OneTimePasswordTextBox.Text, otpEnrollmentKey));
            DialogResult = true;
        }

        #endregion

        #region LiveQuestions

        private void InitializeLiveQuestions()
        {
            LiveAnswers = new List<string> { "", "", "" };

            SelectedQuestions = new List<LiveQuestion>
            {
                null, null, new LiveQuestion()
                {
                     KeyboardLayoutId = 67699721,//EN-us
                     LanguageId = 9,//EN
                     SublanguageId = 1
                } 
            };

            LiveQuestions = new List<LiveQuestion>();

            for (byte i = 1; i <= 10; i++)
            {
                var question = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Policies\DigitalPersona\Altus\SpareKey", "Question" + i, 1);
                if (question == null || question.Equals(1))
                {
                    LiveQuestions.Add(new LiveQuestion(i)
                    {
                        KeyboardLayoutId = 1033,//EN-us
                        LanguageId = 9,//EN
                        SublanguageId = 1,//us
                        Text = "Question " + i //we have only question number :(
                    });
                }
            }

            for (byte i = 1; i <= 3; i++)
            {
                var customQuestion = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Policies\DigitalPersona\Altus\SpareKey", "CustomQuestion10" + i, string.Empty);

                if (!string.IsNullOrEmpty(customQuestion))
                {
                    LiveQuestions.Add(new LiveQuestion((byte)(100 + i))
                    {
                        KeyboardLayoutId = 1033,//EN-us
                        LanguageId = 9,//EN
                        SublanguageId = 1,//us
                        Text = customQuestion
                    });
                }
            }
        }

        private void LiveQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (LiveAnswers.Any(string.IsNullOrWhiteSpace) || SelectedQuestions.Any(p => p == null))
                return;

            var answerData = new List<LiveQuestionAnswer>();
            for (int i = 0; i < 3; i++)
                answerData.Add(new LiveQuestionAnswer(SelectedQuestions[i], LiveAnswers[i]));

            credential = Credential.Create(new LiveQuestionEnrollmentData(answerData));
            DialogResult = true;

        }


        #endregion

        public static IEnrollmentCredential ShowDialog(Window owner, UserName userName)
        {
            var win = new EnrollCredentialsWindow(userName);
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

    }
}
