using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Plugin.Media.Abstractions;

namespace FaceChanger.Face
{
    public class FaceDetector
    {
        public FaceDetector() => InitializeFaceClient();

        /// <summary>
        /// Initializes Azure Face client
        /// </summary>
        void InitializeFaceClient()
        {
            var faceCredentials = new ApiKeyServiceClientCredentials(FACE_DETECTION_KEY);
            _faceClient = new FaceClient(faceCredentials);
            _faceClient.Endpoint = FACE_DETECTION_ENDPOINT;
        }

        /// <summary>
        /// Gets faces from Azure Face API
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<List<DetectedFace>> GetFaces(MediaFile image) =>
            (await _faceClient.Face.DetectWithStreamAsync(
                image.GetStreamWithImageRotatedForExternalStorage(),
                true,
                true,
                Enum.GetValues(typeof(FaceAttributeType)).OfType<FaceAttributeType>().ToList())).ToList();

        private FaceClient _faceClient;
        private const string FACE_DETECTION_KEY = "576319ceb03642169d8ac6bf63be6d0c";
        private const string FACE_DETECTION_ENDPOINT = "https://facedetectionxamarin.cognitiveservices.azure.com/";
    }
}