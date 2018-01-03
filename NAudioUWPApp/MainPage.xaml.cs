using System;
using System.IO;
using Windows.UI.Xaml.Controls;
using NAudio.Wave;
using System.Threading;
using System.Collections.Generic;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NAudioUWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private WaveStream reader;
        private IWaveIn recorder;
        private MemoryStream recordStream;
        public float bigMax;
        public float biggerMax;
        public float biggerMin;
        public float biggerMedian;
        public List<float> bigMaxList;

        public MainPage()
        {
            this.InitializeComponent();

            recorder = new WasapiCaptureRT();
            recorder.DataAvailable += RecorderOnDataAvailable;

            recorder.StartRecording();

            bigMaxList = new List<float>();
        }

        private async void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            if (reader == null)
            {
                recordStream = new MemoryStream();
                reader = new RawSourceWaveStream(recordStream, recorder.WaveFormat);
            }

            await recordStream.WriteAsync(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);

            //gets the max value, adds it to a list. if the list has 100 elements it gets the max form the list and outputs it
            float max = 0;
            var buffer = new WaveBuffer(waveInEventArgs.Buffer);
            // interpret as 32 bit floating point audio
            for (int index = 0; index < waveInEventArgs.BytesRecorded / 4; index++)
            {
                var sample = buffer.FloatBuffer[index];

                // absolute value 
                if (sample < 0) sample = -sample;
                // is this the max value?
                if (sample > max) max = sample;
            }
            bigMax = max * 100;
            bigMaxList.Add(bigMax);
            if (bigMaxList.Count == 100)
            {
                bigMaxList.Sort();
                biggerMax = bigMaxList[bigMaxList.Count - 1];
                biggerMin = bigMaxList[bigMaxList.Count - 100];
                biggerMedian = bigMaxList[bigMaxList.Count - 50];
                FuncAsync();
                bigMaxList.Clear();
            }

            //flushes stream and deletes it to preserve memory
            recordStream.Flush();
            recordStream.Dispose();
            recordStream = new MemoryStream();
        }

        public async System.Threading.Tasks.Task FuncAsync()
        {
            await progressBar1.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { progressBar1.Value = biggerMax; });
            await progressBar2.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { progressBar2.Value = biggerMedian; });
            await progressBar3.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { progressBar3.Value = biggerMin; });
            await textBlock1.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { textBlock1.Text = "Min           " + biggerMin + "\r\nMedian     " + biggerMedian + "     " + System.DateTime.Now + " " + System.DateTime.Now.Millisecond + "\r\nMax          " + biggerMax; });
        }
    }
}
