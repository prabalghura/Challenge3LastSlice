using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BackgroundApplication1
{
    public sealed class StartupTask : IBackgroundTask
    {
        GpioController gpio = GpioController.GetDefault();

        private SerialDevice UartPort;
        private DataReader DataReaderObject = null;
        private DataWriter DataWriterObject;
        private CancellationTokenSource ReadCancellationTokenSource;

        int _A = 2;
        int _B = 3;
        int _D = 4;
        int _L = 5;
        int _N = 6;
        int _P = 7;
        int _R = 8;
        int _T = 9;
        int _U = 10;
        int posX = 1;
        int posY = 1;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            start();

            Thread.Sleep(2000);

            String p = "33320121222222301443343333333344402223222333";

            while (p!="") {
                String q = p.Substring(0, 1);
                p = p.Substring(1);
                int a = Int32.Parse(q);
                if (a == 0)
                    Thread.Sleep(2000);
                else if (a == 1)
                    PressLeft();
                else if (a == 2)
                    PressDown();
                else if (a == 3)
                    PressRight();
                else if (a == 4)
                    PressUp();
                Debug.WriteLine(a);
            }
            /*Thread.Sleep(5000);
            Serial();*/
            /*Debug.WriteLine("kida1");
            Thread.Sleep(5000);
            Debug.WriteLine("kida2");
            uart();
            Debug.WriteLine("kida3");*/
        }

        private async void uart()
        {
            /*PressU();
            PressA();
            PressR();
            PressT();*/

            int a = 10;
            while (a > 0)
            {
                PressUART();
                Debug.WriteLine(a);
                a--;
            }
            Debug.WriteLine("done");
        }

        private async void start() {
            PressUp();
            PressUp();
            PressDown();
            PressDown();
            PressLeft();
            PressRight();
            PressLeft();
            PressRight();
            PressB();
            PressA();

        }

        private void HighButton(GpioPin pin) {
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.High);
            Thread.Sleep(200);
        }

        private void LowButton(GpioPin pin)
        {
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.Low);
            Thread.Sleep(200);
        }

        private void PressUART()
        {
            using (GpioPin u = gpio.OpenPin(_U), a = gpio.OpenPin(_A), r = gpio.OpenPin(_R), t = gpio.OpenPin(_T))
            {
                LowButton(u); LowButton(a); LowButton(r); LowButton(t);
                HighButton(u); HighButton(a); HighButton(r); HighButton(t);
            }
        }

        private void PressU()
        {
            using (GpioPin u = gpio.OpenPin(_U))
            {
                LowButton(u);
                HighButton(u);
            }
        }

        private void PressR()
        {
            using (GpioPin r = gpio.OpenPin(_R))
            {
                LowButton(r);
                HighButton(r);
            }
        }

        private void PressT()
        {
            using (GpioPin t = gpio.OpenPin(_T))
            {
                LowButton(t);
                HighButton(t);
            }
        }

        private void PressUp()
        {
            using (GpioPin u = gpio.OpenPin(_U),
                p = gpio.OpenPin(_P))
            {
                LowButton(u); LowButton(p);
                HighButton(u); HighButton(p);
            }
            posY = posY - 1;
        }

        private void PressDown()
        {
            using (GpioPin d = gpio.OpenPin(_D),
                n = gpio.OpenPin(_N))
            {
                LowButton(d); LowButton(n);
                HighButton(d); HighButton(n);
                posY = posY + 1;
            }
        }

        private void PressLeft()
        {
            using (GpioPin l = gpio.OpenPin(_L),
                t = gpio.OpenPin(_T))
            {
                LowButton(l); LowButton(t);
                HighButton(l); HighButton(t);
                posX = posX - 1;
            }
        }

        private void PressRight()
        {
            using (GpioPin r = gpio.OpenPin(_R),
                t = gpio.OpenPin(_T))
            {
                LowButton(r); LowButton(t);
                HighButton(t); HighButton(t);
                posX = posX + 1;
            }
        }

        private void PressA()
        {
            using (GpioPin a = gpio.OpenPin(_A))
            {
                LowButton(a);
                HighButton(a);
            }
        }

        private void PressB()
        {
            using (GpioPin b = gpio.OpenPin(_B))
            {
                LowButton(b);
                HighButton(b);
            }
        }

        public async void Serial()
        {
            string aqs = SerialDevice.GetDeviceSelector("UART0");                   /* Find the selector string for the serial device   */
            var dis = await DeviceInformation.FindAllAsync(aqs);                    /* Find the serial device with our selector string  */
            SerialDevice SerialPort = await SerialDevice.FromIdAsync(dis[0].Id);    /* Create an serial device with our selected device */

            /* Configure serial settings */
            SerialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            SerialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            SerialPort.BaudRate = 9600;                                             /* mini UART: only standard baudrates */
            SerialPort.Parity = SerialParity.None;                                  /* mini UART: no parities */
            SerialPort.StopBits = SerialStopBitCount.One;                           /* mini UART: 1 stop bit */
            SerialPort.DataBits = 8;

            /* Write a string out over serial */
            string txBuffer = "Hello Serial";
            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteString(txBuffer);
            uint bytesWritten = await SerialPort.OutputStream.WriteAsync(dataWriter.DetachBuffer());

            /* Read data in from the serial port */
            const uint maxReadLength = 1024;
            DataReader dataReader = new DataReader(SerialPort.InputStream);
            uint bytesToRead = await dataReader.LoadAsync(maxReadLength);
            string rxBuffer = dataReader.ReadString(bytesToRead);
        }
    }
}
