﻿using DTK.LPR;
using DTK.Video;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTKSoftDevTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LPRParams parameters = new LPRParams();
            parameters.Countries = "AZ";
            parameters.MinPlateWidth = 80;
            parameters.MaxPlateWidth = 300;
            LPREngine engine = new LPREngine(parameters, true, OnLicensePlateDetected);
            VideoCapture videoCap = new VideoCapture(OnFrameCaptured, OnCaptureError, engine);
            videoCap.StartCaptureFromIPCamera("rtsp://platerec:8S5AZ7YasGc3m4@video.platerecognizer.com:8554/demo");
            Thread.Sleep(600000);
        }

        static void OnFrameCaptured(VideoCapture videoCap, VideoFrame frame, object customObject)
        {
            try
            {
                LPREngine engine = (LPREngine)customObject;
                engine.PutFrame(frame, 0);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        static void OnLicensePlateDetected(LPREngine engine, LicensePlate plate)
        {
            Console.WriteLine(string.Format("Plate text: {0} Country: {1} Confidence: {2}",
                   plate.Text, plate.CountryCode, plate.Confidence));
            plate.Dispose();
        }

        static void OnCaptureError(VideoCapture videoCap, ERR_CAPTURE errorCode, object customObject)
        {
            if (errorCode == ERR_CAPTURE.EOF)
            {
                Console.WriteLine($"Error occured: {JsonConvert.SerializeObject(customObject)}");
            }
            if (errorCode == ERR_CAPTURE.READ_FRAME || errorCode == ERR_CAPTURE.OPEN_VIDEO)
            {
                Console.WriteLine($"Error occured: {JsonConvert.SerializeObject(customObject)}");
            }
        }
    }
}