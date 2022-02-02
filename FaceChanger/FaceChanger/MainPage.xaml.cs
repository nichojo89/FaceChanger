using Acr.UserDialogs;
using FaceChanger.Face;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SkiaSharp;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace FaceChanger
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeFacePainter();
        }

        /// <summary>
        /// Initializes camera, face painter and launches camera
        /// </summary>
        private async void InitializeFacePainter()
        {
            _faceAPI = new FaceDetector();
            await CrossMedia.Current.Initialize();
            CapturedImage = await TakePicture();
            DetectAndPaintFaces();
        }

        /// <summary>
        /// Draws face filters on every face in captured image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaintFaces(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            if(_capturedImageBitmap != null)
            {
                var scale = Math.Min(info.Width / (float)_capturedImageBitmap.Width, info.Height / (float)_capturedImageBitmap.Height);
                var scaledWidth = scale * _capturedImageBitmap.Width;
                var scaledHeight = scale * _capturedImageBitmap.Height;
                var scaledLeft = (info.Width - scaledWidth) / 2;
                var scaledTop = (info.Height - scaledHeight) / 2;
                
                //Draws captured image
                canvas.DrawBitmap(_capturedImageBitmap, new SKRect(scaledLeft, scaledTop, scaledLeft + scaledWidth, scaledTop + scaledHeight));

                //Draws faces over captured image
                FacePaintingCanvas.Faces?.ForEach(face => FacePaintingCanvas.ApplyFaceFilter(canvas, face, scaledLeft, scaledTop, scale));
            }
        }

        /// <summary>
        /// Takes selfy
        /// </summary>
        /// <returns>captured image</returns>
        public async Task<MediaFile> TakePicture()
        {
            MediaFile mediaFile = null;
            if(CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Name = "selfy.jpg",
                    RotateImage = true,
                    //TODO rename
                    Directory = "FaceAPI",
                    PhotoSize = PhotoSize.Medium,
                    DefaultCamera = CameraDevice.Front
                });
            }
            else
            {
                await DisplayAlert("Camera not found", "No Camera Available", "Ok");
            }
            return mediaFile;
        }

        /// <summary>
        /// Uses Azure Face API to detect faces and draws them on SkiaSharp canvas
        /// </summary>
        public async void DetectAndPaintFaces()
        {
            FacePaintingCanvas.Faces.Clear();

            if (CapturedImage != null)
            {
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                _capturedImageBitmap = SKBitmap.Decode(CapturedImage.GetStreamWithImageRotatedForExternalStorage());

                FacePaintingCanvas.Faces = await _faceAPI.GetFaces(CapturedImage);
                if (FacePaintingCanvas.Faces.Count > 0)
                {
                    FacePaintingCanvas.InvalidateSurface();
                }
                else
                {
                    UserDialogs.Instance.Toast("No faces found");
                }
                UserDialogs.Instance.HideLoading();
            }
        }

        public static MediaFile CapturedImage;

        private FaceDetector _faceAPI;
        private SKBitmap _capturedImageBitmap;
    }
}