using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace FaceChanger.Face
{
    public class FaceCanvas : SKCanvasView
    {
        public static readonly BindableProperty FacesProperty = BindableProperty.Create(nameof(Faces),
        typeof(IList<DetectedFace>),
        typeof(FaceCanvas),
        new List<DetectedFace>());

        public IList<DetectedFace> Faces
        {
            get => (IList<DetectedFace>)GetValue(FacesProperty);
            set => SetValue(FacesProperty, value);
        }

        /// <summary>
        /// Adds a silly filter to a face
        /// </summary>
        public void ApplyFaceFilter(SKCanvas canvas, DetectedFace face, float left,float top,float scale)
        {
            if (face.FaceLandmarks != null)
            {
                //Draw eyes
                var eyePadding = 50;

                var leftEye = LoadImage("FaceChanger.Images.LeftEye.png");
                canvas.DrawBitmap(leftEye, new SKRect(
                    left + (scale * (float)(face.FaceLandmarks.EyeLeftOuter.X - eyePadding)),
                    top + (scale * (float)(face.FaceLandmarks.EyeLeftTop.Y - eyePadding)),
                    left + (scale * (float)(face.FaceLandmarks.EyeLeftInner.X + eyePadding)),
                    top + (scale * (float)(face.FaceLandmarks.EyeLeftBottom.Y + eyePadding))));

                var rightEye = LoadImage("FaceChanger.Images.RightEye.png");
                canvas.DrawBitmap(rightEye, new SKRect(
                    left + (scale * (float)(face.FaceLandmarks.EyeRightInner.X - eyePadding)),
                    top + (scale * (float)(face.FaceLandmarks.EyeRightTop.Y - eyePadding)),
                    left + (scale * (float)(face.FaceLandmarks.EyeRightOuter.X + eyePadding)),
                    top + (scale * (float)(face.FaceLandmarks.EyeRightBottom.Y + eyePadding))));

                //Draw nose
                var nosePadding = 30;
                var nose = LoadImage("FaceChanger.Images.Nose.png");
                canvas.DrawBitmap(nose, new SKRect(
                    left + (scale * (float)(face.FaceLandmarks.NoseLeftAlarOutTip.X - nosePadding)),
                    top + (scale * (float)face.FaceLandmarks.NoseLeftAlarTop.Y),
                    left + (scale * (float)(face.FaceLandmarks.NoseRightAlarOutTip.X + nosePadding)),
                    top + (scale * (float)(face.FaceLandmarks.NoseRightAlarOutTip.Y + nosePadding + 30))));

                //Draw mouth
                var mouthPadding = 40;
                var mouth = LoadImage("FaceChanger.Images.Mouth.png");
                canvas.DrawBitmap(mouth, new SKRect(
                    left + (scale * (float)(face.FaceLandmarks.MouthLeft.X - mouthPadding)),
                    top + (scale * (float)(face.FaceLandmarks.UpperLipTop.Y - mouthPadding)),
                    left + (scale * (float)(face.FaceLandmarks.MouthRight.X + mouthPadding)),
                    top + (scale * (float)(face.FaceLandmarks.UnderLipBottom.Y + mouthPadding))));
            }
        }

        /// <summary>
        /// Loads image from embedded resource
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        private SKBitmap LoadImage(string resourceId)
        {
            SKBitmap bitmap;
            var assembly = GetType().GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceId))
            {
                bitmap = SKBitmap.Decode(stream);
            }

            return bitmap;
        }
    }
}